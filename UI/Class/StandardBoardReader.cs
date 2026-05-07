using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace UI.Class
{
    public sealed class StandardBoardReader : IDisposable
    {
        private const byte Pn532CommandSamConfiguration = 0x14;
        private const byte Pn532CommandInListPassiveTarget = 0x4A;
        private const byte Pn532CommandInDataExchange = 0x40;
        private const byte MifareCommandRead = 0x30;

        private SerialPort _serialPort;

        public StandardBoardReader(string portName, int baudRate = 115200)
        {
            PortName = portName;
            BaudRate = baudRate;
        }

        public string PortName { get; private set; }

        public int BaudRate { get; private set; }

        public void Dispose()
        {
            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        public bool Open(out string error)
        {
            error = string.Empty;
            try
            {
                _serialPort = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);
                _serialPort.ReadTimeout = 100;
                _serialPort.WriteTimeout = 100;
                _serialPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                error = string.Format("打开标板串口{0}失败:{1}", PortName, ex.Message);
                return false;
            }
        }

        public bool ReadText(out string text, out string error)
        {
            text = string.Empty;
            error = string.Empty;

            if (_serialPort == null || !_serialPort.IsOpen)
            {
                error = "标板串口未打开";
                return false;
            }

            SendWakeup();

            byte[] targetData = SendCommand(Pn532CommandInListPassiveTarget, new byte[] { 0x01, 0x00 }, 200);
            if (targetData == null || targetData.Length < 7 || targetData[0] == 0)
            {
                error = "未检测到标板卡片";
                return false;
            }

            byte[] rawData = ReadPages(4, 41);
            if (rawData == null || rawData.Length == 0)
            {
                error = "读取标板数据失败";
                return false;
            }

            text = ParseNdefText(rawData);
            if (string.IsNullOrEmpty(text))
            {
                error = "未找到标板NDEF文本记录";
                return false;
            }

            return true;
        }

        private void SendWakeup()
        {
            byte[] wakeupCommand = HexToBytes("55 55 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF 03 FD D4 14 01 17 00");
            _serialPort.DiscardInBuffer();
            _serialPort.Write(wakeupCommand, 0, wakeupCommand.Length);
            _serialPort.BaseStream.Flush();
            Thread.Sleep(50);
            ReadAvailable(100, 100);
        }

        private byte[] ReadPages(int startPage, int count)
        {
            MemoryStream stream = new MemoryStream();
            for (int page = startPage; page < startPage + count; page++)
            {
                byte[] pageData = ReadPage(page);
                if (pageData == null)
                {
                    break;
                }

                stream.Write(pageData, 0, pageData.Length);
            }

            return stream.ToArray();
        }

        private byte[] ReadPage(int page)
        {
            byte[] data = SendCommand(Pn532CommandInDataExchange, new byte[] { 0x01, MifareCommandRead, (byte)page }, 200);
            if (data == null || data.Length < 5 || data[0] != 0x00)
            {
                return null;
            }

            byte[] pageData = new byte[4];
            Array.Copy(data, 1, pageData, 0, 4);
            return pageData;
        }

        private byte[] SendCommand(byte command, byte[] parameters, int waitMs)
        {
            byte[] data = new byte[2 + parameters.Length];
            data[0] = 0xD4;
            data[1] = command;
            Array.Copy(parameters, 0, data, 2, parameters.Length);

            byte[] frame = BuildFrame(data);
            _serialPort.DiscardInBuffer();
            _serialPort.Write(frame, 0, frame.Length);
            _serialPort.BaseStream.Flush();

            byte[] response = ReadAvailable(128, waitMs);
            Pn532Response parsed = ParseResponse(response);
            if (parsed != null && parsed.DataFrameValid)
            {
                return parsed.Data;
            }

            return null;
        }

        private static byte[] BuildFrame(byte[] data)
        {
            int length = data.Length;
            byte lcs = (byte)((-length) & 0xFF);
            int sum = 0;
            foreach (byte item in data)
            {
                sum += item;
            }

            byte dcs = (byte)((-sum) & 0xFF);
            byte[] frame = new byte[6 + data.Length + 1];
            frame[0] = 0x00;
            frame[1] = 0x00;
            frame[2] = 0xFF;
            frame[3] = (byte)length;
            frame[4] = lcs;
            Array.Copy(data, 0, frame, 5, data.Length);
            frame[5 + data.Length] = dcs;
            frame[6 + data.Length] = 0x00;
            return frame;
        }

        private byte[] ReadAvailable(int maxLength, int waitMs)
        {
            MemoryStream stream = new MemoryStream();
            Stopwatch stopwatch = Stopwatch.StartNew();
            Stopwatch idleStopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < waitMs && stream.Length < maxLength)
            {
                int bytesToRead = _serialPort.BytesToRead;
                if (bytesToRead > 0)
                {
                    int count = Math.Min(bytesToRead, maxLength - (int)stream.Length);
                    byte[] buffer = new byte[count];
                    int read = _serialPort.Read(buffer, 0, count);
                    stream.Write(buffer, 0, read);
                    idleStopwatch.Restart();
                }
                else
                {
                    if (stream.Length > 0 && idleStopwatch.ElapsedMilliseconds > 20)
                    {
                        break;
                    }

                    Thread.Sleep(5);
                }
            }

            return stream.ToArray();
        }

        private static Pn532Response ParseResponse(byte[] response)
        {
            if (response == null || response.Length < 8)
            {
                return null;
            }

            for (int i = 0; i < response.Length - 4; i++)
            {
                if (response[i] != 0x00 || response[i + 1] != 0x00 || response[i + 2] != 0xFF)
                {
                    continue;
                }

                int dataLength = response[i + 3];
                int lcs = response[i + 4];
                if (((dataLength + lcs) & 0xFF) != 0)
                {
                    continue;
                }

                int tfiIndex = i + 5;
                int commandResponseIndex = i + 6;
                int dataStart = i + 7;
                int dataEnd = dataStart + dataLength - 2;
                if (dataLength < 2 || dataEnd > response.Length || response[tfiIndex] != 0xD5)
                {
                    continue;
                }

                byte[] data = new byte[dataLength - 2];
                Array.Copy(response, dataStart, data, 0, data.Length);
                return new Pn532Response
                {
                    DataFrameValid = true,
                    CommandResponse = response[commandResponseIndex],
                    Data = data
                };
            }

            return null;
        }

        private static string ParseNdefText(byte[] data)
        {
            int i = 0;
            while (i < data.Length)
            {
                byte tlvType = data[i];
                if (tlvType == 0x03)
                {
                    if (i + 1 >= data.Length)
                    {
                        break;
                    }

                    int lengthByte = data[i + 1];
                    int length;
                    int ndefStart;
                    if (lengthByte == 0xFF)
                    {
                        if (i + 4 >= data.Length)
                        {
                            break;
                        }

                        length = (data[i + 2] << 16) + (data[i + 3] << 8) + data[i + 4];
                        ndefStart = i + 5;
                    }
                    else
                    {
                        length = lengthByte;
                        ndefStart = i + 2;
                    }

                    if (ndefStart + length > data.Length || length < 4)
                    {
                        break;
                    }

                    return ParseTextRecord(data, ndefStart, length);
                }

                if (tlvType == 0xFE)
                {
                    break;
                }

                if (i + 1 >= data.Length)
                {
                    break;
                }

                i += 2 + data[i + 1];
            }

            return string.Empty;
        }

        private static string ParseTextRecord(byte[] data, int start, int length)
        {
            byte flags = data[start];
            if ((flags & 0x80) == 0)
            {
                return string.Empty;
            }

            int typeLength = data[start + 1];
            int payloadLength = data[start + 2];
            int typeStart = start + 3;
            int payloadStart = typeStart + typeLength;
            if (typeLength <= 0 || payloadLength <= 0 || payloadStart + payloadLength > start + length)
            {
                return string.Empty;
            }

            string recordType = Encoding.ASCII.GetString(data, typeStart, typeLength);
            if ((flags & 0x07) != 0x01 || recordType != "T")
            {
                return string.Empty;
            }

            int status = data[payloadStart];
            int languageLength = status & 0x3F;
            int textStart = payloadStart + 1 + languageLength;
            int textLength = payloadLength - 1 - languageLength;
            if (textLength <= 0)
            {
                return string.Empty;
            }

            Encoding encoding = (status & 0x80) != 0 ? Encoding.Unicode : Encoding.UTF8;
            return encoding.GetString(data, textStart, textLength);
        }

        private static byte[] HexToBytes(string hex)
        {
            string[] parts = hex.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] bytes = new byte[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                bytes[i] = Convert.ToByte(parts[i], 16);
            }

            return bytes;
        }

        private sealed class Pn532Response
        {
            public bool DataFrameValid { get; set; }

            public byte CommandResponse { get; set; }

            public byte[] Data { get; set; }
        }
    }
}
