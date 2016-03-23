using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Transit.Framework.UI.Toolbar.Infos;

namespace Transit.Framework.ExtensionPoints.UI
{
    public class TAMGameToolbarItemManager : Singleton<TAMGameToolbarItemManager>
    {
        private ICollection<IToolbarItemInfo> _items = new List<IToolbarItemInfo>();
        public IEnumerable<IToolbarItemInfo> Items { get { return _items.ToArray(); } }

        public void Reset()
        {
            _items = new List<IToolbarItemInfo>();
        }

        public void RemoveItem<T>()
            where T : IToolbarItemInfo
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
            where T : IToolbarItemInfo, new()
        {
            if (!_items.OfType<T>().Any())
            {
                Log.Info("TFW: Adding toolbar item of type " + typeof(T));
                _items.Add(new T());
            }
        }
    }
}
