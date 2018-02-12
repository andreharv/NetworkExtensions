namespace Transit.Framework.UI
{
    using System;
    using ColossalFramework.UI;
    using UnityEngine;
    using System.Linq;
    using System.Collections.Generic;
    public class ParkingRoadCustomizerUI : UIPanel
    {
        public int trackStyle = 0;

        private BulldozeTool m_bulldozeTool;
        private NetTool m_netTool;
        private UIButton m_upgradeButtonTemplate;
        private NetInfo m_currentNetInfo;
        private bool m_activated = false;
        public static ParkingRoadCustomizerUI instance;

        private UIButton btnParkingInfo;
        private UIButton btnGrassInfo;
        private UIButton btnTreeInfo;
        private UIButton btnPavementInfo;

        public override void Awake()
        {

        }

        public override void Update()
        {
            if (m_netTool == null)
            {
                return;
            }
            try
            {
                var toolInfo = m_netTool.enabled ? m_netTool.m_prefab : null;
                if (toolInfo == m_currentNetInfo)
                {
                    return;
                }
                NetInfo finalInfo = null;
                if (toolInfo != null)
                {
                    //RestoreStationTrackStyle(toolInfo);
                    NetInfo basedPrefab = null;
                    try
                    {
                        var basedName = toolInfo.StripName();
                        basedPrefab = Prefabs.Find<NetInfo>(basedName);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Stripped prefab {toolInfo.StripName()} Could not be loaded");
                        Debug.Log(ex);
                    }
                    if (basedPrefab != null && basedPrefab.m_hasParkingSpaces)
                    {
                        finalInfo = toolInfo;
                    }
                }
                if (finalInfo == m_currentNetInfo)
                {
                    return;
                }
                if (finalInfo != null)
                {
                    Activate(finalInfo);
                }
                else
                {
                    Deactivate();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                Deactivate();
            }

        }
        public override void Start()
        {
            m_netTool = FindObjectOfType<NetTool>();
            if (m_netTool == null)
            {
#if DEBUG
                Debug.Log("NetTool Not Found");
#endif
                enabled = false;
                return;
            }

            m_bulldozeTool = FindObjectOfType<BulldozeTool>();
            if (m_bulldozeTool == null)
            {
#if DEBUG
                Debug.Log("BulldozeTool Not Found");
#endif
                enabled = false;
                return;
            }
            m_netTool = FindObjectOfType<NetTool>();
            if (m_netTool == null)
            {
#if DEBUG
                Debug.Log("NetTool Not Found");
#endif
                enabled = false;
                return;
            }

            try
            {
                m_upgradeButtonTemplate = GameObject.Find("RoadsSmallPanel").GetComponent<GeneratedScrollPanel>().m_OptionsBar.Find<UIButton>("Upgrade");
            }
            catch
            {
#if DEBUG
                Debug.Log("Upgrade button template not found");
#endif
            }

            CreateUI();
            trackStyle = 0;
            SetNetToolPrefab();
        }

        private void CreateUI()
        {
#if DEBUG
            Debug.Log("Parking Road GUI Created");
#endif

            backgroundSprite = "GenericPanel";
            color = new Color32(73, 68, 84, 170);
            width = 200;
            height = 150;
            opacity = 90;
            position = Vector2.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            UIPanel dragHandlePanel = AddUIComponent<UIPanel>();
            dragHandlePanel.atlas = atlas;
            dragHandlePanel.backgroundSprite = "GenericPanel";
            dragHandlePanel.width = width;
            dragHandlePanel.height = 20;
            dragHandlePanel.opacity = 100;
            dragHandlePanel.color = new Color32(21, 34, 140, 255);
            dragHandlePanel.relativePosition = Vector3.zero;

            UIDragHandle dragHandle = dragHandlePanel.AddUIComponent<UIDragHandle>();
            dragHandle.width = width;
            dragHandle.height = dragHandle.parent.height;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;

            UILabel titleLabel = dragHandlePanel.AddUIComponent<UILabel>();
            titleLabel.relativePosition = new Vector3() { x = 5, y = 5, z = 0 };
            titleLabel.textAlignment = UIHorizontalAlignment.Center;
            titleLabel.text = "Parking Space Options";
            titleLabel.isInteractive = false;

            btnParkingInfo = CreateButton("Parking", new Vector3(8, 50), (c, v) =>
            {
                trackStyle = 0;
                SetNetToolPrefab();
            });

            btnPavementInfo = CreateButton("Paved", new Vector3(8 + (0.5f * width) - 16, 50), (c, v) =>
            {
                trackStyle = 1;
                SetNetToolPrefab();
            });
            btnGrassInfo = CreateButton("Grass", new Vector3(8, 100), (c, v) =>
            {
                trackStyle = 2;
                SetNetToolPrefab();
            });

            btnTreeInfo = CreateButton("Trees", new Vector3(8 + (0.5f * width) - 16, 100), (c, v) =>
            {
                trackStyle = 3;
                SetNetToolPrefab();
            });
        }

        private UIButton CreateButton(string text, Vector3 pos, MouseEventHandler eventClick)
        {
            var button = this.AddUIComponent<UIButton>();
            button.width = 80;
            button.height = 30;
            button.normalBgSprite = "ButtonMenu";
            button.color = new Color32(150, 150, 150, 255);
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.hoveredColor = new Color32(163, 16, 255, 255);
            button.focusedBgSprite = "ButtonMenu";
            button.focusedColor = new Color32(163, 16, 255, 255);
            button.pressedBgSprite = "ButtonMenuPressed";
            button.pressedColor = new Color32(163, 16, 255, 255);
            button.textColor = new Color32(255, 255, 255, 255);
            button.normalBgSprite = "ButtonMenu";
            button.focusedBgSprite = "ButtonMenuFocused";
            button.playAudioEvents = true;
            button.opacity = 75;
            button.dropShadowColor = new Color32(0, 0, 0, 255);
            button.dropShadowOffset = new Vector2(-1, -1);
            button.text = text;
            button.relativePosition = pos;
            button.eventClick += eventClick;

            return button;
        }
        private void SetBtn(UIButton btn)
        {
            btn.color = new Color32(163, 16, 255, 255);
            btn.normalBgSprite = "ButtonMenuFocused";
            btn.useDropShadow = true;
            btn.opacity = 95;
        }
        private void SetBtnsBack()
        {
            var btnList = new List<UIButton>() { btnParkingInfo, btnPavementInfo, btnGrassInfo, btnTreeInfo };
            for (var i = 0; i < btnList.Count(); i++)
            {
                var btn = btnList[i];
                btn.color = new Color32(150, 150, 150, 255);
                btn.normalBgSprite = "ButtonMenu";
                btn.useDropShadow = false;
                btn.opacity = 75;
            }
        }
        private void SetNetToolPrefab()
        {
            SetBtnsBack();
            switch (trackStyle)
            {
                case 0:
                    SetBtn(btnParkingInfo);
                    break;
                case 1:
                    SetBtn(btnPavementInfo);
                    break;
                case 2:
                    SetBtn(btnGrassInfo);
                    break;
                case 3:
                    SetBtn(btnTreeInfo);
                    break;
            }
            NetInfo prefab = m_netTool.m_prefab;
            if (prefab != null)
            {
            var prefabName  = prefab.StripName();
            switch (trackStyle)
            {
                case 1:
                    prefabName += " Decoration Pavement";
                    break;
                case 2:
                    prefabName += " Decoration Grass";
                    break;
                case 3:
                    prefabName += " Decoration Trees";
                    break;
            }
            var chosenPrefab = Prefabs.Find<NetInfo>(prefabName);
            if (chosenPrefab == null)
            {
            }
            else
            {
                m_netTool.m_prefab = chosenPrefab;
            }
            }

        }
        
        private void Activate(NetInfo nInfo)
        {
            m_activated = true;
            m_currentNetInfo = nInfo;
            isVisible = true;
            SetNetToolPrefab();
        }
        private void Deactivate()
        {
            if (!m_activated)
            {
                return;
            }
            m_currentNetInfo = null;
            isVisible = false;
            m_activated = false;
        }
    }
}