using ColossalFramework.UI;
using System;
using Transit.Addon.TM.TrafficLight;
using Transit.Addon.TM;
using Transit.Addon.TM.Tools;
using UnityEngine;

namespace Transit.Addon.TM.UI {
	public class TMBaseUI : UICustomControl {
		private bool _uiShown;

		public TMBaseUI() {
			Log._Debug("##### Initializing UIBase.");

			// Get the UIView object. This seems to be the top-level object for most
			// of the UI.
			var uiView = UIView.GetAView();

			// Add a new button to the view.
			var button = (UIButton)uiView.AddUIComponent(typeof(UIButton));

			// Set the text to show on the button.
			button.text = "Traffic President";

			// Set the button dimensions.
			button.width = 150;
			button.height = 30;

			// Style the button to look like a menu button.
			button.normalBgSprite = "ButtonMenu";
			button.disabledBgSprite = "ButtonMenuDisabled";
			button.hoveredBgSprite = "ButtonMenuHovered";
			button.focusedBgSprite = "ButtonMenuFocused";
			button.pressedBgSprite = "ButtonMenuPressed";
			button.textColor = new Color32(255, 255, 255, 255);
			button.disabledTextColor = new Color32(7, 7, 7, 255);
			button.hoveredTextColor = new Color32(7, 132, 255, 255);
			button.focusedTextColor = new Color32(255, 255, 255, 255);
			button.pressedTextColor = new Color32(30, 30, 44, 255);

			// Enable button sounds.
			button.playAudioEvents = true;

			// Place the button.
			button.relativePosition = new Vector3(180f, 20f);

			// Respond to button click.
			button.eventClick += ButtonClick;
		}

		private void ButtonClick(UIComponent uiComponent, UIMouseEventParameter eventParam) {
			if (!_uiShown) {
				Show();
			} else {
				Close();
			}
		}

		public bool IsVisible() {
			return _uiShown;
		}

		public void Show() {
			if (ToolModuleV2.Instance != null) {
				try {
					ToolsModifierControl.mainToolbar.CloseEverything();
				} catch (Exception e) {
					Log.Error("Error on Show(): " + e.ToString());
				}
				var uiView = UIView.GetAView();
				var trafficManager = uiView.FindUIComponent("TMMenuUI");
				if (trafficManager != null) {
					Log._Debug("Showing TM UI");
					trafficManager.Show();
				} else {
					Log._Debug("Showing TM UI: create");
					uiView.AddUIComponent(typeof(TMMenuUI));
				}
				ToolModuleV2.Instance.SetToolMode(Mode.Activated);
				_uiShown = true;
			} else {
				Log._Debug("TM UI Show: LoadingExtension.Instance is null!");
			}
		}

		public void Close() {
			if (ToolModuleV2.Instance != null) {
				var uiView = UIView.GetAView();
				var trafficManager = uiView.FindUIComponent("TMMenuUI");

				if (trafficManager != null) {
					Log._Debug("Hiding TM UI");
					trafficManager.Hide();
				} else {
					Log._Debug("Hiding TM UI: null!");
				}

				TMMenuUI.deactivateButtons();
				TrafficManagerTool.SetToolMode(TrafficManagerToolMode.None);
				ToolModuleV2.Instance.SetToolMode(Mode.Disabled);

				_uiShown = false;
			} else {
				Log._Debug("TM UI Close: LoadingExtension.Instance is null!");
			}
		}
	}
}
