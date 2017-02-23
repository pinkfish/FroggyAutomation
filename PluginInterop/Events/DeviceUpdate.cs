using FroggyPlugin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FroggyPlugin.Events
{
    /// <summary>
    /// This class communicates in both directions.  To the plugin and from the plugin.
    /// </summary>
    public class DeviceUpdate
    {
        public enum UpdateType
        {
            /// <summary>
            /// One of the states has updated.
            /// </summary>
            State,
            /// <summary>
            /// The device basic properties updated.
            /// </summary>
            Device
        }

        private readonly UpdateType update;
        private readonly BasicDevice device;
        private readonly DeviceStateBase state;

        /// <summary>
        /// Creates a new object object.
        /// </summary>
        /// <param name="update"></param>
        /// <param name="device"></param>
        public DeviceUpdate(UpdateType update, BasicDevice device, DeviceStateBase state)
        {
            this.update = update;
            this.device = device;
            this.state = state;
        }

        /// <summary>
        /// The type of the update.
        /// </summary>
        public UpdateType Type { get { return update; } }
        /// <summary>
        /// The device to update.
        /// </summary>
        public BasicDevice Device { get { return device; } }
        /// <summary>
        /// The data associated with the update.
        /// </summary>
        public DeviceStateBase State { get { return state; } }
    }
}
