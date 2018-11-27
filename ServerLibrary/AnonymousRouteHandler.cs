using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class AnonymousRouteHandler : RouteHandler
    {
        public AnonymousRouteHandler(Func<Session, Dictionary<string, object>, 
            ResponsePacket> handler = null)
            : base(handler)
        {

        }
        
    }
}
