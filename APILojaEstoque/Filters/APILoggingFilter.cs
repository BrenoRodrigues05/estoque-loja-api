using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace APILojaEstoque.Filters
{
    public class APILoggingFilter : IActionFilter
    {
        private readonly ILogger<APILoggingFilter> _logger;

        public APILoggingFilter(ILogger<APILoggingFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var parameters = context.ActionArguments;

            _logger.LogInformation("➡️ Executando ação: {Action} no controller {Controller}", actionName, 
                controllerName);
            _logger.LogInformation("🟡 Parâmetros: {Params}", JsonSerializer.Serialize(parameters));
            _logger.LogInformation($"{DateTime.Now.ToShortTimeString()}");
            _logger.LogInformation($"{context.ModelState.IsValid}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];

            if (context.Exception == null)
            {
                _logger.LogInformation("✅ Ação concluída com sucesso: {Action} no controller {Controller}",
                    actionName, controllerName);
                _logger.LogInformation($"{DateTime.Now.ToShortTimeString()}");
            }
            else
            {
                _logger.LogError(context.Exception, "❌ Erro ao executar ação: {Action} no controller {Controller}",
                    actionName, controllerName);
                _logger.LogInformation($"{DateTime.Now.ToShortTimeString()}");
            }
        }

       
    }
}
