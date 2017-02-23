using Db4objects.Db4o;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FroggyPlugin.Data;
using FroggyPlugin;

namespace FroggyAutomation.Database
{
    /// <summary>
    /// Access to the data in the database around the specific config and modules.
    /// </summary>
    public interface Database : IDisposable
    {
        /// <summary>
        /// The system configuration.
        /// </summary>
        Configuration Config { get; set; }

        /// <summary>
        /// All the known devices.
        /// </summary>
        IEnumerable<BasicDevice> Devices { get; }

        /// <summary>
        /// Adds the device into the system.
        /// </summary>
        /// <param name="device">The device to add in</param>
        void AddOrUpdateDevice(BasicDevice device);


        /// <summary>
        /// Finds the device in system.
        /// </summary>
        /// <param name="device">The device to find</param>
        BasicDevice FindDevice(DeviceReference reference);

        /// <summary>
        /// Find the config for the specific type of plugin.
        /// </summary>
        /// <param name="type">Type of the plugin</param>
        /// <returns>The config object</returns>
        PluginConfig GetPluginConfig(Type type);


        /// <summary>
        /// Stores the plugin config into the database for this specific plugin.
        /// </summary>
        /// <param name="cofig">The config o use</param>
        /// <param name="type">Type of the data</param>
        void SetPluginConfig(PluginConfig cofig, Type type);
    }
}
