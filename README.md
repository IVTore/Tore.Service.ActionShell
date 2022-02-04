# Tore.Service.ActionShell

Language: C#.

Nuget package: [Tore.Service.ActionShell](https://www.nuget.org/packages/Tore.Service.ActionShell/)

Dependencies: <br/>

ActionShell v5.0.0 for net5 .<br/>
&emsp; .net5<br/>
&emsp; Microsoft.AspNetCore.Mvc.NewtonsoftJson (5.0.10) [Please refer to note 2 below]<br/>
<br/>
ActionShell v6.0.0+ for net6 .<br/>
&emsp; .net6<br/>
&emsp; Microsoft.AspNetCore.Mvc.NewtonsoftJson (>= 6) [Please refer to note 2 below]<br/>


## ActionShell :

A standard action filter attribute class for .Net web API <br/>

Why?<br/>

* Bored of writing the same thing every time for services.
* This code contains the common behaviour of an action filter attribute.
* Having project dependent request scope data is so good.
* I can gather any data requied and process it and/or check sessions, access caches, databases, get the IP's etc.
<br/>
In the multi tenant server mode (Kestrel default) each request arriving is processed in its own thread,<br/>
The request context is built, then the target controller instance is created.<br/>
If there are action filters:<br/>
Filter OnActionExecuting methods are called before the endpoint is invoked and,<br/>
Filter OnActionExecuted methods are called after the endpoint is left.<br/>
<br/>
ActionShell class as a filter attribute:<br/>

* Constructs a developer defined object descendant of RequestScopeBase class gathering request scope data.
* Stores the scope in HttpContext.
* Before endpoint invocation, calls ActionShell.enter delegate if bound, with scope.
* After endpoint completion, calls ActionShell.leave delegate if bound, with scope.


For using it, modifications must be done before service kicks in, e.g.: in startup.cs:
```C#
// Add this to your startup.cs usings.

using Tore.Service;

```

Add bindings at service configure method:

```C#
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      ...
      ActionShell.requestScopeType = typeof(DeveloperDefinedClassDescendingFromRequestScopeBase);
      ActionShell.enter = SomeClass.aStaticMethodToCallBeforeEnteringEndpoint;
      ActionShell.leave = SomeClass.aStaticMethodToCallAfterLeavingEndpoint;
      ...
      
      app.UseRouting();
 
      app.UseAuthorization();

      app.UseEndpoints(endpoints => {endpoints.MapControllers();});
  }
```

The enter and leave methods are not mandatory.<br/>
They receive scope object which is RequestScopeBase descendant as parameter.<br/>
So if defined they should be a delegate of type:
```C#
public delegate void ActionShellDelegate(RequestScopeBase scope);
```

The methods should be bound as:
```C#
    ActionShell.enter = SomeClass.aStaticMethodToCallBeforeEnteringEndpoint;
    ActionShell.leave = SomeClass.aStaticMethodToCallAfterLeavingEndpoint;
```

Binding to controllers:
```C#
using Tore.Service; // <-- Needed.

namespace WorldDomination.Secret.Project.Controllers {

    [ApiController]
    [ActionShell] // <-- Easy.
    public class TheSuperDuperController: ControllerBase {
        // The super duper controller things here.
    }
}
```



## RequestScopeBase :

To generate project oriented request information, developers must extend the RequestScopeBase class. <br/>
It is already populated as follows : <br/>

```C#
    routerPath = context.HttpContext.Request.Path;
    actionName = context.ActionDescriptor.RouteValues["Action"];
    modelValid = context.ModelState.IsValid;
    controller = (ControllerBase)context.Controller;
    leftAction = false;   // this is set to true at OnActionExecuted.
```

Here is an example, additionally collecting the IP address of the client:

```C#
public class ExampleRequestScope : RequestScopeBase {
    public string ipAddress {get; private set;} = null // We will store IP address of requester here.
    // other things needed...
    
    public ExampleRequestScope(ActionExecutingContext context):base(context){
        ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        /// collect other data needed.
    }
}
```
For the extended class to take control it has to be assigned like this: <br/>
```C#
    ActionShell.requestScopeType = typeof(ExampleRequestScope);
```

Wherever HttpContext object is accessible then scope is accessible via:
```C#
    var ctx = Somewhere.to.obtain.HttpContext;
    var scope = (ExampleRequestScope)ActionShell.fetchScope(ctx);
```


---

**Notes:**<br/>
<br/>
1] ActionShell assignments should be done at configuration.<br/>
&emsp; After service starts, since system goes multithreading, do not change assignments.<br/>
&emsp; Turkish proverb :While crossing the river, one does not switch horses...<br/>
<br/>
2] Why Microsoft.AspNetCore.Mvc.NewtonsoftJson? <br/>
&emsp; Weirdly enough default http abstractions miss some methods.<br/>
&emsp; So it saves me from a lot of class chasings and abstractions and I use it in my API's anyway.
