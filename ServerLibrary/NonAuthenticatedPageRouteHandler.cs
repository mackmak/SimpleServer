using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class NonAuthenticatedPageRouteHandler : RouteHandler
    {
        public NonAuthenticatedPageRouteHandler(Func<Session, Dictionary<string, string>, string> handler) :
            base(handler)
        {
        }
        public override string Handle(Session session, Dictionary<string, string> parameters)
        {
            return handler(session, parameters);
        }
    }
}
