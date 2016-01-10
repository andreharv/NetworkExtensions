using System;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.AI
{
    public partial class RExRoadBaseAI : RoadBaseAI
    {
        private void CreateZoneBlocks(ushort segment, ref NetSegment data)
        {
            try
            {
                switch (this.m_info.name)
                {
                    case "Two-Lane Alley No Zoning":
                        CreateZoneBlocksNew(this.m_info, segment, ref data);
                        break;

                    default:
                        CreateZoneBlocksOriginal(this.m_info, segment, ref data);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("NExt: Crashed-CreateZoneBlocks");
                Debug.Log("NExt: " + ex.Message);
                Debug.Log("NExt: " + ex.ToString());
            }
        }
    }
}