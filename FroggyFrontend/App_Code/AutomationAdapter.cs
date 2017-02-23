using Plugin.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace FroggyFrontend.App_Code
{
    public class AutomationAdapter
    {
        public AutomationAdapter()
        {
        }

        public IList<BasicDevice> Devices { get; set; }
    }
}