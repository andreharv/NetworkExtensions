using Transit.Framework.Interfaces;

namespace Transit.Framework.Builders
{
    public interface IPrefabBuilder
    {
        string BasedPrefabName { get; }
    }

    public interface IPrefabBuilder<in T> : IPrefabBuilder, IIdentifiable
        where T : PrefabInfo
    {
        void BuildUp(T newPrefab);
    }
}
