using ColossalFramework.UI;
using NetworkExtensions2.Roads.Common;
using System.Linq;
using Transit.Addon.RoadExtensions.Roads.Common;
using UnityEngine;

namespace NetworkExtensions2.Framework.UI
{
    internal class TestUI : UIPanel
    {
        private NetTool m_NetTool;
        private T GetExactTool<T>() where T : UnityEngine.Object => FindObjectsOfType<T>().Where(x => x.GetType() == typeof(T)).FirstOrDefault();
        public override void Start()
        {
            m_NetTool = GetExactTool<NetTool>();
            if (m_NetTool == null)
            {
                enabled = false;
                return;
            }
            CreateUI();
        }
        protected virtual void CreateUI()
        {
            backgroundSprite = "GenericPanel";
            color = new Color32(73, 68, 84, 170);
            width = 250;
            height = 250;//0
            opacity = 90;
            name = GetType().ToString();
            position = Vector2.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };
            var button = AddUIComponent<UIButton>();
            button.name = "btnTest";
            button.text = "TEST";
            button.width = 80;
            button.height = 30;
            button.position = new Vector3(100, -100);
            button.opacity = 95;
            button.normalBgSprite = "ButtonMenu";
            button.color = new Color32(150, 150, 150, 255);
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.disabledColor = new Color32(204, 204, 204, 255);
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.hoveredColor = new Color32(163, 255, 16, 255);
            button.focusedBgSprite = "ButtonMenu";
            button.focusedColor = new Color32(163, 255, 16, 255);
            button.pressedBgSprite = "ButtonMenuPressed";
            button.pressedColor = new Color32(163, 255, 16, 255);
            button.textColor = new Color32(255, 255, 255, 255);
            button.normalBgSprite = "ButtonMenu";
            button.focusedBgSprite = "ButtonMenuFocused";
            button.eventClick += (s, e) =>
            {
                Saving.SaveAsset("Testt", "Testu", "ipsev");
            };

            var button2 = AddUIComponent<UIButton>();
            button2.name = "btnTest2";
            button2.text = "TEST2";
            button2.width = 80;
            button2.height = 30;
            button2.position = new Vector3(100, -150);
            button2.opacity = 95;
            button2.normalBgSprite = "ButtonMenu";
            button2.color = new Color32(150, 150, 150, 255);
            button2.disabledBgSprite = "ButtonMenuDisabled";
            button2.disabledColor = new Color32(204, 204, 204, 255);
            button2.hoveredBgSprite = "ButtonMenuHovered";
            button2.hoveredColor = new Color32(163, 255, 16, 255);
            button2.focusedBgSprite = "ButtonMenu";
            button2.focusedColor = new Color32(163, 255, 16, 255);
            button2.pressedBgSprite = "ButtonMenuPressed";
            button2.pressedColor = new Color32(163, 255, 16, 255);
            button2.textColor = new Color32(255, 255, 255, 255);
            button2.normalBgSprite = "ButtonMenu";
            button2.focusedBgSprite = "ButtonMenuFocused";
            button2.eventClick += (s, e) =>
            {
                var tool = ToolsModifierControl.GetCurrentTool<NetTool>();
                if (tool != null)
                {
                    var info = tool.m_prefab;
                    if (info != null)
                    {
                        var segments = RoadHelper.CreateSegments(out float halfWidth, info.m_segments[0],
                            new NetStrip("Curb_3", "Curb_ConcreteSegmented_p5-6"),
                            new NetStrip("Road_3", "Road_Lane_Unmarked_1-6"),
                            new NetStrip("Road_3", "Road_Lane_Solid_1-6"),
                            new NetStrip("Median_4", "Median_ConcreteSegmented_1-6"),
                            new NetStrip("Road_4", "Road_Median_Turn_2-6", true),
                            new NetStrip("Road_3", "Road_Lane_Solid_1-6"),
                            new NetStrip("Road_3", "Road_Lane_Unmarked_1-6"),
                            new NetStrip("Curb_3", "Curb_ConcreteSegmented_p5-6")
                        );

                        info.m_halfWidth = halfWidth;
                        info.m_segments = segments;
                    }
                }
            };
        }
        public override void Update()
        {
                isVisible = m_NetTool?.m_prefab != null && m_NetTool.enabled;
        }
    }
}
