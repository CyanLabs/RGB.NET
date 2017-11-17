using RGB.NET.Core;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace RGB.NET.Devices.Philips
{
    public class PhilipsDevice : AbstractRGBDevice
    {
        private static readonly HttpClient client = new HttpClient();

        public override IRGBDeviceInfo DeviceInfo { get; }

        public PhilipsDevice(PhilipsDeviceInfo deviceinfo)
        {
            DeviceInfo = deviceinfo;
        }

        public void Initialize()
        {
            InitializeLed(new PhilipsLedId(this), new Rectangle(0, 0, 10, 10));

            System.Net.ServicePointManager.UseNagleAlgorithm = false;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.SetTcpKeepAlive(false, int.MaxValue, int.MaxValue);

            HttpContent data = new StringContent("{'current': 'manual'}", Encoding.UTF8, "application/json");
            client.PostAsync($"http://{((PhilipsDeviceInfo)DeviceInfo).IP}:1925/ambilight/mode", data).Wait();
        }

        protected override void UpdateLeds(IEnumerable<Led> ledsToUpdate)
        {
            Led led = ledsToUpdate.FirstOrDefault(x => x.Color.A > 0);
            if (led == null) return;

            try
            {
                HttpContent colorData = new StringContent($"{{\"r\": {led.Color.R},\"g\": {led.Color.G}, \"b\": {led.Color.B}}}", Encoding.UTF8, "application/json");
                client.PostAsync($"http://{((PhilipsDeviceInfo)DeviceInfo).IP}:1925/ambilight/cached", colorData).Wait(); //Philips Ambiliight TV
            }
            catch { /* it works so idc */ }
        }
    }
}
