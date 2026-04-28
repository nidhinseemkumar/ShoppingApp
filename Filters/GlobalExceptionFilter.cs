using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace ShoppingApp.Filters
{
    public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger = logger;


        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception occurred during action execution.");

            var result = new ViewResult { ViewName = "Error" };
            
            if (context.Exception is SqlException || context.Exception is DbUpdateException)
            {
                // Specialized handling for database errors
                _logger.LogCritical(context.Exception, "Database error detected.");
            }

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}
