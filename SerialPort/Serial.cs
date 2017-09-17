using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace SerialPort
{
    public class Serial : System.IO.Ports.SerialPort
    {
        /// <summary>
        /// Field to store lost bytest
        /// </summary>
        public byte[] LostBytes { get; set; }

        /// <summary>
        /// Class consrtuctor
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
        /// Read data from port
        /// </summary>
        /// <returns>Byte array with data</returns>
        public byte[] ReadBytes()
        {
            var data = new byte[BytesToRead];
            Read(data, 0, data.Length);
            return ByteStuffer.Decode(data);
        }

        /// <summary>
        /// Write data to port
        /// </summary>
        /// <param name="dataBytes">Byte array with data</param>
        public void WriteData(byte[] dataBytes)
        {
            while (true)
            {
                if (BytesToRead == 0)
                {
                    var temp =ByteStuffer.Encode(dataBytes);
                    Write(temp, 0, temp.Length);
                    Thread.Sleep(100);
                    RtsEnable = false;
                }
                else
                {
                    LostBytes = ReadBytes();
                    continue;
                }
                break;
            }
        }
    }
}