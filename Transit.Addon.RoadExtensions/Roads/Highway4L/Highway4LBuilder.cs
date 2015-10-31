using System;
using System.Linq;
using Transit.Framework;
using UnityEngine;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.Highway4L
{
    public partial class Highway4LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 49; } }
        public int UIOrder { get { return 14; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.HIGHWAY_3L; } }
        public string Name { get { return "Four-Lane Highway"; } }
        public string DisplayName { get { return "Four-Lane Highway"; } }
        public string Description { get { return "A four-lane, one-way road suitable for very high and dense traffic between metropolitan areas. Lanes going the opposite direction need to be built separately. Highway does not allow zoning next to it!"; } }
        public string UICategory { get { return "RoadsHighway"; } }

        public string ThumbnailsPath { get { return @"Roads\Highway4L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highway4L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup24mMesh(version);


            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY4L);
            info.m_surfaceLevel = 0;
            info.m_createPavement = !(version == NetInfoVersion.Ground || version == NetInfoVersion.Tunnel);
            info.m_createGravel = version == NetInfoVersion.Ground;
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;
            info.m_halfWidth = (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated) ? 11 : 12;
            info.m_pavementWidth = 2;
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
            }


            ///////////////////////////
            // Set up lanes          //
            ///////////////////////////
            //info.DisableHighwayParkingsAndPeds();
            var leftHwLane = info.SetHighwayLeftShoulder(highwayInfo, version);
            var rightHwLane = info.SetHighwayRightShoulder(highwayInfo, version);
            var vehicleLanes = info.SetHighwayVehicleLanes(1);


            ///////////////////////////
            // Set up props          //
            ///////////////////////////
            var leftHwLaneProps = leftHwLane.m_laneProps.m_props.ToList();
            var rightHwLaneProps = rightHwLane.m_laneProps.m_props.ToList();

            if (version == NetInfoVersion.Slope)
            {
                leftHwLaneProps.SetHighwaySignsSlope();
                rightHwLaneProps.SetHighwaySignsSlope();
            }

            // Lightning
            rightHwLaneProps.SetLights(version);
            if (version == NetInfoVersion.Slope)
            {
                leftHwLaneProps.AddLeftWallLights(1);
                rightHwLaneProps.AddRightWallLights(-1);
            }

            leftHwLane.m_laneProps.m_props = leftHwLaneProps.ToArray();
            rightHwLane.m_laneProps.m_props = rightHwLaneProps.ToArray();

            info.TrimNonHighwayProps();
        }
    }
}