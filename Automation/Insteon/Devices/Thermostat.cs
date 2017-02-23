using FluffInsteon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluffInsteon.Devices
{
    /// <summary>
    /// Thermostat.
    /// </summary>
    public class Thermostat : DeviceBase
    {
        public enum ZoneInformation
        {
            Temperature = 0,
            SetPoint = 1,
            Deadband = 2,
            Humidity = 3
        }

        private enum ControlMode
        {
            LoadInitializationValues = 0,
            LoadEEPROMFromRam = 1,
            GetThermostatMode = 2,
            GetAmbientTemperature = 3,
            SetModeToHeat = 4,
            SetModeToCool = 5,
            SetModeToAuto = 6,
            TurnFanOn = 7,
            TurnFanOff = 8,
            TurnAllOff = 9,
            ProgramHeat = 0xA,
            ProgramCool = 0xB,
            ProgramAuto = 0xC,
            GetEquipmentState = 0xD,
            SetEquipmentState = 0xE,
            GetTemperatureUnits = 0xF,
            SetFarenheit = 0x10,
            SetCelsuius = 0x11,
            GetFanOnSpeed = 0x12,
            SetFanOnSpeedLow = 0x13,
            SetFanOnSpeedMedium = 0x14,
            SetFanOnSPeedHigh = 0x15,
            EnableStatusChangeMessage = 0x16,
            DisableStatusChangeMessage = 0x17
        }

        public enum ThermostatMode
        {
            Off,
            Heat,
            Cool,
            Auto
            Fan,
            Program,
            ProgramHeat,
            ProgramCool
        }

        public Thermostat(InsteonCommunication coms, DeviceId id, byte category, byte subcategory)
            : base(coms, id, category, subcategory)
        {
        }

        /// <summary>
        /// Turn the temperature up.
        /// </summary>
        /// <param name="changeBy">The temperature to change by</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse TurnTemperatureUp(byte changeBy)
        {
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.ThermostatTemperatureUp, changeBy);
            return ret;
        }

        /// <summary>
        /// Turn the temperature down.
        /// </summary>
        /// <param name="changeBy">The temperature to change by</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse TurnTemperatureDown(byte changeBy)
        {
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.ThermostatTemperatureDown, changeBy);
            return ret;
        }

        /// <summary>
        /// Set cool setpoint.
        /// </summary>
        /// <param name="temp">The temperature to set</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse SetCoolSetpoint(byte temp)
        {
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.ThermostatSetCoolSetpoint, temp);
            return ret;
        }

        /// <summary>
        /// Set heat setpoint.
        /// </summary>
        /// <param name="temp">The temperature to set</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse SetHeatSetpoint(byte temp)
        {
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.ThermostatSetHeatSetpoint, temp);
            return ret;
        }

        /// <summary>
        /// Get zone information.
        /// </summary>
        /// <param name="changeBy">The temperature to change by</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse GetZoneInformation(byte zone, ZoneInformation info, out byte data)
        {
            byte control = (byte)((zone & 0xf) | ((byte)info << 4));

            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommandWithResponse(InsteonPacket.Command.ThermostatTemperatureDown, control, out data);
            return ret;
        }

        /// <summary>
        /// Gets the mode of the thermostat.
        /// </summary>
        /// <param name="mode">The current mode of the thermostat</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse GetThermostatMode(out ThermostatMode mode) {
            byte result;

            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommandWithResponse(InsteonPacket.Command.ThermostatControl, (byte)ControlMode.GetThermostatMode, out result);
            mode = (ThermostatMode)result;
            return ret;
        }

        /// <summary>
        /// Sets the thermostat mode.
        /// </summary>
        /// <param name="mode">The mode of the thermostat</param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse SetThermostatMode(ThermostatMode mode) {
            ControlMode control;

            switch (mode){
                case ThermostatMode.Program:
                    control = ControlMode.ProgramAuto;
                    break;
                case ThermostatMode.ProgramCool:
                    control = ControlMode.ProgramCool;
                    break;
                case ThermostatMode.ProgramHeat:
                    control = ControlMode.ProgramHeat;
                    break;
                case ThermostatMode.Cool:
                    control = ControlMode.SetModeToCool;
                    break;
                case ThermostatMode.Heat:
                    control = ControlMode.SetModeToHeat;
                    break;
                case ThermostatMode.Auto:
                    control = ControlMode.SetModeToAuto;
                    break;
                case ThermostatMode.Fan:
                    control =ControlMode.TurnFanOn;
                    break;
                case ThermostatMode.Off:
                    control = ControlMode.TurnAllOff;
                    break;
                default:
                    return Messages.PowerLineModemMessage.MessageResponse.Invalid;
            }
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendStandardCommand(InsteonPacket.Command.ThermostatControl, (byte)control);
            return ret;
        }
 
        /// <summary>
        /// Set the cool setpoint.
        /// </summary>
        /// <param name="zoneNumber"></param>
        /// <param name="temperature"></param>
        /// <param name="deadband"></param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse SetThermostatCoolZoneSetpoint(byte zoneNumber, int temperature, int deadband) {
            byte[] data = new byte[14];
            data[0] = (byte)(temperature * 2);
            data[1] = (byte)(deadband * 2);
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommand(InsteonPacket.ExtendedCommand.ThermostatSetZoneCoolSetpoint, zoneNumber, data);
            return ret;
        }
 
        /// <summary>
        /// Set the heat setpoint.
        /// </summary>
        /// <param name="zoneNumber"></param>
        /// <param name="temperature"></param>
        /// <param name="deadband"></param>
        /// <returns></returns>
        public Messages.PowerLineModemMessage.MessageResponse SetThermostatHeatZoneSetpoint(byte zoneNumber, int temperature, int deadband) {
            byte[] data = new byte[14];
            data[0] = (byte)(temperature * 2);
            data[1] = (byte)(deadband * 2);
            Messages.PowerLineModemMessage.MessageResponse ret = this.SendExtendedCommand(InsteonPacket.ExtendedCommand.ThemostatSetZoneHeatSetpoint, zoneNumber, data);
            return ret;
        }
    }
}
