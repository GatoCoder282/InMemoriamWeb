using InMemoriam.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace InMemoriam.Infraestructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Manejo específico de BusinessException
            if (context.Exception is BusinessException be)
            {
                var validation = new
                {
                    Status = 400,
                    Title = "Bad Request",
                    Detail = be.Message
                };

                context.Result = new BadRequestObjectResult(new { errors = new[] { validation } });
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.ExceptionHandled = true;
                return;
            }

            // Manejo genérico para otras excepciones (evita InvalidCastException)
            var err = new
            {
                Status = 500,
                Title = "Internal Server Error",
                Detail = context.Exception.Message
            };

            context.Result = new ObjectResult(new { errors = new[] { err } })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            context.ExceptionHandled = true;
        }
    }
}
