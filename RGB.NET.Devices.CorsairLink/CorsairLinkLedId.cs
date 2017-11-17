using RGB.NET.Core;

namespace RGB.NET.Devices.CorsairLink
{
    // we don't need identification since we only have one led anyway
    public class CorsairLinkLedId : ILedId
    {
        public IRGBDevice Device { get; }

        public bool IsValid => true;

        public CorsairLinkLedId(IRGBDevice device)
        {
            this.Device = device;
        }
    }
}
