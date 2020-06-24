using CineGest.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CineGest
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class VerifyTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public VerifyTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, CineGestDB context)
        {

            var token = httpContext.Request.Headers["token"].ToString();

            // pesquisar o utilizador pelo token na base de dados e verifica a data de expiração do token
            var user = context.User.Where(u => u.Token == token && u.TokenExpiresAt > DateTime.Now).FirstOrDefault();

            // Call the next delegate/middleware in the pipeline
            if (user != null)
            {
                //role do user
                var userRole = context.Roles.Where(r => r.Id == user.RoleFK).Select(r => r.Name).First();

                var routeValues = httpContext.Request.RouteValues;

                var allowed = Routes.Rules.Any(rule => rule.Action.Equals(routeValues.ElementAt(0).Value)
                && rule.Controller.Equals(routeValues.ElementAt(1).Value) && rule.Roles.Contains(userRole));

                if (allowed)
                {
                    await _next(httpContext);
                }
                else
                {
                    httpContext.Response.StatusCode = 401;
                    httpContext.Response.Headers.Clear();
                }
            }
            else
            {
                httpContext.Response.StatusCode = 401;
                httpContext.Response.Headers.Clear();
            }
        }
    }


    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseVerifyToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VerifyTokenMiddleware>();
        }
    }
}
