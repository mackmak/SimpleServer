using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    /// <summary>
    /// Visible for authenticated user and session is NOT active
    /// </summary>
    public class AuthenticatedPageRouteHandler : RouteHandler
    {
        public AuthenticatedPageRouteHandler(Func<Session, Dictionary<string, object>, ResponsePacket> handler) :
            base(handler)
        {
        }

        public override ResponsePacket Handle(Session session, Dictionary<string, object> parameters)
        {
            ResponsePacket response;

            if (session.isAuthenticated)
            {
                response = InvokeHandler(session, parameters);
            }
            else
            {
                response = server.Redirect(server.OnError(Server.ServerError.NotAuthorized));
            }
            return response;
        }
    }
}
