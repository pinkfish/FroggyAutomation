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
using Automation.Insteon.Devices;
using Automation.Insteon.Messages;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestInsteon
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program).Name);

        static void Main(string[] args)
        {
            /*
            Plm fluff = new Plm("COM4");
            var database = fluff.GetAllLinkDatabase();
            foreach (var record in database.Records)
            {
                // You can attempt to connect to each device
                // to figure out what it is:
                DeviceBase device;
                if (fluff.Network
                    .TryConnectToDevice(record.DeviceId, out device))
                {
                    // It responded.  You can get identification info like this:
                    string category = device.DeviceCategory;
                    string subcategory = device.DeviceSubcategory;
                }
                else
                {
                    // couldn't connect - device may have been removed?
                }
            }
             */

            FluffInsteon insteon = new FluffInsteon("COM4", null);
            insteon.DeviceAdded += insteon_DeviceAdded;
            insteon.Startup();
            while (true)
            {
                Thread.Sleep(10000);
            }
            System.Console.WriteLine("Frog");
        }

        static void insteon_DeviceAdded(object sender, DeviceAddedEventArgs args)
        {
            if (args.Device is OnOffDevice)
            {
                OnOffDevice dimmer = (OnOffDevice)args.Device;
                log.InfoFormat("{0} OnLevel {1}", dimmer.DeviceName, dimmer.OnLevel);
            }
        }
    }
}
        
        
