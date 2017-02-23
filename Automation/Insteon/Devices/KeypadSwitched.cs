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
    /// <summary>
    /// A keypad device that is switched.
    /// </summary>
    class KeypadSwitched : SwitchedLight
    {
        private enum ExtendedGetSetCommands
        {
            DataRequest = 0,
            DataResponse = 1,
            SetLedFOllowMask = 2,
            SetLedOffMask = 3,
            SetX10AddressForButton = 4,
            SetRampRateForButton = 5,
            SetOnLevelForButton = 6,
            SetGlobalLedBrightness = 7,
            SetNonToggleStateForButton = 8,
            SetLedStateForButton = 9,
            SetX10AllOnStateForButton = 0xa,
            SetNonToggleOnOffStateForButton = 0xB,
            SetTriggerAllLinkStateForButton = 0xC
        }
        private int numKeys = 6;

        /// <summary>
        /// Create a new switched keypad device.
        /// </summary>
        /// <param name="coms"></param>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <param name="subcategory"></param>
        public KeypadSwitched(InsteonCommunication coms, DeviceId id, byte category, byte subcategory)
            : base(coms, id, category, subcategory)
        {
            switch (subcategory)
            {
                case 0x5:
                    numKeys = 8;
                    break;
                case 0x0f:
                case 0x1e:
                    numKeys = 6;
                    break;
            }
        }

        /// <summary>
        /// Gets the button state for the device.
        /// </summary>
        /// <param name="buttonNumber">Button number to get the state for</param>
        /// <param name="state">The button state</param>
        /// <returns>the status of the send message</returns>
        public Messages.PowerLineModemMessage.MessageResponse GetButtonState(byte buttonNumber, out ButtonState state)
        {
            byte[] data = new byte[14];
            data[0] = buttonNumber;
            data[1] = (byte)ExtendedGetSetCommands.DataRequest;
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommandWithResponse(InsteonPacket.ExtendedCommand.ExtendedGetSet, 0, data, out response);
            if (ret == Messages.PowerLineModemMessage.MessageResponse.Ack)
            {
                data = response.UserData;
                state = new ButtonState();
                state.FollowMask = data[2];
                state.OffMask = data[3];
                state.X10HouseCode = data[4];
                state.X10UnitCode = data[5];
                state.RampRate = data[6];
                state.OnLevel = data[7];
                state.GlobalLEDBrightness = data[8];
                state.Toggle = data[9] != 0;
                state.LedState = data[10] != 0;
                state.X10AllOnOff = data[11] != 0;
                state.ButtonNonToggleSendsOn = data[12] != 0;
                state.TriggerAllLinkCommand = data[13] != 0;
            }
            else
            {
                state = null;
            }
            return ret;
        }

        /// <summary>
        /// Sets if the button follows the led state.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="buttonLedFollows"></param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse SetLedFollowMaskForButton(byte button, bool buttonLedFollows)
        {
            byte[] data = new byte[14];
            data[0] = button;
            data[1] = (byte)ExtendedGetSetCommands.SetLedFOllowMask;
            data[2] = (byte)(buttonLedFollows ? 0x00 : 0xff);
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommandWithResponse(InsteonPacket.ExtendedCommand.ExtendedGetSet, 0, data, out response);
            return ret;
        }

        public Messages.PowerLineModemMessage.MessageResponse SetLedOffMaskForButton(byte button, bool ledOffMask)
        {
            byte[] data = new byte[14];
            data[0] = button;
            data[1] = (byte)ExtendedGetSetCommands.SetLedOffMask;
            data[2] = (byte)(ledOffMask ? 0x00 : 0xff);
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommandWithResponse(InsteonPacket.ExtendedCommand.ExtendedGetSet, 0, data, out response);
            return ret;
        }

        public Messages.PowerLineModemMessage.MessageResponse SetNonToggleStateForButton(byte button, bool nonToggle)
        {
            byte[] data = new byte[14];
            data[0] = button;
            data[1] = (byte)ExtendedGetSetCommands.SetNonToggleStateForButton;
            data[2] = (byte)(nonToggle ? 0x00 : 0xff);
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommandWithResponse(InsteonPacket.ExtendedCommand.ExtendedGetSet, 0, data, out response);
            return ret;
        }

        public Messages.PowerLineModemMessage.MessageResponse SetLedStateForButton(byte button, bool ledState)
        {
            byte[] data = new byte[14];
            data[0] = button;
            data[1] = (byte)ExtendedGetSetCommands.SetLedStateForButton;
            data[2] = (byte)(ledState ? 0x00 : 0xff);
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommandWithResponse(InsteonPacket.ExtendedCommand.ExtendedGetSet, 0, data, out response);
            return ret;
        }

        public Messages.PowerLineModemMessage.MessageResponse SetNonToggleOnOffStateForButton(byte button, bool nonToggleOnOffState)
        {
            byte[] data = new byte[14];
            data[0] = button;
            data[1] = (byte)ExtendedGetSetCommands.SetNonToggleOnOffStateForButton;
            data[2] = (byte)(nonToggleOnOffState ? 0x00 : 0xff);
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommandWithResponse(InsteonPacket.ExtendedCommand.ExtendedGetSet, 0, data, out response);
            return ret;
        }

        public Messages.PowerLineModemMessage.MessageResponse SetTriggerAllLinkStateForButton(byte button, bool triggerAllLinkStateForButton)
        {
            byte[] data = new byte[14];
            data[0] = button;
            data[1] = (byte)ExtendedGetSetCommands.SetTriggerAllLinkStateForButton;
            data[2] = (byte)(triggerAllLinkStateForButton ? 0x00 : 0xff);
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommandWithResponse(InsteonPacket.ExtendedCommand.ExtendedGetSet, 0, data, out response);
            return ret;
        }

        public Messages.PowerLineModemMessage.MessageResponse SetX10AddressForButton(byte button, byte x10HouseCOde, byte x10UnitCode)
        {
            byte[] data = new byte[14];
            data[0] = button;
            data[1] = (byte)ExtendedGetSetCommands.SetTriggerAllLinkStateForButton;
            data[2] = x10HouseCOde;
            data[3] = x10UnitCode;
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommandWithResponse(InsteonPacket.ExtendedCommand.ExtendedGetSet, 0, data, out response);
            return ret;
        }

        public Messages.PowerLineModemMessage.MessageResponse SetRampRateForButton(byte button, byte rampRate)
        {
            byte[] data = new byte[14];
            data[0] = button;
            data[1] = (byte)ExtendedGetSetCommands.SetTriggerAllLinkStateForButton;
            data[2] = rampRate;
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommandWithResponse(InsteonPacket.ExtendedCommand.ExtendedGetSet, 0, data, out response);
            return ret;
        }

        public Messages.PowerLineModemMessage.MessageResponse SetOnLevelForButton(byte button, byte onLevel)
        {
            byte[] data = new byte[14];
            data[0] = button;
            data[1] = (byte)ExtendedGetSetCommands.SetTriggerAllLinkStateForButton;
            data[2] = onLevel;
            InsteonPacket response;
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommandWithResponse(InsteonPacket.ExtendedCommand.ExtendedGetSet, 0, data, out response);
            return ret;
        }

        /// <summary>
        /// Captures the button state.
        /// </summary>
        public class ButtonState
        {
            /// <summary>
            /// LED Follow mask.
            /// </summary>
            public byte FollowMask { get; set; }
            /// <summary>
            /// LED Off mask.
            /// </summary>
            public byte OffMask { get; set; }
            /// <summary>
            /// Button's x10 house code
            /// </summary>
            public byte X10HouseCode { get; set; }
            /// <summary>
            /// BUtton's x10 unit code.
            /// </summary>
            public byte X10UnitCode { get; set; }
            /// <summary>
            /// Ramp rate for the button.
            /// </summary>
            public byte RampRate { get; set; }
            /// <summary>
            /// Button's on level.
            /// </summary>
            public byte OnLevel { get; set; }
            /// <summary>
            /// Global led brightness.
            /// </summary>
            public byte GlobalLEDBrightness { get; set; }
            /// <summary>
            /// If the button will or won't toggle.
            /// </summary>
            public bool Toggle { get; set; }
            /// <summary>
            /// The current state of the led.
            /// </summary>
            public bool LedState { get; set; }
            /// <summary>
            /// If this button triggers an x10- all on/off command.
            /// </summary>
            public bool X10AllOnOff { get; set; }
            /// <summary>
            /// Button is toggle and sends on, or off, on press.
            /// </summary>
            public bool ButtonNonToggleSendsOn { get; set; }
            /// <summary>
            /// Button triggers an all link command.
            /// </summary>
            public bool TriggerAllLinkCommand { get; set; }

            public ButtonState()
            {
            }
        }
    }
}
