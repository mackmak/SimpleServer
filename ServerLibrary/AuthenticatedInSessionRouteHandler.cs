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

        public AuthenticatedInSessionRouteHandler(Func<Session, Dictionary<string, object>, ResponsePacket> handler):
            base(handler)
        {

        }

        public override ResponsePacket Handle(Session session, Dictionary<string, object> parameters)
        {
            ResponsePacket response;

            if (session.IsExpired(Server.ExpirationTimeInSeconds))
            {
                session.Expire();
                response = server.Redirect(server.OnError(Server.ServerError.ExpiredSession));
            }
            else
            {
                response = base.Handle(session, parameters);
            }

            return response;
        }
    }
}
