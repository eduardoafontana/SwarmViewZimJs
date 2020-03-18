using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;

namespace SwarmViewZimJs.Service
{
    public class ProjectService
    {
        public IEnumerable<dynamic> GetDistinctProjects()
        {
            using (Entities context = new Entities())
            {
                return context.Sessions.GroupBy(s => s.ProjectName).Select(s => s.FirstOrDefault()).OrderBy(s => s.ProjectName).Select(s => new { key = s.Id, value = s.ProjectName }).ToList();
            }
        }
    }
}
