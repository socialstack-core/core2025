using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Api.Vcs
{
    /// <summary>
    /// Used for managing git hooks. 
    /// </summary>
    public partial class GitService : AutoService
    {

        public GitService()
        {
            InstallHooks();
        }

        private void InstallHooks()
        {
            if (IsGitInstalled() && IsNodeInstalled())
            {

                if (!Directory.Exists(".git/hooks"))
                {
                    Directory.CreateDirectory(".git/hooks");
                }

                if (Directory.GetFiles(".git/hooks").Any(file => !file.EndsWith(".sample")))
                {
                    Log.Info("GIT", "Git hooks already installed, skipping");
                    return;
                }

                Log.Info("GIT", "Installing git hooks");

                string dir;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    dir = "Api/ThirdParty/Vcs/hooks/cmd";
                }
                else
                {
                    dir = "Api/ThirdParty/Vcs/hooks/bash";
                }

                dir += "/";

                File.Copy(dir + "/commit-msg", ".git/hooks/commit-msg");
                File.Copy(dir + "/pre-commit", ".git/hooks/pre-commit");
                File.Copy(dir + "/pre-push"  , ".git/hooks/pre-push");
            }
        }

        /// <summary>
        /// Checks whether Git is installed and available in the system PATH.
        /// </summary>
        public bool IsGitInstalled()
        {
            return IsToolAvailable("git", "--version") && Directory.Exists(".git");
        }

        /// <summary>
        /// Checks whether Node.js is installed and available in the system PATH.
        /// </summary>
        public bool IsNodeInstalled()
        {
            return IsToolAvailable("node", "--version");
        }

        /// <summary>
        /// Runs the specified command to check if a tool is available.
        /// </summary>
        private bool IsToolAvailable(string command, string args)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = command,
                        Arguments = args,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    } 
}
