using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transit.Framework.Builders
{
    public class MenuItemVersionedBuilder : MenuItemBuilder, IMenuItemVersionedBuilder
    {
        public NetInfoVersion DefaultVersion { get; set; }
    }
}
