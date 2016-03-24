using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Transit.Framework.Builders;

namespace Transit.Framework.UI
{
    public partial class MenuManager : Singleton<MenuManager>
    {
        private readonly ICollection<IToolbarItemBuilder> _toolbarItems = new List<IToolbarItemBuilder>();

        public void RegisterToolbarItem(Type toolbarItemBuilderType)
        {
            if (!typeof(IToolbarItemBuilder).IsAssignableFrom(toolbarItemBuilderType))
            {
                throw new Exception(string.Format("Type {0} is not supported by the MenuManager", toolbarItemBuilderType));
            }

            if (_toolbarItems.Any(i => i.GetType() == toolbarItemBuilderType))
            {
                return;
            }

            var toolbarItemBuilder = (IToolbarItemBuilder)Activator.CreateInstance(toolbarItemBuilderType);

            Log.Info("TFW: Adding toolbar item of type " + toolbarItemBuilderType);
            _toolbarItems.Add(toolbarItemBuilder);

            foreach (var cat in toolbarItemBuilder.CategoryBuilders)
            {
                RegisterCategoryInstance(cat);
            }
        }

        private bool IsToolbarItemRequired(IToolbarItemBuilder item)
        {
            foreach (var cat in item.CategoryBuilders)
            {
                if (IsCategoryRequired(cat))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<IToolbarItemBuilder> GetRequiredToolbarItems()
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
