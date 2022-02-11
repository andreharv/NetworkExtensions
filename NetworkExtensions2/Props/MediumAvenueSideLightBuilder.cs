using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Props
{
    public class MediumAvenueSideLightBuilder : IPrefabBuilder<PropInfo>, IModulePart, IIdentifiable
    {
        public const string NAME = "Medium Avenue Side Light";

        public string Name { get { return NAME; } }
        public string BasedPrefabName { get { return "New Street Light"; } }

        public void BuildUp(PropInfo newProp)
        {
            var newEffect = EffectCollection.FindEffect("Street Light Medium Road");

            if (newEffect != null)
            {
                for (var i = 0; i < newProp.m_effects.Length; i++)
                {
                    newProp.m_effects[i].m_effect = newEffect;
                }
            }
        }
    }
}
