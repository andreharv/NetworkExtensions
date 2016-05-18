

using System;
using Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Pavement
{
    public partial class ZonablePedestrianPavementBuilder : Activable, INetInfoBuilderPart, INetInfoLateBuilder
    {
        public int Order { get { return 310; } }
        public int UIOrder { get { return 20; } }

        public string BasedPrefabName { get { return ZPBB.GetBasedPrefabName(); } }
        public string UICategory { get { return ZPBB.GetUICategory(); } }
        public const string NAME = NetInfos.New.ZONEABLE_PED_PAVEMENT;
        public string Name { get { return NAME; } }
        public string DisplayName { get { return "[BETA] Zonable Pedestrian Concrete Road"; } }
        public string Description { get { return "Paved roads are nicer to walk on than gravel."; } }
        public string ShortDescription { get { return "Parking, zoneable, restricted traffic [Traffic++ V2 required]"; } }
    
        public string ThumbnailsPath { get { return @"Roads\PedestrianRoads\Pavement\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\PedestrianRoads\Pavement\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.Elevated; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_createGravel = false;
            info.m_createPavement = true;
            ZPBB.SetInfo(info, version);
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                ZPBBTexture.SetNakedGroundTexture(info, version);
            }
            else
            {
                SetupTextures(info, version);
            }
            ///////////////////////////
            // AI                    //
            ///////////////////////////
            var pedestrianVanilla = Prefabs.Find<NetInfo>(NetInfos.Vanilla.PED_PAVEMENT);
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var vanillaplayerNetAI = pedestrianVanilla.GetComponent<PlayerNetAI>();
                        var playerNetAI = info.GetComponent<PlayerNetAI>();

                        if (playerNetAI != null)
                        {
                            playerNetAI.m_constructionCost = vanillaplayerNetAI.m_constructionCost * 2;
                            playerNetAI.m_maintenanceCost = vanillaplayerNetAI.m_maintenanceCost * 2;
                        }
                    }
                    break;
            }
        }

        public void LateBuildUp(NetInfo info, NetInfoVersion version)
        {
            var bollardName = "RetractBollard";
            var bollardInfo = PrefabCollection<PropInfo>.FindLoaded($"478820060.{bollardName}_Data");
            if (bollardInfo == null)
            {
                bollardInfo = PrefabCollection<PropInfo>.FindLoaded($"{bollardName}.{bollardName}_Data");
            }
            ZPBB.LateBuildUpInfo(info, version, bollardInfo);
        }
    }
}
