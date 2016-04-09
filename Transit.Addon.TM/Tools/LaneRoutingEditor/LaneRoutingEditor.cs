using Transit.Addon.TM.Overlays.LaneRouting;
using Transit.Framework;
using Transit.Framework.UI;
using UnityEngine;

namespace Transit.Addon.TM.Tools.LaneRoutingEditor
{
    public class LaneRoutingEditor : ToolBase
    {
        protected override void Awake()
        {
            base.Awake();

            StartCoroutine(NodeRoutesOverlay.instance.LoadMarkers());
        }

        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            if (m_toolController.IsInsideUI)
                return;

            var iEvent = new InputEvent();

            if (Input.GetKeyUp(KeyCode.PageDown))
            {
                iEvent.KeyCode = KeyCode.PageDown;
            }
            else if (Input.GetKeyUp(KeyCode.PageUp))
            {
                iEvent.KeyCode = KeyCode.PageUp;
            }

            if (Input.GetMouseButtonUp((int) MouseKeyCode.LeftButton))
            {
                iEvent.MouseKeyCode = MouseKeyCode.LeftButton;
            }
            else if (Input.GetMouseButtonUp((int)MouseKeyCode.RightButton))
            {
                iEvent.MouseKeyCode = MouseKeyCode.RightButton;
            }

            NodeRoutesOverlay.instance.Update(iEvent);
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            NodeRoutesOverlay.instance.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            NodeRoutesOverlay.instance.Disable();
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            NodeRoutesOverlay.instance.Render(cameraInfo);
        }
    }
}
