using System.IO.Ports;

namespace SerialPort
{
    public class Serial : System.IO.Ports.SerialPort
    {
        private const int AttemptsToSend = 10;
        private const byte JamSignal = 0xA1;
        private const byte EndMessage = 0x0A;
        private const int CollisionGapTime = 20;
        private const int SlotTime = 50;

        /// <inheritdoc />
        /// <summary>
        ///     Class consrtuctor
        /// </summary>
        /// <param name="portName">Name of port</param>
        /// <param name="baudRate">Baud rate</param>
        /// <param name="parity">Parity bit</param>
        /// <param name="dataBits">Number of data bits</param>
        /// <param name="stopBits">Number of stop bits</param>
        public Serial(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits) : base(portName,
            baudRate, parity, dataBits, stopBits)
        {
        }

        /// <summary>
        ///     Read data from port
        /// </summary>
        /// <returns>Byte array with data</returns>
        public byte[] ReadBytes()
        {
            var data = new byte[BytesToRead];
            Read(data, 0, data.Length);
            return data;
        }

        /// <summary>
        ///     Write data to port
        /// </summary>
        /// <param name="dataBytes">Byte array with data</param>
        public void WriteData(byte[] dataBytes)
        {
            Write(dataBytes, 0, dataBytes.Length);
        }
    }
}