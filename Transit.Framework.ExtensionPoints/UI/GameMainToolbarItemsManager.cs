using System;
using System.Collections.Generic;
using System.Linq;

namespace Transit.Framework.ExtensionPoints.UI
{
    public interface IToolbarMenuItemInfo : IToolbarItemInfo
    {
        string Name { get; }
        string Description { get; }
        string ThumbnailsPath { get; }
        Type PanelType { get; }
    }

    public interface IToolbarItemInfo
    {
        int Order { get; }
    }

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

    public class VanillaToolbarItemInfo : IToolbarItemInfo
    {
        public string Name { get; set; }
        public string UnlockText { get; set; }
        public string SpriteBase { get; set; }
        public bool Enabled { get; set; }
        public int Order { get; set; }
    }

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

        //public static void AddMenu<T>(string name, string unlockText, string spriteBase, bool enabled, int order)
        //    where T : GeneratedGroupPanel, new()
        //{
        //    s_customEntries.Add(new ToolbarItemInfo
        //    {
        //        Type = typeof(T),
        //        Name = name,
        //        UnlockText = unlockText,
        //        SpriteBase = spriteBase,
        //        Enabled = enabled,
        //        Order = order
        //    });
        //}

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
