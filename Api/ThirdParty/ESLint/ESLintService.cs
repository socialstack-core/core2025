using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Api.Eventing;

namespace Api.ESLint
{
    /// <summary>
    /// Handles ESLinting. 
    /// </summary>
    public partial class ESLintService : AutoService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ESLintService()
        {
            Events.Compiler.BeforeCompile.AddEventListener((context, container) =>
            {
                // we don't want to await this, the only thing we want to do is get the messages to show in
                // the log, ESLint can be a little slow due to the amount of files it has to traverse.
                _ = RunESLint();
                return ValueTask.FromResult(container);
            });
            Events.ESLint.Change.AddEventListener((context, container) =>
            {
                // we don't want to await this, the only thing we want to do is get the messages to show in
                // the log, ESLint can be a little slow due to the amount of files it has to traverse.
                _ = RunESLint();
                return ValueTask.FromResult(container);
            });
        }

        private static async Task RunESLint()
        {
            Log.Info("ESLINT", "[ESLINT] Checking JS/TS for validity, please wait...");
            string fileName;
            string arguments;

            // Adjust the command based on OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName = "cmd.exe";
                arguments = "/c npx eslint --ext .ts,.tsx UI Email Admin";
            }
            else
            {
                fileName = "/bin/bash";
                arguments = "-c \"npx eslint --ext .ts,.tsx UI Email Admin\"";
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                {
                    // Process each output line and format it
                    var formattedOutput = FormatESLintOutput(args.Data);
                    if (formattedOutput != null)
                    {
                        if (args.Data.Contains("error"))
                        {
                            Log.Error("ESLINT", formattedOutput);
                        }
                        else
                        {
                            Log.Info("ESLINT", formattedOutput);
                        }
                    }
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                {
                    // Process error output separately with a different prefix
                    var formattedError = FormatESLintOutput(args.Data, true);
                    if (formattedError != null)
                    {
                        Log.Error("ESLINT", formattedError);
                    }
                }
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    Console.Error.WriteLine($"[ESLint] failed with exit code {process.ExitCode}. Please check the errors above.");
                }
                else
                {
                    Console.WriteLine("[ESLint] completed successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ESLint] failed to run: {ex.Message}");
            }
        }

        /// <summary>
        /// Formats the ESLint output to make it cleaner and more readable.
        /// </summary>
        /// <param name="data">The raw output from ESLint.</param>
        /// <param name="isError">If true, this is error output; otherwise, it's standard output.</param>
        /// <returns>A formatted string, or null if the data isn't relevant.</returns>
        private static string FormatESLintOutput(string data, bool isError = false)
        {
            // If it's an ESLint error message, we want to capture the file and the error description.
            if (data.Contains("error"))
            {
                // Try to extract file, line number, and the error message
                var pattern = @"(?<file>.*?)(?::(?<line>\d+))?[\s]+(?<message>.+)";
                var match = System.Text.RegularExpressions.Regex.Match(data, pattern);

                if (match.Success)
                {
                    var file = match.Groups["file"].Value;
                    var line = match.Groups["line"].Value;
                    var message = match.Groups["message"].Value;

                    // Format the output as: [File Path] [Line Number] - Error: [Error Message]
                    return isError
                        ? $"{file}{(string.IsNullOrEmpty(line) ? "" : $":{line}")} - {message}"
                        : $"{file}{(string.IsNullOrEmpty(line) ? "" : $":{line}")} - {message}";
                }
            }

            // Return null if the line does not match expected ESLint format
            return null;
        }


    }
}
