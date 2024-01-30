using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Common.RabbitMQEvents
{

    public class EventHostedService : BackgroundService
    {
        private readonly ILogger<EventHostedService> _logger;
        private readonly IEventCommandQueue _eventQueue;
        private readonly IServiceProvider _serviceProvider;

        public EventHostedService(ILogger<EventHostedService> logger, IEventCommandQueue eventQueue, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _eventQueue = eventQueue;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _eventQueue.WaitAsync();

                using (var scope = _serviceProvider.CreateScope())
                {
                    while (_eventQueue.Dequeue(out var registeredCommands) && !stoppingToken.IsCancellationRequested)
                    {
                        foreach (var command in registeredCommands)
                        {
                            try
                            {
                                var mediator = scope.ServiceProvider.GetService<IMediator>();
                                if (mediator != null)
                                {
                                    await mediator.Send(command, stoppingToken);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error occurred executing {command}.");
                            }
                        }
                    }
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
