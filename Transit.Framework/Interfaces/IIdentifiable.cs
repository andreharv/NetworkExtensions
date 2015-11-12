using Transit.Framework.Modularity;

namespace Transit.Framework.Interfaces
{
    public interface IIdentifiable
    {
        string Name { get; }
    }

    public static class IdentifiableExtensions
    {
        public static string GetCodeName(this IIdentifiable id)
        {
            return id
                .Name
                .ToUpper()
                .Replace(" ", "_")
                .Replace("+", "PLUS")
                .Replace("-", "_");
        }
    }
}
