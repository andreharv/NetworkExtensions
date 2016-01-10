using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Props
{
    public class BusLaneTextPropBuilder : IPrefabBuilder<PropInfo>, IModulePart, IIdentifiable
    {
        public string Name { get { return "BusLaneText"; } }
        public string BasedPrefabName { get { return "Road Arrow F"; } }

        public void BuildUp(PropInfo newProp)
        {
            var renderer = newProp.GetComponent<Renderer>();
            newProp.m_useColorVariations = false;

            Material newMat = new Material(renderer.sharedMaterial);
            newMat.SetTexture("_MainTex", AssetManager.instance.GetTexture(@"Props\Textures\BusLaneText.png", TextureType.Default));
            newMat.SetTexture("_ACIMap", AssetManager.instance.GetTexture(@"Props\Textures\BusLaneText-aci.png", TextureType.Default));
            newMat.SetTexture("_XYSMap", AssetManager.instance.GetTexture(@"Props\Textures\BusLaneText-xys.png", TextureType.Default));
            newMat.color = new Color32(255, 255, 255, 100);
            renderer.sharedMaterial = newMat;
        }
    }
}
