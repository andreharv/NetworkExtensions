using System.Collections.Generic;
using UnityEngine;

namespace Transit.Framework
{
    public static class NetLanePropsExtensions
    {
        public static NetLaneProps Clone(this NetLaneProps nLP, string newName = null)
        {
            var newNLP = ScriptableObject.CreateInstance<NetLaneProps>();

            if (newName != null)
            {
                newNLP.name = newName;
            }

            if (nLP.m_props == null)
            {
                newNLP.m_props = new NetLaneProps.Prop[0];
            }
            else
            {
                var newList = new List<NetLaneProps.Prop>();
                newList.AddRange(nLP.m_props);
                newNLP.m_props = newList.ToArray();
            }

            return newNLP;
        }
    }
}
