using System.Reflection;
using ColossalFramework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using UnityEngine;

namespace Transit.Framework
{
    public static class PrefabInfoExtensions
    {
        public static T Clone<T>(this T originalPrefabInfo, string newName, Transform parentTransform)
            where T : PrefabInfo
        {
            var instance = Object.Instantiate(originalPrefabInfo.gameObject);
            instance.name = newName;
            //instance.transform.SetParent(parentTransform);
            instance.transform.localPosition = new Vector3(-7500, -7500, -7500);
            var newPrefab = instance.GetComponent<T>();
            instance.SetActive(false);
            newPrefab.m_prefabInitialized = false;
            return newPrefab;
        }
        public static T Clone<T>(this T originalPrefabInfo, string newName)
    where T : PrefabInfo
        {
            var gameObject = Object.Instantiate(originalPrefabInfo.gameObject);
            //gameObject.transform.SetParent(originalPrefabInfo.gameObject.transform);
            //ameObject.transform.parent = originalPrefabInfo.gameObject.transform; // N.B. This line is evil and removing it is killoing the game's performances
            gameObject.name = newName;

            var info = gameObject.GetComponent<T>();
            info.m_prefabInitialized = false;

            return info;
        }

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
