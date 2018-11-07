using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    /// <summary>
    /// Visible for authenticated user and session is active
    /// </summary>
    public class AuthenticatedInSessionRouteHandler : AuthenticatedPageRouteHandler
    {

        public AuthenticatedInSessionRouteHandler(Func<Session, Dictionary<string, string>, string> handler):
            base(handler)
        {

        }

        public override string Handle(Session session, Dictionary<string, string> parameters)
        {
            string response;

            if (session.IsExpired(Server.ExpirationTimeInSeconds))
            {
                session.isAuthenticated = false;
                response = new Server().OnError(Server.ServerError.ExpiredSession);
            }
            else
            {
                response = base.Handle(session, parameters);
            }

            return response;
        }
    }
}
