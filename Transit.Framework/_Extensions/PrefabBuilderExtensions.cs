using Transit.Framework.Builders;

namespace Transit.Framework
{
    public static class PrefabBuilderExtensions
    {
        public static T Build<T>(this IPrefabBuilder<T> builder) 
            where T : PrefabInfo
        {
            var info = Prefabs
                .Find<T>(builder.BasedPrefabName)
                .Clone(builder.Name);

            builder.BuildUp(info);

            return info;
        }
    }
}
