using Plugin.Data;
using Plugin.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Adapters
{
    /// <summary>
    /// The device adapter.
    /// </summary>
    public abstract class DeviceAdapter
    {
        private readonly IList<BasicDevice> devices;

        public DeviceAdapter(IList<BasicDevice> devices)
        {
            this.devices = devices;
        }

        /// <summary>
        /// Returns the devices known in this system at startup.
        /// </summary>
        public IList<BasicDevice> Devices { get { return this.devices; } }

        public abstract void UpdateDevice(BasicDevice device, DeviceUpdate update);
    }
}
