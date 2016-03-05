using System;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public static class ExtendedPathManager
    {
        public static ExtendedPathFindFacade[] PathFindFacades { get; set; }

        private static Type sm_pathFindingType = typeof(VanillaPathFind);

        public static void DefinePathFinding<T>()
            where T : IExtendedPathFind, new()
        {
            sm_pathFindingType = typeof(T);
        }

        public static void ResetPathFinding()
        {
            sm_pathFindingType = typeof(VanillaPathFind);
        }    
        
        public static IExtendedPathFind CreatePathFinding(this ExtendedPathFindFacade facade)
        {
            var pf = (IExtendedPathFind)Activator.CreateInstance(sm_pathFindingType);
            pf.Facade = facade;

            return pf;
        }
    }
}
