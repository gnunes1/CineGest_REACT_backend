using System.Collections.Generic;

namespace CineGest
{
    public static class Routes
    {
        //private routes
        public static List<Rule> Rules = new List<Rule>()
        {
            //Users
            new Rule("GetAuthenticated", "Users", new string[]{"Admin" }),
            new Rule("GetUser", "Users", new string[]{"Admin" }),
            new Rule("GetOthers", "Users", new string[]{"Admin" }),
            new Rule("PostUser", "Users", new string[]{"Admin" }),
            new Rule("PutUser", "Users", new string[]{"Admin" }),
            new Rule("PutCurrentUser", "Users", new string[]{"Admin", "User" }),
            new Rule("DeleteUser", "Users", new string[]{"Admin"}),
            new Rule("DeleteCurrent", "Users", new string[]{"User"}),

            //Cinemas
            new Rule("GetCinemas", "Cinemas", new string[]{"Admin"}),
            new Rule("GetCinema", "Cinemas", new string[]{"Admin"}),
            new Rule("PutCinema", "Cinemas", new string[]{"Admin"}),
            new Rule("PostCinema", "Cinemas", new string[]{"Admin"}),
            new Rule("DeleteCinema", "Cinemas", new string[]{"Admin"}),

            //Movies
            new Rule("GetMoviesDetails", "Movies", new string[]{"Admin"}),
            new Rule("PutMovie", "Movies", new string[]{"Admin"}),
            new Rule("PostMovie", "Movies", new string[]{"Admin"}),
            new Rule("DeleteMovie", "Movies", new string[]{"Admin"}),

            //Tickets
            new Rule("GetTickets", "Tickets", new string[]{"Admin"}),
            new Rule("PostTicket", "Tickets", new string[]{"Admin", "User"}),
            new Rule("GetTicketsCurrent", "Tickets", new string[]{"Admin", "User"}),
        };
    }

    public class Rule
    {
        public Rule(string action, string controller, string[] roles)
        {
            Action = action;
            Controller = controller;
            Roles = roles;
        }

        public string Action { get; set; }

        public string Controller { get; set; }

        public string[] Roles { get; set; }
    }
}
