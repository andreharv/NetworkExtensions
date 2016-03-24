using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework.UI.Infos;

namespace Transit.Framework.UI
{
    public partial class MenuManager : Singleton<MenuManager>
    {
        private readonly ICollection<IMenuToolbarItemInfo> _toolbarItems = new List<IMenuToolbarItemInfo>();

        public void RegisterToolbarItem(Type menuToolbarItemInfo)
        {
            if (!typeof(IMenuToolbarItemInfo).IsAssignableFrom(menuToolbarItemInfo))
            {
                throw new Exception(string.Format("Type {0} is not supported by the MenuManager", menuToolbarItemInfo));
            }

            if (_toolbarItems.Any(i => i.GetType() == menuToolbarItemInfo))
            {
                return;
            }

            var toolbarItemBuilder = (IMenuToolbarItemInfo)Activator.CreateInstance(menuToolbarItemInfo);

            Log.Info("TFW: Registering menu toolbar item of type " + menuToolbarItemInfo);
            _toolbarItems.Add(toolbarItemBuilder);

            foreach (var cat in toolbarItemBuilder.Categories)
            {
                RegisterCategoryInstance(cat);
            }
        }

        private bool IsToolbarItemRequired(IMenuToolbarItemInfo item)
        {
            foreach (var cat in item.Categories)
            {
                if (IsCategoryRequired(cat))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<IMenuToolbarItemInfo> GetRequiredToolbarItems()
        {
            foreach (var item in _toolbarItems)
            {
                if (IsToolbarItemRequired(item))
                {
                    yield return item;
                }
            }
        }
    }
}
