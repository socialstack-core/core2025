using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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
                _ = RunESLint();
                return ValueTask.FromResult(container);
            });

            Events.ESLint.Change.AddEventListener((context, container) =>
            {
                _ = RunESLint();
                return ValueTask.FromResult(container);
            });
        }

        private static async Task RunESLint()
        {
            Log.Info("ESLINT", "Checking JS/TS for validity, please wait...");
            string fileName;
            string arguments;

            // Choose correct shell command based on OS
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

            string stdout = "";
            string stderr = "";

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                {
                    stdout += args.Data + "\n";
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                {
                    stderr += args.Data + "\n";
                }
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                PrintFormattedOutput(stdout);

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
        /// Parses and prints ESLint output, associating errors with the correct file.
        /// </summary>
        private static void PrintFormattedOutput(string rawOutput)
        {
            var lines = rawOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string currentFile = null;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                // If it's a full path (file), set it as current
                if (trimmed.EndsWith(".ts") || trimmed.EndsWith(".tsx") || trimmed.EndsWith(".js") || trimmed.EndsWith(".jsx"))
                {
                    currentFile = trimmed;
                    continue;
                }

                // Match ESLint error format: line:col  severity  message  rule
                var match = Regex.Match(trimmed, @"^(?<line>\d+):(?<column>\d+)\s+(?<severity>error|warning)\s+(?<message>.+?)\s+(?<ruleId>[a-zA-Z\-]+)$");

                if (match.Success && currentFile != null)
                {
                    var lineNumber = match.Groups["line"].Value;
                    var column = match.Groups["column"].Value;
                    var severity = match.Groups["severity"].Value.ToUpper();
                    var message = match.Groups["message"].Value;
                    var ruleId = match.Groups["ruleId"].Value;

                    var formatted = $"{currentFile}:{lineNumber}:{column} ({ruleId}) - {message} ";

                    if (severity == "ERROR")
                    {
                        Log.Error("ESLINT", formatted);
                    }
                    else
                    {
                        Log.Warn("ESLINT", formatted);
                    }
                }
                else
                {
                    Log.Info("ESLINT", line);
                }
            }
        }
    }
}
