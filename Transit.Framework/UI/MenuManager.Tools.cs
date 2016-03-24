using System;
using System.Collections.Generic;
using Transit.Framework.Builders;

namespace Transit.Framework.UI
{
    public partial class MenuManager
    {
        private readonly ICollection<IToolBuilder> _toolBuilders = new HashSet<IToolBuilder>();

        public void RegisterTool(IToolBuilder toolBuilder)
        {
            _toolBuilders.Add(toolBuilder);
        }

        public void UnregisterTool(IToolBuilder toolBuilder)
        {
            _toolBuilders.RemoveIfAny(toolBuilder);
        }

        public IEnumerable<IToolBuilder> GetToolsForCategory(IMenuCategoryBuilder menuCategoryBuilder)
        {
            foreach (var tool in _toolBuilders)
            {
                if (tool.UICategory == menuCategoryBuilder.Name)
                {
                    yield return tool;
                }
            }
        }
    }
}
