
using System;

namespace CSL_Traffic
{
    public static partial class ExtendedPathManager
    {
        public static ExtendedPathFindFacade[] PathFindFacades { get; set; }

        private static Type sm_pathFindingType = typeof(DefaultPathFind);

        public static void DefinePathFinding<T>()
            where T : IExtendedPathFind, new()
        {
            sm_pathFindingType = typeof(T);
        }

        public static void ResetPathFinding()
        {
            sm_pathFindingType = typeof(DefaultPathFind);
        }    
        
        public static IExtendedPathFind CreatePathFinding(this ExtendedPathFindFacade facade)
        {
            var pf = (IExtendedPathFind)Activator.CreateInstance(sm_pathFindingType);
            pf.Facade = facade;

            return pf;
        }
    }
}
