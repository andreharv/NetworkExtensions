using System;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.AI
{
    public partial class RExRoadAI : RoadAI
    {
        [RedirectFrom(typeof(RoadAI))]
        private void CreateZoneBlocks(ushort segment, ref NetSegment data)
        {
            Debug.Log(">>>> REx: Redirection is on");

            try
            {
                CreateZoneBlocksNew(this.m_info, segment, ref data);
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