using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SwarmViewZimJs.Service.DTOModels;
using Repository;
using System.Web.Script.Serialization;

namespace SwarmViewZimJs.Service
{
    public class VisualizationService
    {
        public class Breakpoint
        {
            public int line { get; set; }
            public int positionIndex { get; set; }
            public string data { get; set; }
        }

        public class Event
        {
            public int line { get; set; }
            public string data { get; set; }
            public int positionIndex { get; set; }
            public string eventId { get; set; }
        }

        public class File
        {
            public string originalId { get; set; }
            public string fileId { get; set; }
            public string fileName { get; set; }
            public string filePath { get; set; }
            public string sessionId { get; set; }
            public int groupId { get; set; }
            public int groupIndex { get; set; }
            public int lines { get; set; }

            public int nodePoints { get; set; }
            public int nodeSpaceBefore { get; set; }
            public int nodeSpaceAfter { get; set; }

            public List<Breakpoint> breakpoints { get; set; } = new List<Breakpoint>();
            public List<Event> events { get; set; } = new List<Event>();
            public List<Node> nodes { get; set; } = new List<Node>();
        }

        public class Node
        {
            public string fileId { get; set; }
            public int line { get; set; }
            public string eventId { get; set; }
            public bool evaluated { get; set; }
        }

        public class Session
        {
            public string sessionId { get; set; }
            public string name { get; set; }
            public List<File> files { get; set; } = new List<File>();
            public List<Node> pathnodes { get; set; } = new List<Node>();
        }

        public class Group
        {
            public int groupId { get; set; }
            public string path { get; set; }
            public int maxIndexWidthQuantity { get; set; }
        }

        public class User
        {
            public string userName { get; set; }
            public string taskName { get; set; }
            public string projectName { get; set; }
        }

        private class SessionFilter
        {
            public string sessionId { get; set; }
            public string name { get; set; }
            public int breakpointCount { get; set; }
            public int eventCount { get; set; }
        }

        public class View
        {
            public List<Session> sessions { get; set; } = new List<Session>();
            public List<Group> groups { get; set; } = new List<Group>();
        }

        public View GetView3dData(SessionFilterModel filter)
        {
            View view = new View { sessions = new List<Session>(), groups = new List<Group>() };

            if (filter == null)
                return view;

            Guid[] idsList = filter.list.Select(x => Guid.Parse(x.id)).ToArray();

            using (Entities context = new Entities())
            {
                var sessions = context.Sessions
                .Where(s => idsList.Contains(s.Id))
                .OrderBy(s => s.Started)
                .Select(s => s.Id)
                .ToList();

                view.groups = context.CodeFiles
                    .Where(c => sessions.Contains(c.Sessions.Id))
                    .OrderBy(c => c.Created)
                    .AsEnumerable()
                    .Select(c => new { pathOnly = System.IO.Path.GetDirectoryName(c.Path).ToLower() })
                    .Distinct()
                    .Select((po, i) => new Group { groupId = i, maxIndexWidthQuantity = 0, path = po.pathOnly })
                    .ToList();

                view.sessions = getSessions(context.Sessions
                        .Include("CodeFiles")
                        .Include("Breakpoints")
                        .Include("Events")
                        .Include("PathNodes")
                        .Where(s => sessions.Contains(s.Id))
                        .OrderBy(s => s.Started).ToList(),
                    view.groups);
            }

            return view;
        }

        public string GetView3dSourceCode(string originalId)
        {
            string sourceCode = "Something wrong on process source code search.";

            if (String.IsNullOrWhiteSpace(originalId))
                return sourceCode + " Original Id is null or empty.";

            using (Entities context = new Entities())
            {
                CodeFiles codeFile = context.CodeFiles.Include("Session").Where(c => c.Id.ToString() == originalId).FirstOrDefault();
                sourceCode = codeFile.Content;

                if (String.IsNullOrWhiteSpace(sourceCode))
                    return "No source code found.";

                sourceCode = ProcessUnzipString(codeFile.Sessions.Description, sourceCode);
            }

            return sourceCode;
        }

        private string ProcessUnzipString(string from, string content)
        {
            if (String.IsNullOrWhiteSpace(from))
                return Base64StringZip.UnZipString(content);

            if (from.Equals("Swarm on Pharo"))
            {
                byte[] gzBuffer = Convert.FromBase64String(content);

                return Encoding.Default.GetString(gzBuffer);
            }

            return Base64StringZip.UnZipString(content);
        }

        public object GetView3dTaskProjectDataFilter()
        {
            List<TaskProjectModel.TaskProject> list = new List<TaskProjectModel.TaskProject>();

            using (Entities context = new Entities())
            {
                list = context.Sessions.Include("CodeFiles")
                    .Where(s => s.Started >= new DateTime(2018, 12, 25, 0, 0, 0))

                    .Where(s => s.TaskName.Trim().ToLower() != "jhjh".ToLower())
                    .Where(s => s.TaskName.Trim().ToLower() != "Tarefa genérica V2".ToLower())
                    .Where(s => s.TaskName.Trim().ToLower() != "Atividades".ToLower())
                    .Where(s => s.TaskName.Trim().ToLower() != "Contencioso".ToLower())
                    .Where(s => s.TaskName.Trim().ToLower() != "Tarefa generica v2".ToLower())
                    .Where(s => s.TaskName.Trim().ToLower() != "Generic task v2".ToLower())
                    .Where(s => s.TaskName.Trim().ToLower() != "Reload Test".ToLower())
                    .Where(s => s.TaskName.Trim().ToLower() != "PN Event_Id Test".ToLower())

                    .Where(s => s.CodeFiles.Count() > 0)
                    .Where(s => s.DeveloperName != null && s.DeveloperName.Trim() != String.Empty)
                    .Where(s => s.TaskName != null && s.TaskName.Trim() != String.Empty)
                    .Where(s => s.ProjectName != null && s.ProjectName.Trim() != String.Empty)
                    .GroupBy(s => s.TaskName)
                    .Select(s => s.FirstOrDefault())
                    .OrderByDescending(s => s.Started)
                    .Select(s => new TaskProjectModel.TaskProject
                    {
                        taskName = s.TaskName,
                        projectName = s.ProjectName
                    }).ToList();
            }

            return list;
        }

        public object GetView3dUserDataFilter(TaskProjectModel filter)
        {
            List<User> list = new List<User>();

            if (filter == null)
                return list;

            string[] taskProjectTuple = filter.list.Select(x => x.taskName + "|" + x.projectName).ToArray();

            using (Entities context = new Entities())
            {
                list = context.Sessions.Include("CodeFiles")
                    .Where(s => s.Started >= new DateTime(2018, 12, 25, 0, 0, 0))
                    .Where(s => s.CodeFiles.Count() > 0)
                    .Where(s => s.DeveloperName != null && s.DeveloperName.Trim() != String.Empty)
                    .Where(s => s.TaskName != null && s.TaskName.Trim() != String.Empty)
                    .Where(s => s.ProjectName != null && s.ProjectName.Trim() != String.Empty)
                    .Where(s => taskProjectTuple.Contains(s.TaskName + "|" + s.ProjectName ))
                    .GroupBy(s => s.DeveloperName)
                    .Select(s => s.FirstOrDefault())
                    .OrderByDescending(s => s.Started)
                    .Select(s => new User
                    {
                        userName = s.DeveloperName,
                        taskName = s.TaskName,
                        projectName = s.ProjectName
                    }).ToList();
            }

            return list;
        }

        public object GetView3dSessionDataFilter(UserModel filter)
        {
            List<SessionFilter> list = new List<SessionFilter>();

            if (filter == null)
                return list;

            string[] tuplesFilter = filter.list.Select(x => x.userName + "|" + x.taskName + "|" + x.projectName).ToArray();

            using (Entities context = new Entities())
            {
                var sessions = context.Sessions
                .Where(s => s.Started >= new DateTime(2018, 12, 25, 0, 0, 0))
                .Where(s => s.DeveloperName != null && s.DeveloperName.Trim() != String.Empty)
                .Where(s => s.TaskName != null && s.TaskName.Trim() != String.Empty)
                .Where(s => s.ProjectName != null && s.ProjectName.Trim() != String.Empty)
                .Where(s => tuplesFilter.Contains(s.DeveloperName + "|" + s.TaskName + "|" + s.ProjectName))
                .Select(s => s.Id)
                .ToList();

                list = context.Sessions
                    .Include("CodeFiles")
                    .Include("Breakpoints")
                    .Include("Events")
                    .Include("PathNodes")
                    .Where(s => s.CodeFiles.Count() > 0)
                    .Where(s => sessions.Contains(s.Id))
                    .OrderByDescending(s => s.Started)
                    .AsEnumerable()
                    .Select((s, i) => new SessionFilter
                    {
                        sessionId = s.Id.ToString(),
                        name = String.Format("{0:dd/MM/yyyy HH:mm:ssZ}", s.Started),
                        breakpointCount = s.Breakpoints.Count,
                        eventCount = s.Events.Where(e => e.EventKind == "StepInto" || e.EventKind == "StepOver" || e.EventKind == "BreakpointHitted").Count()
                    }).ToList();
            }

            return list;
        }

        private List<Session> getSessions(List<Repository.Sessions> listSession, List<Group> generatedGroups)
        {
            List<Session> sessions = new List<Session>();

            for (int i = 0; i < listSession.Count; i++)
            {
                Repository.Sessions s = listSession[i];

                Session session = new Session();

                session.sessionId = s.Id.ToString();
                session.name = String.Format("{0:yyyy-MM-ddTHH:mm:ssZ}", s.Started) + "  " + s.Description;
                session.files = getFiles(s, s.CodeFiles
                    .GroupBy(c => c.Path.ToLower())
                    .Select(c => c.FirstOrDefault())
                    .OrderBy(c => c.Created)
                    .ToList(), sessions.SelectMany(p => p.files).ToList(), generatedGroups);
                session.pathnodes = getValidPathNodes(s, session.files);

                foreach (var node in session.pathnodes)
                {
                    File file = session.files.Where(f => f.fileId == node.fileId).First();
                    file.nodePoints++;

                    node.evaluated = true;

                    foreach (var fileItem in session.files.Where(f => f.fileId != file.fileId))
                    {
                        if (fileItem.nodePoints == 0)
                        {
                            fileItem.nodeSpaceBefore++;
                            continue;
                        }

                        int remainNodes = session.pathnodes.Where(n => fileItem.fileId == n.fileId && n.evaluated == false).Count();

                        if (remainNodes > 0)
                            fileItem.nodeSpaceAfter++;
                    }
                }

                foreach (var itemFile in session.files)
                {
                    foreach (var itemEvent in itemFile.events)
                    {
                        itemEvent.positionIndex = session.pathnodes
                            .Select((pn, index) => new { index = index, eventId = pn.eventId })
                            .Where(pn => pn.eventId == itemEvent.eventId)
                            .Select(pn => pn.index)
                            .FirstOrDefault();
                    }

                    int notHittedCount = 0;
                    foreach (var itemBreakpoint in itemFile.breakpoints)
                    {
                        Event eventInFile = itemFile.events
                            .Where(e => e.line == itemBreakpoint.line)
                            .FirstOrDefault();

                        if (eventInFile == null)
                            notHittedCount--;

                        itemBreakpoint.positionIndex = eventInFile == null ? notHittedCount : eventInFile.positionIndex;
                    }

                    //--Problem of Continue has no breakpoint associeted on graph
                    //setar no evento que este tem breakpoint associado
                    var eventsWithOutBreakpoints = itemFile.events
                        .Where(e => itemFile.breakpoints.Any(b => b.line == e.line && b.positionIndex != e.positionIndex))
                        .Select(e => { return e; }).ToList();

                    foreach(var e in eventsWithOutBreakpoints)
                    {
                        Breakpoint breakpoint = itemFile.breakpoints.Where(b => b.line == e.line).First();
                        itemFile.breakpoints.Add(new Breakpoint()
                        {
                            data = breakpoint.data,
                            line = breakpoint.line,
                            positionIndex = e.positionIndex
                        });
                    }
                }

                sessions.Add(session);
            }

            return sessions;
        }

        private List<File> getFiles(Sessions s, List<CodeFiles> listCodeFiles, List<File> generetedFiles, List<Group> generatedGroups)
        {
            List<File> files = new List<File>();

            for (int i = 0; i < listCodeFiles.Count; i++)
            {
                CodeFiles c = listCodeFiles[i];

                File file = new File();
                file.originalId = c.Id.ToString();
                file.fileId = i.ToString();
                file.fileName = System.IO.Path.GetFileName(c.Path);
                file.filePath = c.Path;
                file.sessionId = s.Id.ToString();
                file.lines = Regex.Matches(ProcessUnzipString(s.Description, c.Content), Environment.NewLine, RegexOptions.Multiline).Count;

                file.events = s.Events
                                .Where(e => e.CodeFilePath.ToLower() == c.Path.ToLower())
                                .Where(e => e.EventKind == "StepInto" || e.EventKind == "StepOver" || e.EventKind == "BreakpointHitted")
                                .Where(e => s.PathNodes.Any(pn => pn.Event_Id == e.Id && pn.Origin != "Trace"))
                                .OrderBy(e => e.Created)
                                .Select(e => new Event
                                {
                                    line = e.LineNumber ?? 0,
                                    eventId = e.Id.ToString(),
                                    data = new JavaScriptSerializer().Serialize(e)
                                })
                                .ToList();

                file.breakpoints = s.Breakpoints
                                    .Where(b => b.CodeFilePath.ToLower() == c.Path.ToLower())
                                    .Select(b => new Breakpoint
                                    {
                                        line = b.LineNumber ?? 0,
                                        data = new JavaScriptSerializer().Serialize(b)
                                    }).ToList();

                File alreadyExistFile = generetedFiles
                    .Where(f => f.filePath.ToLower() == c.Path.ToLower())
                    .FirstOrDefault();

                if (alreadyExistFile != null)
                {
                    file.groupId = alreadyExistFile.groupId;
                    file.groupIndex = alreadyExistFile.groupIndex;
                }
                else
                {
                    Group group = generatedGroups
                        .Where(g => g.path.ToLower() == System.IO.Path.GetDirectoryName(c.Path).ToLower())
                        .FirstOrDefault();

                    if (group == null)
                        continue;

                    file.groupId = group.groupId;

                    int total = generetedFiles
                        .Where(f => f.groupId == file.groupId)
                        .GroupBy(f => f.sessionId)
                        .Select(gouped => new { total = gouped.Count() })
                        .OrderByDescending(o => o.total)
                        .Select(o => o.total)
                        .FirstOrDefault();

                    file.groupIndex = total;

                    if (file.groupIndex > group.maxIndexWidthQuantity)
                        group.maxIndexWidthQuantity = file.groupIndex;
                }

                files.Add(file);
                generetedFiles.Add(file);
            }

            return files;
        }

        private List<Node> getValidPathNodes(Sessions s, List<File> generetedFilesSession)
        {
            List<Node> nodes = new List<Node>();

            List<PathNodes> pathNodes = s.PathNodes.Where(pn => pn.Origin != "Trace").OrderBy(pn => pn.Created).ToList();

            foreach (var item in pathNodes)
            {
                Events eventFounded = s.Events.Where(e => e.Id == item.Event_Id).FirstOrDefault();

                if (eventFounded == null)
                    continue;

                string fileId = generetedFilesSession
                    .Where(gf => gf.filePath.ToLower() == eventFounded.CodeFilePath.ToLower())
                    .Select(gf => gf.fileId).FirstOrDefault();

                if (String.IsNullOrWhiteSpace(fileId))
                    continue;

                nodes.Add(new Node
                {
                    fileId = fileId,
                    line = eventFounded.LineNumber ?? 0,
                    eventId = item.Event_Id.ToString(),
                    evaluated = false
                });
            }

            return nodes;
        }
    }
}
