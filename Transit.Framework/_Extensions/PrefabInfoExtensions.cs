using System.Reflection;
using ColossalFramework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;

namespace Transit.Framework
{
    public static class PrefabInfoExtensions
    {
        public static void SetUICategory(this PrefabInfo info, string category)
        {
            typeof(PrefabInfo).GetField("m_UICategory", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(info, category);
        }

        public static void SetMenuItemConfig(this PrefabInfo info, IMenuItemBuilder config)
        {
            info.m_UIPriority = config.UIOrder;
            info.SetUICategory(config.UICategory);

            if (!config.ThumbnailsPath.IsNullOrWhiteSpace())
            {
                var thumbnails = AssetManager.instance.GetThumbnails(config.GetCodeName(), config.ThumbnailsPath);
                info.m_Atlas = thumbnails;
                info.m_Thumbnail = thumbnails.name;
            }

            if (!config.InfoTooltipPath.IsNullOrWhiteSpace())
            {
                var infoTips = AssetManager.instance.GetInfoTooltip(config.GetCodeName(), config.InfoTooltipPath);
                info.m_InfoTooltipAtlas = infoTips;
                info.m_InfoTooltipThumbnail = infoTips.name;
            }
        }
    }
}
