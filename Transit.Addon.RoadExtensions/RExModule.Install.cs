using ColossalFramework;
using ColossalFramework.Globalization;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.AI;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.Alley2L;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.OneWay1L;
using Transit.Addon.RoadExtensions.UI.Toolbar.Roads;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.ExtensionPoints.AI.Networks;
using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.Modularity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        private GameObject _container = null;
        private NetCollection _roads = null;
        private PropCollection _props = null;

        private RoadsInstaller _roadsInstaller = null;

        private IEnumerable<Action> _lateOperations;

        public override void OnInstallingLocalization(Locale locale)
        {
            base.OnInstallingLocalization(locale);

            var menuItemBuilders = new List<IMenuItemBuilder>();
            menuItemBuilders.AddRange(Parts.OfType<IMenuItemBuilder>());
            menuItemBuilders.AddRange(Parts.OfType<IMenuItemBuildersProvider>().SelectMany(mib => mib.MenuItemBuilders));

            foreach (var builder in menuItemBuilders)
            {
                locale.CreateNetTitleLocalizedString(builder.Name, builder.DisplayName);
                locale.CreateNetDescriptionLocalizedString(builder.Name, builder.Description);
            }
        }

        public override void OnInstallingContent()
        {
            _container = new GameObject(REX_OBJECT_NAME);

            RoadZoneBlocksCreationManager.RegisterCustomCreator<TinyRoadZoneBlocksCreator>(Alley2LBuilder.NAME);
            RoadZoneBlocksCreationManager.RegisterCustomCreator<TinyRoadZoneBlocksCreator>(OneWay1LBuilder.NAME);

            RoadSnappingModeManager.RegisterCustomSnapping<TinyRoadSnappingMode>(Alley2LBuilder.NAME);
            RoadSnappingModeManager.RegisterCustomSnapping<TinyRoadSnappingMode>(OneWay1LBuilder.NAME);

            _roadsInstaller = _container.AddInstallerComponent<RoadsInstaller>();
            _roadsInstaller.Host = this;
        }
		
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (_lateOperations != null)
            {
                foreach (var op in _lateOperations)
                {
                    op();
                }

                _lateOperations = null;
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();

            if (_roadsInstaller != null)
            {
                Object.Destroy(_roadsInstaller);
                _roadsInstaller = null;
            }

            if (_roads != null)
            {
                Object.Destroy(_roads);
                _roads = null;
            }

            if (_props != null)
            {
                Object.Destroy(_props);
                _props = null;
            }

            if (_container != null)
            {
                Object.Destroy(_container);
                _container = null;
            }
        }
    }
}
