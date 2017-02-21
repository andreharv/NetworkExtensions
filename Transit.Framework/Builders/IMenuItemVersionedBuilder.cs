using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transit.Framework.Builders
{
    public interface IMenuItemVersionedBuilder:IMenuItemBuilder
    {
        NetInfoVersion DefaultVersion { get; set; }
    }
}
