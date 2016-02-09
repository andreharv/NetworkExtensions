using ColossalFramework;

namespace Transit.Addon.TrafficTools.Common.Markers
{
    public abstract class UIMarkerBase
    {
        private UIState _state;

        protected UIMarkerBase()
        {
            _state = UIState.Default;
        }

        protected UIState GetState()
        {
            return _state;
        }

        private void SetState(UIState state)
        {
            _state = state;
        }

        public bool IsEnabled
        {
            get { return _state.IsFlagSet(UIState.Disabled); }
            set
            {
                if (value)
                {
                    SetState(UIState.Default);
                }
                else
                {
                    SetState(UIState.Disabled);
                }
            }
        }

        public bool IsHovered
        {
            get { return _state.IsFlagSet(UIState.Hovered); }
        }

        public virtual void SetHoveringStarted()
        {
            switch (_state)
            {
                case UIState.Default:
                    SetState(UIState.Hovered);
                    break;
                case UIState.Selected:
                    SetState(UIState.Selected | UIState.Hovered);
                    break;
            }
        }

        public virtual void SetHoveringEnded()
        {
            if (_state.IsFlagSet(UIState.Hovered))
            {
                SetState(_state & ~UIState.Hovered);
            }
        }

        public bool IsSelected { get { return _state.IsFlagSet(UIState.Selected); } }

        public virtual void Select()
        {
            switch (_state)
            {
                case UIState.Default:
                    SetState(UIState.Selected);
                    break;
                case UIState.Hovered:
                    SetState(UIState.Selected | UIState.Hovered);
                    break;
            }
        }

        public virtual void UnSelect()
        {
            if (_state.IsFlagSet(UIState.Hovered))
            {
                SetState(UIState.Default | UIState.Hovered);
            }
            else
            {
                SetState(UIState.Default);
            }
        }

        public virtual void OnHovering() { }

        public abstract void OnRendered(RenderManager.CameraInfo camera);
    }
}
