using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExpertSystem.WPF.Services
{
    public class ServerService : IServerService
    {
        private Process? _process { get; set; }
        
        public async Task StartServer()
        {
            if (IsRunning())
            {
                return;
            }

            var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
            var pythonFileRoot = Path.GetFullPath(Path.Combine(baseFolder, "..", "..", "..", ".."));
            var pythonFile = Path.Combine(pythonFileRoot, "main.py");

            if (!File.Exists(pythonFile))
                throw new FileNotFoundException($"File main.py in {pythonFile} not found.");

            var startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = "-m uvicorn main:app --host 127.0.0.1 --port 8000",
                WorkingDirectory = pythonFileRoot,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _process = Process.Start(startInfo);
           

            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(1) };
            const int maxTriesToConnect = 20;

            for (int i = 0; i < maxTriesToConnect; i++)
            {
                try
                {
                    var r = await client.GetAsync("http://127.0.0.1:8000/ServerStatus");
                    if (r.IsSuccessStatusCode)
                        return;
                }
                catch
                {
                    // mozna dodac potem kontrolke do UI?
                }

                await Task.Delay(500);
            }

            throw new TimeoutException("Server didn't respond.");

        }

        public async Task StopServer() 
        {
            if (_process != null && !_process.HasExited)
            {
                try
                {
                    _process.Kill();
                    await _process.WaitForExitAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Couldn't stop the server.");
                }
                finally
                {
                    _process.Dispose(); 
                    _process = null;
                }
            }


        }   

        public bool IsRunning()
        {
            return _process != null && !_process.HasExited;
        }
    }
}
