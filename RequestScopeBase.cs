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
                Passed to ActionShell.enter, ActionShell.leave.
                Can be used in GlobalExceptionMiddleware.ExceptionResponder too.
                Extend this according to project requirements.
    ————————————————————————————————————————————————————————————————————————————*/
    public class RequestScopeBase {
        /// <summary> Router Path (request path). </summary>
        public string routerPath { get; set; }
        /// <summary> Action name. </summary>
        public string actionName { get; set; }
        /// <summary> Model validity status. </summary>
        public bool modelValid { get; set; }
        /// <summary> Left action. </summary>
        public bool leftAction { get; set; } = false; 
        /// <summary> Current controller instance about to be invoked. </summary>
        [JsonIgnore]                                    // Maybe logged in json.
        public ControllerBase controller { get; set; }
        
        public RequestScopeBase(ActionExecutingContext context){
            routerPath = context.HttpContext.Request.Path;
            actionName = context.ActionDescriptor.RouteValues["Action"];
            modelValid = context.ModelState.IsValid;
            controller = (ControllerBase)context.Controller;
        }
    }
}
