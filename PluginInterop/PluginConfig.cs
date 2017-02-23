using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FroggyPlugin
{
    /// <summary>
    /// Basic plugin config.  Plugins should extend this to add in extra data.
    /// </summary>
    [Serializable]
    public abstract class PluginConfig
    {
        /// <summary>
        /// Name to display when talking about the plugin.
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// The name of the plugin, this is used to mark the objects not to display.
        /// </summary>
        public abstract string Name { get; }
    }
}
