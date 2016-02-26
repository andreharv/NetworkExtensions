
using System;

namespace CSL_Traffic
{
    public static partial class ExtendedPathManager
    {
        public static ExtendedPathFindFacade[] PathFindFacades { get; set; }

        private static Type sm_pathFindingType = typeof(ExtendedPathFind);

        public static void DefinePathFinding<T>()
            where T : IExtendedPathFind, new()
        {
            sm_pathFindingType = typeof(T);
        }

        public static void ResetPathFinding()
        {
            sm_pathFindingType = typeof(ExtendedPathFind);
        }    
        
        public static IExtendedPathFind CreatePathFinding()
        {
            return (IExtendedPathFind)Activator.CreateInstance(sm_pathFindingType);
        }
    }
}
