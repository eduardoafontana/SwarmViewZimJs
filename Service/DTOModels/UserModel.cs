using System;
using System.Collections.Generic;

namespace SwarmViewZimJs.Service.DTOModels
{
    public class UserModel
    {
        public class User
        {
            public string userName { get; set; }
            public string taskName { get; set; }
            public string projectName { get; set; }
        }

        public List<User> list { get; set; } = new List<User>();
    }
}