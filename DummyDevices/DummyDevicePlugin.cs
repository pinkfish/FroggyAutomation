using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FroggyPlugin;
using Mono.Addins;

namespace DummyDevices
{
    [assembly: Addin]
    [assembly: AddinDependency("FroggyAutomation", "1.0")]

    /// <summary>
    /// The dummy plugin devices to use.  These are ones that can store values and pretend to do things.
    /// </summary>
    [Extension]
    public class DummyDevicePlugin : Plugin
    {
        private PluginConfig config;
        private FroggySystem system;

        public PluginConfig Configuration
        {
            get
            {
                return config;
            }
            set
            {
                config = value;
            }
        }

        public bool Startup(FroggySystem callbacks)
        {
            this.system = callbacks;
            return true;
        }

        /// <summary>
        /// Apply a specific update to the device.
        /// </summary>
        /// <param name="update">the update to apply</param>
        public void UpdateDevice(FroggyPlugin.Events.DeviceUpdate update)
        {
        }

        /// <summary>
        /// Clean up the plugin
        /// </summary>
        public void Dispose()
        {
            this.system = null;
            this.config = null;
        }
    }
}
