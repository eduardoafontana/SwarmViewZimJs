using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Repository;

namespace SwarmViewZimJs.Controllers.Api
{
    public class SessionController : ApiController
    {
        public class SessionModel
        {
            public String Id { get; set; }
            public String ProjectName { get; set; }
            public String DeveloperName { get; set; }
        }

        // GET: api/Session
        public IEnumerable<SessionModel> Get()
        {
            using (Entities context = new Entities())
            {
                return context.Sessions.Select(t => new SessionModel
                {
                    Id = t.Id.ToString(),
                    ProjectName = t.ProjectName,
                    DeveloperName = t.DeveloperName,
                }).ToList();
            }
        }
    }
}
