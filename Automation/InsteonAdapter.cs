#region License
// Copyright (c) 2012, David Bennett. All rights reserved.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
// MA 02110-1301  USA
#endregion
using Automation.Insteon;
using Automation.Insteon.Data;
using Automation.Insteon.Devices;
using FroggyPlugin;
using FroggyPlugin.Data;
using FroggyPlugin.Events;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    [assembly:Addin]
    [assembly:AddinDependency("FroggyAutomation", "1.0")]

    /// <summary>
    /// The insteon plugin interface to use.
    /// </summary>
    [Extension]
    public class InsteonPlugin : Plugin, DeviceProvider
    {
        private Insteon.FluffInsteon insteon;
        private InsteonConfiguration config;
        private FroggySystem callbacks;

        /// <summary>
        /// Converts to and from the insteon configuration object.
        /// </summary>
        public PluginConfig Configuration
        {
            get
            {
                return config;
            }
            set
            {
                config = (InsteonConfiguration)value;
            }
        }

        /// <summary>
        /// Starts up the insteon system.
        /// </summary>
        /// <param name="callbacks">The parameters for all the callbacks</param>
        /// <returns>true if we successful started up</returns>
        public bool Startup(FroggySystem callbacks)
        {
            this.callbacks = callbacks;
            insteon = new Insteon.FluffInsteon(config.ComPort, this);
            insteon.DeviceAdded += insteon_DeviceAdded;
            insteon.DeviceChanged += insteon_DeviceChanged;
            return insteon.Startup();
        }

        void insteon_DeviceChanged(object sender, DeviceChangedEventArgs args)
        {
            DeviceId id = args.State.Device.Address;
            DeviceReference reference = new DeviceReference(id.ToString(), PluginName);
            BasicDevice device = callbacks.Devices.FindDevice(reference);
            if (device != null)
            {
                InsteonDeviceData deviceData = (InsteonDeviceData)device.AutomationData;
                if (args.State.Device is DimmingLight)
                {
                    DimmingLight onOffDevice = (DimmingLight)args.State.Device;
                    FroggyPlugin.Devices.PercentLevelDevice basicDevice = (FroggyPlugin.Devices.PercentLevelDevice)callbacks.Devices.FindDevice(reference);

                    // Update the value, this will automatically propogate events everywhere (even back to this plugin).
                    basicDevice.Level = onOffDevice.OnLevel * 100 / 255;
                }
                else if (args.State.Device is OnOffDevice)
                {
                    OnOffDevice onOffDevice = (OnOffDevice)args.State.Device;
                    FroggyPlugin.Devices.OnOffDevice basicDevice = (FroggyPlugin.Devices.OnOffDevice)callbacks.Devices.FindDevice(reference);

                    basicDevice.On = onOffDevice.OnLevel != 0;
                }
            }
        }

        void insteon_DeviceAdded(object sender, DeviceAddedEventArgs args)
        {
            DeviceId id = args.Device.Address;
            DeviceReference reference = new DeviceReference(id.ToString(), PluginName);
            BasicDevice device = callbacks.Devices.FindDevice(reference);
            if (device != null)
            {
                InsteonDeviceData deviceData = (InsteonDeviceData)device.AutomationData;

            }
            else
            {
                InsteonDeviceData deviceData = new InsteonDeviceData(args.Device.Category, args.Device.Subcategory);
                if (args.Device is DimmingLight)
                {
                    DimmingLight dimmer = (DimmingLight)args.Device;
                    FroggyPlugin.Devices.PercentLevelDevice basic = new FroggyPlugin.Devices.PercentLevelDevice();
                    basic.Address = reference;
                    basic.Name = args.Device.DeviceName;
                    basic.Level = dimmer.OnLevel * 100 / 255;
                    basic.AutomationData = deviceData;
                    callbacks.Devices.AddDevice(basic);
                }
                else if (args.Device is OnOffDevice)
                {
                    OnOffDevice onOffDevice = (OnOffDevice)args.Device;
                    FroggyPlugin.Devices.OnOffDevice basic = new FroggyPlugin.Devices.OnOffDevice();
                    basic.Address = reference;
                    basic.Name = args.Device.DeviceName;
                    basic.On = onOffDevice.OnLevel != 0;
                    basic.AutomationData = deviceData;
                    callbacks.Devices.AddDevice(basic);
                }
            }
        }

        /// <summary>
        /// Do any device updates based on this update.
        /// </summary>
        /// <param name="update">The update to apply</param>
        public void UpdateDevice(FroggyPlugin.Events.DeviceUpdate update)
        {
            DeviceId id = new DeviceId(update.Device.Address.Address);
            DeviceBase device;
            if (insteon.FindDevice(id, out device))
            {
                switch (update.Type)
                {
                    case FroggyPlugin.Events.DeviceUpdate.UpdateType.State:
                        if (device is DimmingLight)
                        {
                            DimmingLight dimmer = (DimmingLight)device;
                            DeviceState<int> percentLevel = (DeviceState<int>)update.State;
                            dimmer.OnLevel = (percentLevel.Value * 255 / 100);
                        }
                        else if (device is OnOffDevice)
                        {
                            OnOffDevice onOffdevice = (OnOffDevice)device;
                            DeviceState<bool> boolUpdate = (DeviceState<bool>)update.State;
                            onOffdevice.OnLevel = boolUpdate.Value ? 255 : 0;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// The name of the plugin, this should be used in all devices to the know to reference back here.
        /// </summary>
        public string PluginName
        {
            get { return "Insteon"; }
        }

        /// <summary>
        /// The insteon configuration.
        /// </summary>
        [Serializable]
        public class InsteonConfiguration : PluginConfig
        {
            /// <summary>
            /// The communication port to use for the insteon system.
            /// </summary>
            public string ComPort { get; set; }

            /// <summary>
            /// The display name for this plugin.
            /// </summary>
            public override string DisplayName
            {
                get { return "Insteon Plugin"; }
            }

            /// <summary>
            /// The name to use in ll references for addresses.
            /// </summary>
            public override string Name
            {
                get { return "Inteon"; }
            }
        }

        /// <summary>
        /// Data to put on all the insteron objects to track things.
        /// </summary>
        [Serializable]
        public class InsteonDeviceData
        {
            /// <summary>
            /// The device data associated with this device.
            /// </summary>
            /// <param name="category">category of the device</param>
            /// <param name="subcategory">subcategory of the device</param>
            public InsteonDeviceData(byte category, byte subcategory)
            {
                Category = category;
                SubCategory = subcategory;
            }

            /// <summary>
            /// The category of the device.
            /// </summary>
            public byte Category { get; set; }
            /// <summary>
            /// The sub category of the device.
            /// </summary>
            public byte SubCategory { get; set; }
        }

        /// <summary>
        /// Disposes the system, but turning off insteon and everything else.
        /// </summary>
        public void Dispose()
        {
            insteon.Dispose();
            insteon = null;
        }

        /// <summary>
        /// Finds the device info off the insteon thingy in the basic system.
        /// </summary>
        /// <param name="id">The device id to lookup</param>
        /// <returns>The category, or null if none known</returns>
        public Tuple<byte, byte> GetDeviceCategory(DeviceId id)
        {
            DeviceReference reference = new DeviceReference(id.ToString(), PluginName);
            BasicDevice device = callbacks.Devices.FindDevice(reference);
            if (device != null)
            {
                InsteonDeviceData deviceData = (InsteonDeviceData)device.AutomationData;
                return Tuple.Create(deviceData.Category, deviceData.SubCategory);
            }
            return null;
        }
    }
}
