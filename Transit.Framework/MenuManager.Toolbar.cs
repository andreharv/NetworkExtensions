using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Transit.Framework.Builders;

namespace Transit.Framework
{
    public partial class MenuManager : Singleton<MenuManager>
    {
        private ICollection<IToolbarItemBuilder> _toolbarItems = new List<IToolbarItemBuilder>();
        public IEnumerable<IToolbarItemBuilder> ToolbarItems { get { return _toolbarItems.ToArray(); } }

        public void AddToolbarItem<T>()
            where T : IToolbarItemBuilder, new()
        {
            if (!_toolbarItems.OfType<T>().Any())
            {
                Log.Info("TFW: Adding toolbar item of type " + typeof(T));
                _toolbarItems.Add(new T());
            }
        }

        public void RemoveToolbarItem<T>()
            where T : IToolbarItemBuilder
        {
            var items = _toolbarItems.OfType<T>().ToArray();

            if (items.Any())
            {
                Log.Info("TFW: Removing toolbar item of type " + typeof(T));

                foreach (var i in items)
                {
                    _toolbarItems.Remove(i);
                }
            }
        }
    }
}
