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
using Alchemy.Classes;
using FroggyAutomation.Messages;
using FroggyPlugin.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FroggyAutomation
{
    /// <summary>
    /// Deals with all the updates to devices, from the automation systems and from
    /// the frontend.
    /// </summary>
    public class DeviceHandler
    {
        private readonly Database.Database db;
        private readonly WebSocketChannel channel;

        /// <summary>
        /// The device handler to deal with all updates to devices.
        /// </summary>
        /// <param name="db">The database to use for updates</param>
        public DeviceHandler(Database.Database db, WebSocketChannel channel)
        {
            this.db = db;
            this.channel = channel;
            channel.AddCallback(typeof(DeviceList), doDeviceList);
            channel.AddCallback(typeof(SetDeviceState), doSetDeviceState);
            channel.AddCallback(typeof(SetDeviceName), doSetDeviceName);
        }

        private void doSetDeviceState(UserContext context, object data)
        {
            // Send this message into the plugins to find the right system to address.

        }

        /// <summary>
        /// Sets the device name.
        /// </summary>
        /// <param name="context">The context for the call</param>
        /// <param name="data">The set device name data</param>
        private void doSetDeviceName(UserContext context, object data)
        {
            SetDeviceName name = (SetDeviceName)data;
            BasicDevice device = db.FindDevice(name.Device);
            device.Name = name.NewName;
            db.AddOrUpdateDevice(device);
        }

        /// <summary>
        /// Called when the device list request is called.
        /// </summary>
        /// <param name="state"></param>
        private void doDeviceList(UserContext context, object state)
        {
            DeviceList list = new DeviceList();
            list.Devices = new List<BasicDevice>(db.Devices);
            context.Send(JsonConvert.SerializeObject(list));
        }
    }
}
