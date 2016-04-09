using ColossalFramework;
using UnityEngine;

namespace Transit.Framework.UI.Ingame
{
    public abstract class UIMarker
    {
        protected UIState UIState { get; private set; }

        protected UIMarker()
        {
            UIState = UIState.Default;
        }

        public bool IsEnabled
        {
            get { return !UIState.IsFlagSet(UIState.Disabled); }
        }

        public void SetEnable(bool isEnable)
        {
            if (isEnable)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        public void Enable()
        {
            if (UIState.IsFlagSet(UIState.Disabled))
            {
                UIState = UIState & ~UIState.Disabled;
            }
        }

        public void Disable()
        {
            UIState = UIState.Disabled;
        }

        public bool IsHovered
        {
            get { return UIState.IsFlagSet(UIState.Hovered); }
        }

        public void HoveringStarted()
        {
            if (!IsEnabled)
            {
                return;
            }

            switch (UIState)
            {
                case UIState.Default:
                    UIState = UIState.Hovered;
                    break;
                case UIState.Selected:
                    UIState = UIState.Selected | UIState.Hovered;
                    break;
            }
        }

        public void HoveringEnded()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (UIState.IsFlagSet(UIState.Hovered))
            {
                UIState = UIState & ~UIState.Hovered;
            }
        }

        public void Hovering()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!IsHovered)
            {
                HoveringStarted();
            }

            OnHovering();
        }

        protected virtual void OnHovering() { }

        public bool IsSelected { get { return UIState.IsFlagSet(UIState.Selected); } }

        public void Select()
        {
            if (!IsEnabled)
            {
                return;
            }

            switch (UIState)
            {
                case UIState.Default:
                    UIState = UIState.Selected;
                    break;
                case UIState.Hovered:
                    UIState = UIState.Selected | UIState.Hovered;
                    break;
            }

            OnSelected();
        }

        protected virtual void OnSelected() { }

        public void Unselect()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (UIState.IsFlagSet(UIState.Selected))
            {
                UIState = UIState & ~UIState.Selected;

                OnUnselected();
            }
        }

        protected virtual void OnUnselected() { }

        /// <summary>
        /// </summary>
        public bool LeftClick()
        {
            if (!IsEnabled)
            {
                return false;
            }

            return OnLeftClick();
        }

        protected virtual bool OnLeftClick() { return false; }

        /// <summary>
        /// </summary>
        public bool RightClick()
        {
            if (!IsEnabled)
            {
                return false;
            }

            return OnRightClick();
        }

        protected virtual bool OnRightClick() { return false; }

        public void Update(Ray mouseRay)
        {
            OnUpdate(mouseRay);
        }

        protected virtual void OnUpdate(Ray mouseRay) { }

        public void Render(RenderManager.CameraInfo cameraInfo)
        {
            OnRendered(cameraInfo);
        }

        protected abstract void OnRendered(RenderManager.CameraInfo cameraInfo);
    }
}
