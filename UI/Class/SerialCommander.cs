using MotionCtrl;
using System;
using System.IO.Ports;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Markup;
using UI.Class;
using static SerialCommander;

public class SerialCommander : IDisposable
{
    private SerialPort _serialPort;

    public void Dispose()
    {
        _serialPort?.Close();
        _serialPort?.Dispose();
    }
    //public static SerialCommander COMM1 = new SerialCommander("COM8");
    //public static SerialCommander COMM2 = new SerialCommander("COM9");
    //public static SerialCommander COMM3= new SerialCommander("COM10");
    private readonly byte[] Header = new byte[] { 0x55, 0xAA, 0xAA, 0x55 };
    public event Action<AutoInspectionParameter> DataUpdated;
    private double _currentLux;
    private double _currentCct;
    private double _currentDistance;
    private string _currentStatus;
    private GyroscopeData gyroscopeData { set; get; }
    private JitterThreshold jitterThreshold { set; get; }
    private readonly object _dataLock = new object();

    public event Action<GyroscopeData> GyroUpdated;
    public event Action<JitterThreshold> JitterThresholdUpdated;

    public enum GyroCheckState
    {
        Disabled = 0,     // 完全不检测
        Idle = 1,         // 空闲可检测
        Running = 2,      // 主流程运行中
        Error = 3,        // 异常状态
        Stop = 4          // 停机
    }

    public class GyroscopeData
    {
        public bool IsJittering { get; set; }

        public double GyroX { get; set; }
        public double GyroY { get; set; }
        public double GyroZ { get; set; }

        public double AccelX { get; set; }
        public double AccelY { get; set; }
        public double AccelZ { get; set; }

        public double AccelVector { get; set; }
    }
    public class JitterThreshold
    {
        public double UpperLimit { get; set; }
        public double LowerLimit { get; set; }
    }

    public double CurrentDistance
    {
        get
        {
            lock (_dataLock)  // 使用已有锁保证线程安全
            {
                return _currentDistance;
            }
        }
    }
    public event Action<double> DistanceUpdated;
    public event Action<double> LuxUpdated;
    public event Action<double> CctUpdated;
    public SerialCommander(string portName, int baudRate = 115200)
    {
        if (string.IsNullOrEmpty(portName))
            throw new ArgumentNullException(nameof(portName), "COM口名称不能为空");

        _serialPort = new SerialPort(portName, baudRate);
        _serialPort.DataReceived += SerialDataReceived;
        try
        {
            _serialPort.Open(); // 打开COM口
        }
        catch (Exception ex)
        {
            // 打开失败时释放已创建的资源
            Dispose();
            throw new InvalidOperationException($"无法打开COM口 {portName}", ex);
        }
    }

    // 数据接收事件（异步处理）
    private void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        int len = _serialPort.BytesToRead;
        byte[] buffer = new byte[len];
        _serialPort.Read(buffer, 0, len);

        string json = ExtractJsonFromBuffer(buffer);
        if (json != null)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    AutoInspectionParameter newData = new AutoInspectionParameter();
                    lock (_dataLock)
                    {

                        if (doc.RootElement.TryGetProperty("lux", out JsonElement luxElem))
                        {
                            _currentLux = Math.Floor(Math.Abs(luxElem.GetDouble())); // 直接获取整数
                            newData.LUX = _currentLux;
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "设备状态LUX: " + _currentLux.ToString("E3"));
                            LuxUpdated?.Invoke(_currentLux);
                        }
                        if (doc.RootElement.TryGetProperty("cct", out JsonElement cctElem))
                        {
                            _currentCct = Math.Floor(Math.Abs(cctElem.GetDouble())); // 直接获取整数
                            newData.CCT = _currentCct;
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "设备状态CCT: " + _currentCct.ToString("E3"));
                            CctUpdated?.Invoke(_currentCct);
                        }
                        if (doc.RootElement.TryGetProperty("distance", out JsonElement distanceElem))
                        {
                            _currentDistance = PT_SET.distance_coefficient * Math.Abs(Convert.ToInt32(distanceElem.GetInt32()));
                            newData.Distance = _currentDistance;
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "设备状态Distance: " + _currentDistance+ "距离系数: "+ PT_SET.distance_coefficient);
                            DistanceUpdated?.Invoke(_currentDistance);
                        }


                        if (doc.RootElement.TryGetProperty("gyro_x", out JsonElement _out))
                        {
                          
                            ////gyroscopeData= Math.Floor(Math.Abs(GyroX.GetDouble()));
                            GyroscopeData data = new GyroscopeData
                            {
                                //IsJittering = doc.RootElement.GetProperty("is_jittering").GetBoolean(),
                                GyroX = doc.RootElement.GetProperty("gyro_x").GetDouble(),
                                GyroY = doc.RootElement.GetProperty("gyro_y").GetDouble(),
                                GyroZ = doc.RootElement.GetProperty("gyro_z").GetDouble(),
                                AccelX = doc.RootElement.GetProperty("accel_x").GetDouble(),
                                AccelY = doc.RootElement.GetProperty("accel_y").GetDouble(),
                                AccelZ = doc.RootElement.GetProperty("accel_z").GetDouble(),
                                //AccelVector = doc.RootElement.GetProperty("accel_vector").GetDouble()
                            };
                            //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "陀螺仪GyroX值为: " + gyroscopeData.GyroX);

                            GyroUpdated?.Invoke(data);
                        }
                        if (doc.RootElement.TryGetProperty("jitter_upper_limit", out JsonElement out1))
                        {
                            JitterThreshold threshold = new JitterThreshold
                            {
                                UpperLimit = doc.RootElement.GetProperty("jitter_upper_limit").GetDouble(),
                                LowerLimit = doc.RootElement.GetProperty("jitter_lower_limit").GetDouble()
                            };
                            //jitterThreshold.UpperLimit = Math.Floor(Math.Abs(jitter_upper_limit.GetDouble()));
                            //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, " UpperLimit值为: " + jitterThreshold.UpperLimit );
                            JitterThresholdUpdated?.Invoke(threshold);
                        }
                        if (doc.RootElement.TryGetProperty("status", out JsonElement statusElem))
                        {
                            string status = statusElem.GetString();
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "设备状态: " + status);
                        }
                    }
            }

            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "解析失败: " + ex.Message);
            }
        }
    }

    // 通用发送函数
    private void SendCommand(ushort cmd, string jsonPayload = null)
    {
        byte[] payload = string.IsNullOrEmpty(jsonPayload) ? Array.Empty<byte>() : Encoding.ASCII.GetBytes(jsonPayload);
        ushort len = (ushort)payload.Length;

        byte[] cmdBytes = BitConverter.GetBytes(cmd);      // 小端
        byte[] lenBytes = BitConverter.GetBytes(len);      // 小端

        byte[] packet = new byte[Header.Length + 2 + 2 + payload.Length];
        Buffer.BlockCopy(Header, 0, packet, 0, 4);
        Buffer.BlockCopy(cmdBytes, 0, packet, 4, 2);
        Buffer.BlockCopy(lenBytes, 0, packet, 6, 2);
        if (payload.Length > 0)
            Buffer.BlockCopy(payload, 0, packet, 8, payload.Length);

        _serialPort.Write(packet, 0, packet.Length);
    }

    // 指令封装函数
    public void ReadGyroscope()
    {
        SendCommand(0x9001);
    }

    public void ReadJitterThreshold()
    {
        SendCommand(0x9002);
    }
    public void ReadLightCalibration() => SendCommand(0x7002);

    public void ReadDistance() => SendCommand(0x8001);

    public void SetLaserStatus(bool on)
    {
        string json = $"{{\"laser\": {(on ? 1 : 0)} }}";
        SendCommand(0x8003, json);
    }

    // 从返回包中提取 JSON 部分
    private string ExtractJsonFromBuffer(byte[] buffer)
    {
        if (buffer.Length < 8) return null;

        ushort len = BitConverter.ToUInt16(buffer, 6);
        if (buffer.Length < 8 + len) return null;

        byte[] payload = new byte[len];
        Buffer.BlockCopy(buffer, 8, payload, 0, len);

        return Encoding.ASCII.GetString(payload);
    }
    public double ReadDistanceWithWait(int timeoutMs)
    {
        double distance = -1;
        int count = 0;
        using (ManualResetEvent wait = new ManualResetEvent(false))
        {
            void OnUpdate(double val)
            {
                distance = val;
                wait.Set();
            }

            DistanceUpdated += OnUpdate;
            while(count<50)
            {
                ReadDistance();
                Thread.Sleep(200);
                if(distance>0)
                {
                    break;
                }
                count++;

            }
            

            //if (!wait.WaitOne(timeoutMs))
            //{
            //    DistanceUpdated -= OnUpdate;
            //    throw new TimeoutException();
            //}

            DistanceUpdated -= OnUpdate;
        }


        return distance;
    }
    public (double lux, double cct) ReadLuxCctWithWait(int timeoutMs)
    {
        
        double lux = -1, cct = -1;
        int count = 0;
        using (ManualResetEvent wait = new ManualResetEvent(false))
        { 
        void OnLux(double v) { lux = v; count++; if (count >= 2) wait.Set(); }
        void OnCct(double v) { cct = v; count++; if (count >= 2) wait.Set(); }

        LuxUpdated += OnLux;
        CctUpdated += OnCct;
            while (count < 30)
            {
                ReadLightCalibration();
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"循环度数{count},lux:{lux},cct:{cct}");
                Thread.Sleep(500);
                if (lux > 0&&cct>0) break;
                count++;
            }
            if (!wait.WaitOne(timeoutMs))
            {  
                throw new TimeoutException();
            }
        LuxUpdated -= OnLux;
        CctUpdated -= OnCct;
    }
        return (lux, cct);
    }
    public GyroscopeData ReadGyroscopeWithWait(int timeoutMs)
    {

        GyroscopeData result = null;
        using (ManualResetEvent wait = new ManualResetEvent(false))
        {
            Action<GyroscopeData> handler = data =>
            {
                result = data;
                wait.Set();
            };

            GyroUpdated += handler;
            ReadGyroscope();

            if (!wait.WaitOne(timeoutMs))
            {
                GyroUpdated -= handler;
                throw new TimeoutException("读取陀螺仪超时");
            }

            GyroUpdated -= handler;
        }

        return result;
    }
    public JitterThreshold ReadJitterThresholdWithWait(int timeoutMs)
    {
        JitterThreshold result =null;

        using (ManualResetEvent wait = new ManualResetEvent(false))
        {
            Action<JitterThreshold> handler = t =>
            {
                result = t;
                wait.Set();
            };

            JitterThresholdUpdated += handler;
            ReadJitterThreshold();

            if (!wait.WaitOne(timeoutMs))
            {
                JitterThresholdUpdated -= handler;
                throw new TimeoutException("读取抖动阈值超时");
            }

            JitterThresholdUpdated -= handler;
        }

        return result;
    }
    public void Close() => _serialPort?.Close();
}
