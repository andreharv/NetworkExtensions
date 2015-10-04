using Transit.Framework.Modularity;

namespace Transit.Framework.Interfaces
{
    public interface IIdentifiable
    {
        string Name { get; }
    }

    public static class IdentifiableExtensions
    {
        public static string GetSerializableName(this IIdentifiable id)
        {
            return id.Name.Replace(" ", "_");
        }
    }
}
