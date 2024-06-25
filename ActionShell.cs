/*————————————————————————————————————————————————————————————————————————————
    ——————————————————————————————————————————————
    |   ActionShell : Endpoint entry exit.       |
    ——————————————————————————————————————————————

    © Copyright 2023 İhsan Volkan Töre.

Author              : IVT.  (İhsan Volkan Töre)
Version             : 202304111012
License             : MIT.

History             :
202304111012: IVT   : Upgraded to .NET 7.0 with nullable enabled.
202201070900: IVT   : added.
————————————————————————————————————————————————————————————————————————————*/
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

using System;


namespace Tore.Service {

    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  ActionShell.                                            <summary>
        USAGE:                                                          <br/>
                Builds a request scoped context data.                   <br/>
                enter delegate is called before entry to endpoint.      <br/>
                leave delegate is called after  endpoint is run.        <br/>
                The request scope is passed to these delegates and      <br/>
                since it is also stored into HttpContext, it can be     <br/>
                obtained from any place where HttpContext is accessible.</summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public class ActionShell : ActionFilterAttribute, IActionFilter {

        private static Type _requestScopeType = typeof(RequestScopeBase); // Default.

        /**———————————————————————————————————————————————————————————————————————————
          TYPE:  ActionShellDelegate                                        <summary>
          TASK:  Method delegate type to call before and after endpoint.    <br/>
          ARGS:  scope: RequestScopeBase : Current request scope instance.  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public delegate void ActionShellDelegate(RequestScopeBase? scope);
       
        /**———————————————————————————————————————————————————————————————————————————
          PROP: enter.                                                  <summary>
          TASK: Method delegate to call before entry to endpoint.       <br/>
                Method delegate type must be ActionShellDelegate.       <br/>
                The method should be static.                            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static ActionShellDelegate? enter {get; set;}

        /**———————————————————————————————————————————————————————————————————————————
          PROP: leave.                                                  <summary>
          TASK: Method delegate to call after leaving the endpoint.     <br/>
                Method delegate type must be ActioShellDelegate.        <br/>
                The method should be static.                            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static ActionShellDelegate? leave {get; set;}

        /**———————————————————————————————————————————————————————————————————————————
          PROP: requestScopeType.                                       <summary>
          TASK: This must be a descendant of class RequestScopeBase.    <br/>
                This allows developers to define a class that gathers   <br/>
                data tailored for the project from the context.         <br/>
                Instance will be created by OnActionExecuting,          <br/>
                Just before ActionShell.enter is called.                <para/>
                This is an assignment because attribute classes does not<br/>
                allow generics.                                         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Type requestScopeType {
            get => _requestScopeType;
            set {
                if (value == null)
                    throw new ArgumentNullException("requestScopeType.");
                if (!typeof(RequestScopeBase).IsAssignableFrom(value))
                    throw new ArgumentException("Type Mismatch.");
                _requestScopeType = value;
            }
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: fetchScope                                              <summary>
          TASK: Accesses to scope data stored in HttpContext.           <br/>
          ARGS: httpContext : HttpContext      : Current http context.  <br/>
          RETV:             : RequestScopeBase : Scope if any or null.  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static RequestScopeBase? fetchScope(HttpContext httpContext){
            if (!httpContext.Items.ContainsKey("RequestScope"))
                return null;
            return (RequestScopeBase?) httpContext.Items["RequestScope"] ?? null;
        }

        /**<inheritdoc/>*/
        public override void OnActionExecuting(ActionExecutingContext context) {
            RequestScopeBase? scope;

            scope = (RequestScopeBase?)
                Activator.CreateInstance(requestScopeType, [context]);
            context.HttpContext.Items.Add("RequestScope", scope);
            enter?.Invoke(scope);
        } 

        /**<inheritdoc/>*/
        public override void OnActionExecuted(ActionExecutedContext context) {
            RequestScopeBase? scope = fetchScope(context.HttpContext);
            leave?.Invoke(scope);
            if (scope != null)
                scope.leftAction = true;
        }

    }   //  End class ActionShell.

}   //  End namespace.
