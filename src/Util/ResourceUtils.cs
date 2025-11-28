
namespace kms.Util
{

    using System.Reflection;
    using System.Text.Json;

    using kms.Models;

    /// <summary>
    /// File resource utilities.
    /// </summary>
    public static class ResourceUtils
    {

        public static readonly string CONFIG_FILE = "config.json";
        private static readonly string SPLASH_FILE = "splash.txt";

        public static readonly Configuration CONFIG_TEMPLATE = GetConfigurationTemplate();
        public static readonly string SPLASH_TEXT = GetEmbeddedResource(SPLASH_FILE);

        public static string GetSqlResource(StorageType type, string queryRef, string? where = null)
        {
            string query = GetEmbeddedResource($"{type.ToString().ToLower()}.{queryRef}.sql");
            if(where != null) query += $" WHERE {where}";
            return query;
        }

        private static Configuration GetConfigurationTemplate()
        {
            return JsonSerializer.Deserialize<Configuration>(GetEmbeddedResource(CONFIG_FILE))
                ?? throw new InvalidOperationException("Failed to deserialize configuration from embedded resource!");
        }

        private static string GetEmbeddedResource(string resourceName)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"kms.Resources.{resourceName}")
                ?? throw new NullReferenceException($"Failed to find {resourceName} resource file!");
            return new StreamReader(stream).ReadToEnd();
        }

    }

}
