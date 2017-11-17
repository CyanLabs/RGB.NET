using RGB.NET.Core;
using System;

namespace RGB.NET.Devices.Philips
{
    public class PhilipsDeviceInfo : IRGBDeviceInfo
    {
        // we could add parameters for stuff like the url here

    #region Properties & Fields

        public RGBDeviceType DeviceType => RGBDeviceType.LedStripe;
        public string Manufacturer => "Philips";
        public string Model => "Jointspace";
        public RGBDeviceLighting Lighting => RGBDeviceLighting.Device;
        public Uri Image { get; protected set; }
        public string IP { get; }
     #endregion

       public PhilipsDeviceInfo(string IP)
        {
            this.IP = IP;
        }
    }
}
