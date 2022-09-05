using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.IO;
using ColossalFramework.Packaging;
using ColossalFramework.PlatformServices;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
//using static ColossalFramework.Packaging.PackageX;

namespace NetworkExtensions2.Framework
{
    internal class Saving
    {
        private static NetTool m_Tool;

        protected static NetTool Tool
        {
            get {
                if (m_Tool == null)
                    m_Tool = ToolsModifierControl.GetCurrentTool<NetTool>();
                return m_Tool; 
            }
        }

        private static Task m_PackageSaveTask;
        public static IEnumerator SaveAsset(string saveName, string assetName, string description)
        {
            Debug.Log("Checkpoint1p");
            AsyncTask task = Singleton<SimulationManager>.instance.AddAction(ThreadSaveDecoration(Tool.m_prefab));

            HashSet<BuildingInfo> pillars = null;
            Dictionary<string, NetInfo> elevations = null;
            NetInfo ni = Tool.m_prefab;
            SteamHelper.DLC_BitMask roadDLCMask = SteamHelper.DLC_BitMask.None;
            Debug.Log("Checkpoint2p");
            if (ni != null)
            {
                pillars = AssetEditorRoadUtils.GetPillars(ni);
                NetInfo netInfo = AssetEditorRoadUtils.TryGetElevated(ni);
                NetInfo netInfo2 = AssetEditorRoadUtils.TryGetBridge(ni);
                NetInfo netInfo3 = AssetEditorRoadUtils.TryGetSlope(ni);
                NetInfo netInfo4 = AssetEditorRoadUtils.TryGetTunnel(ni);
                elevations = new Dictionary<string, NetInfo>();
                if (netInfo != null)
                {
                    elevations["elevated"] = netInfo;
                }
                if (netInfo2 != null)
                {
                    elevations["bridge"] = netInfo2;
                }
                if (netInfo3 != null)
                {
                    elevations["slope"] = netInfo3;
                }
                if (netInfo4 != null)
                {
                    elevations["tunnel"] = netInfo4;
                }
                roadDLCMask = AssetEditorRoadUtils.GetDLCMask(ni);
            }
            Debug.Log("Checkpoint3p");
            //string templateName = (!(ToolsModifierControl.toolController.m_templatePrefabInfo != null)) ? null : ToolsModifierControl.toolController.m_templatePrefabInfo.name;
            m_PackageSaveTask = Task.Create(() =>
            {
                try
                {
                    Debug.Log("Checkpoint4p");
                    string savePathName = GetSavePathName(saveName);
                    Debug.Log("SavePathName is " + savePathName);
                    PackageX p = new PackageX(saveName);
                    Debug.Log("Checkpoint4pa");
                    if (Singleton<SimulationManager>.instance.m_metaData.m_WorkshopPublishedFileId != PublishedFileId.invalid)
                    {
                        Debug.Log("Checkpoint4pb");
                        p.packageName = Singleton<SimulationManager>.instance.m_metaData.m_WorkshopPublishedFileId.ToString();
                        Debug.Log("Checkpoint4pc");
                    }
                    if (!Singleton<SimulationManager>.instance.m_metaData.m_BuiltinAsset && PlatformService.active)
                    {
                        Debug.Log("Checkpoint4pd");
                        p.packageAuthor = "steamid:" + PlatformService.user.userID.AsUInt64.ToString();
                        Debug.Log("Checkpoint4pe");
                    }
                    Debug.Log("Checkpoint4pf");
                    //UITextureAtlas tempThumbAtlas = Tool.m_prefab.m_Atlas;
                    Debug.Log("Checkpoint4pg");
                    //UITextureAtlas tempInfoAtlas = Tool.m_prefab.m_InfoTooltipAtlas;
                    Debug.Log("Checkpoint4ph");
                    int triangles = 0;
                    int textureHeight = 0;
                    int textureWidth = 0;
                    try
                    {
                        Debug.Log("Checkpoint5p");
                        GameObject gameObject = Tool.m_prefab.gameObject;
                        Debug.Log("Checkpoint5pa");
                        triangles = gameObject.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
                        Debug.Log("Checkpoint5pb");
                        Texture mainTexture = gameObject.GetComponent<MeshRenderer>().material.mainTexture;
                        Debug.Log("Checkpoint5pc");
                        textureHeight = mainTexture.height;
                        textureWidth = mainTexture.width;
                        Debug.Log("Checkpoint5pd");
                    }
                    catch
                    {
                    }
                    int lodTriangles = 0;
                    int lodTextureHeight = 0;
                    int lodTextureWidth = 0;
                    try
                    {
                        //GameObject lODObject = this.GetLODObject();
                        //lodTriangles = lODObject.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
                        //Texture mainTexture2 = lODObject.GetComponent<MeshRenderer>().material.mainTexture;
                        //lodTextureHeight = mainTexture2.height;
                        //lodTextureWidth = mainTexture2.width;
                    }
                    catch
                    {
                    }
                    //this.PerformanceWarning(triangles, textureHeight, textureWidth, lodTriangles, lodTextureHeight, lodTextureWidth);
                    int index = 0;
                    foreach (BuildingInfo pillar in pillars)
                    {
                        Debug.Log("Checkpoint6p");
                        CustomAssetMetaData customAssetMetaData6 = new CustomAssetMetaData();
                        customAssetMetaData6.name = assetName + "_pillar_" + index.ToString();
                        customAssetMetaData6.timeStamp = DateTime.Now;
                        SaveAssetPanel.TempAtlasData atlasData3 = TempNullAtlas(pillar);
                        Debug.Log("Checkpoint6pa");
                        Task<PackageX.Asset> task7 = ThreadHelper.dispatcher.Dispatch(() => p.AddAsset(StripName(pillar.gameObject.name), pillar.gameObject, false));
                        Debug.Log("Checkpoint6pb");
                        task7.Wait();
                        Debug.Log("Checkpoint6pc");
                        RestoreAtlas(pillar, atlasData3);
                        customAssetMetaData6.assetRef = task7.result;
                        customAssetMetaData6.type = CustomAssetMetaData.Type.Pillar;
                        Debug.Log("Checkpoint6pd");
                        p.AddAsset(assetName + "_pillar_" + index.ToString() + "_metadata", customAssetMetaData6, UserAssetType.CustomAssetMetaData, false);
                        Debug.Log("Checkpointpe");
                        //	Task task8 = ThreadHelper.dispatcher.Dispatch(delegate
                        //	{
                        //	if (this.$this.PillarRequiresLocale(pillar))
                        //		{
                        //		Locale locale4 = new Locale();
                        //		string key = this.$this.StripName(pillar.gameObject.name);
                        //		locale4.AddLocalizedString(new Locale.Key
                        //		{
                        //			m_Identifier = "BUILDING_TITLE",
                        //			m_Key = key
                        //		}, assetName);
                        //		locale4.AddLocalizedString(new Locale.Key
                        //		{
                        //			m_Identifier = "BUILDING_DESC",
                        //			m_Key = key
                        //		}, description);
                        //		locale4.AddLocalizedString(new Locale.Key
                        //		{
                        //			m_Identifier = "BUILDING_SHORT_DESC",
                        //			m_Key = key
                        //		}, description);
                        //		p.AddAsset(assetName + "_pillar_locale_" + index.ToString(), locale4, false, false);
                        //	}
                        //});
                        //task8.Wait();
                        index++;
                    }
                    foreach (KeyValuePair<string, NetInfo> pair in elevations)
                    {
                        Debug.Log("Checkpoint7p");
                        CustomAssetMetaData customAssetMetaData7 = new CustomAssetMetaData();
                        //customAssetMetaData7.mods = this.$this.EmbedModInfo();
                        customAssetMetaData7.name = assetName + "_" + pair.Key;
                        customAssetMetaData7.timeStamp = DateTime.Now;
                        Task<PackageX.Asset> task9 = ThreadHelper.dispatcher.Dispatch<PackageX.Asset>(() => p.AddAsset(StripName(pair.Value.gameObject.name), pair.Value.gameObject, false));
                        task9.Wait();
                        customAssetMetaData7.assetRef = task9.result;
                        customAssetMetaData7.type = CustomAssetMetaData.Type.RoadElevation;
                        p.AddAsset(assetName + "_" + pair.Key + "_metadata", customAssetMetaData7, UserAssetType.CustomAssetMetaData, false);
                    }
                    Debug.Log("Checkpoint8p");
                    CustomAssetMetaData metaData = new CustomAssetMetaData();
                    //metaData.mods = this.$this.EmbedModInfo();
                    metaData.name = assetName;
                    metaData.timeStamp = DateTime.Now;
                    //metaData.dlcMask = Tool.m_prefab.m_dlcRequired;

                    //metaData.templateName = templateName;

                    Task<PackageX.Asset> task10 = ThreadHelper.dispatcher.Dispatch<PackageX.Asset>(() => p.AddAsset(assetName + "_Data", Tool.m_prefab.gameObject, false));
                    task10.Wait();
                    metaData.assetRef = task10.result;
                    Task<PackageX.Asset> task11 = ThreadHelper.dispatcher.Dispatch<PackageX.Asset>(delegate
                    {
                        AssetDataWrapper.UserAssetData assetData = Singleton<LoadingManager>.instance.m_AssetDataWrapper.OnAssetSaved(metaData.name, Tool.m_prefab);
                        return p.AddAsset(LoadingManager.kUserAssetDataRef, assetData, PackageX.AssetType.Object, false);
                    });
                    task11.Wait();
                    metaData.userDataRef = task11.result;
                    Tool.m_prefab.m_InfoTooltipAtlas = Tool.m_prefab.m_Atlas;
                    //if (ignoreThumbnail)
                    //{
                        //Tool.m_prefab.m_Atlas = tempThumbAtlas;
                        //Tool.m_prefab.m_InfoTooltipAtlas = tempInfoAtlas;
                    //}
                    Debug.Log("Checkpoint9p");
                    metaData.steamTags = Tool.m_prefab.GetSteamTags();
                    metaData.guid = Singleton<SimulationManager>.instance.m_metaData.m_gameInstanceIdentifier;
                    string text3 = null;
                    string text4 = null;
                    string text5 = null;
                    if (Tool.m_prefab is NetInfo)
                    {
                        metaData.type = CustomAssetMetaData.Type.Road;
                        text3 = "NET_TITLE";
                        text4 = "NET_DESC";
                        metaData.dlcMask |= roadDLCMask;
                    }
                    metaData.triangles = triangles;
                    metaData.textureHeight = textureHeight;
                    metaData.textureWidth = textureWidth;
                    metaData.lodTriangles = lodTriangles;
                    metaData.lodTextureHeight = lodTextureHeight;
                    metaData.lodTextureWidth = lodTextureWidth;
                    p.AddAsset(saveName, metaData, UserAssetType.CustomAssetMetaData, false);
                    Debug.Log("Checkpoint10p");
                    if (text3 != null)
                    {
                        Locale locale3 = new Locale();
                        locale3.AddLocalizedString(new Locale.Key
                        {
                            m_Identifier = text3,
                            m_Key = assetName + "_Data"
                        }, assetName);
                        if (text4 != null)
                        {
                            locale3.AddLocalizedString(new Locale.Key
                            {
                                m_Identifier = text4,
                                m_Key = assetName + "_Data"
                            }, description);
                        }
                        if (text5 != null)
                        {
                            locale3.AddLocalizedString(new Locale.Key
                            {
                                m_Identifier = text5,
                                m_Key = assetName + "_Data"
                            }, description);
                        }
                        p.AddAsset(assetName + "_Locale", locale3, false, false);
                    }
                    Debug.Log("Checkpoint11p");
                    p.Save(savePathName, false);

                }
                catch (Exception ex)
                {
                    string text6;
                    if (DiskUtils.IsFull(ex))
                    {
                        text6 = "The disk is full." + Environment.NewLine + "Please free some space before saving.";
                        CODebugBase<LogChannel>.Error(LogChannel.Core, string.Concat(new object[]
                        {
                    text6,
                    "\n",
                    ex.GetType(),
                    " ",
                    ex.Message,
                    " ",
                    ex.StackTrace
                        }));
                    }
                    else
                    {
                        text6 = "An error occurred while packaging asset.";
                        CODebugBase<LogChannel>.Error(LogChannel.Core, string.Concat(new object[]
                        {
                    text6,
                    "\n",
                    ex.GetType(),
                    " ",
                    ex.Message,
                    " ",
                    ex.StackTrace
                        }));
                    }
                }
                //finally
                //{
                //    Thread.Sleep(1000);
                //    SaveAssetPanel.m_IsSaving = false;
                //}
            }).Run();
            LoadSaveStatus.activeTask = new AsyncTaskWrapper("Saving", new Task[]
            {
                m_PackageSaveTask
            });
            Debug.Log("Checkpoint12p");
            return null;
        }
        private static string StripName(string name)
        {
            int num = name.LastIndexOf(".");
            if (num >= 0)
            {
                return name.Remove(0, num + 1);
            }
            return name;
        }
        private static SaveAssetPanel.TempAtlasData TempNullAtlas(PrefabInfo info)
        {
            SaveAssetPanel.TempAtlasData result = default(SaveAssetPanel.TempAtlasData);
            result.m_Atlas = info.m_Atlas;
            result.m_InfoTooltipAtlas = info.m_InfoTooltipAtlas;
            result.m_Thumbnail = info.m_Thumbnail;
            result.m_InfoTooltipThumbnail = info.m_InfoTooltipThumbnail;
            info.m_Atlas = null;
            info.m_InfoTooltipAtlas = null;
            info.m_Thumbnail = null;
            info.m_InfoTooltipThumbnail = null;
            return result;
        }
        private static void RestoreAtlas(PrefabInfo info, SaveAssetPanel.TempAtlasData atlasData)
        {
            info.m_Atlas = atlasData.m_Atlas;
            info.m_InfoTooltipAtlas = atlasData.m_InfoTooltipAtlas;
            info.m_Thumbnail = atlasData.m_Thumbnail;
            info.m_InfoTooltipThumbnail = atlasData.m_InfoTooltipThumbnail;
        }
        private static string GetSavePathName(string saveName)
        {
            string path = PathUtils.AddExtension(PathEscaper.Escape(saveName), PackageManager.packageExtension);
            return Path.Combine(DataLocation.assetsPath, path);
        }
        private static IEnumerator ThreadSaveDecoration(PrefabInfo newInfo)
        {
            if (newInfo != null)
            {
                newInfo.SaveDecorations();
            }
            yield return 0;
        }
    }
}
