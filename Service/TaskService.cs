using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwarmViewZimJs.Service.DTOModels;
using Repository;

namespace SwarmViewZimJs.Service
{
    public class TaskService
    {
        public IEnumerable<TaskGridModel> GetTaskGrid()
        {
            using (Entities context = new Entities())
            {
                List<Sessions> distinctTask = context.Sessions.GroupBy(d => new { d.ProjectName, d.TaskName }).Select(g => g.FirstOrDefault()).ToList();
                Guid[] distinctTaskIds = distinctTask.Select(t => t.Id).ToArray();

                return context.Sessions.Where(d => distinctTaskIds.Contains(d.Id)).Select(t => new TaskGridModel
                {
                    Identifier = t.Id.ToString(),
                    ProjectName = t.ProjectName,
                    Name = t.TaskName,
                    Description = t.TaskDescription,
                    Action = t.TaskAction,
                    Created = t.TaskCreated
                }).ToList();
            }
        }

        public List<ElementModel.Element> GetTaskVisualization(string id)
        {
            ElementModel model = new ElementModel();
            List<PathNodes> pnCollection = new List<PathNodes>();
            List<Breakpoints> bCollection = new List<Breakpoints>();

            using (Entities context = new Entities())
            {
                var sessionFilter = context.Sessions.Where(s => s.Id.ToString() == id).Select(s => new { TaskName = s.TaskName, ProjectName = s.ProjectName }).FirstOrDefault();
                Guid[] sessionIds = context.Sessions.Where(s => s.TaskName == sessionFilter.TaskName && s.ProjectName == sessionFilter.ProjectName).Select(s => s.Id).ToArray();

                pnCollection = context.PathNodes.Where(pn => sessionIds.Contains(pn.Sessions.Id)).GroupBy(pn => pn.Type).Select(pn => pn.FirstOrDefault()).OrderBy(pn => pn.Created).ToList();
                bCollection = context.Breakpoints.Where(b => sessionIds.Contains(b.Sessions.Id)).GroupBy(b => new { b.Namespace, b.Type, b.LineNumber }).Select(b => b.FirstOrDefault()).ToList();
            }

            //load nodes
            foreach (PathNodes pn in pnCollection)
            {
                model.ElementCollection.Add(new ElementModel.Element()
                {
                    data = new ElementModel.Data()
                    {
                        id = pn.Id.ToString(),
                        parent_id = model.ElementCollection.Count() == 0 ? null : model.ElementCollection.Last().data.id,
                        method = pn.Type + " - " + bCollection.Where(b => b.Type == pn.Type).Count().ToString(),
                        size = bCollection.Where(b => b.Type == pn.Type).Count() + 10,
                        color = "#0058f0"
                    }
                });
            }

            //load edges
            List<ElementModel.Element> edgesCollection = new List<ElementModel.Element>();

            foreach (ElementModel.Element element in model.ElementCollection)
            {
                if (String.IsNullOrWhiteSpace(element.data.parent_id))
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

        public List<ElementModel.Element> GetGlobalVisualization(string id)
        {
            ElementModel model = new ElementModel();
            List<Breakpoints> bCollection = new List<Breakpoints>();
            List<PathNodes> pnCollection = new List<PathNodes>();

            using (Entities context = new Entities())
            {
                var sessionFilter = context.Sessions.Where(s => s.Id.ToString() == id).Select(s => new { ProjectName = s.ProjectName }).FirstOrDefault();
                Guid[] sessionIds = context.Sessions.Where(s => s.ProjectName == sessionFilter.ProjectName).Select(s => s.Id).ToArray();

                pnCollection = context.PathNodes.Where(pn => sessionIds.Contains(pn.Sessions.Id)).GroupBy(pn => pn.Type).Select(pn => pn.FirstOrDefault()).OrderBy(pn => pn.Created).ToList();
                bCollection = context.Breakpoints.Where(b => sessionIds.Contains(b.Sessions.Id)).GroupBy(b => new { b.Namespace, b.Type, b.LineNumber }).Select(b => b.FirstOrDefault()).ToList();
            }

            //load nodes
            foreach (var pn in pnCollection)
            {
                model.ElementCollection.Add(new ElementModel.Element()
                {
                    data = new ElementModel.Data()
                    {
                        id = pn.Id.ToString(),
                        parent_id = model.ElementCollection.Count() == 0 ? null : model.ElementCollection.Last().data.id,
                        method = pn.Type + " - " + bCollection.Where(b => b.Type == pn.Type).Count().ToString(),
                        size = bCollection.Where(b => b.Type == pn.Type).Count() + 10,
                        color = "#0058f0"
                    }
                });
            }

            //load edges
            List<ElementModel.Element> edgesCollection = new List<ElementModel.Element>();

            foreach (ElementModel.Element element in model.ElementCollection)
            {
                if (String.IsNullOrWhiteSpace(element.data.parent_id))
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
