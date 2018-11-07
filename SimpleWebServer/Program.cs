using ServerLibrary;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConsoleWebServer
{
    public class Program
    {
        public static Server server;
        static void Main(string[] args)
        {
            string webSitePath = GetWebSitePath();
            server = new Server();
            server.OnError = ErrorHandler;

            server.AddRoute(new Route()
            {
                HttpVerb = Router.POST, Path = "/demo/redirect",
                Handler = new NonAuthenticatedPageRouteHandler(Redirect)
            });

            //authentication logic
            server.AddRoute(new Route()
            {
                HttpVerb = Router.POST,
                Path = "/demo/redirect",
                Handler = new AuthenticatedPageRouteHandler(Redirect)
            });

            //expiration logic
            server.AddRoute(new Route()
            {
                HttpVerb = Router.POST,
                Path = "/demo/redirect",
                Handler = new AuthenticatedInSessionRouteHandler(Redirect)
            });

            server.Start(webSitePath);
            Console.ReadLine();
        }

        public static string GetWebSitePath()
        {
            string webSitePath = Assembly.GetExecutingAssembly().Location;
            webSitePath = webSitePath.LeftOfRightmostOf("\\").
                LeftOfRightmostOf("\\").LeftOfRightmostOf("\\") + "\\Website";

            return webSitePath;
        }

        public static string ErrorHandler(Server.ServerError error)
        {
            string response = null;

            switch (error)
            {
                case Server.ServerError.ExpiredSession:
                    response = "/ErrorPages/expiredSession.html";
                    break;
                case Server.ServerError.FileNotFound:
                    response = "/ErrorPages/fileNotFound.html";
                    break;
                case Server.ServerError.NotAuthorized:
                    response = "/ErrorPages/notAuthorized.html";
                    break;
                case Server.ServerError.PageNotFound:
                    response = "/ErrorPages/pageNotFound.html";
                    break;
                case Server.ServerError.ServerError:
                    response = "/ErrorPages/serverError.html";
                    break;
                case Server.ServerError.UnknownType:
                    response = "/ErrorPages/unknownType.html";
                    break;
                case Server.ServerError.ValidationError:
                    response = "/ErrorPages/validationError.html";
                    break;
            }

            return response;
        }

        public static string Redirect(Session session, Dictionary<string, string> parameters)
        {
            return server.Redirect("/demo/clicked");
        }
    }
}
