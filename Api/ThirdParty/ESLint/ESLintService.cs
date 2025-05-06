using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
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
#if DEBUG
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
#endif
        }

        private static async Task RunESLint()
        {
            Log.Info("ESLINT", "Checking JS/TS for validity");
            string fileName;
            string arguments;

            // Choose correct shell command based on OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName = "cmd.exe";
                arguments = "/c npx eslint --ext .ts,.tsx UI Email Admin -f json";
            }
            else
            {
                fileName = "/bin/bash";
                arguments = "-c \"npx eslint --ext .ts,.tsx UI Email Admin -f json\"";
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

                // If the process is successful, parse and log the output
                if (process.ExitCode == 0)
                {
                    Log.Info("ESLINT", "[ESLint] All checks passed");
                }
                else if (process.ExitCode == 2)
                {
                    Log.Error("ESLINT", stderr);
                }

                // Parse and log the formatted output
                PrintFormattedOutput(stdout);
            }
            catch (Exception ex)
            {
                Log.Error("ESLINT", $"[ESLint] failed to run: {ex.Message}");
            }
        }

        /// <summary>
        /// Parses and prints ESLint JSON output, associating errors with the correct file.
        /// </summary>
        private static void PrintFormattedOutput(string rawOutput)
        {
            try
            {
                // Deserialize the ESLint JSON output
                var eslintResults = JsonSerializer.Deserialize<JsonArray>(rawOutput);

                if (eslintResults == null || eslintResults.Count == 0)
                {
                    Log.Info("ESLINT", "No issues found.");
                    return;
                }

                // Process each file's linting results
                foreach (var fileResult in eslintResults)
                {
                    var fileObject = fileResult.AsObject();

                    var filePath = fileObject["filePath"]?.GetValue<string>();
                    var messages = fileObject["messages"]?.AsArray();

                    if (string.IsNullOrEmpty(filePath) || messages == null || messages.Count == 0)
                    {
                        continue;
                    }

                    filePath = filePath.Replace(Environment.CurrentDirectory + '\\', "");

                    // Log the issues for this file
                    foreach (var message in messages)
                    {
                        var severity = message["severity"]?.GetValue<int>() == 2 ? "ERROR" : "WARNING";
                        var lineNumber = message["line"]?.GetValue<int>();
                        var column = message["column"]?.GetValue<int>();
                        var messageText = message["message"]?.GetValue<string>();
                        var ruleId = message["ruleId"]?.GetValue<string>();

                        if (lineNumber.HasValue && column.HasValue && !string.IsNullOrEmpty(messageText))
                        {
                            var formatted = $"{filePath}:{lineNumber}:{column} ({ruleId}) - {messageText}";

                            if (severity == "ERROR")
                            {
                                Log.Error("ESLINT", formatted);
                            }
                            else
                            {
                                Log.Warn("ESLINT", formatted);
                            }
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                Log.Error("ESLINT", $"Error parsing ESLint output: {ex.Message}");
            }
        }
    }
}
