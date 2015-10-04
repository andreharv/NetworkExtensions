using System.Collections.Generic;

namespace ObjUnity3D
{
    public class OBJGroup
    {
        private readonly List<OBJFace> m_Faces = new List<OBJFace>();

        public string m_Name
        {
            get;
            private set;
        }

        public OBJMaterial m_Material
        {
            get;
            set;
        }

        public IList<OBJFace> Faces
        {
            get
            {
                return this.m_Faces;
            }
        }

        public OBJGroup(string lName)
        {
            this.m_Name = lName;
        }

        public void AddFace(OBJFace lFace)
        {
            this.m_Faces.Add(lFace);
        }
    }
}
