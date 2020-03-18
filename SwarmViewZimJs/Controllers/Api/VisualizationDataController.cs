using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SwarmViewZimJs.Service;
using SwarmViewZimJs.Service.DTOModels;
using SwarmViewZimJs.General;
using System.Web.Script.Serialization;
using System.Text;

namespace SwarmViewZimJs.Controllers.Api
{
    public class VisualizationDataController : ApiController
    {
        [HttpGet]
        [Route("api/Visualization/Session")]
        public IEnumerable<SessionGridModel> GetSessionGrid()
        {
            try
            {
                SessionService sessionService = new SessionService();
                return sessionService.GetSessionGrid();
            }
            catch (Exception ex)
            {
                throw InternalError.ThrowError(ex);
            }
        }

        [HttpGet]
        [Route("api/Visualization/Session/{id}")]
        public List<ElementModel.Element> GetSessionVisualization(string id)
        {
            try
            {
                SessionService sessionService = new SessionService();
                return sessionService.GetSessionVisualization(id);
            }
            catch (Exception ex)
            {
                throw InternalError.ThrowError(ex);
            }
        }

        [HttpGet]
        [Route("api/Visualization/Task")]
        public IEnumerable<TaskGridModel> GetTaskGrid()
        {
            try
            {
                TaskService taskService = new TaskService();
                return taskService.GetTaskGrid();
            }
            catch (Exception ex)
            {
                throw InternalError.ThrowError(ex);
            }
        }

        [HttpGet]
        [Route("api/Visualization/Task/{id}")]
        public List<ElementModel.Element> GetTaskVisualization(string id)
        {
            try
            {
                TaskService taskService = new TaskService();
                return taskService.GetTaskVisualization(id);
            }
            catch (Exception ex)
            {
                throw InternalError.ThrowError(ex);
            }
        }

        [HttpGet]
        [Route("api/Visualization/Global/Projects")]
        public HttpResponseMessage GetGlobalProjects()
        {
            try
            {
                ProjectService projectService = new ProjectService();
                var projects = projectService.GetDistinctProjects();

                return Request.CreateResponse(HttpStatusCode.OK, projects, "application/json");
            }
            catch (Exception ex)
            {
                throw InternalError.ThrowError(ex);
            }
        }

        [HttpGet]
        [Route("api/Visualization/Global/{id}")]
        public List<ElementModel.Element> GetGlobalVisualization(string id)
        {
            try
            {
                TaskService taskService = new TaskService();
                return taskService.GetGlobalVisualization(id);
            }
            catch (Exception ex)
            {
                throw InternalError.ThrowError(ex);
            }
        }

        [HttpGet]
        [Route("api/Visualization/View3dTaskProjectFilter")]
        public HttpResponseMessage GetView3dTaskProjectFilterVisualization()
        {
            try
            {
                VisualizationService visualizationService = new VisualizationService();
                var view3dData = visualizationService.GetView3dTaskProjectDataFilter();

                var data = new JavaScriptSerializer().Serialize(view3dData);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(data, Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                throw InternalError.ThrowError(ex);
            }
        }

        [HttpPost]
        [Route("api/Visualization/View3dUserFilter")]
        public HttpResponseMessage GetView3UserFilterVisualization(TaskProjectModel filter)
        {
            try
            {
                VisualizationService visualizationService = new VisualizationService();
                var view3dData = visualizationService.GetView3dUserDataFilter(filter);

                var data = new JavaScriptSerializer().Serialize(view3dData);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(data, Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                throw InternalError.ThrowError(ex);
            }
        }

        [HttpPost]
        [Route("api/Visualization/View3dSessionFilter")]
        public HttpResponseMessage GetView3SessionFilterVisualization(UserModel filter)
        {
            try
            {
                VisualizationService visualizationService = new VisualizationService();
                var view3dData = visualizationService.GetView3dSessionDataFilter(filter);

                var data = new JavaScriptSerializer().Serialize(view3dData);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(data, Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                throw InternalError.ThrowError(ex);
            }
        }

        [HttpPost]
        [Route("api/Visualization/View3d")]
        public HttpResponseMessage GetView3dVisualization(SessionFilterModel filter)
        {
            try
            {
                VisualizationService visualizationService = new VisualizationService();
                var view3dData = visualizationService.GetView3dData(filter);

                var data = new JavaScriptSerializer().Serialize(view3dData);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(data, Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                throw InternalError.ThrowError(ex);
            }
        }

        [HttpGet]
        [Route("api/Visualization/SourceCode")]
        public HttpResponseMessage GetSourceCodeisualization(string originalId)
        {
            try
            {
                VisualizationService visualizationService = new VisualizationService();
                var view3dData = visualizationService.GetView3dSourceCode(originalId);

                var data = new JavaScriptSerializer().Serialize(view3dData);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(data, Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                throw InternalError.ThrowError(ex);
            }
        }
    }
}
