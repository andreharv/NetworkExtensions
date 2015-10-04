using System.Collections.Generic;
using ColossalFramework;
using Transit.Framework.Modularity;

namespace Transit.Framework
{
    public static class NetInfoBuilderExtensions
    {
        public static IEnumerable<NetInfo> Build(this INetInfoBuilder builder)
        {
            var newNetInfos = new List<NetInfo>();


            // Ground version--------------------------------------------------
            var mainInfo = builder.BuildVersion(NetInfoVersion.Ground, newNetInfos);
            mainInfo.m_UIPriority = builder.Priority;

            if (!builder.CodeName.IsNullOrWhiteSpace() && !builder.ThumbnailsPath.IsNullOrWhiteSpace())
            {
                var thumbnails = AssetManager.instance.GetThumbnails(builder.CodeName, builder.ThumbnailsPath);
                mainInfo.m_Atlas = thumbnails;
                mainInfo.m_Thumbnail = thumbnails.name;
            }

            if (!builder.CodeName.IsNullOrWhiteSpace() && !builder.InfoTooltipPath.IsNullOrWhiteSpace())
            {
                var infoTips = AssetManager.instance.GetInfoTooltip(builder.CodeName, builder.InfoTooltipPath);
                mainInfo.m_InfoTooltipAtlas = infoTips;
                mainInfo.m_InfoTooltipThumbnail = infoTips.name;
            }


            // Other versions -------------------------------------------------
            var mainInfoAI = mainInfo.GetComponent<RoadAI>();

            mainInfoAI.m_elevatedInfo = builder.BuildVersion(NetInfoVersion.Elevated, newNetInfos);
            mainInfoAI.m_bridgeInfo = builder.BuildVersion(NetInfoVersion.Bridge, newNetInfos);
            mainInfoAI.m_tunnelInfo = builder.BuildVersion(NetInfoVersion.Tunnel, newNetInfos);
            mainInfoAI.m_slopeInfo = builder.BuildVersion(NetInfoVersion.Slope, newNetInfos);

            return newNetInfos;
        }

        private static NetInfo BuildVersion(this INetInfoBuilder builder, NetInfoVersion version, ICollection<NetInfo> holdingCollection)
        {
            if (builder.SupportedVersions.HasFlag(version))
            {
                var completePrefabName = NetInfos.Vanilla.GetPrefabName(builder.TemplatePrefabName, version);
                var completeName = builder.GetNewName(version);

                var info = Prefabs
                    .Find<NetInfo>(completePrefabName)
                    .Clone(completeName);

                info.SetUICategory(builder.UICategory);
                builder.BuildUp(info, version);

                holdingCollection.Add(info);

                return info;
            }
            else
            {
                return null;
            }
        }

        private static string GetNewName(this INetInfoBuilder builder, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    return builder.Name;

                default:
                    return builder.Name + " " + version;
            }
        }
    }
}
