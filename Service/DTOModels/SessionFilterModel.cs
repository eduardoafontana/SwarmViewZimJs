using System;
using System.Collections.Generic;

namespace SwarmViewZimJs.Service.DTOModels
{
    public class SessionFilterModel
    {
        public class Session
        {
            public string id { get; set; }
        }

        public List<Session> list { get; set; } = new List<Session>();
    }
}