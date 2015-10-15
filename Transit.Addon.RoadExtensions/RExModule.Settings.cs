using ICities;
using Transit.Framework;
using Transit.Framework.Interfaces;

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        public override void OnSettingsUI(UIHelperBase helper)
        {
            var optionsChanged = false;

            foreach (var part in ActivableParts)
            {
                var partLocal = part;
                var partName = part.GetSerializableName();

                if (!Options.Instance.PartsEnabled.ContainsKey(partName))
                {
                    Options.Instance.PartsEnabled[partName] = true;
                    optionsChanged = true;
                }

                helper.AddCheckbox(
                    part.DisplayName, 
                    null, // TODO: add description of road here -> part.DisplayDescription ?
                    Options.Instance.PartsEnabled[partName], 
                    isChecked =>
                    {
                        Options.Instance.PartsEnabled[partName] = partLocal.IsEnabled = isChecked;
                        Options.Instance.Save();
                    },
                    true);
            }

            if (optionsChanged)
            {
                Options.Instance.Save();
            }
        }
    }
}
