using RGB.NET.Core;
using System;
using System.Collections.Generic;

namespace RGB.NET.Devices.CorsairLink
{
    public class CorsairLinkDeviceProvider : IRGBDeviceProvider
    {

        public static CorsairLinkDeviceProvider _instance;
        public static CorsairLinkDeviceProvider Instance => _instance ?? new CorsairLinkDeviceProvider();

        public CorsairLinkDeviceProvider()
        {
            if (_instance != null) throw new InvalidOperationException($"There can be only one instance of type {nameof(CorsairLinkDeviceProvider)}");
            _instance = this;
        }

        public bool IsInitialized { get; private set; }

        public IEnumerable<IRGBDevice> Devices { get; private set; }

        public bool HasExclusiveAccess => false; // we don't really need this

        public bool Initialize(bool exclusiveAccessIfPossible = false, bool throwExceptions = false)
        {
            IsInitialized = false;
            try
            {
                // you could just add than ping thing here
                CorsairLinkDevice device = new CorsairLinkDevice(new CorsairLinkDeviceInfo());
                device.Initialize();
                Devices = new List<IRGBDevice> { device };
            }
            catch
            {
                if (throwExceptions)
                    throw;
                else
                    return false;
            }

            IsInitialized = true;

            return true;
        }

        public void ResetDevices()
        {
            // we don't really need this
        }


    }
}


