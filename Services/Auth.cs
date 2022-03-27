namespace server_api.Services
{
    /// <summary>
    /// 
    /// </summary>
    public static class AuthService
    {
        /// <summary>
        /// Checks the token against the stored token. Generates a stored token if it is missing.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsTokenValid(string token)
        {
            // check if token.txt exists, create it if not
            if (!System.IO.File.Exists("token.txt"))
            {
                RegenToken();
            }

            var storedToken = System.IO.File.ReadAllText("token.txt");
            return storedToken == token;
        }

        /// <summary>
        /// Creates a new stored token
        /// </summary>
        public static void RegenToken()
        {
            var newToken = System.Guid.NewGuid().ToString();
            System.IO.File.WriteAllText("token.txt", newToken);
        }
    }
}