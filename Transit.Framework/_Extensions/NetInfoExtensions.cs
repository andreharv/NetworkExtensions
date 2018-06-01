using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Globalization;
using UnityEngine;
using Transit.Framework.Builders;
using Transit.Framework.Texturing;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Framework
{
    public static partial class NetInfoExtensions
    {
        public static void DisplayLaneProps(this NetInfo info)
        {
            foreach (var lane in info.m_lanes)
            {
                if (lane.m_laneProps != null)
                {
                    Debug.Log(string.Format("TFW: Lane name {0}", lane.m_laneProps.name));

                    if (lane.m_laneProps.m_props != null)
                    {
                        foreach (var prop in lane.m_laneProps.m_props)
                        {
                            if (prop.m_prop != null)
                            {
                                Debug.Log(string.Format("TFW:     Prop name {0}", prop.m_prop.name));
                            }
                        }
                    }
                }
            }
        }

        public static NetInfo.Lane FindLane(this NetInfo info, Func<string, bool> predicate, bool crashOnNotFound = true)
        {
            var lane = info
                .m_lanes
                .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                .FirstOrDefault(l => predicate(l.m_laneProps.name.ToLower()));

            if (lane == null)
            {
                if (crashOnNotFound)
                {
                    throw new Exception("TFW: Lane not found");
                }
            }

            return lane;
        }

        public static NetInfo.Lane FindLane(this NetInfo info, NetInfo.LaneType predicate, bool crashOnNotFound = true)
        {
            var lane = info
                .m_lanes
                .FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.Vehicle);

            if (lane == null)
            {
                if (crashOnNotFound)
                {
                    throw new Exception("TFW: Lane not found");
                }
            }

            return lane;
        }

        public static void ModifyTitle(this NetInfo info, string newTitle)
        {
            var localizedStringsField = typeof(Locale).GetFieldByName("m_LocalizedStrings");
            var locale = SingletonLite<LocaleManager>.instance.GetLocale();
            var localizedStrings = (Dictionary<Locale.Key, string>)localizedStringsField.GetValue(locale);

            var kvp =
                localizedStrings
                .FirstOrDefault(kvpInternal =>
                    kvpInternal.Key.m_Identifier == "NET_TITLE" &&
                    kvpInternal.Key.m_Key == info.name);

            if (!Equals(kvp, default(KeyValuePair<Locale.Key, string>)))
            {
                localizedStrings[kvp.Key] = newTitle;
            }
        }
        public static int LaneBalance(this NetInfo info)
        {
            var forwardLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle && l.m_direction == NetInfo.Direction.Forward).Count();
            var backwardLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle && l.m_direction == NetInfo.Direction.Backward).Count();

            return forwardLanes - backwardLanes;
        }
        public static void DoBuildupMulti(this NetInfo info, NetInfoVersion version)
        {
            if (version.IsGroundDecorated())
            {
                info.m_availableIn = ItemClass.Availability.Editors;
            }
        }

        public static void SetupLaneProps(this NetInfo info, NetInfoVersion version)
        {
            // Setting up lanes            
            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            //Setting Up Props
            var leftRoadProps = new List<NetLaneProps.Prop>();
            leftRoadProps.AddRange(leftPedLane.m_laneProps.m_props.ToList());

            var rightRoadProps = new List<NetLaneProps.Prop>();
            rightRoadProps.AddRange(rightPedLane.m_laneProps.m_props.ToList());
            //for (var i = 0; i < rightRoadProps.Count; i++)
            //{
            //    if ((rightRoadProps[i].m_flagsRequired & NetLane.Flags.Stop) == 0)
            //    {
            //        rightRoadProps[i].m_position.x = -info.m_halfWidth + ((3 / 4) * info.m_pavementWidth);
            //    }
            //}
            //for (var i = 0; i < leftRoadProps.Count; i++)
            //{
            //    if ((leftRoadProps[i].m_flagsRequired & NetLane.Flags.Stop) == 0)
            //    {
            //        leftRoadProps[i].m_position.x = info.m_halfWidth - ((3 / 4) * info.m_pavementWidth);
            //    }
            //}
            if (version == NetInfoVersion.GroundTrees)
            {
                var treeInfo = Prefabs.Find<TreeInfo>("Tree2variant");
                var leftTreeProp = new NetLaneProps.Prop()
                {
                    m_tree = treeInfo.ShallowClone(),
                    m_finalTree = treeInfo.ShallowClone(),
                    m_repeatDistance = 20,
                    m_probability = 100,
                };
                leftTreeProp.m_position.x = 2.5f;
                leftTreeProp.m_flagsForbidden |= NetLane.Flags.Stop;
                leftRoadProps.Add(leftTreeProp);
                var rightTreeProp = new NetLaneProps.Prop()
                {
                    m_tree = treeInfo.ShallowClone(),
                    m_finalTree = treeInfo.ShallowClone(),
                    m_repeatDistance = 20,
                    m_probability = 100,
                };
                rightTreeProp.m_position.x = -2.5f;
                rightTreeProp.m_flagsForbidden |= NetLane.Flags.Stop;
                rightRoadProps.Add(rightTreeProp);
            }
            else if (version == NetInfoVersion.Slope)
            {
                leftRoadProps.AddLeftWallLights(info.m_pavementWidth);
                rightRoadProps.AddRightWallLights(info.m_pavementWidth);
            }

            leftPedLane.m_laneProps.m_props = leftRoadProps.ToArray();
            rightPedLane.m_laneProps.m_props = rightRoadProps.ToArray();
            info.TrimAboveGroundProps(version);
        }
        public static void SetupConnectGroup(this NetInfo info, string meshCat, ConnextGroup ciGroup, params ConnextGroup[] cnGroups)
        {
            var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            var baseInfoName = info.StripName();

            NetInfo baseInfo = null;
            baseInfo = Prefabs.Find<NetInfo>(baseInfoName);
            if (baseInfo == null)
            {
                baseInfo = Prefabs.FindResource<NetInfo>(baseInfoName);
            }

            info.m_connectGroup = ciGroup.GetConnectGroup();
            if (info.m_nodes == null)
            {
            }
            var nodeList = info.m_nodes.ToList();
            NetInfo.Node newNode = null;
            NetInfo.Node newNode2 = null;
            if (baseInfo == null)
            {
            }
            var myWidth = (float)Math.Round(baseInfo.m_halfWidth * 2);
            var paveWidth = myWidth == 16 ? 3 : 5;
            var isVanilla = ciGroup == ConnextGroup.OneMidL || ciGroup == ConnextGroup.TwoPlusTwo || ciGroup == ConnextGroup.ThreeMidL;
            if (info.name.EndsWith("Tunnel") || info.name.EndsWith("Slope"))
            {
                if (nodeList.Count > 1)
                {
                    newNode = nodeList[1].ShallowClone();
                    newNode.m_material = brElInfo.m_segments[0].m_material;
                    newNode.m_lodMaterial = brElInfo.m_segments[0].m_lodMaterial;

                    newNode2 = nodeList[1].ShallowClone();
                    newNode2.m_material = brElInfo.m_segments[0].m_material;
                    newNode2.m_lodMaterial = brElInfo.m_segments[0].m_lodMaterial;

                    nodeList[1].m_flagsForbidden |= NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward;
                }
            }
            else
            {
                if (nodeList.Count > 0)
                {
                    newNode = nodeList[0].ShallowClone();
                    newNode2 = nodeList[0].ShallowClone();

                    nodeList[0].m_flagsForbidden |= NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward;
                }
            }
            if (meshCat.Length > 0)
            {
                if (isVanilla && myWidth != 16)
                {
                    newNode.SetMeshes(
                        $@"Roads\Common\Meshes\{myWidth}m\{meshCat}\Ground_Node_Parking.obj");
                }
            }
            newNode.m_flagsRequired |= NetNode.Flags.AsymForward;
            newNode2.m_flagsRequired |= NetNode.Flags.AsymBackward;

            RemoveCrosswalks(newNode, myWidth, paveWidth, isVanilla);
            nodeList.Add(newNode);
            RemoveCrosswalks(newNode2, myWidth, paveWidth, isVanilla);
            nodeList.Add(newNode2);


            if (cnGroups != null && cnGroups.Count() > 0)
            {
                var symGroups = new[] { ConnextGroup.OneMidL, ConnextGroup.TwoMidL, ConnextGroup.ThreeMidL, ConnextGroup.OnePlusOne, ConnextGroup.TwoPlusTwo, ConnextGroup.ThreePlusThree, ConnextGroup.FourPlusFour };
                for (var i = 0; i < cnGroups.Count(); i++)
                {
                    if (i == 0)
                    {
                        info.m_nodeConnectGroups = cnGroups[i].GetConnectGroup();
                    }
                    else
                    {
                        info.m_nodeConnectGroups |= cnGroups[i].GetConnectGroup();
                    }
                }
                if (!isVanilla)
                    info.m_halfWidth += (float)((Math.Log((float)ciGroup)) / 10000);
                var myCnxName = ciGroup.ToString();
                var meshTextureName = $@"Roads\Common\Textures\{myWidth}m\Median";
                var brElMaterial = brElInfo.m_segments[0].m_material;
                var brElLODMaterial = brElInfo.m_segments[0].m_lodMaterial;
                var wildCard = NetNode.Flags.Ambiguous;
                for (var i = 0; i < cnGroups.Count(); i++)
                {
                    var newNodeStartForward = info.m_nodes[0].ShallowClone();
                    var newNodeEndForward = info.m_nodes[0].ShallowClone();
                    var newNodeStartBackward = info.m_nodes[0].ShallowClone();
                    var newNodeEndBackward = info.m_nodes[0].ShallowClone();
                    var aCnxName = cnGroups[i].ToString();
                    var meshBaseName = $@"Roads\Common\Meshes\{myWidth}m\{meshCat}\{myCnxName}\{aCnxName}\MedCon_{aCnxName}";
                    //var required = symGroups.Contains(cnGroups[i]) ? NetNode.Flags.None : NetNode.Flags.AsymForward;

                    var sym = (symGroups.Contains(ciGroup) || symGroups.Contains(cnGroups[i]));
                    //if (sym)
                    //{
                    //    var flag = NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward;
                    //    var flaga = NetNode.Flags.None;
                    //    if (ciGroup == ConnextGroup.ThreeMidL || cnGroups[i] == ConnextGroup.ThreeMidL)
                    //    {
                    //        flag |= wildCard;
                    //    }
                    //    else
                    //    {
                    //        flaga = wildCard;
                    //    }
                    //    DoStartEndStuff(newNodeStartForward, meshBaseName, meshTextureName, true,sym, flag, flaga, cnGroups[i], brElMaterial, brElLODMaterial);
                    //    DoStartEndStuff(newNodeEndForward, meshBaseName, meshTextureName, false,sym, flag, flaga, cnGroups[i], brElMaterial, brElLODMaterial);
                    //    nodeList.Add(newNodeStartForward);
                    //    nodeList.Add(newNodeEndForward);
                    //}
                    //else
                    //{
                    var flag1 = NetNode.Flags.AsymForward;
                    var flag2 = NetNode.Flags.AsymBackward;
                    var flaga = NetNode.Flags.None;
                    if (ciGroup == ConnextGroup.ThreeMidL || cnGroups[i] == ConnextGroup.ThreeMidL || ciGroup == ConnextGroup.ThreePlusThree || cnGroups[i] == ConnextGroup.ThreePlusThree || ciGroup == ConnextGroup.FourPlusFour || cnGroups[i] == ConnextGroup.FourPlusFour)
                    {
                        flag1 |= wildCard;
                        flag2 |= wildCard;
                    }
                    else
                    {
                        flaga = wildCard;
                    }
                    DoStartEndStuff(newNodeStartForward, meshBaseName, meshTextureName, true, sym, flag1, flaga, cnGroups[i], brElMaterial, brElLODMaterial);
                    DoStartEndStuff(newNodeEndForward, meshBaseName, meshTextureName, false, sym, flag1, flaga, cnGroups[i], brElMaterial, brElLODMaterial);
                    nodeList.Add(newNodeStartForward);
                    nodeList.Add(newNodeEndForward);
                    DoStartEndStuff(newNodeStartBackward, meshBaseName, meshTextureName, true, sym, flag2, flaga, cnGroups[i], brElMaterial, brElLODMaterial);
                    DoStartEndStuff(newNodeEndBackward, meshBaseName, meshTextureName, false, sym, flag2, flaga, cnGroups[i], brElMaterial, brElLODMaterial);
                    nodeList.Add(newNodeStartBackward);
                    nodeList.Add(newNodeEndBackward);
                    //}
                }
            }
            info.m_nodes = nodeList.ToArray();
            if (baseInfoName == "Basic Road")
            {
                info.m_segments[0].SetTextures(
                    new TextureSet(
                        @"Roads\Common\Textures\16m\3mSWa__MainTex.png",
                        @"Roads\Common\Textures\16m\3mSWa__AlphaMap.png"),
                    new LODTextureSet(
                        @"Roads\Common\Textures\16m\3mSWa__MainTex_LOD.png",
                        @"Roads\Common\Textures\16m\3mSWa__AlphaMap_LOD.png",
                         @"Roads\Common\Textures\16m\3mSWa__XYSMap_LOD.png"));
            }
            if (myWidth == 32)
            {
                info.m_nodes[0].SetMeshes(
                    $@"Roads\Common\Meshes\32m\5mSW\Ground_Node_Parking.obj");
            }
        }

        private static void DoStartEndStuff(NetInfo.Node node, string meshBaseName, string meshTextureName, bool startEnd, bool sym, NetNode.Flags flag, NetNode.Flags flaga, ConnextGroup cnGroup, Material brElMaterial, Material brElLODMaterial)
        {
            node.m_material = brElMaterial;
            node.m_lodMaterial = brElLODMaterial;
            string se = "";
            string inv = !sym && flag == NetNode.Flags.AsymBackward ? "_Inv" : "";
            NetInfo.ConnectGroup owGroup;
            if (startEnd)
            {
                se = "Start";
                owGroup = NetInfo.ConnectGroup.OnewayStart;
            }
            else
            {
                se = "End";
                owGroup = NetInfo.ConnectGroup.OnewayEnd;
            }
            node
                .SetFlags(flag, flaga)
                .SetMeshes(
                $"{meshBaseName}_{se}{inv}.obj",
                $"{meshBaseName}_{se}{inv}_LOD.obj")
                .SetConsistentUVs()
                .SetTextures(
            new TextureSet
                ($@"{meshTextureName}__MainTex.png",
               $@"{meshTextureName}__AlphaMap.png"),
            new LODTextureSet
                ($@"{meshTextureName}__MainTex_LOD.png",
               $@"{meshTextureName}__AlphaMap_LOD.png",
               $@"{meshTextureName}__XYSMap_LOD.png"));
            node.m_connectGroup = cnGroup.GetConnectGroup() | owGroup;
            node.m_directConnect = true;
        }
        private static void RemoveCrosswalks(NetInfo.Node node, float rWidth, float pWidth, bool isVanilla)
        {
            var ts = new TextureSet
                ($@"Roads\Common\Textures\{rWidth}m\{pWidth}mSw__MainTex.png",
                $@"Roads\Common\Textures\{rWidth}m\{pWidth}mSw__AlphaMap.png");
            var lts = new LODTextureSet
                 ($@"Roads\Common\Textures\{rWidth}m\{pWidth}mSw__MainTex_LOD.png",
                 $@"Roads\Common\Textures\{rWidth}m\{pWidth}mSw__AlphaMap_LOD.png",
                 $@"Roads\Common\Textures\{rWidth}m\{pWidth}mSw__XYSMap_LOD.png");
            if (isVanilla)
            {
                node.SetVanillaTextures(ts, lts);
            }
            else
            {
                node.SetTextures(ts, lts);
            }
        }
        public static string StripName(this NetInfo info)
        {

            var retval = info?.name;
            if (retval != null)
            {
                var suffix = "";
                if (retval.EndsWith("Decoration Grass"))
                {
                    suffix = "Decoration Grass";
                }
                else if (retval.EndsWith("Decoration Trees"))
                {
                    suffix = "Decoration Trees";
                }
                else if (retval.EndsWith("Decoration Pavement"))
                {
                    suffix = "Decoration Pavement";
                }
                else if (retval.EndsWith("Elevated"))
                {
                    suffix = "Elevated";
                }
                else if (retval.EndsWith("Bridge"))
                {
                    suffix = "Bridge";
                }
                else if (retval.EndsWith("Slope"))
                {
                    suffix = "Slope";
                }
                else if (retval.EndsWith("Tunnel"))
                {
                    suffix = "Tunnel";
                }
                if (suffix.Length > 0)
                    retval = retval.Substring(0, retval.LastIndexOf(suffix)).Trim();
            }


            return retval;
        }
    }
}
