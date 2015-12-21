using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public class RoadPropertyHelper
    {
        public int LanesToAdd { get; set; }
        public float PedPropOffsetX { get; set; }
        public float SpeedLimit { get; set; }
        public float LaneWidth { get; set; }
        public bool IsTwoWay { get; set; }
        public CenterLaneVersion CLVersion { get; set; }

        public RoadPropertyHelper()
        {
            LanesToAdd = 0;
            PedPropOffsetX = 0.0f;
            SpeedLimit = -1;
            LaneWidth = 3.0f;
            IsTwoWay = false;
            CLVersion = CenterLaneVersion.Default;
        }
    }
}
