using System.Text.Json;
#pragma warning disable CS1591
namespace GoogleApis.Blazor.Extensions
{
    public static class JsonElementExtenstion
    {
        public static JsonElement? GetPropertyExtension(this JsonElement jsonElement, string propertyName)
        {
            if (jsonElement.TryGetProperty(propertyName, out JsonElement returnElement))
            {
                return returnElement;
            }

            return null;
        }
    }
}
