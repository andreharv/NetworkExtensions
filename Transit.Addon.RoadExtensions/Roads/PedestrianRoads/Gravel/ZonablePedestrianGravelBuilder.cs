using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Gravel
{
    public partial class ZonablePedestrianGravelBuilder :Activable, INetInfoBuilderPart, INetInfoLateBuilder
    {
        public int Order { get { return 300; } }
        public int UIOrder { get { return 10; } }

        public string BasedPrefabName { get { return ZPBB.GetBasedPrefabName(); } }
        public string UICategory { get { return ZPBB.GetUICategory(); } }

        public const string NAME = "Zonable Pedestrian Gravel";
        public string Name { get { return NAME; } }
        public string DisplayName { get { return "[BETA] Zonable Pedestrian Gravel Road"; } }
        public string Description { get { return "Gravel roads allow pedestrians to walk fast and easy."; } }
        public string ShortDescription { get { return "Parking, zoneable, restricted traffic [Traffic++ V2 required]"; } }

        public string ThumbnailsPath { get { return @"Roads\PedestrianRoads\Gravel\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\PedestrianRoads\Gravel\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.Elevated; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_createGravel = true;
            info.m_createPavement = false;
            ZPBB.SetInfo(info, version, false);
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            info.Setup8mNoSwWoodMesh(version);
            SetupTextures(info, version);

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
            var bollardName = "WoodBollard";
            var bollardInfo = PrefabCollection<PropInfo>.FindLoaded($"478820060.{bollardName}_Data");
            if (bollardInfo == null)
            {
                bollardInfo = PrefabCollection<PropInfo>.FindLoaded($"{bollardName}.{bollardName}_Data");
            }

            BuildingInfo pillarInfo = null;
            if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
            {
                var pillarName = "Wood8mEPillar";
                pillarInfo = PrefabCollection<BuildingInfo>.FindLoaded($"478820060.{pillarName}_Data");
                if (pillarInfo == null)
                {
                    pillarInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{pillarName}.{pillarName}_Data");
                }
            }
            ZPBB.LateBuildUpInfo(info, version, bollardInfo, pillarInfo );
        }
    }
}