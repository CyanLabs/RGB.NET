using RGB.NET.Core;

namespace RGB.NET.Devices.Philips
{
    // we don't need identification since we only have one led anyway
    public class PhilipsLedId : ILedId
    {
        public IRGBDevice Device { get; }
        
        public bool IsValid => true;

        public PhilipsLedId(IRGBDevice device)
        {
            this.Device = device;
        }
    }
}
