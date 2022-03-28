namespace server_api.Services
{
    /// <summary>
    /// Execute commands in various shells
    /// </summary>
    public static class ExecuteCommand
    {
        /// <summary>
        /// Runs a CMD command and returns the output.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public static string ExecuteCMD(string command, string? workingDirectory = null)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C {command}";
            // process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            var response = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            FileLogger.Log($"CMD: {command}\n{error}");
            return response;
        }

        /// <summary>
        /// Runs a PowerShell command and returns the output.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public static string ExecutePowerShell(string command, string? workingDirectory = null)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = $"-Command {command}";
            // process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            var response = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            FileLogger.Log($"PS: {command}\n{error}");
            return response;
        }
    }
}