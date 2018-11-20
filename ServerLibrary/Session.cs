using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    /// <summary>
    /// Sessions are associated with the client IP.
    /// </summary>
    public class Session
    {
        public DateTime LastConnection { get; set; }
        public bool isAuthenticated { get; set; }
        private Dictionary<string, object> AdditionalInfo { get; set; }


        public object this[string ObjectsIndex]
        {
            get
            {
                object val = null;
                AdditionalInfo.TryGetValue(ObjectsIndex, out val);

                return val;
            }
            set { AdditionalInfo[ObjectsIndex] = value; }
        }

        /// <summary>
        /// Object collection getter with type conversion.
        /// Note that if the object does not exist in the session, the default value is returned.
        /// Therefore, session objects like "isAdmin" or "isAuthenticated" should always be true for their "yes" state.
        /// </summary>
        public T GetObject<T>(string objectKey)
        {
            object val = null;
            T result = default(T);

            if (AdditionalInfo.TryGetValue(objectKey, out val))
            {
                result = (T)Converter.Convert(val, typeof(T));
            }

            return result;
        }


        public Session()
        {
            AdditionalInfo = new Dictionary<string, object>();
            UpdateLastConnectionTime();
        }

        public void UpdateLastConnectionTime()
        {
            LastConnection = DateTime.Now;
        }

        /// <summary>
        /// Returns true if the last request exceeds the specified expiration time in seconds.
        /// </summary>
        public bool IsExpired(int expirationInSeconds)
        {
            return (DateTime.Now - LastConnection).TotalSeconds > expirationInSeconds;
        }

        /// <summary>
        /// De-authorize the session.
        /// </summary>
        public void Expire()
        {
            isAuthenticated = false;
            // Don't remove the validation token, as we still essentially have a session, we just want the user to log in again.
        }
    }

    
}
