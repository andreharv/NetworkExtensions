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
        public IEnumerable<IToolbarItemBuilder> ToolbarItems { get { return _toolbarItems.ToArray(); } }

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
        }
    }
}
