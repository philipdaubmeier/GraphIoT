using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Grafana.Services
{
    public class GrafanaBackendProcessService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _rootPath;
        private Process _process = null;

        public GrafanaBackendProcessService(ILogger<GrafanaBackendProcessService> logger, IHostEnvironment env)
        {
            _logger = logger;

            if (env.IsDevelopment())
                _rootPath = Path.Combine(Path.GetFullPath(Path.Combine(env.ContentRootPath, "..")), "GraphIoT.Grafana");
            else
                _rootPath = env.ContentRootPath;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Grafana Backend Service is starting.");

            _process = new Process();
            _process.StartInfo.FileName = Path.Combine(_rootPath, "Grafana", "bin", "grafana-server.exe");
            _process.StartInfo.WorkingDirectory = Path.Combine(_rootPath, "Grafana", "bin");
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;

            _process.EnableRaisingEvents = true;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.UseShellExecute = false;
            try
            {
                var started = _process.Start();
                if (!started)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    _logger.LogInformation($"{DateTime.Now} Grafana Backend Service did not start, result code: {errorCode}");
                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Grafana Backend Service did not start, exception: {ex.Message}");
                return Task.CompletedTask;
            }

            // redirect stdout, stderr to logger
            _process.BeginOutputReadLine();
            _process.OutputDataReceived += (s, e) =>
            {
                _logger.LogInformation(e.Data);
            };
            _process.BeginErrorReadLine();
            _process.ErrorDataReceived += (s, e) =>
            {
                _logger.LogError(e.Data);
            };
            _process.Exited += (s, e) =>
            {
                _logger.LogInformation($"{DateTime.Now} Grafana Backend Service exited.");
            };

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Grafana Backend Service is stopping.");

            _process?.Kill();
            await _process?.WaitForExitAsync(cancellationToken);
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
    }

    public static class ProcessExtensions
    {
        /// <summary>
        /// Waits asynchronously for the process to exit.
        /// </summary>
        /// <param name="process">The process to wait for cancellation.</param>
        /// <param name="cancellationToken">A cancellation token. If invoked, the task will return 
        /// immediately as canceled.</param>
        /// <returns>A Task representing waiting for the process to end.</returns>
        public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if (cancellationToken != default)
                cancellationToken.Register(tcs.SetCanceled);

            return tcs.Task;
        }
    }
}