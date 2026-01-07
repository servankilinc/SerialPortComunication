using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    public static class DispenserCommands
    {
        /// <summary>
        /// Varsayılan kart verme komutu
        /// </summary>
        public static byte[] DispenseDC()
        {
            var cmd = new List<byte>();
            cmd.Add(0x02);
            cmd.Add(Convert.ToByte('D'));
            cmd.Add(Convert.ToByte('C'));
            cmd.Add(0x03);
            cmd.Add(CalculateBCC(cmd.ToArray()));
            return cmd.ToArray();
        }

        /// <summary>
        /// Seçili pozisyondan kart verme komutu
        /// </summary>
        public static byte[] DispenseFC(DispenserEnums.Position position)
        {
            var cmd = new List<byte>
            {
                0x02,
                Convert.ToByte('F'),
                Convert.ToByte('C'),
                Convert.ToByte(((byte)position).ToString()[0]),
                0x03
            };
            cmd.Add(CalculateBCC(cmd.ToArray()));
            return cmd.ToArray();
        }

        /// <summary>
        /// Cihaz durumunu sorgula komutu
        /// </summary>
        public static byte[] StatusRF()
        {
            var cmd = new List<byte>
            {
                0x02,
                Convert.ToByte('S'),
                Convert.ToByte('F'),
                0x03
            };
            cmd.Add(CalculateBCC(cmd.ToArray()));
            return cmd.ToArray();
        }

        /// <summary>
        /// Cihaz durumunu sorgula komutu
        /// </summary>
        public static byte[] StatusAP()
        {
            var cmd = new List<byte>
            {
                0x02,
                Convert.ToByte('A'),
                Convert.ToByte('P'),
                0x03
            };
            cmd.Add(CalculateBCC(cmd.ToArray()));
            return cmd.ToArray();
        }

        /// <summary>
        /// Kapı kontrol komutu
        /// </summary>
        public static byte[] DoorSetIN(DispenserEnums.DoorStatus doorStatus)
        {
            var cmd = new List<byte>
            {
                0x02,
                Convert.ToByte('I'),
                Convert.ToByte('N'),
                Convert.ToByte(((byte)doorStatus).ToString()[0]),
                0x03
            };
            cmd.Add(CalculateBCC(cmd.ToArray()));
            return cmd.ToArray();
        }

        /// <summary>
        /// Kapının mevcut durumunu sorgulama komutu
        /// </summary>
        public static byte[] DoorStatusSI()
        {
            var cmd = new List<byte>
            {
                0x02,
                Convert.ToByte('S'),
                Convert.ToByte('I'),
                0x03
            };
            cmd.Add(CalculateBCC(cmd.ToArray()));
            return cmd.ToArray();
        }

        /// <summary>
        /// Kart içeri(hazneye) atma komutu
        /// </summary>
        public static byte[] CaptureCP()
        {
            var cmd = new List<byte>
            {
                0x02,
                Convert.ToByte('C'),
                Convert.ToByte('P'),
                0x03
            };
            cmd.Add(CalculateBCC(cmd.ToArray()));
            return cmd.ToArray();
        }

        /// <summary>
        /// Cihazı resetleme komutu
        /// </summary>
        public static byte[] ResetRS()
        {
            var cmd = new List<byte>
            {
                0x02,
                Convert.ToByte('R'),
                Convert.ToByte('S'),
                0x03
            };
            cmd.Add(CalculateBCC(cmd.ToArray()));
            return cmd.ToArray();
        }

        /// <summary>
        /// BCC(Block Check Character), iletişim sırasında veri bütünlüğünü kontrol etmek için komuttaki tüm byte’ları XOR (^) işlemi ile birleştirir.
        /// </summary>
        private static byte CalculateBCC(byte[] cmd)
        {
            byte checkSumByte = 0x00;
            for (int i = 0; i < cmd.Length; i++)
                checkSumByte ^= cmd[i];

            return checkSumByte;
        }
    }
}
