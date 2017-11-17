using RGB.NET.Core;
using System;

namespace RGB.NET.Devices.CorsairLink
{
    public class CorsairLinkDeviceInfo : IRGBDeviceInfo
    {
        // we could add parameters for stuff like the url here

        #region Properties & Fields

        public RGBDeviceType DeviceType => RGBDeviceType.LedStripe;
        public string Manufacturer => "Corsair";
        public string Model => "Link";
        public RGBDeviceLighting Lighting => RGBDeviceLighting.Device;
        public Uri Image { get; protected set; }

        #endregion
    }
}
