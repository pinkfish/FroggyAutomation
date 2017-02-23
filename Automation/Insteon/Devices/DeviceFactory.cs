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
using Automation.Insteon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Insteon.Devices
{
    public class DeviceFactory
    {
        private InsteonCommunication coms;

        #region Device Categories
        public enum DeviceCategory : byte
        {
            DimmableLightingControl = 0x1,
            SwitchedLightingControl = 0x2,
            NetworkBridges = 0x3,
            IrrigationControl = 0x4,
            ClimateControl = 0x5,
            PoolAndSpaControl = 0x6,
            SensorsAndActuators = 0x7,
            HomeEntertainment = 0x8,
            EnergyManagement = 0x9,
            BuiltInApplicanceCOntrol = 0xa,
            Plumbing = 0xb,
            Communication = 0xc,
            ComputerControl = 0xd,
            WindowCoverings = 0xe,
            AccessControl = 0xf,
            SecurityHealthSafety = 0x10,
            Surveillance = 0x11,
            Automotive = 0x12,
            PetCare = 0x13,
            Toys = 0x14,
            Timekeeping = 0x15,
            Holiday = 0x16
        }
        #endregion

        public DeviceFactory(InsteonCommunication coms)
        {
            this.coms = coms;
        }

        /// <summary>
        /// 
        /// Create a new device based on the exciting address and category details.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        public DeviceBase CreateDevice(DeviceId id, byte category, byte subCategory)
        {
            switch ((DeviceCategory)category)
            {
                case DeviceCategory.DimmableLightingControl:
                    return new DimmingLight(coms, id, category, subCategory);
                case DeviceCategory.SwitchedLightingControl:
                    switch (subCategory)
                    {
                        case 0x0F:
                        case 0x1E:
                            return new KeypadSwitched(coms, id, category, subCategory);
                        default:
                            return new SwitchedLight(coms, id, category, subCategory);
                    }
                default:
                    return new DeviceBase(coms, id, category, subCategory);
            }
        }
    }
}
