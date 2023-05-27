using ColossalFramework;
using ColossalFramework.Math;
using Transit.Framework;
using UnityEngine;
using Transit.Framework.ExtensionPoints.AI;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Addon.RoadExtensions.AI
{
    public class TinyRoadZoneBlocksCreator : IZoneBlocksCreator
    {
        private const float MIN_HALFWIDTH_TINY_CURVE = 6f;
        private const float MIN_HALFWIDTH_TINY_STRAIGHT = 4f;

        public void CreateZoneBlocks(NetInfo info, ushort segmentId, ref NetSegment segment)
        {
            var netManager = Singleton<NetManager>.instance;
            var randomizer = new Randomizer((int)segmentId);

            var startNode = netManager.m_nodes.m_buffer[(int)segment.m_startNode];
            var endNode = netManager.m_nodes.m_buffer[(int)segment.m_endNode];

            var startPosition = startNode.m_position;
            var endPosition = endNode.m_position;
            var startDirection = segment.m_startDirection;
            var endDirection = segment.m_endDirection;
            var isCurve = !NetSegment.IsStraight(startPosition, startDirection, endPosition, endDirection);

            if (isCurve)
            {
                CreateZoneBlocksTiny_Curve(info, randomizer, segmentId, ref segment);
            }
            else
            {
                CreateZoneBlocksTiny_Straight(info, randomizer, segmentId, ref segment, startNode, endNode);
            }
        }

        private static void CreateZoneBlocksTiny_Curve(NetInfo info, Randomizer randomizer, ushort segmentID, ref NetSegment segment)
        {
            var minHalfWidth = MIN_HALFWIDTH_TINY_CURVE;

            NetManager instance = Singleton<NetManager>.instance;
            Vector3 startPosition = instance.m_nodes.m_buffer[(int)segment.m_startNode].m_position;
            Vector3 endPosition = instance.m_nodes.m_buffer[(int)segment.m_endNode].m_position;
            Vector3 startDirection = segment.m_startDirection;
            Vector3 endDirection = segment.m_endDirection;
            float num = startDirection.x * endDirection.x + startDirection.z * endDirection.z;
            float num2 = Mathf.Max(minHalfWidth, info.m_halfWidth);
            float num3 = 32f;
            int distance = Mathf.RoundToInt(num2);
            float num4 = VectorUtils.LengthXZ(endPosition - startPosition);
            bool flag2 = startDirection.x * endDirection.z - startDirection.z * endDirection.x > 0f;
            bool flag3 = num < -0.8f || num4 > 50f;
            if (flag2)
            {
                num2 = -num2;
                num3 = -num3;
            }
            Vector3 vector = startPosition - new Vector3(startDirection.z, 0f, -startDirection.x) * num2;
            Vector3 vector2 = endPosition + new Vector3(endDirection.z, 0f, -endDirection.x) * num2;
            Vector3 vector3;
            Vector3 vector4;
            NetSegment.CalculateMiddlePoints(vector, startDirection, vector2, endDirection, true, true, out vector3, out vector4);
            if (flag3)
            {
                float num5 = num * 0.025f + 0.04f;
                float num6 = num * 0.025f + 0.06f;
                if (num < -0.9f)
                {
                    num6 = num5;
                }
                Bezier3 bezier = new Bezier3(vector, vector3, vector4, vector2);
                vector = bezier.Position(num5);
                vector3 = bezier.Position(0.5f - num6);
                vector4 = bezier.Position(0.5f + num6);
                vector2 = bezier.Position(1f - num5);
            }
            else
            {
                Bezier3 bezier2 = new Bezier3(vector, vector3, vector4, vector2);
                vector3 = bezier2.Position(0.86f);
                vector = bezier2.Position(0.14f);
            }
            float num7;
            Vector3 vector5 = VectorUtils.NormalizeXZ(vector3 - vector, out num7);
            int num8 = Mathf.FloorToInt(num7 / 8f + 0.01f);
            float num9 = num7 * 0.5f + (float)(num8 - 8) * ((!flag2) ? -4f : 4f);
            if (num8 != 0)
            {
                float angle = (!flag2) ? Mathf.Atan2(vector5.x, -vector5.z) : Mathf.Atan2(-vector5.x, vector5.z);
                Vector3 position3 = vector + new Vector3(vector5.x * num9 - vector5.z * num3, 0f, vector5.z * num9 + vector5.x * num3);
                if (flag2)
                {
                    Singleton<ZoneManager>.instance.CreateBlock(
                        out segment.m_blockStartRight, 
                        ref randomizer,
                        segmentID,
                        position3, 
                        angle, 
                        num8, 
                        distance,
                        segment.m_buildIndex);
                }
                else
                {
                    Singleton<ZoneManager>.instance.CreateBlock(
                        out segment.m_blockStartLeft, 
                        ref randomizer, 
                        segmentID,
                        position3, 
                        angle, 
                        num8,
                        distance,
                        segment.m_buildIndex);
                }
            }
            if (flag3)
            {
                vector5 = VectorUtils.NormalizeXZ(vector2 - vector4, out num7);
                num8 = Mathf.FloorToInt(num7 / 8f + 0.01f);
                num9 = num7 * 0.5f + (float)(num8 - 8) * ((!flag2) ? -4f : 4f);
                if (num8 != 0)
                {
                    float angle2 = (!flag2) ? Mathf.Atan2(vector5.x, -vector5.z) : Mathf.Atan2(-vector5.x, vector5.z);
                    Vector3 position4 = vector4 + new Vector3(vector5.x * num9 - vector5.z * num3, 0f, vector5.z * num9 + vector5.x * num3);
                    if (flag2)
                    {
                        Singleton<ZoneManager>.instance.CreateBlock(
                            out segment.m_blockEndRight, 
                            ref randomizer, 
                            segmentID,
                            position4, 
                            angle2, 
                            num8,
                        distance,
                            segment.m_buildIndex + 1u);
                    }
                    else
                    {
                        Singleton<ZoneManager>.instance.CreateBlock(
                            out segment.m_blockEndLeft, 
                            ref randomizer, 
                            segmentID,
                            position4, 
                            angle2, 
                            num8,
                        distance,
                            segment.m_buildIndex + 1u);
                    }
                }
            }
            Vector3 vector6 = startPosition + new Vector3(startDirection.z, 0f, -startDirection.x) * num2;
            Vector3 vector7 = endPosition - new Vector3(endDirection.z, 0f, -endDirection.x) * num2;
            Vector3 b;
            Vector3 c;
            NetSegment.CalculateMiddlePoints(vector6, startDirection, vector7, endDirection, true, true, out b, out c);
            Bezier3 bezier3 = new Bezier3(vector6, b, c, vector7);
            Vector3 vector8 = bezier3.Position(0.5f);
            Vector3 vector9 = bezier3.Position(0.25f);
            vector9 = Line2.Offset(VectorUtils.XZ(vector6), VectorUtils.XZ(vector8), VectorUtils.XZ(vector9));
            Vector3 vector10 = bezier3.Position(0.75f);
            vector10 = Line2.Offset(VectorUtils.XZ(vector7), VectorUtils.XZ(vector8), VectorUtils.XZ(vector10));
            Vector3 vector11 = vector6;
            Vector3 a = vector7;
            float d;
            float num10;
            if (Line2.Intersect(VectorUtils.XZ(startPosition), VectorUtils.XZ(vector6), VectorUtils.XZ(vector11 - vector9), VectorUtils.XZ(vector8 - vector9), out d, out num10))
            {
                vector6 = startPosition + (vector6 - startPosition) * d;
            }
            if (Line2.Intersect(VectorUtils.XZ(endPosition), VectorUtils.XZ(vector7), VectorUtils.XZ(a - vector10), VectorUtils.XZ(vector8 - vector10), out d, out num10))
            {
                vector7 = endPosition + (vector7 - endPosition) * d;
            }
            if (Line2.Intersect(VectorUtils.XZ(vector11 - vector9), VectorUtils.XZ(vector8 - vector9), VectorUtils.XZ(a - vector10), VectorUtils.XZ(vector8 - vector10), out d, out num10))
            {
                vector8 = vector11 - vector9 + (vector8 - vector11) * d;
            }
            float num11;
            Vector3 vector12 = VectorUtils.NormalizeXZ(vector8 - vector6, out num11);
            int num12 = Mathf.FloorToInt(num11 / 8f + 0.01f);
            float num13 = num11 * 0.5f + (float)(num12 - 8) * ((!flag2) ? 4f : -4f);
            if (num12 != 0)
            {
                float angle3 = (!flag2) ? Mathf.Atan2(-vector12.x, vector12.z) : Mathf.Atan2(vector12.x, -vector12.z);
                Vector3 position5 = vector6 + new Vector3(vector12.x * num13 + vector12.z * num3, 0f, vector12.z * num13 - vector12.x * num3);
                if (flag2)
                {
                    Singleton<ZoneManager>.instance.CreateBlock(
                        out segment.m_blockStartLeft, 
                        ref randomizer, 
                        segmentID,
                        position5, 
                        angle3, 
                        num12,
                        distance,
                        segment.m_buildIndex);
                }
                else
                {
                    Singleton<ZoneManager>.instance.CreateBlock(
                        out segment.m_blockStartRight, 
                        ref randomizer, 
                        segmentID,
                        position5, 
                        angle3, 
                        num12,
                        distance,
                        segment.m_buildIndex);
                }
            }
            vector12 = VectorUtils.NormalizeXZ(vector7 - vector8, out num11);
            num12 = Mathf.FloorToInt(num11 / 8f + 0.01f);
            num13 = num11 * 0.5f + (float)(num12 - 8) * ((!flag2) ? 4f : -4f);
            if (num12 != 0)
            {
                float angle4 = (!flag2) ? Mathf.Atan2(-vector12.x, vector12.z) : Mathf.Atan2(vector12.x, -vector12.z);
                Vector3 position6 = vector8 + new Vector3(vector12.x * num13 + vector12.z * num3, 0f, vector12.z * num13 - vector12.x * num3);
                if (flag2)
                {
                    Singleton<ZoneManager>.instance.CreateBlock(
                        out segment.m_blockEndLeft, 
                        ref randomizer, 
                        segmentID,
                        position6, 
                        angle4, 
                        num12,
                        distance,
                        segment.m_buildIndex + 1u);
                }
                else
                {
                    Singleton<ZoneManager>.instance.CreateBlock(
                        out segment.m_blockEndRight, 
                        ref randomizer, 
                        segmentID,
                        position6, 
                        angle4, 
                        num12,
                        distance,
                        segment.m_buildIndex + 1u);
                }
            }
        }

        private static void CreateZoneBlocksTiny_Straight(NetInfo info, Randomizer randomizer, ushort segmentID, ref NetSegment segment, NetNode startNode, NetNode endNode)
        {
            var minHalfWidth = MIN_HALFWIDTH_TINY_STRAIGHT;
            float num2 = Mathf.Max(minHalfWidth, info.m_halfWidth) + 32f;
            int distance = Mathf.RoundToInt(num2);
            var cellOffset = 0f;

            if (ZoneBlocksOffset.Mode == ZoneBlocksOffsetMode.HalfCell)
            {
                cellOffset = 0.5f;
            }

            const float ROW_UNIT_SIZE = 8f;
            Vector3 startDirection = segment.m_startDirection;
            Vector3 startPosition = startNode.m_position - ROW_UNIT_SIZE * cellOffset * startDirection;
            float startAngle = Mathf.Atan2(startDirection.x, -startDirection.z);

            Vector3 endDirection = segment.m_endDirection;
            Vector3 endPosition = endNode.m_position - ROW_UNIT_SIZE * cellOffset * endDirection;
            float endAngle = Mathf.Atan2(endDirection.x, -endDirection.z);

            Vector2 magnitudeVector = new Vector2(endPosition.x - startPosition.x, endPosition.z - startPosition.z);
            float magnitude = magnitudeVector.magnitude;
            int rows = Mathf.FloorToInt(magnitude / ROW_UNIT_SIZE + 0.1f);
            int startRows = (rows <= 8) ? rows : ((rows + 1) >> 1);
            int endRows = (rows <= 8) ? 0 : (rows >> 1);

            if (startRows > 0)
            {

                Vector3 position = startPosition + new Vector3(
                    startDirection.x * 32f - startDirection.z * num2, 
                    0f, 
                    startDirection.z * 32f + startDirection.x * num2);
                Singleton<ZoneManager>.instance.CreateBlock(
                    out segment.m_blockStartLeft, 
                    ref randomizer, 
                    segmentID,
                    position, 
                    startAngle, 
                    startRows,
                        distance,
                    segment.m_buildIndex);

                position = startPosition + new Vector3(
                    startDirection.x * (float)(startRows - 4) * 8f + startDirection.z * num2, 
                    0f, 
                    startDirection.z * (float)(startRows - 4) * 8f - startDirection.x * num2);
                Singleton<ZoneManager>.instance.CreateBlock(
                    out segment.m_blockStartRight, 
                    ref randomizer,
                    segmentID,
                    position, 
                    startAngle + 3.14159274f, 
                    startRows,
                        distance,
                    segment.m_buildIndex);
            }

            if (endRows > 0)
            {
                float num18 = magnitude - (float)rows * 8f;

                Vector3 position = endPosition + new Vector3(
                    endDirection.x * (32f + num18) - endDirection.z * num2,
                    0f,
                    endDirection.z * (32f + num18) + endDirection.x * num2);
                Singleton<ZoneManager>.instance.CreateBlock(
                    out segment.m_blockEndLeft,
                    ref randomizer,
                    segmentID,
                    position,
                    endAngle,
                    endRows,
                        distance,
                    segment.m_buildIndex + 1u);

                position = endPosition + new Vector3(
                    endDirection.x * ((float)(endRows - 4) * 8f + num18) + endDirection.z * num2,
                    0f,
                    endDirection.z * ((float)(endRows - 4) * 8f + num18) - endDirection.x * num2);
                Singleton<ZoneManager>.instance.CreateBlock(
                    out segment.m_blockEndRight,
                    ref randomizer,
                    segmentID,
                    position,
                    endAngle + 3.14159274f,
                    endRows,
                        distance,
                    segment.m_buildIndex + 1u);
            }
        }
    }
}