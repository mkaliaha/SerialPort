using System;

namespace SerialPort
{
    public class Crc16
    {
        private const ushort Polynomial = 0xA001;
        private readonly ushort[] _table = new ushort[256];

        public Crc16()
        {
            for (ushort i = 0; i < _table.Length; ++i)
            {
                ushort value = 0;
                var temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                        value = (ushort) ((value >> 1) ^ Polynomial);
                    else
                        value >>= 1;
                    temp >>= 1;
                }
                _table[i] = value;
            }
        }

        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            foreach (var t in bytes)
            {
                var index = (byte) (crc ^ t);
                crc = (ushort) ((crc >> 8) ^ _table[index]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            var crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }
    }
}