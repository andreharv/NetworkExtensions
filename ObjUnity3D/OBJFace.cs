using System;
using System.Collections.Generic;

namespace ObjUnity3D
{
    public class OBJFace
    {
        private readonly List<OBJFaceVertex> m_Vertices = new List<OBJFaceVertex>();

        public OBJFaceVertex this[int i]
        {
            get
            {
                return this.m_Vertices[i];
            }
        }

        public int Count
        {
            get
            {
                return this.m_Vertices.Count;
            }
        }

        public void AddVertex(OBJFaceVertex lVertex)
        {
            this.m_Vertices.Add(lVertex);
        }

        public void ParseVertex(string lVertexString)
        {
            string[] array = lVertexString.Split(new char[]
            {
                '/'
            }, StringSplitOptions.None);
            int num = array[0].ParseInvariantInt();
            OBJFaceVertex oBJFaceVertex = new OBJFaceVertex
            {
                m_VertexIndex = num - 1
            };
            if (array.Length > 1)
            {
                num = ((array[1].Length == 0) ? 0 : array[1].ParseInvariantInt());
                oBJFaceVertex.m_UVIndex = num - 1;
            }
            if (array.Length > 2)
            {
                num = ((array[2].Length == 0) ? 0 : array[2].ParseInvariantInt());
                oBJFaceVertex.m_NormalIndex = num - 1;
            }
            if (array.Length > 3)
            {
                num = ((array[3].Length == 0) ? 0 : array[3].ParseInvariantInt());
                oBJFaceVertex.m_UV2Index = num - 1;
            }
            if (array.Length > 4)
            {
                num = ((array[4].Length == 0) ? 0 : array[4].ParseInvariantInt());
                oBJFaceVertex.m_ColorIndex = num - 1;
            }
            this.AddVertex(oBJFaceVertex);
        }

        public string ToString(int lIndex)
        {
            OBJFaceVertex oBJFaceVertex = this.m_Vertices[lIndex];
            string text = (oBJFaceVertex.m_VertexIndex + 1).ToString();
            if (oBJFaceVertex.m_UVIndex > -1)
            {
                text += string.Format("/{0}", (oBJFaceVertex.m_UVIndex + 1).ToString());
            }
            if (oBJFaceVertex.m_NormalIndex > -1)
            {
                text += string.Format("/{0}", (oBJFaceVertex.m_NormalIndex + 1).ToString());
            }
            if (oBJFaceVertex.m_UV2Index > -1)
            {
                text += string.Format("/{0}", (oBJFaceVertex.m_UV2Index + 1).ToString());
            }
            if (oBJFaceVertex.m_ColorIndex > -1)
            {
                text += string.Format("/{0}", (oBJFaceVertex.m_ColorIndex + 1).ToString());
            }
            return text;
        }
    }
}
