//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Repository
{
    using System;
    using System.Collections.Generic;
    
    public partial class Breakpoints
    {
        public System.Guid Id { get; set; }
        public string BreakpointKind { get; set; }
        public string Namespace { get; set; }
        public string Type { get; set; }
        public Nullable<int> LineNumber { get; set; }
        public string LineOfCode { get; set; }
        public string Origin { get; set; }
        public System.DateTime Created { get; set; }
        public Nullable<System.Guid> Session_Id { get; set; }
        public string CodeFilePath { get; set; }
    
        public virtual Sessions Sessions { get; set; }
    }
}
