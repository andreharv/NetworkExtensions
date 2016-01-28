using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using Transit.Framework.Extenders.AI;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Tools
{
    public class ZoneBlocksOffsetModifier : ThreadingExtensionBase
    {
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && 
                (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
            {
                if (Input.GetKeyDown(KeyCode.O))
                {
                    // CTRL + ALT + O to toggle HalfCell zonings
                    switch (ZoneBlocksOffset.Mode)
                    {
                        case ZoneBlocksOffsetMode.Default:
                            ZoneBlocksOffset.Mode = ZoneBlocksOffsetMode.HalfCell;
                            break;
                        case ZoneBlocksOffsetMode.HalfCell:
                            ZoneBlocksOffset.Mode = ZoneBlocksOffsetMode.Default;
                            break;
                    }
                    Debug.Log("REx: Changed ZoneBlocksOffset for " + ZoneBlocksOffset.Mode);
                }
            }
        }
    }
}
