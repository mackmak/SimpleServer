using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public abstract class RouteHandler
    {
        protected Server server;
        protected Func<Session, Dictionary<string, object>, ResponsePacket> handler;
        
        protected RouteHandler(Server server, Func<Session, Dictionary<string, object>, ResponsePacket> handler)
        {
            this.server = server;
            this.handler = handler;
        }

        protected RouteHandler(Func<Session, Dictionary<string, object>, ResponsePacket> handler)
        {
            this.handler = handler;
        }

        public virtual ResponsePacket Handle(Session session, Dictionary<string, object> parameters)
        {
            return InvokeHandler(session, parameters);
        }

        protected ResponsePacket InvokeHandler(Session session, Dictionary<string, object> parameters)
        {
            ResponsePacket responsePacket = null;
            if(handler != null)
            {
                responsePacket = handler(session, parameters);
            }

            return responsePacket;
        }
    }
    
}
