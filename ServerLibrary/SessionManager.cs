using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class SessionManager
    {
        protected Server server;

        /// <summary>
        /// Track all sessions.
        /// </summary>
        protected Dictionary<IPAddress, Session> sessionMap = new Dictionary<IPAddress, Session>();

        // TODO: We need a way to remove very old sessions so that the server doesn't accumulate thousands of stale endpoints.

        public SessionManager(Server server)
        {
            this.server = server;
            sessionMap = new Dictionary<IPAddress, Session>();
        }

        /// <summary>
        /// Creates or returns the existing session for this remote endpoint.
        /// </summary>
        public Session GetSession(IPEndPoint remoteEndPoint)
        {
            if (sessionMap.TryGetValue(remoteEndPoint.Address))
            {

            }
                /*if (!sessionMap.TryGetValue(remoteEndPoint.Address, out session))
                {
                    session = new Session();
                    session[server.ValidationTokenName] = Guid.NewGuid().ToString();
                    sessionMap[remoteEndPoint.Address] = session;
                }*/

                return session;
        }

    }
}
