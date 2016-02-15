using System.Collections.Generic;
using System.Linq;
using Transit.Framework.UI.Menus.Toolbar.Items;

namespace Transit.Framework.ExtensionPoints.UI
{
    public class GameMainToolbarItemsManager
    {
        private static ICollection<IToolbarItemInfo> s_customEntries = new List<IToolbarItemInfo>();
        public static IEnumerable<IToolbarItemInfo> CustomEntries { get { return s_customEntries.ToArray(); } }

        public static void Reset()
        {
            s_customEntries = new List<IToolbarItemInfo>();
        }
        
        public static void AddItem<T>()
            where T : IToolbarItemInfo, new()
        {
            s_customEntries.Add(new T());
        }

        public static IToolbarItemInfo AddBigSeparator(int order)
        {
            var item = new ToolbarBigSeparatorItemInfo(order);
            s_customEntries.Add(item);
            return item;
        }

        public static IToolbarItemInfo AddSmallSeparator(int order)
        {
            var item = new ToolbarSmallSeparatorItemInfo(order);
            s_customEntries.Add(item);
            return item;
        }
    }
}
