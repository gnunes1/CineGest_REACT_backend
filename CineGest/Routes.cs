using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System.Collections;

namespace CineGest
{
    public static class Routes
    {
        public static Hashtable allowedMethods = new Hashtable(){
             { "api/users/authenticated", "GET" },
             { "api/movies", "GET,POST" },
             { "api/movies/{id}", "GET,POST" }

        };
        public static Hashtable allowedRoles = new Hashtable(){
             { "api/users/authenticated", "Admin,User" },
             { "api/movies", "Admin,User" },
             { "api/movies/{id}", "Admin,User" }


        };


        public static RouteValueDictionary Match(string routeTemplate, string requestPath)
        {
            var template = TemplateParser.Parse(routeTemplate);

            var matcher = new TemplateMatcher(template, GetDefaults(template));

            var values = new RouteValueDictionary();

            return matcher.TryMatch(requestPath, values) ? values : null;
        }

        // This method extracts the default argument values from the template.
        private static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();

            foreach (var parameter in parsedTemplate.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }

            return result;
        }
    }
}
