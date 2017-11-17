using RGB.NET.Core;
using RGB.NET.Devices.Philips;
using System;
using System.Collections.Generic;

namespace RGB.NET.Devices.Philips
{
    public class PhilipsDeviceProvider : IRGBDeviceProvider
    {

        private string IP { get; set; }

        public static PhilipsDeviceProvider _instance;
        //public static JsonAmbilightDeviceProvider Instance => _instance ?? new JsonAmbilightDeviceProvider(IPAddress);

        public PhilipsDeviceProvider(string IPAddress)
        {
            IP = IPAddress;
            if (_instance != null) throw new InvalidOperationException($"There can be only one instance of type {nameof(PhilipsDeviceProvider)}");
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
                PhilipsDevice device = new PhilipsDevice(new PhilipsDeviceInfo(IP));

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
