using ColossalFramework.UI;

namespace Transit.Framework.UI
{
    public abstract class CustomScrollPanelBase : GeneratedScrollPanel
    {
        protected UIScrollablePanel _scrollablePanel;

        public override ItemClass.Service service
        {
            get { return ItemClass.Service.None; }
        }

        protected override void Awake()
        {
            base.Awake();

            _scrollablePanel = GetComponentInChildren<UIScrollablePanel>();
        }
    }
}
