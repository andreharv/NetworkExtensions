
namespace Transit.Framework.UI.Menus.Toolbar.Items
{
    public abstract class ToolbarSeparatorItemInfo : IToolbarItemInfo
    {
        public int Order { get; private set; }
        protected ToolbarSeparatorItemInfo(int order)
        {
            Order = order;
        }
    }

    public class ToolbarBigSeparatorItemInfo : ToolbarSeparatorItemInfo
    {
        public ToolbarBigSeparatorItemInfo(int order) : base(order) { }
    }

    public class ToolbarSmallSeparatorItemInfo : ToolbarSeparatorItemInfo
    {
        public ToolbarSmallSeparatorItemInfo(int order) : base(order) { }
    }
}
