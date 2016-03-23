using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Transit.Framework.UI.Toolbar.Items;

namespace Transit.Framework.ExtensionPoints.UI
{
    public class TAMGameToolbarItemManager : Singleton<TAMGameToolbarItemManager>
    {
        private ICollection<IToolbarMenuItemInfo> _items = new List<IToolbarMenuItemInfo>();
        public IEnumerable<IToolbarMenuItemInfo> Items { get { return _items.ToArray(); } }

        public void Reset()
        {
            _items = new List<IToolbarMenuItemInfo>();
        }

        public void RemoveItem<T>()
            where T : IToolbarMenuItemInfo
        {
            var items = _items.OfType<T>().ToArray();

            if (items.Any())
            {
                Log.Info("TFW: Removing toolbar item of type " + typeof(T));

                foreach (var i in items)
                {
                    _items.Remove(i);
                }
            }
        }

        public void AddItem<T>()
            where T : IToolbarMenuItemInfo, new()
        {
            if (!_items.OfType<T>().Any())
            {
                Log.Info("TFW: Adding toolbar item of type " + typeof(T));
                _items.Add(new T());
            }
        }
    }
}
