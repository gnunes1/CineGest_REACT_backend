using System.Collections.Generic;

namespace CineGest
{
    public static class Routes
    {
        //private routes
        public static List<Rule> Rules = new List<Rule>()
        {
            //Users
            new Rule(new string[]{"GET"}, "/api/users", new string[]{"Admin"}),
            new Rule(new string[]{"GET"}, "/api/users/authenticated", new string[]{"Admin", "User"}),
            new Rule(new string[]{"PUT"}, "/api/users/{id}", new string[]{"Admin", "User"}),

            //Movies
            new Rule(new string[]{"GET", "POST"}, "/api/users/authenticated", new string[]{"Admin", "User"}),
            new Rule(new string[]{"PUT" }, "/api/users/{id}", new string[]{"Admin", "User"})
        };
    }

    public class Rule
    {
        public Rule(string[] method, string route, string[] roles)
        {
            Method = method;
            Route = route;
            Roles = roles;
        }

        public string[] Method { get; set; }

        public string Route { get; set; }

        public string[] Roles { get; set; }
    }
}
