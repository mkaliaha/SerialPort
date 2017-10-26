using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace SerialPort
{
    public class Serial : System.IO.Ports.SerialPort
    {
        private const int AttemptsToSend = 10;
        private const byte JamSignal = 0xA7;
        private const byte EndMessage = 10;
        private const int CollisionGapTime = 20;
        private const int SlotTime = 50;

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
            var msg = new List<byte>();
            while (true)
            {
                if (BytesToRead == 0)
                    continue;
                var receiveByte = (byte) ReadByte();

                if (receiveByte == JamSignal)
                {
                    if (msg.Count > 0)
                        msg.RemoveAt(msg.Count - 1);
                    continue;
                }
                if (receiveByte == EndMessage)
                {
                    msg.Add(receiveByte);
                    break;
                }
                msg.Add(receiveByte);
            }
            return msg.ToArray();
        }

        /// <summary>
        ///     Write data to port
        /// </summary>
        /// <param name="dataBytes">Byte array with data</param>
        public void WriteData(byte[] dataBytes)
        {
            // RtsEnable = true;
            foreach (var t in dataBytes)
                for (var j = 0; j < AttemptsToSend; j++)
                {
                    while (IsChannelFree())
                    {
                    }

                    Write(new[] {t}, 0, 1);

                    Console.Write((char) t + "\n");

                    Thread.Sleep(CollisionGapTime);
                    if (!IsCollision())
                        break;
                    Write(new[] {JamSignal}, 0, 1);

                    Console.Write((char) JamSignal + "\n");

                    Thread.Sleep(new Random().Next(0, (int) Math.Pow(2, Math.Min(j, 10))) * SlotTime);
                    if (j == AttemptsToSend - 1) throw new Exception("Attempts ended!");
                }
            //RtsEnable = false;
        }

        private bool IsCollision()
        {
            return DateTime.Now.TimeOfDay.Milliseconds % 2 == 0;
        }

        private bool IsChannelFree()
        {
            return DateTime.Now.TimeOfDay.Milliseconds % 2 != 0;
        }
    }
}