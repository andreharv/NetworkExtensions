using System.Collections.Generic;
using System.Linq;
using Transit.Framework.UI.Toolbar.Items;

namespace Transit.Framework.ExtensionPoints.UI
{
    public class GameMenuManager
    {
        private static ICollection<IToolbarItemInfo> s_toolbarItems = new List<IToolbarItemInfo>();
        public static IEnumerable<IToolbarItemInfo> ToolbarItems { get { return s_toolbarItems.ToArray(); } }

        public static void Reset()
        {
            s_toolbarItems = new List<IToolbarItemInfo>();
        }

        public static void AddToolbarItem<T>()
            where T : IToolbarItemInfo, new()
        {
            s_toolbarItems.Add(new T());
        }

        public static IToolbarItemInfo AddBigSeparator(int order)
        {
            var item = new ToolbarBigSeparatorItemInfo(order);
            s_toolbarItems.Add(item);
            return item;
        }

        public static IToolbarItemInfo AddSmallSeparator(int order)
        {
            var item = new ToolbarSmallSeparatorItemInfo(order);
            s_toolbarItems.Add(item);
            return item;
        }
    }
}
