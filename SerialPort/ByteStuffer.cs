using System.Collections.Generic;
using System.Linq;

namespace SerialPort
{
    public class ByteStuffer
    {
        private const byte Flag = 0x7E; //Control flag
        private const byte Esc = 0x7D; //Escape byte
        private const byte FlagCh = 0x5E; //byte to replace flag
        private const int DataBytes = 16; //payload
        private static readonly byte[] Header = {Flag, 0xFF, 03, 0x00}; //header of packet

        /// <summary>
        ///     Еncode bytes
        /// </summary>
        /// <param name="data">bytes to encode</param>
        /// <returns>Encoded bytes</returns>
        public static byte[] Encode(byte[] data)
        {
            var t = new Crc16();
            var chunks = data.ToList().ChunkBy(DataBytes);
            var resBytes = new List<byte>();
            foreach (var chunk in chunks) //encode data
            {
                chunk.AddRange(t.ComputeChecksumBytes(chunk.ToArray())); //Add crc16
                for (var i = 0; i < chunk.Count; i++) //Encode
                    switch (chunk[i])
                    {
                        case Flag:
                            chunk[i] = Esc;
                            i++;
                            chunk.Insert(i, FlagCh);
                            continue;
                        case Esc:
                            i++;
                            chunk.Insert(i, Esc);
                            break;
                    }
                /*Form packet*/
                resBytes.AddRange(Header);
                resBytes.AddRange(chunk);
                resBytes.Add(Flag);
            }
            return resBytes.ToArray();
        }

        /// <summary>
        ///     Decode data
        /// </summary>
        /// <param name="input">Bytes to decode</param>
        /// <returns>Decoded bytes</returns>
        public static byte[] Decode(byte[] input)
        {
            var inp = input.ToList();
            var resBytes = new List<byte>();
            var counter = 0;
            var t = new Crc16();
            var prevPos = 0;
            for (var i = 0; i < inp.Count; i++)
            {
                if (inp[i] == Flag) counter++;
                if (counter == 2)
                {
                    var chunk = inp.GetRange(prevPos, i + 1 - prevPos);
                    var adress = chunk[1];
                    chunk.RemoveRange(0, 4);
                    chunk.RemoveAt(chunk.Count - 1); //Get rid from header and ending flag
                    for (var j = 0; j < chunk.Count; j++) //Decode
                        if (chunk[j] == Esc)
                        {
                            chunk.RemoveAt(j);
                            if (chunk[j] == FlagCh) chunk[j] = Flag;
                        }
                    if (!t.ComputeChecksumBytes(chunk.GetRange(0, chunk.Count - 2).ToArray())
                            .SequenceEqual(chunk.GetRange(chunk.Count - 2, 2)) &&
                        adress != 0xFF) //Check crc16 and adress
                    {
                        prevPos = i + 1; //Skip packet if wrong crc
                        counter = 0;
                        continue;
                    }
                    chunk.RemoveRange(chunk.Count - 2, 2); //Get rid from crc bytes
                    resBytes.AddRange(chunk);
                    prevPos = i + 1; //go to next packet
                    counter = 0;
                }
            }
            return resBytes.ToArray();
        }
    }
}