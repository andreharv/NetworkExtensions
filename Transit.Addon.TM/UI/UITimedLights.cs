using ColossalFramework.UI;
using Transit.Addon.TM.TrafficLight;
using Transit.Addon.TM;
using Transit.Addon.TM.Tools;
using UnityEngine;

namespace Transit.Addon.TM.UI
{
    class UITimedLights : UIPanel
    {
        public static TrafficManagerTool TrafficLightTool;

        public override void Start()
        {
            TrafficLightTool = ToolModuleV2.Instance.TrafficManagerTool;
            //this makes the panel "visible", I don't know what sprites are available, but found this value to work
            backgroundSprite = "GenericPanel";
            color = new Color32(75, 75, 135, 255);
            width = 800;
            height = 400;
            relativePosition = new Vector3(10.48f, 80f);

            var l = AddUIComponent<UILabel>();
            l.text = "Timed Scripts";
            l.relativePosition = new Vector3(65.0f, 5.0f);
        }
    }
}
