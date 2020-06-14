using CineGest.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
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

            // pesquisar pelo token na base de dados
            var user = context.User.Where(u => u.Token == token).FirstOrDefault();

            // Call the next delegate/middleware in the pipeline
            if (user != null)
            {

                //verifica se utilizador tem autorização para aceder ao método do controller com route e metodo pedidos
                foreach (DictionaryEntry route in Routes.allowedMethods)
                {
                    if (Routes.Match(route.Key.ToString(), httpContext.Request.Path) != null)
                    {
                        if (route.Value.ToString().Split(',').Contains(httpContext.Request.Method))
                        {

                            //role do user
                            var userRole = context.Roles.Where(r => r.Id == user.RoleFK).Select(r => r.Name).First();

                            if (Routes.allowedRoles[route.Key.ToString()].ToString().Split(',').Contains(userRole) == true)
                            {
                                await _next(httpContext);
                            }
                            else
                            {
                                httpContext.Response.StatusCode = 403;
                                httpContext.Response.Headers.Clear();
                            }
                        }
                        else
                        {
                            httpContext.Response.StatusCode = 403;
                            httpContext.Response.Headers.Clear();
                        }
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 403;
                        httpContext.Response.Headers.Clear();
                    }
                }
            }
            else
            {
                httpContext.Response.StatusCode = 403;
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
