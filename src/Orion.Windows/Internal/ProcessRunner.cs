using System.Diagnostics;
using System.Text;
using Orion.Windows.Models;

namespace Orion.Windows.Internal;

/// <summary>
/// Ejecuta un proceso capturando salida y error. La cancelación termina el
/// proceso (mata el árbol).
/// </summary>
internal static class ProcessRunner
{
    public static async Task<ShellResult> RunAsync(string fileName, string arguments, CancellationToken cancellationToken)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            }
        };

        var output = new StringBuilder();
        var error = new StringBuilder();
        process.OutputDataReceived += (_, e) => { if (e.Data is not null) { output.AppendLine(e.Data); } };
        process.ErrorDataReceived += (_, e) => { if (e.Data is not null) { error.AppendLine(e.Data); } };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await using var registration = cancellationToken.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (InvalidOperationException)
            {
                // El proceso ya salió.
            }
        });

        await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);

        return new ShellResult(process.ExitCode, output.ToString().TrimEnd(), error.ToString().TrimEnd());
    }
}
