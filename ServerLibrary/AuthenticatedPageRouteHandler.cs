using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class AuthenticatedPageRouteHandler : RouteHandler
    {
        public AuthenticatedPageRouteHandler(Func<Session, Dictionary<string, string>, string> handler) :
            base(handler)
        {
        }

        public override string Handle(Session session, Dictionary<string, string> parameters)
        {
            string response;

            if (session.isAuthenticated)
            {
                response = handler(session, parameters);
            }
            else
            {
                response = new Server().OnError(Server.ServerError.NotAuthorized);
            }
            return response;
        }
    }
}
