using System.Collections.Generic;
using System.Linq;

namespace SerialPort
{
    public class PackageComposer
    {
        private const byte StartingDelimeter = 0xA1;
        private const byte EndingDelimeter = 0xA1;
        private const byte Control = 0x00;
        private const byte Message = 0x01;
        public byte[] Token => new[] {StartingDelimeter, Control, EndingDelimeter};

        public bool IsToken(byte[] package)
        {
            return package[1] == Control;
        }

        public bool IsMessage(byte[] package)
        {
            return !IsToken(package);
        }

        public byte GetSourceAddress(byte[] package)
        {
            return package[3];
        }

        public byte GetDestAddress(byte[] package)
        {
            return package[2];
        }

        public byte[] GetDataFromMessage(byte[] package)
        {
            return package.ToList().GetRange(4, package.Length - 5).ToArray();
        }

        public byte[] ComposeMessage(byte[] data, byte dest, byte source)
        {
            var result = new List<byte> {StartingDelimeter, Message, dest, source};
            result.AddRange(data);
            result.Add(EndingDelimeter);
            return result.ToArray();
        }
    }
}