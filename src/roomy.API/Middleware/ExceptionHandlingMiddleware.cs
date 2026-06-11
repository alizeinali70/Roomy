namespace roomy.API.Middleware
{
    // 1. Mark the class as partial so the source generator can inject the logging code
    internal partial class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                LogUnhandledException(_logger, ex);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = new { message = "An internal error occurred", detail = ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
        }

        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Error,
            Message = "An unhandled exception occurred"
        )]
        static partial void LogUnhandledException(ILogger logger, Exception exception);
    }
}