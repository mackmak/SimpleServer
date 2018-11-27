using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class ResponsePacket
    {
        public string Redirect { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public Encoding Encoding { get; set; }
        public Server.ServerError Error { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public ResponsePacket()
        {
            Error = Server.ServerError.OK;
            StatusCode = HttpStatusCode.OK;
        }
    }

    public class Route
    {
        public string HttpVerb { get; set; }
        public string Path { get; set; }
        public RouteHandler Handler { get; set; }

    }

    internal class ExtensionInfo
    {
        public string ContentType { get; set; }
        public Func<Session, string, string, ExtensionInfo, ResponsePacket> Loader { get; set; }
    }

    public class Router
    {

        private Dictionary<string, ExtensionInfo> extensionFolderMap;
        public string SitePath { get; set; }
        
        public const string POST = "post";
        public const string GET = "get";
        public const string PUT = "put";
        public const string DELETE = "delete";

        protected List<Route> routes;

        public Router()
        {
            routes = new List<Route>();
            extensionFolderMap = new Dictionary<string, ExtensionInfo>()
            {
              {"ico", new ExtensionInfo() {Loader=ImageLoader, ContentType="image/ico"}},
              {"png", new ExtensionInfo() {Loader=ImageLoader, ContentType="image/png"}},
              {"jpg", new ExtensionInfo() {Loader=ImageLoader, ContentType="image/jpg"}},
              {"gif", new ExtensionInfo() {Loader=ImageLoader, ContentType="image/gif"}},
              {"bmp", new ExtensionInfo() {Loader=ImageLoader, ContentType="image/bmp"}},
              {"html", new ExtensionInfo() {Loader=PageLoader, ContentType="text/html"}},
              {"css", new ExtensionInfo() {Loader=TextFileLoader, ContentType="text/css"}},
              {"js", new ExtensionInfo() {Loader=TextFileLoader, ContentType="text/javascript"}},
              {"", new ExtensionInfo() {Loader=PageLoader, ContentType="text/html"}},
            };
        }

        /// <summary>
        /// Read in an image file and returns a ResponsePacket with the raw data.
        /// </summary>
        private ResponsePacket ImageLoader(Session session, string fullPath, string extension, ExtensionInfo extensionInfo)
        {
            var file = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(file);
            var response = new ResponsePacket() { Data = reader.ReadBytes((int)file.Length), ContentType = extensionInfo.ContentType };

            reader.Close();
            file.Close();

            return response;
        }

        /// <summary>
        /// Read in what is basically a text file and return a ResponsePacket with the text UTF8 encoded.
        /// </summary>
        private ResponsePacket TextFileLoader(Session session, string fullPath, string extension, ExtensionInfo extensionInfo)
        {
            var text = File.ReadAllText(fullPath);
            var response = new ResponsePacket() { Data = Encoding.UTF8.GetBytes(text), ContentType = extensionInfo.ContentType, Encoding = Encoding.UTF8 };

            return response;
        }

        /// <summary>
        /// Load an HTML file, taking into account missing extensions and a file-less IP/domain, 
        /// which should default to index.html.
        /// </summary>
        private ResponsePacket PageLoader(Session session, string fullPath, string extension, ExtensionInfo extensionInfo)
        {
            var response = new ResponsePacket();

            // If nothing follows the domain name or IP, then default to loading index.html.
            if (fullPath == SitePath)
            {
                response = Route(session, GET, "/index.html", null);
            }
            else
            {
                // No extension, so we make it ".html"
                if (string.IsNullOrEmpty(extension))
                {
                    fullPath = fullPath + ".html";
                }

                // Inject the "Pages" folder into the path
                //fullPath = SitePath + "\\Pages" + fullPath.Substring(fullPath.IndexOf(SitePath), fullPath.Length);
                fullPath = SitePath + "\\Pages" + fullPath.RightOf(SitePath);
                response = TextFileLoader(session, fullPath, extension, extensionInfo);
            }

            return response;
        }

        public ResponsePacket Route(Session session, string httpVerb, string urlPath, 
            Dictionary<string, object> handler)
        {
            var extension = urlPath.RightOfRightmostOf('.');
            ExtensionInfo extensionInfo = null;
            ResponsePacket response = null;
            httpVerb = httpVerb.ToLower();
            
            if (extensionFolderMap.TryGetValue(extension, out extensionInfo))
            {
                string windowsPath = urlPath.Substring(1).Replace('/', '\\');//replace with windows path separator
                string fullPath = Path.Combine(SitePath, windowsPath);
                
                Route routeHandler = routes.SingleOrDefault(selectedRoutes =>
                httpVerb == selectedRoutes.HttpVerb.ToLower() && urlPath == selectedRoutes.Path.ToLower());

                if(routeHandler != null)
                {
                    //There's a handler for this route
                    ResponsePacket handlerResponse = routeHandler.Handler.Handle(session, handler);

                    if (handlerResponse == null)
                    {
                        //Respond with default content loader
                        response = extensionInfo.Loader(session, fullPath, extension, extensionInfo);
                    }
                    else
                    {
                        //Respond with redirect
                        response = handlerResponse;
                    }
                }
                else
                {
                    //default behaviour
                    response = extensionInfo.Loader(session, fullPath, extension, extensionInfo);
                }
            }
            else
            {
                response = new ResponsePacket() { Error = Server.ServerError.UnknownType };
            }

            return response;
        }

        public void AddRoute(Route route)
        {
            routes.Add(route);
        }

    }
    
}
