using System;
using System.Collections.Generic;
using System.Linq;

namespace Transit.Framework.ExtensionPoints.UI
{
    public class ToolbarItemInfo
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public string UnlockText { get; set; }
        public string SpriteBase { get; set; }
        public bool Enabled { get; set; }
        public int Order { get; set; }
        public bool SmallSeparator { get; set; }
        public bool BigSeparator { get; set; }
    }

    public class GameMainToolbarItemsProvider
    {
        private static ICollection<ToolbarItemInfo> s_customEntries = new List<ToolbarItemInfo>();
        public static IEnumerable<ToolbarItemInfo> CustomEntries { get { return s_customEntries.ToArray(); } }

        public static void Reset()
        {
            s_customEntries = new List<ToolbarItemInfo>();
        }

        public static void SpawnEntry(Type type, string name, string unlockText, string spriteBase, bool enabled, int order = 1000)
        {
            s_customEntries.Add(new ToolbarItemInfo
            {
                Type = type,
                Name = name,
                UnlockText = unlockText,
                SpriteBase = spriteBase,
                Enabled = enabled,
                Order = order
            });
        }

        public static void SpawnBigSeparator(int order)
        {
            s_customEntries.Add(new ToolbarItemInfo
            {
                BigSeparator = true,
                Order = order
            });
        }

        public static void SpawnSmallSeparator(int order)
        {
            s_customEntries.Add(new ToolbarItemInfo
            {
                SmallSeparator = true,
                Order = order
            });
        }
    }
}
