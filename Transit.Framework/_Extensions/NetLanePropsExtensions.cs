using System.Collections.Generic;
using System.Linq;
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
                newNLP.m_props = nLP.m_props.Where(p => p != null).Select(p => p.ShallowClone()).ToArray();
            }

            return newNLP;
        }
    }
}
