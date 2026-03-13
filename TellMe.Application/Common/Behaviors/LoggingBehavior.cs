using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace TellMe.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            try
            {
                _logger.LogInformation("Handling {RequestName}: {@Request}", requestName, request);
                var sw = Stopwatch.StartNew();
                var response = await next();
                sw.Stop();
                _logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds}ms", requestName, sw.ElapsedMilliseconds);
                return response;
            }
            catch (Exception ex)
            {
                string payload;
                try { payload = JsonSerializer.Serialize(request); } catch { payload = "<unserializable>"; }

                _logger.LogError(ex, "Exception handling {RequestName}. Payload: {Payload}", requestName, payload);
                throw;
            }
        }
    }
}
