using System.Text.Json;

namespace server_api.Services
{
    public static class FileManager
    {
        /// <summary>
        /// Gets a value from env.json belonging to the specified key.
        /// </summary>
        /// <param name="keyPath">["parentkey","childKey"]</param>
        /// <param name="errorOnNull">If true, throws an exception if the value is null</param>
        /// <typeparam name="T">Expected value type</typeparam>
        /// <returns>The value, converted to T type</returns>
        public static T GetEnvironmentVariable<T>(string[] keyPath) where T: IConvertible
        {
            // read and parse env.json
            var jsonElement = JsonDocument.Parse(
                System.IO.File.ReadAllText("env.json")
            ).RootElement;

            // drill down to the desired element
            foreach (var key in keyPath)
            {
                jsonElement = jsonElement.GetProperty(key);
            }

            // return the value
            var value = JsonSerializer.Deserialize<T>(jsonElement);

            if (value == null)
            {
                throw new System.ArgumentException($"The value is null for key {string.Join(".", keyPath)}");
            }

            return value;
        }
        
        /// <summary>
        /// Returns a list of subfolders of the specified folder. Elements are returned as full absolute paths.
        /// </summary>
        /// <param name="parentFolderPath"></param>
        /// <returns></returns>
        public static List<string> GetSubFolders(string parentFolderPath)
        {
            var dirs = System.IO.Directory.GetDirectories(parentFolderPath);
            return dirs.ToList();
        }
    }
}
