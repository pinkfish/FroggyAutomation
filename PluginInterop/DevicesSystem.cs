using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FroggyPlugin.Data;
using FroggyPlugin.Events;

namespace FroggyPlugin
{
    /// <summary>
    /// Interface for getting all the devices.
    /// </summary>
    public interface DevicesSystem
    {
        /// <summary>
        /// Finds the specific device in the system.
        /// </summary>
        /// <param name="reference">The device reference to use for lookup</param>
        /// <returns>The device found, null if none found</returns>
        BasicDevice FindDevice(DeviceReference reference);

        /// <summary>
        /// The devices in the system.
        /// </summary>
        IEnumerable<BasicDevice> Devices { get; }

        /// <summary>
        /// Adds the device to the system.  Can throw an exception if the device already
        /// exists.
        /// </summary>
        /// <param name="device">The device to add</param>
        void AddDevice(BasicDevice device);

        /// <summary>
        /// A device update event.
        /// </summary>
        /// <param name="update">The update</param>
        void UpdateDevice(DeviceUpdate update);
    }
}
