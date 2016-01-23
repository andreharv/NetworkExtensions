using System;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.Alley2L;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.OneWay1L;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.AI
{
    public partial class RExRoadAI : RoadAI
    {
        [RedirectFrom(typeof(RoadAI))]
        private void CreateZoneBlocks(ushort segment, ref NetSegment data)
        {
            try
            {
                switch (this.m_info.name)
                {
                    case Alley2LBuilder.NAME:
                    case OneWay1LBuilder.NAME:
                        CreateZoneBlocksTiny(this.m_info, segment, ref data);
                        break;

                    default:
                        CreateZoneBlocksVanilla(this.m_info, segment, ref data);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("REx: Crashed-CreateZoneBlocks");
                Debug.Log("REx: " + ex.Message);
                Debug.Log("REx: " + ex.ToString());
            }
        }
    }
}