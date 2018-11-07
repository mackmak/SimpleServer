using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public abstract class RouteHandler
    {
        protected Func<Session, Dictionary<string, string>, string> handler;
        public RouteHandler(Func<Session, Dictionary<string, string>, string> handler)
        {
            this.handler = handler;
        }

        public abstract string Handle(Session session, Dictionary<string, string> parameters);
    }
    
}
