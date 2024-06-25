/*————————————————————————————————————————————————————————————————————————————
    ——————————————————————————————————————————————————————
    |  RequestScopeBase : Base class for request scope.  |
    ——————————————————————————————————————————————————————

    © Copyright 2022 İhsan Volkan Töre.

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

    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  RequestScopeBase                                        <summary>
        USAGE:  Class to contain request data required.                 <br/>
                Passed to ActionShell.enter, ActionShell.leave.         <br/>
                Can be used in anywhere accessing current HttpContext.  <br/>
                Extend this according to project requirements.          </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public class RequestScopeBase {

        /// <summary> Router Path (request path). </summary>
        public string routerPath { get; set; } = "";
    
        /// <summary> Action name. </summary>
        public string actionName { get; set; } = "";
        
        /// <summary> Model validity status. </summary>
        public bool modelValid { get; set; } = false;
        
        /// <summary> Left action. </summary>
        public bool leftAction { get; set; } = false; 
        
        /// <summary> Current controller instance about to be invoked. </summary>
        [JsonIgnore]                                    // Maybe logged in json.
        public ControllerBase? controller { get; set; } = null;

        /**——————————————————————————————————————————————————————————————————————————
          CTOR: RequestScopeBase                                            <summary>
          TASK:                                                             <br/>
                Constructs a RequestScopeBase object.                       <br/>
                This is called per request by ActionShell.OnActionExecuting.<br/>
          ARGS: context : ActionExecutingContext :
                Current action context.                                     </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public RequestScopeBase(ActionExecutingContext? context){
            if (context == null)
                return;
            routerPath = context.HttpContext.Request.Path;
            actionName = context.ActionDescriptor.RouteValues["Action"] ?? "";
            modelValid = context.ModelState.IsValid;
            controller = (ControllerBase)context.Controller;
        }
    }

}
