namespace server_api.Services
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileLogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            // TODO: rework output:
            // date - Caller Class\Method
            // message
            // ----------------------------

            // TODO: new logs should be on top
            // TODO: actually add logging to the app

            string output = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {message}\n";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "log.txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(output);
            }
        }
    }
}