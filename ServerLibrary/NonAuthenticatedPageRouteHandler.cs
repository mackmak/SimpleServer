using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class NonAuthenticatedPageRouteHandler : RouteHandler
    {
        public NonAuthenticatedPageRouteHandler(Func<Session, Dictionary<string, object>, ResponsePacket> handler) :
            base(handler)
        {
        }
        public override ResponsePacket Handle(Session session, Dictionary<string, object> parameters)
        {
            return handler(session, parameters);
        }
    }
}
