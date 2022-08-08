
using System.Collections.Generic;
using System.Threading;

namespace NetworkExtensions2.Framework
{
    internal class Threading
    {
        public enum AreType
        {
            Model,
            Texture
        }
        private static Dictionary<AreType, AutoResetEvent> m_AreDict;
        protected static Dictionary<AreType, AutoResetEvent> AreDict
        {
            get
            { 
                if (m_AreDict == null)
                {
                    m_AreDict = new Dictionary<AreType,AutoResetEvent>();
                }
                return m_AreDict;
            }
            set
            {
                m_AreDict = value;
            }
        }

        public static void SetAre(AreType areType)
        {
            if (!AreDict.ContainsKey(areType))
                AreDict.Add(areType, new AutoResetEvent(true));
            AreDict[areType].Set();
        }

        public static void WaitOneAre(AreType areType)
        {
            if (!AreDict.ContainsKey(areType))
                AreDict.Add(areType, new AutoResetEvent(true));
            AreDict[areType].WaitOne();
        }
    }
}
