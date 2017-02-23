using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FroggyPlugin.Data;
using FroggyPlugin;

namespace FroggyAutomation.Database
{
    /// <summary>
    /// Everything in here it serialized out to the file on read/write.
    /// </summary>
    [Serializable]
    public class FileDatabaseData
    {
        /// <summary>
        /// Create a new empty config objects.
        /// </summary>
        public FileDatabaseData()
        {
            this.Config = new Configuration();
            this.Devices = new Dictionary<DeviceReference, BasicDevice>();
            this.PluginConfig = new Dictionary<Type, PluginConfig>();
        }
        /// <summary>
        /// The main system config.
        /// </summary>
        public Configuration Config { get; set; }
        /// <summary>
        /// The devices in the system.
        /// </summary>
        public Dictionary<DeviceReference, BasicDevice> Devices { get; set; }
        /// <summary>
        /// Keeps track of the plugin config data.
        /// </summary>
        public Dictionary<Type, PluginConfig> PluginConfig { get; set; }
    }
}
