using HidLibrary;
using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace RGB.NET.Devices.CorsairLink
{
    public class CorsairLinkDevice : AbstractRGBDevice
    {
        public override IRGBDeviceInfo DeviceInfo { get; }

        private HidDevice corsairLNP { get; set; }

        public void Initialize()
        {
            try
            {
                InitializeLed(new CorsairLinkLedId(this), new Rectangle(0, 0, 10, 10));
                HidDevice[] HidDeviceList;
                int[] hids = new int[] { 0x0C10, 0x0C08 };
                HidDeviceList = HidDevices.Enumerate(0x1B1C, hids).ToArray();

                if (HidDeviceList.Length > 0)
                {
                    corsairLNP = HidDeviceList[0];
                    corsairLNP.MonitorDeviceEvents = true;
                    corsairLNP.OpenDevice();
                    corsairLNP.Write(new byte[] {0x00,
                   0x38, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00});
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            
            //1 time code
        }

        public CorsairLinkDevice(IRGBDeviceInfo info)
        {
            this.DeviceInfo = info;
        }

        protected override void UpdateLeds(IEnumerable<Led> ledsToUpdate)
        {
            Led led = ledsToUpdate.FirstOrDefault(x => x.Color.A > 0);
            if (led == null) return;

            try
            {
                corsairLNP.Write(new byte[] {0x00,
                   0x34, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00});

                byte[][] fanInfo = BuldColorPacket(led.Color);

                for (int i = 0; i < fanInfo.Length; i++)
                {
                    corsairLNP.Write(fanInfo[i]);
                }
                corsairLNP.Write(StringToByteArray("00 33 FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00"));
            }
            catch
            {
            }
        }
        //CREDIT TO Zeinaro for this
        internal static byte[][] BuldColorPacket(Color c1, byte Channel = 0x00, byte LEDs = 0x24)
        {

            byte[] red1 = new byte[] {0x00,
                   0x32, Channel, 0x00, LEDs, 0x00, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R,
                   c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R,
                   c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R,
                   c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R, c1.R};

            byte[] green1 = new byte[] {0x00,
                   0x32, Channel, 0x00, LEDs, 0x01, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G,
                   c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G,
                   c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G,
                   c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G, c1.G};

            byte[] blue1 = new byte[] {0x00,
                   0x32, Channel, 0x00, LEDs, 0x02, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B,
                   c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B,
                   c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B,
                   c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B, c1.B};

            return new byte[3][] { red1, green1, blue1, };
        }

        public static byte[] StringToByteArray(string hex)
        {
            hex = hex.Replace(" ", "");
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

    }
}

