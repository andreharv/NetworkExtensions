namespace Transit.Framework.Builders
{
    public static class PrefabBuilderExtensions
    {
        public static T BuildEmergencyFallback<T>(this IPrefabBuilder<T> builder) 
            where T : PrefabInfo
        {
            return Prefabs
                .Find<T>(builder.BasedPrefabName)
                .Clone(builder.Name);
        }

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
