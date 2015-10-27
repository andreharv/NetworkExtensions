using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Globalization;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.MediumAvenue4L
{
    public class MediumAvenue4LBuilder : Activable, INetInfoBuilderPart, INetInfoModifier
    {
        public int Order { get { return 20; } }
        public int UIOrder { get { return 4; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "Medium Avenue"; } }
        public string DisplayName { get { return "Four-Lane Road"; } }
        public string Description { get { return "A four-lane road with parking spaces. Supports medium traffic."; } }
        public string UICategory { get { return "RoadsMedium"; } }
        
        public string ThumbnailsPath    { get { return @"Roads\MediumAvenue4L\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"Roads\MediumAvenue4L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var mediumRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.AVENUE_4L);


            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\MediumAvenue4L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\MediumAvenue4L\Textures\Ground_Segment__AlphaMap.png"),
                        new LODTexturesSet
                           (@"Roads\MediumAvenue4L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"Roads\MediumAvenue4L\Textures\Ground_SegmentLOD__AlphaMap.png",
                            @"Roads\MediumAvenue4L\Textures\Ground_SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (null,
                            @"Roads\MediumAvenue4L\Textures\Ground_Node__AlphaMap.png"));
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_class = mediumRoadInfo.m_class.Clone(NetInfoClasses.NEXT_MEDIUM_ROAD);
            info.m_UnlockMilestone = mediumRoadInfo.m_UnlockMilestone;

            // Setting up lanes
            var vehicleLaneTypes = new[]
            {
                NetInfo.LaneType.Vehicle,
                NetInfo.LaneType.PublicTransport,
                NetInfo.LaneType.CargoVehicle,
                NetInfo.LaneType.TransportVehicle
            };

            var vehicleLanes = mediumRoadInfo
                .m_lanes
                .Where(l => vehicleLaneTypes.Contains(l.m_laneType))
                .Select(l => l.ShallowClone())
                .OrderBy(l => l.m_position)
                .ToArray();

            var nonVehicleLanes = info.m_lanes
                .Where(l => !vehicleLaneTypes.Contains(l.m_laneType))
                .ToArray();

            info.m_lanes = vehicleLanes
                .Union(nonVehicleLanes)
                .ToArray();

            for (var i = 0; i < vehicleLanes.Length; i++)
            {
                var lane = vehicleLanes[i];

                switch (i)
                {
                    // Inside lane
                    case 1:
                    case 2:
                        if (lane.m_position < 0)
                        {
                            lane.m_position += 0.5f;
                        }
                        else
                        {
                            lane.m_position += -0.5f;
                        }
                        break;
                }
            }

            info.Setup50LimitProps();


            if (version == NetInfoVersion.Ground)
            {
                var mrPlayerNetAI = mediumRoadInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (mrPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = mrPlayerNetAI.m_constructionCost * 9 / 10; // 10% decrease
                    playerNetAI.m_maintenanceCost = mrPlayerNetAI.m_maintenanceCost * 9 / 10; // 10% decrease
                }

                var mrRoadBaseAI = mediumRoadInfo.GetComponent<RoadBaseAI>();
                var roadBaseAI = info.GetComponent<RoadBaseAI>();

                if (mrRoadBaseAI != null && roadBaseAI != null)
                {
                    roadBaseAI.m_noiseAccumulation = mrRoadBaseAI.m_noiseAccumulation;
                    roadBaseAI.m_noiseRadius = mrRoadBaseAI.m_noiseRadius;
                }
            }
        }

        public void ModifyExistingNetInfo()
        {
            var localizedStringsField = typeof(Locale).GetFieldByName("m_LocalizedStrings");
            var locale = SingletonLite<LocaleManager>.instance.GetLocale();
            var localizedStrings = (Dictionary<Locale.Key, string>)localizedStringsField.GetValue(locale);

            var kvp =
                localizedStrings
                .FirstOrDefault(kvpInternal =>
                    kvpInternal.Key.m_Identifier == "NET_TITLE" &&
                    kvpInternal.Key.m_Key == NetInfos.Vanilla.AVENUE_4L);

            if (!Equals(kvp, default(KeyValuePair<Locale.Key, string>)))
            {
                localizedStrings[kvp.Key] = "Four-Lane Road with Median";
            }
        }
    }
}
