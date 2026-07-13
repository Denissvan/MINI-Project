import serial
import time

class PN532_HSU:
    """PN532 HSU模式高速读写类（优化版）"""
    
    PN532_COMMAND_GETFIRMWAREVERSION = 0x02
    PN532_COMMAND_SAMCONFIGURATION = 0x14
    PN532_COMMAND_INLISTPASSIVETARGET = 0x4A
    PN532_COMMAND_INDATAEXCHANGE = 0x40

    MIFARE_CMD_READ = 0x30
    MIFARE_CMD_WRITE = 0xA2

    def __init__(self, port='COM4', baudrate=115200):
        self.port = port
        self.baudrate = baudrate
        self.ser = None
        self.timeout = 0.1
        self.uid = None
        self.atqa = None
        self.sak = None

    def open(self):
        try:
            self.ser = serial.Serial(
                port=self.port,
                baudrate=self.baudrate,
                bytesize=serial.EIGHTBITS,
                parity=serial.PARITY_NONE,
                stopbits=serial.STOPBITS_ONE,
                timeout=self.timeout
            )
            print(f"[OK] 已连接 {self.port} @ {self.baudrate}")
            return True
        except serial.SerialException as e:
            print(f"[ERROR] 打开串口失败: {e}")
            return False

    def close(self):
        if self.ser and self.ser.is_open:
            self.ser.close()
            print("[OK] 串口已关闭")

    def parse_response_fixed(self, response):
        """解析PN532响应帧，返回数据部分"""
        if not response or len(response) < 8:
            return None
        ack_frame = response[0:6]
        if ack_frame != bytes.fromhex('00 00 FF 00 FF 00'):
            return None
        for i in range(6, min(10, len(response))):
            if response[i] == 0xFF:
                if i + 4 >= len(response):
                    return None
                data_len = response[i + 1]
                lcs = response[i + 2]
                tfi = response[i + 3]
                if (data_len + lcs) & 0xFF != 0 or tfi != 0xD5:
                    return None
                if i + 4 < len(response):
                    cmd_response = response[i + 4]
                    data_start = i + 5
                    data_end = data_start + (data_len - 2)
                    data = response[data_start:data_end] if data_end <= len(response) else b''
                    return {
                        'data_frame_valid': True,
                        'command_response': cmd_response,
                        'data': data
                    }
        return None

    def send_wakeup_sequence_fixed(self):
        wakeup_cmd = bytes.fromhex('55 55 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF 03 FD D4 14 01 17 00')
        self.ser.write(wakeup_cmd)
        self.ser.flush()
        time.sleep(0.05)
        response = self.ser.read(100)
        if not response:
            return False
        parsed = self.parse_response_fixed(response)
        return parsed and parsed['data_frame_valid'] and parsed['command_response'] == 0x15

    def get_firmware_version_fixed(self):
        firmware_cmd = bytes.fromhex('00 00 FF 02 FE D4 02 2A 00')
        self.ser.write(firmware_cmd)
        self.ser.flush()
        time.sleep(0.05)
        response = self.ser.read(100)
        if not response:
            return None
        parsed = self.parse_response_fixed(response)
        if parsed and parsed['data_frame_valid'] and parsed['command_response'] == 0x03:
            ver = parsed['data'][1:4]
            print(f"[OK] 固件版本: {ver[0]}.{ver[1]}.{ver[2]}")
            return ver
        return None

    def _build_frame(self, data: bytes) -> bytes:
        length = len(data)
        lcs = (-length) & 0xFF
        dcs = (-sum(data)) & 0xFF
        return bytes([0x00, 0x00, 0xFF, length, lcs]) + data + bytes([dcs, 0x00])

    def send_command(self, command, params=b'', read_len=64):
        """发送命令并等待响应（无固定sleep，依赖串口超时）"""
        data = bytes([0xD4, command]) + params
        frame = self._build_frame(data)
        self.ser.write(frame)
        self.ser.flush()
        # 直接读取，利用串口超时等待响应（通常<10ms）
        response = self.ser.read(read_len)
        parsed = self.parse_response_fixed(response)
        if parsed and parsed['data_frame_valid']:
            return parsed['data']
        return None

    def in_list_passive_target(self):
        resp = self.send_command(self.PN532_COMMAND_INLISTPASSIVETARGET, bytes([1, 0]), read_len=32)
        if resp is None or len(resp) < 2 or resp[0] == 0:
            return False
        idx = 1
        idx += 1  # target number
        self.atqa = resp[idx:idx+2]; idx += 2
        self.sak = resp[idx]; idx += 1
        uid_len = resp[idx]; idx += 1
        self.uid = resp[idx:idx+uid_len]
        return True

    def read_page_fast(self, page, retry=1):
        """快速读取单页，支持重试"""
        apdu = bytes([self.MIFARE_CMD_READ, page])
        for _ in range(retry):
            resp = self.send_command(self.PN532_COMMAND_INDATAEXCHANGE, bytes([0x01]) + apdu, read_len=32)
            if resp and len(resp) > 0 and resp[0] == 0x00:
                return resp[1:5]
        return None

    def read_pages_fast(self, start_page, count, retry=1):
        """连续读取多页，每页超时极短"""
        data = bytearray()
        for p in range(start_page, start_page + count):
            page_data = self.read_page_fast(p, retry)
            if page_data is None:
                print(f"[WARN] 读取页{p}失败，停止")
                break
            data.extend(page_data)
        return bytes(data)

    def write_page(self, page, data, retry=1):
        if len(data) != 4:
            raise ValueError("数据必须为4字节")
        apdu = bytes([self.MIFARE_CMD_WRITE, page]) + data
        for _ in range(retry):
            resp = self.send_command(self.PN532_COMMAND_INDATAEXCHANGE, bytes([0x01]) + apdu, read_len=32)
            if resp and resp[0] == 0x00:
                return True
        return False

    def parse_ndef_from_data(self, data: bytes):
        """从任意偏移的数据中解析NDEF文本记录（支持长TLV）"""
        i = 0
        while i < len(data):
            tlv_type = data[i]
            if tlv_type == 0x03:          # NDEF消息TLV
                # 读取长度
                if i + 1 >= len(data):
                    break
                length_byte = data[i+1]
                if length_byte == 0xFF:
                    # 3字节长度
                    if i + 4 >= len(data):
                        break
                    length = int.from_bytes(data[i+2:i+5], 'big')
                    ndef_start = i + 5
                else:
                    length = length_byte
                    ndef_start = i + 2
                # 确保数据足够
                if ndef_start + length > len(data):
                    break
                ndef_data = data[ndef_start:ndef_start+length]
                # 解析NDEF记录头
                if len(ndef_data) < 4:
                    break
                flags = ndef_data[0]
                if not (flags & 0x80):      # 不是消息开始
                    i += 2 + (3 if length_byte == 0xFF else 0) + length
                    continue
                tnf = flags & 0x07
                type_len = ndef_data[1]
                payload_len = ndef_data[2]
                if 3 + type_len + payload_len > len(ndef_data):
                    break
                record_type = ndef_data[3:3+type_len].decode('ascii')
                payload = ndef_data[3+type_len:3+type_len+payload_len]
                if tnf == 0x01 and record_type == 'T':
                    # 文本记录
                    if len(payload) < 1:
                        break
                    status = payload[0]
                    lang_len = status & 0x3F
                    text_bytes = payload[1+lang_len:]
                    encoding = 'UTF-16' if (status & 0x80) else 'UTF-8'
                    return text_bytes.decode(encoding)
                break
            elif tlv_type == 0xFE:          # 终止符
                break
            else:
                # 跳过未知TLV
                if i + 1 >= len(data):
                    break
                length = data[i+1]
                i += 2 + length
        return None

    def read_tag_info(self):
        """高速完整读取标签（一次性读取页4~44，耗时<1秒）"""
        print("\n" + "="*60)
        print("读取NFC标签")
        print("="*60)

        start_time = time.time()

        if not self.in_list_passive_target():
            print("[ERROR] 未检测到卡片")
            return None

        uid_str = self.uid.hex().upper()
        print(f"UID: {uid_str}")
        print(f"ATQA: {self.atqa.hex().upper()}  SAK: {self.sak:02X}")

        # 一次性读取页4至页44（共41页，164字节），覆盖MIFARE 1K全部NDEF区域
        # 页0-3通常为厂商数据，页4开始存放NDEF
        START_PAGE = 4
        PAGE_COUNT = 41   # 4 ~ 44
        raw_data = self.read_pages_fast(START_PAGE, PAGE_COUNT, retry=1)
        if not raw_data:
            print("[ERROR] 读取数据失败")
            return None

        text = self.parse_ndef_from_data(raw_data)
        elapsed = time.time() - start_time
        print(f"读取耗时: {elapsed:.3f}秒")

        if text:
            print(f"标板信息: {text}")
        else:
            print("[WARN] 未找到NDEF文本记录")
        return text

    def write_ndef_text(self, text, language='en'):
        """写入NDEF文本（需要先寻卡）"""
        if not self.uid:
            print("[ERROR] 请先寻卡")
            return False

        lang_bytes = language.encode('ascii')
        text_bytes = text.encode('utf-8')
        status = len(lang_bytes)
        payload = bytes([status]) + lang_bytes + text_bytes

        flags = 0b11010001
        record = bytes([flags, 1, len(payload)]) + b'T' + payload
        tlv = bytes([0x03, len(record)]) + record + bytes([0xFE])

        start_page = 4
        max_bytes = (45 - start_page) * 4
        if len(tlv) > max_bytes:
            print("[ERROR] 数据过长")
            return False

        padded = tlv.ljust(max_bytes, b'\x00')
        for i in range(0, len(padded), 4):
            page = start_page + i // 4
            if not self.write_page(page, padded[i:i+4], retry=2):
                print(f"[ERROR] 写入页{page}失败")
                return False
        print("[OK] 写入完成")
        return True


def main():
    print("PN532 HSU NFC 高速读写工具 (优化版)")
    print("=" * 60)

    pn532 = PN532_HSU(port='COM4', baudrate=115200)
    if not pn532.open():
        return

    try:
        if not pn532.send_wakeup_sequence_fixed():
            print("[WARN] 唤醒异常，尝试继续")
        pn532.get_firmware_version_fixed()

        # 读取标签
        pn532.read_tag_info()

        # 写入示例（取消注释以启用）
        # print("\n尝试写入新文本...")
        # if pn532.in_list_passive_target():
        #     pn532.write_ndef_text("Hello PN532!")
        #     # 验证
        #     pn532.in_list_passive_target()
        #     pn532.read_tag_info()

    except Exception as e:
        print(f"[ERROR] {e}")
    finally:
        pn532.close()

    print("\n测试结束")


if __name__ == "__main__":
    main()
