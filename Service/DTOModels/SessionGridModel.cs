using System;

namespace SwarmViewZimJs.Service.DTOModels
{
    public class SessionGridModel
    {
        public Guid Identifier { get; set; }
        public string TaskName { get; set; }
        public string DeveloperName { get; set; }
        public string TaskAction { get; set; }
        public string TaskProjectName { get; set; }
        public int BreakpointCount { get; set; }
        public int EventCount { get; set; }
        public int PathNodeCount { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Finished { get; set; }
    }
}