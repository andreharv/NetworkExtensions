using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Props
{
    public class LargeAvenueMedianLightBuilder : IPrefabBuilder<PropInfo>, IModulePart, IIdentifiable
    {
        public const string NAME = "Large Avenue Median Light";

        public string Name { get { return NAME; } }
        public string BasedPrefabName { get { return "Avenue Light"; } }

        public void BuildUp(PropInfo newProp)
        {
            var newEffect = EffectCollection.FindEffect("Street Light Large Road");

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
