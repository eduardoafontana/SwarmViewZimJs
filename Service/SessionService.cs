using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwarmViewZimJs.Service.DTOModels;
using Repository;

namespace SwarmViewZimJs.Service
{
    public class SessionService
    {
        public IEnumerable<SessionGridModel> GetSessionGrid()
        {
            using (Entities context = new Entities())
            {
                return context.Sessions.OrderByDescending(s => s.Started).Select(s => new SessionGridModel
                {
                    Identifier = s.Id,
                    TaskName = s.TaskName,
                    TaskAction = s.TaskAction,
                    TaskProjectName = s.ProjectName,
                    DeveloperName = s.DeveloperName,
                    Started = s.Started,
                    Finished = s.Finished,
                    BreakpointCount = s.Breakpoints.Count,
                    EventCount = s.Events.Count,
                    PathNodeCount = s.PathNodes.Count
                }).ToList();
            }
        }

        public List<ElementModel.Element> GetSessionVisualization(string id)
        {
            ElementModel model = new ElementModel();
            List<PathNodes> pnCollection = new List<PathNodes>();

            using (Entities context = new Entities())
            {
                pnCollection = context.PathNodes
                    .Where(pn => pn.Sessions.Id.ToString() == id).OrderBy(pn => pn.Created).ToList();
            }

            //load nodes
            foreach (PathNodes pn in pnCollection)
            {
                model.ElementCollection.Add(new ElementModel.Element()
                {
                    data = new ElementModel.Data()
                    {
                        id = pn.Id.ToString(),
                        parent_id = pn.Parent_Id.ToString(),
                        method = pn.Method,
                        nodeinfo = new ElementModel.NodeInfo()
                        {
                            name_space = pn.Namespace,
                            type = pn.Type,
                            method = pn.Method,
                            returntype = pn.ReturnType,
                            origin = pn.Origin,
                            created = pn.Created.ToShortDateString()
                        }
                    }
                });
            }

            //load edges
            List<ElementModel.Element> edgesCollection = new List<ElementModel.Element>();

            foreach (ElementModel.Element element in model.ElementCollection)
            {
                if (element.data.parent_id == Guid.Empty.ToString())
                    continue;

                edgesCollection.Add(new ElementModel.Element()
                {
                    data = new ElementModel.Data()
                    {
                        id = element.data.id + "-" + element.data.id,
                        source = element.data.parent_id,
                        target = element.data.id
                    }
                });
            }

            model.ElementCollection.AddRange(edgesCollection);

            return model.ElementCollection;
        }
    }
}
