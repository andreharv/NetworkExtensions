namespace ObjUnity3D
{
    public class OBJFaceVertex
    {
        public int m_VertexIndex = -1;

        public int m_UVIndex = -1;

        public int m_UV2Index = -1;

        public int m_NormalIndex = -1;

        public int m_ColorIndex = -1;

        public override int GetHashCode()
        {
            return this.m_VertexIndex ^ this.m_UVIndex ^ this.m_UV2Index ^ this.m_NormalIndex ^ this.m_ColorIndex;
        }

        public override bool Equals(object obj)
        {
            OBJFaceVertex oBJFaceVertex = (OBJFaceVertex)obj;
            return this.m_VertexIndex == oBJFaceVertex.m_VertexIndex && this.m_UVIndex == oBJFaceVertex.m_UVIndex && this.m_UV2Index == oBJFaceVertex.m_UV2Index && this.m_NormalIndex == oBJFaceVertex.m_NormalIndex && this.m_ColorIndex == oBJFaceVertex.m_ColorIndex;
        }
    }
}
