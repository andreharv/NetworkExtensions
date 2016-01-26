using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transit.Framework.UI
{
    public abstract class ToolbarSubMenu : GeneratedScrollPanel
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
