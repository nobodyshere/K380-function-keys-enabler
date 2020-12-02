using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace k380_func
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger) => _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (K380Keyboard k in K380Keyboard.GetConnected())
                {
                    try
                    {
                        k.SetFunctionKeys(true, _logger);
                        _logger.LogInformation($"[{DateTime.Now.ToShortTimeString()}] Could enable function keys for {k.Device.DevicePath}");
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(e.Message);
                    }
                }

                // Repeat every 10 minutes
                await Task.Delay(600000, stoppingToken);
            }
        }
    }
}
