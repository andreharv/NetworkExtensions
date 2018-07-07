using System.Linq;
using ColossalFramework;
using ColossalFramework.Globalization;
using ICities;
using System;
using System.Collections.Generic;
using Transit.Addon.RoadExtensions.AI;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Menus.Roads.Textures;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.Alley2L;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.Oneway1L;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.Oneway1LParking;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.Oneway1LBicycle;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.Oneway1LParkingBicycle;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.Oneway2L;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.ExtensionPoints.AI;
using UnityEngine;
using Object = UnityEngine.Object;
using ZonablePedestrianTinyGravelRoadBuilder = Transit.Addon.RoadExtensions.Roads.PedestrianRoads.GravelTiny.ZonablePedestrianTinyGravelRoadBuilder;
using ZonablePedestrianTinyPavedRoadBuilder = Transit.Addon.RoadExtensions.Roads.PedestrianRoads.PavementTiny.ZonablePedestrianTinyPavedRoadBuilder;
using ZonablePedestrianTinyStoneRoadBuilder = Transit.Addon.RoadExtensions.Roads.PedestrianRoads.StoneTiny.ZonablePedestrianTinyStoneRoadBuilder;
using ZonablePedestrianTinyBoardwalkBuilder = Transit.Addon.RoadExtensions.Roads.PedestrianRoads.BoardwalkTiny.ZonablePedestrianBoardwalkRoadBuilder;
namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        private GameObject _container = null;
        private NetCollection _roads = null;
        private PropCollection _props = null;

        private RoadsInstaller _roadsInstaller = null;
        private MenuInstaller _menuInstaller = null;

        private IEnumerable<Action> _lateOperations;

        public override void OnInstallingLocalization()
        {
            base.OnInstallingLocalization();

            var locale = SingletonLite<LocaleManager>.instance.GetLocale();

            locale.CreateMenuTitleLocalizedString(RExExtendedMenus.ROADS_TINY, "Tiny Roads");
            locale.CreateMenuTitleLocalizedString(RExExtendedMenus.ROADS_SMALL_HV, "Small Heavy Roads");
            locale.CreateMenuTitleLocalizedString(RExExtendedMenus.ROADS_BUSWAYS, "Buslane Roads");
            locale.CreateMenuTitleLocalizedString(RExExtendedMenus.ROADS_PEDESTRIANS, "Pedestrian Roads");

            var menuItemBuilders = new List<IMenuItemBuilder>();
            menuItemBuilders.AddRange(Parts.OfType<IMenuItemBuilder>());
            menuItemBuilders.AddRange(Parts.OfType<IMenuItemBuildersProvider>().SelectMany(mib => mib.MenuItemBuilders));

            foreach (var builder in menuItemBuilders)
            {
                locale.CreateNetTitleLocalizedString(builder.Name, builder.DisplayName);
                locale.CreateNetDescriptionLocalizedString(builder.Name, builder.Description);
            }
        }

        public override void OnInstallingAssets()
        {
            base.OnInstallingAssets();

            AtlasManager.instance.Include<RExExtendedSubBarAtlasBuilder>();
        }

        public override void OnInstallingContent()
        {
            _container = new GameObject(REX_OBJECT_NAME);

            var tinyZoneBlockCreators = new[]
            {
                Alley2LBuilder.NAME,
                Oneway1LBuilder.NAME,
                Oneway1LParkingBuilder.NAME,
                Oneway1LBicycleBuilder.NAME,
                Oneway1LParkingBicycleBuilder.NAME,
                Oneway2LBuilder.NAME,
                ZonablePedestrianTinyGravelRoadBuilder.NAME,
                ZonablePedestrianTinyPavedRoadBuilder.NAME,
                ZonablePedestrianTinyStoneRoadBuilder.NAME,
                ZonablePedestrianTinyBoardwalkBuilder.NAME
            };

            foreach (var name in tinyZoneBlockCreators)
            {
                RoadZoneBlocksCreationManager.RegisterCustomCreator<TinyRoadZoneBlocksCreator>(name);
                RoadSnappingModeManager.RegisterCustomSnapping<TinyRoadSnappingMode>(name);
            }

            _menuInstaller = _container.AddInstallerComponent<MenuInstaller>();
            _menuInstaller.Host = this;

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

            if (_menuInstaller != null)
            {
                Object.Destroy(_menuInstaller);
                _menuInstaller = null;
            }

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
