using Automation.Insteon;
using Plugin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin
{
    /// <summary>
    /// The automation system.
    /// </summary>
    public class Plugins
    {
        private FluffInsteon insteon;

        /// <summary>
        /// Setup the automation system.
        /// </summary>
        /// <param name="config">The configuration input</param>
        /// <param name="devices">The existing devices</param>
        public Plugins(Configuration config, IList<BasicDevice> devices)
        {
            insteon = new FluffInsteon(config.InsteonPort, null);
            insteon.Startup();
        }

        private class DeviceSetup : DeviceProvider
        {
            private readonly IList<BasicDevice> devices;

            public DeviceSetup(IList<BasicDevice> devices)
            {
                this.devices = devices;
            }

            public Tuple<byte, byte> GetDeviceCategory(Automation.Insteon.Data.DeviceId id)
            {
                foreach (BasicDevice device in devices)
                {
                    if (device.Address.Namespace == DeviceReference.InterfaceTypes.Insteon &&
                        device.Address.Address == id.ToString())
                    {
                        //string cat = device.AutomationData[Category];
                        //string subcat = device.AutomationData[SubCategory];
                    }
                }
                return null;
            }
        }
    }
}
