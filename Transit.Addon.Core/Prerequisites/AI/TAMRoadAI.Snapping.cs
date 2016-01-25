using System;
using ColossalFramework;
using ColossalFramework.Math;
using Transit.Addon.Core.Extenders.AI;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace Transit.Addon.Core.Prerequisites.AI
{
    public partial class TAMRoadAI : RoadAI
    {
        [RedirectFrom(typeof(RoadAI))]
        public override float GetLengthSnap()
        {
            if (RoadSnappingModeProvider.instance.HasCustomSnapping(this.m_info.name))
            {
                return RoadSnappingModeProvider
                    .instance
                    .GetCustomSnapping(this.m_info.name)
                    .GetLengthSnap();
            }
            else
            {
                return (!this.m_enableZoning) ? 0f : 8f;
            }
        }
    }
}