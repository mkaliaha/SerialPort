using System.IO.Ports;
using System.Threading;

namespace SerialPort
{
    public class Serial : System.IO.Ports.SerialPort
    {
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
        ///     Field to store lost bytest
        /// </summary>
        public byte[] LostBytes { get; set; }

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
            for (var i = 0; i < dataBytes.Length; i++)
                while (true)
                {
                    if (BytesToRead == 0)
                    {
                        RtsEnable = true;
                        Write(dataBytes, i, 1);
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