using FluentValidation;
using InMemoriam.Infraestructure.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace InMemoriam.Infraestructure.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IValidatorService _validationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ValidationFilter> _logger;

        public ValidationFilter(IValidatorService validationService, IServiceProvider serviceProvider, ILogger<ValidationFilter> logger)
        {
            _validationService = validationService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var argumentType = argument.GetType();

                // Verificar si existe un validador para este tipo
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = _serviceProvider.GetService(validatorType);
                if (validator == null) continue; // No hay validador, saltar

                try
                {
                    // Invocar ValidateAsync<T>(T model) del IValidatorService
                    var method = _validationService.GetType().GetMethod("ValidateAsync", BindingFlags.Instance | BindingFlags.Public);
                    if (method == null)
                    {
                        _logger.LogWarning("IValidatorService no expone ValidateAsync.");
                        continue;
                    }

                    var genericMethod = method.MakeGenericMethod(argumentType);
                    var taskObj = (Task)genericMethod.Invoke(_validationService, new[] { argument })!;

                    await taskObj.ConfigureAwait(false);

                    var resultProperty = taskObj.GetType().GetProperty("Result");
                    if (resultProperty == null)
                    {
                        _logger.LogError("Resultado inesperado de ValidateAsync.");
                        continue;
                    }

                    var validationResult = (InMemoriam.Infraestructure.Validators.ValidationResult)resultProperty.GetValue(taskObj)!;

                    if (!validationResult.IsValid)
                    {
                        context.Result = new BadRequestObjectResult(new { Errors = validationResult.Errors });
                        return;
                    }
                }
                catch (TargetInvocationException tie)
                {
                    _logger.LogError(tie.InnerException ?? tie, "Error durante la invocación de la validación.");
                    context.Result = new ObjectResult(new { errors = new[] { "Error interno de validación" } }) { StatusCode = 500 };
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante la validación.");
                    context.Result = new ObjectResult(new { errors = new[] { "Error interno de validación" } }) { StatusCode = 500 };
                    return;
                }
            }

            await next();
        }
    }
}
