using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Power_Supply_Control_WPF.Services
{
    public class PythonManager
    {
        private Process? _process;

        public event Action<string>? MessageReceived;
        public event Action<string>? PythonError;

        public async Task StartAsync(string pythonPath, string scriptPath)
        {
            StopPython();
            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"-Xfrozen_modules=off \"{scriptPath}\"",

                    WorkingDirectory = Path.GetDirectoryName(scriptPath),

                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,

                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            _process.Start();

            _ = Task.Run(ReadStdOutLoop);
            _ = Task.Run(ReadStdErrLoop);
        }

        private async Task ReadStdOutLoop()
        {
            var reader = _process!.StandardOutput;

            while (true)
            {
                string? lengthLine = await reader.ReadLineAsync();
                if (lengthLine == null)
                    break;

                if (!int.TryParse(lengthLine, out int length))
                    continue;

                char[] buffer = new char[length];
                int read = 0;

                while (read < length)
                {
                    using var cts = new CancellationTokenSource(5000);
                    int r = await reader.ReadAsync(
                        buffer.AsMemory(read, length - read),
                        cts.Token
                    );

                    if (r == 0) break;
                    read += r;
                }

                string message = new string(buffer, 0, read);
                MessageReceived?.Invoke(message);
            }
        }

        private async Task ReadStdErrLoop()
        {
            while (_process != null &&
                   !_process.HasExited)
            {
                string? line = await _process.StandardError.ReadLineAsync();

                if (line == null)
                    break;

                PythonError?.Invoke(line);
            }
        }

        public async Task SendAsync(object obj)
        {
            if (_process == null)
                return;

            string json = JsonSerializer.Serialize(obj);
            if(!_process.HasExited)
            {
                await _process.StandardInput.WriteLineAsync(json);

                await _process.StandardInput.FlushAsync();
            }
        }

        public void StopPython()
        {
            try
            {
                if (_process == null)
                    return;

                if (!_process.HasExited)
                {
                    _process.Kill(entireProcessTree: true);
                    _process.WaitForExit(2000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
