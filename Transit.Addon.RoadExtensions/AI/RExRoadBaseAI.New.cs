using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.AI
{
    public partial class RExRoadBaseAI : RoadBaseAI
    {
        private static void CreateZoneBlocksNew(NetInfo info, ushort segment, ref NetSegment data)
        {
            var netManager = Singleton<NetManager>.instance;
            var zoneManager = Singleton<ZoneManager>.instance;
            var randomizer = new Randomizer((int)segment);


            const float halfRotation = 3.14159274f;

            const int blockSize = 4;
            const int roadUnitSize = blockSize;

            var halfWidth = Mathf.Max(roadUnitSize, info.m_halfWidth);

            var startPosition = netManager.m_nodes.m_buffer[(int)data.m_startNode].m_position;
            var endPosition = netManager.m_nodes.m_buffer[(int)data.m_endNode].m_position;

            var startDirection = data.m_startDirection;
            var endDirection = data.m_endDirection;

            var num = startDirection.x * endDirection.x + startDirection.z * endDirection.z;
            var isCurve = !NetSegment.IsStraight(startPosition, startDirection, endPosition, endDirection);
            var num3 = 32f;

            if (isCurve)
            {
                #region Curve
                var length = VectorUtils.LengthXZ(endPosition - startPosition);
                var flag2 = startDirection.x * endDirection.z - startDirection.z * endDirection.x > 0f;
                var flag3 = num < -0.8f || length > 50f;
                if (flag2)
                {
                    halfWidth = -halfWidth;
                    num3 = -num3;
                }
                var vector = startPosition - new Vector3(startDirection.z, 0f, -startDirection.x) * halfWidth;
                var endPosition2 = endPosition + new Vector3(endDirection.z, 0f, -endDirection.x) * halfWidth;
                Vector3 middlePosition1;
                Vector3 middlePosition2;
                NetSegment.CalculateMiddlePoints(vector, startDirection, endPosition2, endDirection, true, true, out middlePosition1, out middlePosition2);
                if (flag3)
                {
                    float num5 = num * 0.025f + 0.04f;
                    float num6 = num * 0.025f + 0.06f;
                    if (num < -0.9f)
                    {
                        num6 = num5;
                    }
                    Bezier3 bezier = new Bezier3(vector, middlePosition1, middlePosition2, endPosition2);
                    vector = bezier.Position(num5);
                    middlePosition1 = bezier.Position(0.5f - num6);
                    middlePosition2 = bezier.Position(0.5f + num6);
                    endPosition2 = bezier.Position(1f - num5);
                }
                else
                {
                    Bezier3 bezier2 = new Bezier3(vector, middlePosition1, middlePosition2, endPosition2);
                    middlePosition1 = bezier2.Position(0.86f);
                    vector = bezier2.Position(0.14f);
                }
                float num7;
                Vector3 vector5 = VectorUtils.NormalizeXZ(middlePosition1 - vector, out num7);
                int num8 = Mathf.FloorToInt(num7 / 8f + 0.01f);
                float num9 = num7 * 0.5f + (float)(num8 - 8f) * ((!flag2) ? -4f : 4f);
                if (num8 != 0)
                {
                    float angle = (!flag2) ? Mathf.Atan2(vector5.x, -vector5.z) : Mathf.Atan2(-vector5.x, vector5.z);
                    Vector3 position3 = vector + new Vector3(vector5.x * num9 - vector5.z * num3, 0f, vector5.z * num9 + vector5.x * num3);
                    if (flag2)
                    {
                        Singleton<ZoneManager>.instance.CreateBlock(out data.m_blockStartRight, ref randomizer, position3, angle, num8, data.m_buildIndex);
                    }
                    else
                    {
                        Singleton<ZoneManager>.instance.CreateBlock(out data.m_blockStartLeft, ref randomizer, position3, angle, num8, data.m_buildIndex);
                    }
                }
                if (flag3)
                {
                    vector5 = VectorUtils.NormalizeXZ(endPosition2 - middlePosition2, out num7);
                    num8 = Mathf.FloorToInt(num7 / 8f + 0.01f);
                    num9 = num7 * 0.5f + (float)(num8 - 8f) * ((!flag2) ? -4f : 4f);
                    if (num8 != 0)
                    {
                        float angle2 = (!flag2) ? Mathf.Atan2(vector5.x, -vector5.z) : Mathf.Atan2(-vector5.x, vector5.z);
                        Vector3 position4 = middlePosition2 + new Vector3(vector5.x * num9 - vector5.z * num3, 0f, vector5.z * num9 + vector5.x * num3);
                        if (flag2)
                        {
                            Singleton<ZoneManager>.instance.CreateBlock(out data.m_blockEndRight, ref randomizer, position4, angle2, num8, data.m_buildIndex + 1u);
                        }
                        else
                        {
                            Singleton<ZoneManager>.instance.CreateBlock(out data.m_blockEndLeft, ref randomizer, position4, angle2, num8, data.m_buildIndex + 1u);
                        }
                    }
                }
                Vector3 vector6 = startPosition + new Vector3(startDirection.z, 0f, -startDirection.x) * halfWidth;
                Vector3 vector7 = endPosition - new Vector3(endDirection.z, 0f, -endDirection.x) * halfWidth;
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
                        Singleton<ZoneManager>.instance.CreateBlock(out data.m_blockStartLeft, ref randomizer, position5, angle3, num12, data.m_buildIndex);
                    }
                    else
                    {
                        Singleton<ZoneManager>.instance.CreateBlock(out data.m_blockStartRight, ref randomizer, position5, angle3, num12, data.m_buildIndex);
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
                        Singleton<ZoneManager>.instance.CreateBlock(out data.m_blockEndLeft, ref randomizer, position6, angle4, num12, data.m_buildIndex + 1u);
                    }
                    else
                    {
                        Singleton<ZoneManager>.instance.CreateBlock(out data.m_blockEndRight, ref randomizer, position6, angle4, num12, data.m_buildIndex + 1u);
                    }
                }
                #endregion
            }
            else
            {
                CreateZoneBlocksNew_Straight(info, segment, ref data);
            }
        }


        private static void CreateZoneBlocksNew_Straight(
            NetInfo info, ushort segmentId, ref NetSegment segment)
        {
            var netManager = Singleton<NetManager>.instance;
            var zoneManager = Singleton<ZoneManager>.instance;
            var randomizer = new Randomizer((int)segmentId);

            const float BLOCK_SIZE = 4f;
            const float ROAD_UNIT_SIZE = 4f;
            const float X_OFFSET_BASE = 32f + 4f;
            const float Z_OFFSET_BASE = 32f;

            var xOffset = X_OFFSET_BASE;
            var zOffset = Z_OFFSET_BASE + Mathf.Max(ROAD_UNIT_SIZE, info.m_halfWidth);

            var startPosition = netManager.m_nodes.m_buffer[(int)segment.m_startNode].m_position;
            var endPosition = netManager.m_nodes.m_buffer[(int)segment.m_endNode].m_position;

            var startDirection = segment.m_startDirection;

            var lengthVector = new Vector2(endPosition.x - startPosition.x, endPosition.z - startPosition.z);
            var length = lengthVector.magnitude;

            var nbBlock = Mathf.FloorToInt(length / BLOCK_SIZE + 0.1f);
            var startRows = (nbBlock <= ROAD_UNIT_SIZE) ? nbBlock : (nbBlock + 1 / 2);

            if (startRows > 0)
            {
                var angle = Mathf.Atan2(startDirection.x, -startDirection.z);

                // BlockStartLeft -------------------------------------------------------------
                var position = startPosition + new Vector3(
                    xOffset,
                    0f,
                    zOffset);

                //+ new Vector3(
                //startDirection.x * blockSize * 8f - startDirection.z * zOffset, 
                //0f,
                //startDirection.z * blockSize * 8f + startDirection.x * zOffset);

                zoneManager.CreateBlock(
                    out segment.m_blockStartLeft,
                    ref randomizer,
                    position,
                    angle,
                    startRows,
                    segment.m_buildIndex);

                // BlockStartRight --------------------------------------------------------
                //position = startPosition + new Vector3(
                //    startDirection.x * (float)(rows1 - 4) * 8f + startDirection.z * zOffset,
                //    0f,
                //    startDirection.z * (float)(rows1 - 4) * 8f - startDirection.x * zOffset);
                //zoneManager.CreateBlock(
                //    out data.m_blockStartRight,
                //    ref randomizer,
                //    position,
                //    angle + halfRotation,
                //    rows1,
                //    data.m_buildIndex);
            }

            var endRows = (nbBlock <= ROAD_UNIT_SIZE) ? 0 : (nbBlock / 2);
            if (endRows > 0)
            {
                //var num18 = length - (float)nbUnit * 8f;
                //var num19 = Mathf.Atan2(endDirection.x, -endDirection.z);
                //var position8 = endPosition + new Vector3(endDirection.x * (32f + num18) - endDirection.z * zOffset, 0f, endDirection.z * (32f + num18) + endDirection.x * zOffset);

                //zoneManager.CreateBlock(out data.m_blockEndLeft, ref randomizer, position8, num19, rows2, data.m_buildIndex + 1u);
                //position8 = endPosition + new Vector3(endDirection.x * ((float)(rows2 - 4) * 8f + num18) + endDirection.z * zOffset, 0f, endDirection.z * ((float)(rows2 - 4) * 8f + num18) - endDirection.x * zOffset);
                //zoneManager.CreateBlock(out data.m_blockEndRight, ref randomizer, position8, num19 + halfRotation, rows2, data.m_buildIndex + 1u);
            }
        }
    }
}