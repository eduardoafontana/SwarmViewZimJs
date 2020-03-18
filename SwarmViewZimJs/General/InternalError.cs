using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SwarmViewZimJs.General
{
    public class InternalError
    {
        public static HttpResponseException ThrowError(Exception ex)
        {
            //TODO: bad smell return internal error. Review later.
            var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.ToString()),
                ReasonPhrase = "Error!"
            };

            return new HttpResponseException(resp);
        }
    }
}