using ColossalFramework;
using ColossalFramework.Packaging;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace NetworkExtensions2.Framework
{
    internal class PackageX : Package
    {
        public PackageX(string name) : base(name)
        {
        }
        public new Asset AddAsset(string name, GameObject go, bool uniqueName = false)
        {
            Asset result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (PackageWriter packageWriter = new PackageWriter(memoryStream))
                {
                    if (AssetSerializerX.SerializeHeader(name, go, packageWriter))
                    {
                        Debug.Log("Checkpointi");
                        PackageSerializerX.Serialize(this, go, packageWriter);
                        Debug.Log("Checkpointii");
                        result = AddAsset(new Asset(name, memoryStream.ToArray(), AssetType.Object, uniqueName));
                        Debug.Log("Checkpointiii");
                    }
                    else
                    {
                        result = null;
                    }
                }
            }
            return result;
        }
        public new Asset AddAsset(string name, object assetData, AssetType type, bool uniqueName = false)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (PackageWriter writer = new PackageWriter(memoryStream))
                {
                    if (AssetSerializerX.SerializeHeader(name, assetData, writer))
                    {
                        PackageSerializerX.Serialize(this, assetData, writer);
                        return AddAsset(new Asset(name, memoryStream.ToArray(), type, uniqueName));
                    }

                    return null;
                }
            }
        }

        //public new Asset AddAsset(Asset assetInfo)
        //{
        //    if (assetInfo.checksum == null)
        //    {
        //        CODebugBase<InternalLogChannel>.Warn(InternalLogChannel.Packer, string.Concat("Asset ", assetInfo, " is transient and can not added to package ", this));
        //        return null;
        //    }

        //    CODebugBase<InternalLogChannel>.VerboseLog(InternalLogChannel.Packer, string.Concat("Asset ", assetInfo, " added to package ", this));
        //    assetInfo.package = this;
        //    if (m_IndexTable.TryGetValue(assetInfo.name, out Asset value))
        //    {
        //        CODebugBase<InternalLogChannel>.VerboseLog(InternalLogChannel.Packer, string.Concat("Asset ", assetInfo, " replaced ", value, " in package ", this));
        //        m_ChecksumTable.Remove(value.checksum);
        //        m_IndexTable[assetInfo.name] = assetInfo;
        //        m_ChecksumTable.Add(assetInfo.checksum, assetInfo);
        //    }
        //    else
        //    {
        //        m_IndexTable.Add(assetInfo.name, assetInfo);
        //        m_ChecksumTable.Add(assetInfo.checksum, assetInfo);
        //    }

        //    return assetInfo;
        //}

    }
    internal static class AssetSerializerX
    {
        public static bool SerializeHeader(string name, object obj, PackageWriter writer)
        {
            if (obj == null || obj.Equals(null))
            {
                writer.Write(value: true);
                return false;
            }

            writer.Write(value: false);
            writer.Write(obj.GetType().AssemblyQualifiedName);
            writer.Write(name);
            return true;
        }

        public static bool SerializeHeader(object obj, PackageWriter writer)
        {
            if (obj == null || obj.Equals(null))
            {
                writer.Write(value: true);
                return false;
            }

            writer.Write(value: false);
            writer.Write(obj.GetType().AssemblyQualifiedName);
            return true;
        }
    }
    internal static class PackageSerializerX
    {
        internal static void Serialize(Package package, GameObject gameObject, PackageWriter writer)
        {
            Debug.Log("CpS1");
            writer.Write(gameObject.tag);
            Debug.Log("CpS2");
            writer.Write(gameObject.layer);
            Debug.Log("CpS3");
            writer.Write(gameObject.activeSelf);
            Debug.Log("CpS4");
            Component[] components = gameObject.GetComponents<Component>();
            Debug.Log("CpS5");
            writer.Write(components.Length);
            Debug.Log("CpS6");
            Component[] array = components;
            Debug.Log("CpS7");
            for (int i = 0; i < array.Length; i++)
            {
                Component component = array[i];
                Debug.Log("CpS8_" + i);
                PackageSerializerX.Serialize(package, component, writer);
                Debug.Log("CpS9_" + i);
            }
        }
        internal static void Serialize(Package package, Component component, PackageWriter writer)
        {
            if (AssetSerializerX.SerializeHeader(component, writer))
            {
                if (component is Transform)
                {
                    Debug.Log("CPx1a");
                    PackageSerializerX.Serialize(package, component as Transform, writer);
                    Debug.Log("CPx1b");
                    return;
                }
                if (component is MeshFilter)
                {
                    Debug.Log("CPx2a");
                    PackageSerializerX.Serialize(package, component as MeshFilter, writer);
                    Debug.Log("CPx2b");
                    return;
                }
                if (component is MeshRenderer)
                {
                    Debug.Log("CPx3a");
                    PackageSerializerX.Serialize(package, component as MeshRenderer, writer);
                    Debug.Log("CPx3b");
                    return;
                }
                if (component is SkinnedMeshRenderer)
                {
                    Debug.Log("CPx4a");
                    PackageSerializerX.Serialize(package, component as SkinnedMeshRenderer, writer);
                    Debug.Log("CPx4b");
                    return;
                }
                if (component is Animator)
                {
                    Debug.Log("CPx5a");
                    PackageSerializerX.Serialize(package, component as Animator, writer);
                    Debug.Log("CPx5b");
                    return;
                }
                if (typeof(MonoBehaviour).IsAssignableFrom(component.GetType()))
                {
                    Debug.Log("CPx6a");
                    PackageSerializerX.Serialize(package, component as MonoBehaviour, writer);
                    Debug.Log("CPx6b");
                }
            }
        }
        internal static void Serialize(Package package, MeshFilter meshFilter, PackageWriter writer)
        {
            Debug.Log("CPSmf1");
            writer.Write(package.AddAsset(meshFilter.sharedMesh.name, meshFilter.sharedMesh, true));
            Debug.Log("CPSmf1");
        }
        internal static void Serialize(Package package, MeshRenderer meshRenderer, PackageWriter writer)
        {
            Debug.Log("CPSmr1");
            Material[] sharedMaterials = meshRenderer.sharedMaterials;
            Debug.Log("CPSmr2");
            writer.Write(sharedMaterials.Length);
            Debug.Log("CPSmr3");
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                writer.Write(package.AddAsset(sharedMaterials[i].name, sharedMaterials[i], true));
                Debug.Log("CPSmr4_" + i);
            }
        }
        internal static void Serialize(Package package, MonoBehaviour monoBehaviour, PackageWriter writer)
        {
            Debug.Log("CPSmb1");
            FieldInfo[] fields = monoBehaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Debug.Log("CPSmb2");
            IEnumerable<FieldInfo> enumerable = from field in fields
                                                where PackageSerializerX.IsSerializable(field)
                                                select field;
            Debug.Log("CPSmb3");
            writer.Write(enumerable.Count<FieldInfo>());
			Debug.Log("CPSmb4 count " + enumerable.Count());
			Debug.Log("CPSmb4 field count " + enumerable.Count<FieldInfo>());
            foreach (FieldInfo current in enumerable)
            {
                object value = current.GetValue(monoBehaviour);
                Debug.Log("CPSmb5");
                if (AssetSerializerX.SerializeHeader(current.Name, value, writer))
                {
                    Debug.Log("CPSmb6");
                    if (current.FieldType.IsArray)
                    {
                        Debug.Log("CPSmb7");
                        Array array = (Array)value;
						Debug.Log("CPSmb7i");
                        writer.Write(array.Length);
						Debug.Log("CPSmb7ii");
                        for (int i = 0; i < array.Length; i++)
                        {
							Debug.Log("CPSmb8_" + i);
							PackageSerializerX.SerializeSingleObject(package, monoBehaviour.name, current.FieldType.GetElementType(), array.GetValue(i), writer);
							Debug.Log("CPSmb8_" + i + "_a");
                        }
                    }
                    else
                    {
						Debug.Log("CPSmb9");
                        PackageSerializerX.SerializeSingleObject(package, monoBehaviour.name, current.FieldType, value, writer);
						Debug.Log("CPSmb10");
					}
                }
            }
        }
        internal static void Serialize(Package package, Transform transform, PackageWriter writer)
        {
			Debug.Log("CPSt1");
			if (transform.parent != null)
            {
				throw new NotImplementedException("Parent serialization is not yet supported");
            }
            writer.Write(transform.localPosition);
            writer.Write(transform.localRotation);
            writer.Write(transform.localScale);
        }
        internal static void Serialize(Package package, object obj, PackageWriter writer)
        {
			Debug.Log("CPsO1");
			Debug.Log("hasCustomSerializer " + PackageSerializer.hasCustomSerializer);
			Debug.Log("CPsO2");
			if (!PackageSerializer.hasCustomSerializer || !PackageHelperX.CustomSerialize(package, obj, writer))
            {
                FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                IEnumerable<FieldInfo> enumerable = from field in fields
                                                    where PackageSerializerX.IsSerializable(field)
                                                    select field;
                writer.Write(enumerable.Count<FieldInfo>());
                foreach (FieldInfo current in enumerable)
                {
                    object value = current.GetValue(obj);
                    if (AssetSerializerX.SerializeHeader(current.Name, value, writer))
                    {
                        if (current.FieldType.IsArray)
                        {
                            Array array = (Array)value;
                            writer.Write(array.Length);
                            for (int i = 0; i < array.Length; i++)
                            {
                                PackageSerializerX.SerializeSingleObject(package, current.Name, current.FieldType.GetElementType(), array.GetValue(i), writer);
                            }
                        }
                        else
                        {
                            PackageSerializerX.SerializeSingleObject(package, current.Name, current.FieldType, value, writer);
                        }
                    }
                }
            }
        }
        internal static void SerializeSingleObject(Package package, string name, Type type, object value, PackageWriter writer)
        {
			Debug.Log("CPsso1");
			Debug.Log("hasCustomSerializer " + PackageSerializer.hasCustomSerializer);
			Debug.Log("CPsso2");
			if (!PackageSerializer.hasCustomSerializer || !PackageHelperX.CustomSerialize(package, value, writer))
            {
				Debug.Log("CPsso3");
				if (typeof(ScriptableObject).IsAssignableFrom(type))
                {
                    ScriptableObject so = (ScriptableObject)value;
                    writer.Write(package.AddAsset(name, so, true));
                    return;
                }
				Debug.Log("CPsso4");
				if (typeof(GameObject).IsAssignableFrom(type))
                {
                    GameObject go = (GameObject)value;
                    writer.Write(package.AddAsset(name, go, true));
                    return;
                }
				Debug.Log("CPsso5");
				if (PackageSerializerX.IsUnityType(type))
                {
                    writer.WriteUnityType(value);
                    return;
                }
				Debug.Log("CPsso6");
				CODebugBase<InternalLogChannel>.Error(InternalLogChannel.Serialization, "Unsupported type for serialization: [" + type.Name + "] " + name);
            }
        }

        internal static bool IsSerializable(FieldInfo field)
        {
            if (field.FieldType.IsArray)
            {
                return !field.IsNotSerialized && PackageSerializerX.IsUnitySerialized(field) && PackageSerializerX.IsSerializable(field.FieldType.GetElementType());
            }
            return !field.IsNotSerialized && PackageSerializerX.IsUnitySerialized(field) && (PackageSerializerX.IsUnityType(field.FieldType) || field.FieldType.IsSerializable);
        }
        internal static bool IsSerializable(Type type)
        {
            if (type.IsArray)
            {
                return PackageSerializerX.IsSerializable(type.GetElementType());
            }
            return PackageSerializerX.IsUnityType(type) || type.IsSerializable;
        }

        internal static bool IsUnitySerialized(FieldInfo field)
        {
            return field.IsPublic || field.GetCustomAttributes(typeof(SerializeField), true).Length > 0;
        }
        internal static bool IsUnityType(Type type)
        {
            return type.IsEnum || type == typeof(bool) || type == typeof(byte) || type == typeof(int) || type == typeof(uint) || type == typeof(ulong) || type == typeof(float) || type == typeof(string) || type == typeof(bool[]) || type == typeof(byte[]) || type == typeof(int[]) || type == typeof(float[]) || type == typeof(string[]) || type == typeof(DateTime) || type == typeof(Package.Asset) || type == typeof(GameObject) || type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4) || type == typeof(Color) || type == typeof(Matrix4x4) || type == typeof(Quaternion) || type == typeof(Vector2[]) || type == typeof(Vector3[]) || type == typeof(Vector4[]) || type == typeof(Color[]) || type == typeof(Matrix4x4[]) || type == typeof(Quaternion[]);
        }

    }
    internal static class PackageHelperX
    {
		public static bool CustomSerialize(Package p, object o, PackageWriter w)
		{
			Debug.Log("CPcs1 object is null " + (o == null));
			Type type = o.GetType();
			Debug.Log("CPcs2 type is " + type.ToString());
			if (type == typeof(TransportInfo))
			{
				TransportInfo transportInfo = (TransportInfo)o;
				w.Write(transportInfo.name);
				return true;
			}
			if (type == typeof(BuildingInfo.PathInfo))
			{
				Debug.Log("CPcsBIPI");
				BuildingInfo.PathInfo pathInfo = (BuildingInfo.PathInfo)o;
				w.Write((!(pathInfo.m_netInfo != null)) ? string.Empty : pathInfo.m_netInfo.name);
				w.Write(pathInfo.m_nodes);
				w.Write(pathInfo.m_curveTargets);
				w.Write(pathInfo.m_invertSegments);
				w.Write(pathInfo.m_maxSnapDistance);
				w.Write(pathInfo.m_forbidLaneConnection);
				var list = new List<int>();
				for (int i = 0; i < pathInfo.m_trafficLights.Length; i++)
                {
					list.Add((int)pathInfo.m_trafficLights[i]);
                }
				w.Write(list.ToArray());
				//w.Write((int[])pathInfo.m_trafficLights);
				w.Write(pathInfo.m_yieldSigns);
				return true;
			}
			if (type == typeof(BuildingInfo.Prop))
			{
				Debug.Log("CPcsBIP");
				BuildingInfo.Prop prop = (BuildingInfo.Prop)o;
				w.Write((!(prop.m_prop != null)) ? string.Empty : prop.m_prop.name);
				w.Write((!(prop.m_tree != null)) ? string.Empty : prop.m_tree.name);
				w.Write(prop.m_position);
				w.Write(prop.m_angle);
				w.Write(prop.m_probability);
				w.Write(prop.m_fixedHeight);
				return true;
			}
			if (type == typeof(PropInfo.Variation))
			{
				Debug.Log("CPcsV");
				PropInfo.Variation variation = (PropInfo.Variation)o;
				w.Write((!(variation.m_prop != null)) ? string.Empty : PackageHelper.StripName(variation.m_prop.name));
				w.Write(variation.m_probability);
				return true;
			}
			if (type == typeof(TreeInfo.Variation))
			{
				Debug.Log("CPcsTV");
				TreeInfo.Variation variation2 = (TreeInfo.Variation)o;
				w.Write((!(variation2.m_tree != null)) ? string.Empty : PackageHelper.StripName(variation2.m_tree.name));
				w.Write(variation2.m_probability);
				return true;
			}
			if (type == typeof(MessageInfo))
			{
				Debug.Log("CPcsMsgI");
				MessageInfo messageInfo = (MessageInfo)o;
				w.Write((messageInfo.m_firstID1 == null) ? string.Empty : messageInfo.m_firstID1);
				w.Write((messageInfo.m_firstID2 == null) ? string.Empty : messageInfo.m_firstID2);
				w.Write((messageInfo.m_repeatID1 == null) ? string.Empty : messageInfo.m_repeatID1);
				w.Write((messageInfo.m_repeatID2 == null) ? string.Empty : messageInfo.m_repeatID2);
				return true;
			}
			if (type == typeof(ItemClass))
			{
				Debug.Log("CPcsIC");
				ItemClass itemClass = (ItemClass)o;
				w.Write(itemClass.name);
				return true;
			}
			if (type == typeof(AudioInfo))
			{
				return true;
			}
			if (type == typeof(ModInfo))
			{
				Debug.Log("CPcsMI");
				ModInfo modInfo = (ModInfo)o;
				w.Write(modInfo.modName);
				w.Write(modInfo.modWorkshopID);
				return true;
			}
			if (type == typeof(DisasterProperties.DisasterSettings))
			{
				Debug.Log("CPcsDS");
				DisasterProperties.DisasterSettings disasterSettings = (DisasterProperties.DisasterSettings)o;
				w.Write(disasterSettings.m_disasterName);
				w.Write(disasterSettings.m_randomProbability);
				return true;
			}
			if (type == typeof(UITextureAtlas))
			{
				Debug.Log("CPcsUITA");
				UITextureAtlas uITextureAtlas = (UITextureAtlas)o;
				w.Write(uITextureAtlas.name);
				Texture2D texture2D = uITextureAtlas.material.mainTexture as Texture2D;
				byte[] array = texture2D.EncodeToPNG();
				w.Write(array.Length);
				w.Write(array);
				w.Write(uITextureAtlas.material.shader.name);
				w.Write(uITextureAtlas.padding);
				List<UITextureAtlas.SpriteInfo> sprites = uITextureAtlas.sprites;
				w.Write(sprites.Count);
				for (int i = 0; i < sprites.Count; i++)
				{
					w.Write(sprites[i].region.x);
					w.Write(sprites[i].region.y);
					w.Write(sprites[i].region.width);
					w.Write(sprites[i].region.height);
					w.Write(sprites[i].name);
				}
				return true;
			}
			if (type == typeof(VehicleInfo.Effect))
			{
				Debug.Log("CPcsVE");
				VehicleInfo.Effect effect = (VehicleInfo.Effect)o;
				w.Write(effect.m_effect.name);
				w.Write((int)effect.m_parkedFlagsForbidden);
				w.Write((int)effect.m_parkedFlagsRequired);
				w.Write((int)effect.m_vehicleFlagsForbidden);
				w.Write((int)effect.m_vehicleFlagsRequired);
				return true;
			}
			if (type == typeof(VehicleInfo.VehicleDoor))
			{
				Debug.Log("CPcsVD");
				VehicleInfo.VehicleDoor vehicleDoor = (VehicleInfo.VehicleDoor)o;
				w.Write((int)vehicleDoor.m_type);
				w.Write(vehicleDoor.m_location);
				return true;
			}
			if (type == typeof(VehicleInfo.VehicleTrailer))
			{
				Debug.Log("CPcsVT");
				VehicleInfo.VehicleTrailer vehicleTrailer = (VehicleInfo.VehicleTrailer)o;
				w.Write(PackageHelper.StripName(vehicleTrailer.m_info.name));
				w.Write(vehicleTrailer.m_probability);
				w.Write(vehicleTrailer.m_invertProbability);
				return true;
			}
			if (type == typeof(ManualMilestone) || type == typeof(CombinedMilestone))
			{
				Debug.Log("CPcsMM|CM");
				MilestoneInfo milestoneInfo = (MilestoneInfo)o;
				w.Write(milestoneInfo.name);
				return true;
			}
			if (type == typeof(TransportInfo))
			{
				Debug.Log("CPcsTI");
				TransportInfo transportInfo2 = (TransportInfo)o;
				w.Write(transportInfo2.name);
				return true;
			}
			if (type == typeof(NetInfo))
			{
				Debug.Log("CPcsNI");
				NetInfo netInfo = (NetInfo)o;
				w.Write(netInfo.name);
				return true;
			}
			if (type == typeof(BuildingInfo.MeshInfo))
			{
				Debug.Log("CPcsBMI");
				BuildingInfo.MeshInfo meshInfo = (BuildingInfo.MeshInfo)o;
				if (meshInfo.m_subInfo != null)
				{
					GameObject gameObject = meshInfo.m_subInfo.gameObject;
					Package.Asset asset = p.AddAsset(gameObject.name, gameObject, false);
					w.Write(asset.checksum);
				}
				else
				{
					w.Write(string.Empty);
				}
				PackageHelper.CustomSerialize(p, meshInfo.m_flagsForbidden, w);
				PackageHelper.CustomSerialize(p, meshInfo.m_flagsRequired, w);
				w.Write(meshInfo.m_position);
				w.Write(meshInfo.m_angle);
				return true;
			}
			if (type == typeof(Building.Flags))
			{
				Debug.Log("CPcsBF");
				Building.Flags value = (Building.Flags)o;
				w.Write((int)value);
				return true;
			}
			if (type == typeof(VehicleInfo.MeshInfo))
			{
				Debug.Log("CPcsVMI");
				VehicleInfo.MeshInfo meshInfo2 = (VehicleInfo.MeshInfo)o;
				if (meshInfo2.m_subInfo != null)
				{
					GameObject gameObject2 = meshInfo2.m_subInfo.gameObject;
					Package.Asset asset2 = p.AddAsset(gameObject2.name, gameObject2, false);
					w.Write(asset2.checksum);
				}
				else
				{
					w.Write(string.Empty);
				}
				PackageHelper.CustomSerialize(p, meshInfo2.m_vehicleFlagsForbidden, w);
				PackageHelper.CustomSerialize(p, meshInfo2.m_vehicleFlagsRequired, w);
				PackageHelper.CustomSerialize(p, meshInfo2.m_parkedFlagsForbidden, w);
				PackageHelper.CustomSerialize(p, meshInfo2.m_parkedFlagsRequired, w);
				return true;
			}
			if (type == typeof(Vehicle.Flags))
			{
				Debug.Log("CPcsVF");
				Vehicle.Flags value2 = (Vehicle.Flags)o;
				w.Write((int)value2);
				return true;
			}
			if (type == typeof(VehicleParked.Flags))
			{
				Debug.Log("CPcsVPF");
				VehicleParked.Flags value3 = (VehicleParked.Flags)o;
				w.Write((int)value3);
				return true;
			}
			if (type == typeof(DepotAI.SpawnPoint))
			{
				Debug.Log("CPcsDSP");
				DepotAI.SpawnPoint spawnPoint = (DepotAI.SpawnPoint)o;
				w.Write(spawnPoint.m_position);
				w.Write(spawnPoint.m_target);
				return true;
			}
			if (type == typeof(PropInfo.Effect))
			{
				Debug.Log("CPcsE");
				PropInfo.Effect effect2 = (PropInfo.Effect)o;
				w.Write(effect2.m_effect.name);
				w.Write(effect2.m_position);
				w.Write(effect2.m_direction);
				return true;
			}
			if (type == typeof(BuildingInfo.SubInfo))
			{
				Debug.Log("CPcsSI");
				BuildingInfo.SubInfo subInfo = (BuildingInfo.SubInfo)o;
				w.Write(PackageHelper.StripName(subInfo.m_buildingInfo.name));
				w.Write(subInfo.m_position);
				w.Write(subInfo.m_angle);
				w.Write(subInfo.m_fixedHeight);
				return true;
			}
			if (type == typeof(PropInfo.ParkingSpace))
			{
				Debug.Log("CPcsPS");
				PropInfo.ParkingSpace parkingSpace = (PropInfo.ParkingSpace)o;
				w.Write(parkingSpace.m_position);
				w.Write(parkingSpace.m_direction);
				w.Write(parkingSpace.m_size);
				return true;
			}
			if (type == typeof(PropInfo.SpecialPlace))
			{
				Debug.Log("CPcsSP");
				PropInfo.SpecialPlace specialPlace = (PropInfo.SpecialPlace)o;
				w.Write((int)specialPlace.m_specialFlags);
				w.Write(specialPlace.m_position);
				w.Write(specialPlace.m_direction);
				return true;
			}
			if (type == typeof(NetInfo.Lane))
			{
				Debug.Log("CPcsL1");
				NetInfo.Lane lane = (NetInfo.Lane)o;
				Debug.Log("CPcsL2");
				w.Write(lane.m_position);
				Debug.Log("CPcsL3");
				w.Write(lane.m_width);
				Debug.Log("CPcsL4");
				w.Write(lane.m_verticalOffset);
				Debug.Log("CPcsL5");
				w.Write(lane.m_stopOffset);
				Debug.Log("CPcsL6");
				w.Write(lane.m_speedLimit);
				Debug.Log("CPcsL7");
				w.Write((int)lane.m_direction);
				Debug.Log("CPcsL8");
				w.Write((int)lane.m_laneType);
				Debug.Log("CPcsL9");
				w.Write((int)lane.m_vehicleType);
				Debug.Log("CPcsL10");
				w.Write((int)lane.m_stopType);
				Debug.Log("CPcsL11");
				PackageHelperX.CustomSerialize(p, lane.m_laneProps, w);
				Debug.Log("CPcsL12");
				w.Write(lane.m_allowConnect);
				Debug.Log("CPcsL13");
				w.Write(lane.m_useTerrainHeight);
				Debug.Log("CPcsL14");
				w.Write(lane.m_centerPlatform);
				Debug.Log("CPcsL15");
				w.Write(lane.m_elevated);
				Debug.Log("CPcsL16");
				return true;
			}
			if (type == typeof(NetLaneProps))
			{
				Debug.Log("CPcsNP1");
				NetLaneProps netLaneProps = (NetLaneProps)o;
				Debug.Log("CPcsNP2");
				int num = netLaneProps.m_props.Length;
				Debug.Log("CPcsNP3");
				w.Write(num);
				Debug.Log("CPcsNP4");
				for (int j = 0; j < num; j++)
				{
					Debug.Log("CPcsNP5_" + j);
					PackageHelper.CustomSerialize(p, netLaneProps.m_props[j], w);
					Debug.Log("CPcsNP6_" + j);
				}
				return true;
			}
			if (type == typeof(NetLaneProps.Prop))
			{
				Debug.Log("CPcsNPP1");
				NetLaneProps.Prop prop2 = (NetLaneProps.Prop)o;
				Debug.Log("CPcsNPP2");
				w.Write((int)prop2.m_flagsRequired);
				Debug.Log("CPcsNPP3");
				w.Write((int)prop2.m_flagsForbidden);
				Debug.Log("CPcsNPP4");
				w.Write((int)prop2.m_startFlagsRequired);
				Debug.Log("CPcsNPP5");
				w.Write((int)prop2.m_startFlagsForbidden);
				Debug.Log("CPcsNPP6");
				w.Write((int)prop2.m_endFlagsRequired);
				Debug.Log("CPcsNPP7");
				w.Write((int)prop2.m_endFlagsForbidden);
				Debug.Log("CPcsNPP8");
				w.Write((int)prop2.m_colorMode);
				Debug.Log("CPcsNPP9");
				w.Write((!(prop2.m_prop != null)) ? string.Empty : prop2.m_prop.name);
				Debug.Log("CPcsNPP10");
				w.Write((!(prop2.m_tree != null)) ? string.Empty : prop2.m_tree.name);
				Debug.Log("CPcsNPP11");
				w.Write(prop2.m_position);
				Debug.Log("CPcsNPP12");
				w.Write(prop2.m_angle);
				Debug.Log("CPcsNPP13");
				w.Write(prop2.m_segmentOffset);
				Debug.Log("CPcsNPP14");
				w.Write(prop2.m_repeatDistance);
				Debug.Log("CPcsNPP15");
				w.Write(prop2.m_minLength);
				Debug.Log("CPcsNPP16");
				w.Write(prop2.m_cornerAngle);
				Debug.Log("CPcsNPP17");
				w.Write(prop2.m_probability);
				Debug.Log("CPcsNPP18");
				w.Write(prop2.m_upgradable);
				Debug.Log("CPcsNPP19");
				return true;
			}
			if (type == typeof(NetInfo.Segment))
			{
				Debug.Log("CPcsS");
				NetInfo.Segment segment = (NetInfo.Segment)o;
				Mesh mesh = segment.m_mesh;
				w.Write((!(mesh != null)) ? string.Empty : p.AddAsset(mesh.name, mesh, true).checksum);
				Material material = segment.m_material;
				w.Write((!(material != null)) ? string.Empty : p.AddAsset(material.name, material, true).checksum);
				Mesh lodMesh = segment.m_lodMesh;
				w.Write((!(lodMesh != null)) ? string.Empty : p.AddAsset(lodMesh.name, lodMesh, true).checksum);
				Material lodMaterial = segment.m_lodMaterial;
				w.Write((!(lodMaterial != null)) ? string.Empty : p.AddAsset(lodMaterial.name, lodMaterial, true).checksum);
				w.Write((int)segment.m_forwardRequired);
				w.Write((int)segment.m_forwardForbidden);
				w.Write((int)segment.m_backwardRequired);
				w.Write((int)segment.m_backwardForbidden);
				w.Write(segment.m_emptyTransparent);
				w.Write(segment.m_disableBendNodes);
				return true;
			}
			if (type == typeof(NetInfo.Node))
			{
				Debug.Log("CPcsN");
				NetInfo.Node node = (NetInfo.Node)o;
				Mesh mesh2 = node.m_mesh;
				w.Write((!(mesh2 != null)) ? string.Empty : p.AddAsset(mesh2.name, mesh2, true).checksum);
				Material material2 = node.m_material;
				w.Write((!(material2 != null)) ? string.Empty : p.AddAsset(material2.name, material2, true).checksum);
				Mesh lodMesh2 = node.m_lodMesh;
				w.Write((!(lodMesh2 != null)) ? string.Empty : p.AddAsset(lodMesh2.name, lodMesh2, true).checksum);
				Material lodMaterial2 = node.m_lodMaterial;
				w.Write((!(lodMaterial2 != null)) ? string.Empty : p.AddAsset(lodMaterial2.name, lodMaterial2, true).checksum);
				w.Write((int)node.m_flagsRequired);
				w.Write((int)node.m_flagsForbidden);
				w.Write((int)node.m_connectGroup);
				w.Write(node.m_directConnect);
				w.Write(node.m_emptyTransparent);
				return true;
			}
			if (type == typeof(BuildingInfo))
			{
				Debug.Log("CPcsB");
				w.Write((o == null) ? string.Empty : ((BuildingInfo)o).name);
				return true;
			}
			if (type == typeof(Dictionary<string, byte[]>))
			{
				Debug.Log("CPcsDi");
				Dictionary<string, byte[]> dictionary = (Dictionary<string, byte[]>)o;
				w.Write(dictionary.Count);
				foreach (KeyValuePair<string, byte[]> current in dictionary)
				{
					w.Write(current.Key);
					w.Write(current.Value.Length);
					w.Write(current.Value);
				}
				return true;
			}
			Debug.Log("CPcs999");
			return false;
		} 
	}
}
