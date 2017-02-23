using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FroggyPlugin;

namespace DummyDevices
{
    /// <summary>
    /// The config for dummy devices.
    /// </summary>
    class DummyDeviceConfig : PluginConfig
    {
        public override string DisplayName
        {
            get { return "Dummy Device Plugin"; }
        }

        public override string Name
        {
            get { return "DummyDevice"; }
        }
    }
}
