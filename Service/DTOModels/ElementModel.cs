using System;
using System.Collections.Generic;

namespace SwarmViewZimJs.Service.DTOModels
{
    public class ElementModel
    {
        public List<Element> ElementCollection { get; set; } = new List<Element>();

        public class Element
        {
            public Data data { get; set; }
        }

        public class Data
        {
            public string id { get; set; }
            public string source { get; set; }
            public string target { get; set; }
            public string parent_id { get; set; }
            public string method { get; set; }
            public int size { get; set; }
            public string color { get; set; }
            public NodeInfo nodeinfo { get; set; }
        }

        public class NodeInfo
        {
            public string name_space { get; set; }
            public string type { get; set; }
            public string method { get; set; }
            public string origin { get; set; }
            public string returntype { get; set; }
            public string created { get; set; }
        }
    }
}