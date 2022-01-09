/*————————————————————————————————————————————————————————————————————————————
    ——————————————————————————————————————————————————————
    |  RequestScopeBase : Base class for request scope.  |
    ——————————————————————————————————————————————————————

    © Copyright 2020 İhsan Volkan Töre.

Author              : IVT.  (İhsan Volkan Töre)
Version             : 202201070900
License             : MIT.

History             :
202201070900: IVT   : added.
————————————————————————————————————————————————————————————————————————————*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Tore.Service {

    /*————————————————————————————————————————————————————————————————————————————
        CLASS:  RequestScopeBase
        USAGE:  Class to contain request data required. 
                Passed to EnterAction, LeaveAction.
                Can be used in GlobalExceptionMiddleware.ExceptionResponder too.
                Extend this according to project requirements.
    ————————————————————————————————————————————————————————————————————————————*/  
    public class RequestScopeBase {
        public string routerPath { get; set; }          // Router path.
        public string actionName { get; set; }          // Action name.
        public bool modelValid { get; set; }            // Model state validity.
        public bool leftAction { get; set; } = false;   // Left action properly.
        [JsonIgnore]                                    // Maybe logged in json.
        public ControllerBase controller { get; set; }  // Controller.
        
        public RequestScopeBase(ActionExecutingContext context){
            routerPath = context.HttpContext.Request.Path;
            actionName = context.ActionDescriptor.RouteValues["Action"];
            modelValid = context.ModelState.IsValid;
            controller = (ControllerBase)context.Controller;
        }
    }
}
