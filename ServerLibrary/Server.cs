using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class Server
    {
        public enum ServerError
        {
            OK,
            ExpiredSession,
            NotAuthorized,
            FileNotFound,
            PageNotFound,
            ServerError,
            UnknownType,
            ValidationError,
            AjaxError,
        }

        public Func<ServerError, string> OnError { get; set; }
        public int MaxSimultaneousConnections { get; }
        public static int ExpirationTimeInSeconds { get; set; }
        public string ValidationTokenName { get; set; }


        private static Semaphore semaphore;

        public static Semaphore Semaphore
        {
            get { return semaphore; }
            set { semaphore = value; }
        }


        protected string publicIP = null;

        private static Router router;
        protected SessionManager sessionManager;

        public Server()
        {
            MaxSimultaneousConnections = 20;

            semaphore = new Semaphore(MaxSimultaneousConnections, MaxSimultaneousConnections);
            router = new Router();
            sessionManager = new SessionManager(this);
        }

        public static void Log(HttpListenerRequest request)
        {
            Console.WriteLine(request.RemoteEndPoint + " " + request.HttpMethod + " /" + request.Url.AbsoluteUri);
        }
        public static void Log(Dictionary<string, string> keyValueParams)
        {
            keyValueParams.ForEach(kvp => Console.WriteLine(kvp.Key + " : " + kvp.Value));
        }

        /// <summary>
        /// Returns list of IP addresses assigned to localhost network devices, such as hardwired ethernet, wireless, etc.
        /// </summary>
        private static List<IPAddress> GetLocalHostIPs()
        {
            var hostName = Dns.GetHostName();
            var host = Dns.GetHostEntry(hostName);
            var localHostIPs = host.AddressList.Where(
                ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .ToList();

            return localHostIPs;
        }
        



        private static HttpListener InitializeListener(List<IPAddress> localHostIPs)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost/");

            //Listen to IP address as well
            localHostIPs.ForEach(ip =>
            {
                Console.WriteLine("Listening on IP " + "http://" + ip.ToString() + "/");
                listener.Prefixes.Add("http://" + ip.ToString() + "/");
            });

            return listener;
        }


        /// <summary>
        /// Start awaiting for connections, up to the "maxSimultaneousConnections" value.
        /// This code runs in a separate thread.
        /// </summary>
        private void RunServer(HttpListener listener)
        {
            //ALWAYS awaiting for connections
            while (true)
            {
                semaphore.WaitOne();
                StartConnectionListener(listener);
            }
        }

        private static Dictionary<string, string> GetKeyValues(string data, Dictionary<string, string> keyValues = null)
        {
            keyValues.IfNull(() => keyValues = new Dictionary<string, string>());
            data.If(d => d.Length > 0, (d) => d.Split('&').ForEach(keyValue => keyValues[keyValue.LeftOf('=')] = System.Uri.UnescapeDataString(keyValue.RightOf('='))));

            return keyValues;
        }

        private void Respond(HttpListenerRequest request, HttpListenerResponse httpResponse, ResponsePacket responsePacket)
        {
            //Is it redirecting?
            if (string.IsNullOrEmpty(responsePacket.Redirect))
            {
                //not redirecting
                //is there any response?
                if (responsePacket.Data != null)
                {
                    //yes there is
                    httpResponse.ContentType = responsePacket.ContentType;
                    httpResponse.ContentLength64 = responsePacket.Data.Length;
                    httpResponse.OutputStream.Write(responsePacket.Data, 0, responsePacket.Data.Length);
                    httpResponse.ContentEncoding = responsePacket.Encoding;
                }

                // Whether we do or not, no error occurred, so the response code is OK.
                // For example, we may have just processed an AJAX callback that does not have a data response.
                // Use the status code in the response packet, so the controller has an opportunity to set the response.
                httpResponse.StatusCode = (int)responsePacket.StatusCode;
            }
            else
            {
                httpResponse.StatusCode = (int)HttpStatusCode.Redirect;
                
                var rediredtedUrl = request.Url.Scheme + "://" + request.Url.Host + responsePacket.Redirect;
                httpResponse.Redirect(rediredtedUrl);
            }

            httpResponse.OutputStream.Close();
        }

        /// <summary>
        /// Await connections
        /// </summary>
        private async void StartConnectionListener(HttpListener listener)
        {
            // Wait for a connection. Return to caller while we wait.
            HttpListenerContext context = await listener.GetContextAsync();
            //Session session = sessionManager.GetSession(context.Request.RemoteEndPoint);

            // Release the semaphore so that another listener can be immediately started up.
            semaphore.Release();

            //calling log
            Log(context.Request);

            //Serving content
            HttpListenerRequest request = context.Request;
            var url = request.RawUrl;
            var urlPath = url.LeftOf("?"); // Only the path, not any of the parameters
            var httpVerb = request.HttpMethod;// get, post, delete, etc.
            var urlParams = url.RightOf("?");// Params on the URL itself follow the URL and are separated by a ?

            var keyValueParams = GetKeyValues(urlParams);
            //Passing info to the router
            string data = new StreamReader(context.Request.InputStream, 
                context.Request.ContentEncoding).ReadToEnd();

            GetKeyValues(data, keyValueParams);
            Log(keyValueParams);

            Session session = sessionManager.GetSession(context.Request.RemoteEndPoint);

            // We have a connection, do something...
            var response = router.Route(session, httpVerb, urlPath, keyValueParams);
            
            if (response.Error != ServerError.OK)
            {
                response.Redirect = OnError(response.Error);
            }

            try
            {
                Respond(request, context.Response, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            session.UpdateLastConnectionTime();

        }

        /// <summary>
        /// Begin listening to connections on a separate worker thread.
        /// </summary>
        private void Start(HttpListener listener)
        {
            listener.Start();
            Task.Run(() => RunServer(listener));
        }
        
        /// <summary>
        /// Starts the web server.
        /// </summary>
        public void Start(string sitePath)
        {
            router.SitePath = sitePath;

            List<IPAddress> localHostIPs = GetLocalHostIPs();
            HttpListener listener = InitializeListener(localHostIPs);
            Start(listener);
        }

        public void AddRoute(Route route)
        {
            router.AddRoute(route);
        }

        public string Redirect(string url, string parameter = null)
        {
            string response = "";
            if(parameter != null)
            {
                 response = url + "?" + parameter;
            }

            return response;
        }
    }
}
