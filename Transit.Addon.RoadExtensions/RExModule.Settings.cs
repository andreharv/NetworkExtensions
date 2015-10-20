using System.Linq;
using ICities;
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
            var uIHelperBase = helper.AddGroup("Road Extensions Options");

            foreach (var part in Parts.OfType<IActivablePart>())
            {
                var partLocal = part;
                var partName = part.GetCodeName();

                uIHelperBase.AddCheckbox(
                    part.DisplayName,
                    Options.Instance.IsPartEnabled(partName), 
                    isChecked =>
                    {
                        partLocal.IsEnabled = isChecked;
                        Options.Instance.SetPartEnabled(partName, isChecked);
                    });
            }
        }
    }
}
