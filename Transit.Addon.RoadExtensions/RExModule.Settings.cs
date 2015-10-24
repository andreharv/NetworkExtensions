using System.Linq;
﻿using ICities;
using Transit.Framework;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        protected override bool IsPartActivatedOnLoad(IActivablePart part)
        {
            return Options.Instance.IsPartEnabled(part.GetCodeName());
        }

        public override void OnSettingsUI(UIHelperBase helper)
        {
            foreach (var part in Parts.OfType<IActivablePart>())
            {
                var partLocal = part;
                var partName = part.GetCodeName();

                helper.AddCheckbox(
                    part.DisplayName, 
                    null, // TODO: add description of road here -> part.DisplayDescription ?
                    Options.Instance.IsPartEnabled(partName), 
                    isChecked =>
                    {
                        partLocal.IsEnabled = isChecked;
                        Options.Instance.SetPartEnabled(partName, isChecked);
                    },
                    true);
            }
        }
    }
}
