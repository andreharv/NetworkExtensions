using ColossalFramework.DataBinding;
using ColossalFramework.PlatformServices;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Transit.Framework
{
    public class NotificationPanel : DisclaimerPanel
    {
        protected static NotificationPanel s_instance;

        protected UILabel _title;
        protected UILabel _description;
        protected UIScrollbar _scrollbar;
        protected UIButton _okButton;
        protected UIButton _cancelButton;
        protected UICheckBox _dontShowAgainCheckbox;
        Material[] m_fireworkMaterials;
        ParticleSystem m_fireworks;

        public static NotificationPanel Panel
        {
            get
            {
                if (s_instance == null)
                {
                    DisclaimerPanel disclaimerPanel = Resources.FindObjectsOfTypeAll<DisclaimerPanel>().FirstOrDefault();
                    if (disclaimerPanel == null)
                    {
                        //Logger.LogError("Disclaimer Panel not found.");
                        return null;
                    }

                    UIPanel notificationUIPanel = UIView.GetAView().AttachUIComponent(GameObject.Instantiate(disclaimerPanel.gameObject)) as UIPanel;
                    GameObject.Destroy(notificationUIPanel.GetComponent<DisclaimerPanel>());
                    NotificationPanel notificationPanel = notificationUIPanel.gameObject.AddComponent<NotificationPanel>();
                    if (notificationPanel.InitPanel())
                    {
                        s_instance = notificationPanel;
                    }
                    else
                    {
                        GameObject.Destroy(notificationUIPanel);
                    }
                            
                }

                return s_instance;
            }
        }

        public virtual bool InitPanel()
        {
            try
            {
                // Get Title
                _title = transform.Find("Caption").GetComponent<UILabel>();

                // Get Description
                _description = transform.Find("Scrollable Panel/Message").GetComponent<UILabel>();

                // Remove Close Button
                GameObject.Destroy(transform.Find("Caption/Close").gameObject);

                // Hide scrollbar
                _scrollbar = transform.Find("Scrollbar").GetComponent<UIScrollbar>();
                _scrollbar.opacity = 0;

                // Change Dont show text
                _dontShowAgainCheckbox = transform.Find("DontShow").GetComponent<UICheckBox>();
                transform.Find("DontShow/OnOff").GetComponent<UILabel>().text = "Don't show on future updates";

                // Change ok to Options
                _okButton = transform.Find("Ok").GetComponent<UIButton>();
                GameObject.Destroy(_okButton.GetComponent<BindEvent>());

                // Change cancel to Later
                _cancelButton = transform.Find("Cancel").GetComponent<UIButton>();
                GameObject.Destroy(_cancelButton.GetComponent<BindEvent>());

                // Get fireworks
                UnlockingPanel paradoxPanel = UIView.GetAView().GetComponentInChildren<UnlockingPanel>();
                if (paradoxPanel != null)
                {
                    m_fireworks = paradoxPanel.m_Fireworks;
                    if (m_fireworks != null)
                    {
                        Renderer[] componentsInChildren = this.m_fireworks.GetComponentsInChildren<Renderer>();
                        m_fireworkMaterials = new Material[componentsInChildren.Length];
                        for (int i = 0; i < componentsInChildren.Length; ++i)
                        {
                            m_fireworkMaterials[i] = componentsInChildren[i].material;
                        }
                    }
                }

                // Add panel to dynamic panels library
                this.name = this.GetType().Name;
                UIDynamicPanels.DynamicPanelInfo panelInfo = new UIDynamicPanels.DynamicPanelInfo();
                typeof(UIDynamicPanels.DynamicPanelInfo).GetFieldByName("m_Name").SetValue(panelInfo, this.name);
                typeof(UIDynamicPanels.DynamicPanelInfo).GetFieldByName("m_PanelRoot").SetValue(panelInfo, this.GetComponent<UIPanel>());
                typeof(UIDynamicPanels.DynamicPanelInfo).GetFieldByName("m_IsModal").SetValue(panelInfo, true);
                panelInfo.viewOwner = UIView.GetAView();
                panelInfo.AddInstance(this.GetComponent<UIPanel>());

                Dictionary<string, UIDynamicPanels.DynamicPanelInfo> cachedPanels = typeof(UIDynamicPanels).GetFieldByName("m_CachedPanels").GetValue(UIView.library) as Dictionary<string, UIDynamicPanels.DynamicPanelInfo>;
                if (!cachedPanels.ContainsKey(panelInfo.name))
                    cachedPanels.Add(panelInfo.name, panelInfo);

                return true;
            }
            catch (Exception)
            {
                // TODO: log error
                return false;
            }
        }

        public virtual void Show(string title, string description, bool showScrollbar, string okButtonText, Action okCallback, string cancelButtonText, Action cancelCallback, bool playFireworks)
        {
            _title.text = title;
            _description.text = description;
            _scrollbar.opacity = showScrollbar ? 1 : 0;

            // self-unsubscribing events
            MouseEventHandler okEventHandler = null;
            MouseEventHandler cancelEventHandler = null;

            if (okCallback != null)
            {
                okEventHandler = (c, e) =>
                {
                    okCallback();
                    _okButton.eventClick -= okEventHandler;
                    _cancelButton.eventClick -= cancelEventHandler;
                };
                _okButton.text = okButtonText;
                _okButton.eventClick += okEventHandler;
            }

            if (cancelCallback != null)
            {
                cancelEventHandler = (c, e) =>
                {
                    cancelCallback();
                    _okButton.eventClick -= okEventHandler;
                    _cancelButton.eventClick -= cancelEventHandler;
                };
                _cancelButton.text = cancelButtonText;
                _cancelButton.eventClick += cancelEventHandler;
            }

            UIView.library.ShowModal(this.name);

            if (m_fireworks != null && playFireworks)
                m_fireworks.Play();
        }

        public virtual void Show(string title, string description, bool showScrollbar, bool playFireworks)
        {
            _title.text = title;
            _description.text = description;
            _scrollbar.opacity = showScrollbar ? 1 : 0;
            _okButton.text = "OK";
            _cancelButton.enabled = _cancelButton.isVisible = false;
            _dontShowAgainCheckbox.enabled = _dontShowAgainCheckbox.isVisible = false;

            _okButton.eventClicked += (UIComponent component, UIMouseEventParameter eventParam) =>
            {
                OnClosed();
            };

            UIView.library.ShowModal(this.name);

            if (m_fireworks != null && playFireworks)
                m_fireworks.Play();
        }

        public virtual void Show(string title, string description, bool showScrollbar, string url, string urlButtonText, bool playFireworks)
        {
            _title.text = title;
            _description.text = description;
            _scrollbar.opacity = showScrollbar ? 1 : 0;
            _okButton.text = "OK";
            _cancelButton.text = urlButtonText;
            _dontShowAgainCheckbox.enabled = _dontShowAgainCheckbox.isVisible = false;

            _okButton.eventClicked += (UIComponent component, UIMouseEventParameter eventParam) =>
            {
                OnClosed();
            };

            _cancelButton.eventClicked += (UIComponent component, UIMouseEventParameter eventParam) =>
            {
                OnClosed();
                PlatformService.ActivateGameOverlayToWebPage(url);
            };

            UIView.library.ShowModal(this.name);

            if (m_fireworks != null && playFireworks)
                m_fireworks.Play();
        }
    }
}
