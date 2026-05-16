using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Conscripts.Helpers
{
    internal class ScriptRunResult
    {
        public bool Started { get; init; }

        public int? ExitCode { get; init; }
    }

    internal static class ScriptLauncher
    {
        public static async Task<ScriptRunResult> RunAsync(string scriptPath, bool runAsAdministrator, bool runWithoutWindow, string? workingDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                return new ScriptRunResult
                {
                    Started = false,
                    ExitCode = null
                };
            }

            string extension = Path.GetExtension(scriptPath);

            Process? process = null;

            try
            {
                var processInfo = new ProcessStartInfo
                {
                    CreateNoWindow = !runAsAdministrator && runWithoutWindow,
                    UseShellExecute = runAsAdministrator || !runWithoutWindow,
                    WorkingDirectory = string.IsNullOrWhiteSpace(workingDirectory)
                        ? Path.GetDirectoryName(scriptPath) ?? string.Empty
                        : workingDirectory,
                    RedirectStandardError = false,
                    RedirectStandardOutput = false,
                };

                if (string.Equals(extension, ".ps1", StringComparison.OrdinalIgnoreCase))
                {
                    processInfo.FileName = "powershell.exe";
                    processInfo.ArgumentList.Add("-NoProfile");
                    processInfo.ArgumentList.Add("-ExecutionPolicy");
                    processInfo.ArgumentList.Add("Bypass");
                    processInfo.ArgumentList.Add("-File");
                    processInfo.ArgumentList.Add(scriptPath);
                }
                else if (string.Equals(extension, ".bat", StringComparison.OrdinalIgnoreCase))
                {
                    // processInfo.FileName = scriptPath;
                    processInfo.FileName = "cmd.exe";
                    processInfo.ArgumentList.Add("/c");
                    processInfo.ArgumentList.Add(scriptPath);
                }
                else
                {
                    return new ScriptRunResult
                    {
                        Started = false,
                        ExitCode = null
                    };
                }

                if (runAsAdministrator)
                {
                    processInfo.Verb = "runas";
                }

                process = Process.Start(processInfo);
                if (process is null)
                {
                    return new ScriptRunResult
                    {
                        Started = false,
                        ExitCode = null
                    };
                }

                await process.WaitForExitAsync();

                return new ScriptRunResult
                {
                    Started = true,
                    ExitCode = process.ExitCode
                };
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);

                return new ScriptRunResult
                {
                    Started = false,
                    ExitCode = null
                };
            }
            finally
            {
                process?.Dispose();
            }
        }
    }
}
