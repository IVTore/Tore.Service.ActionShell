# Tore.Service.ActionShell

Language: C#.

Nuget package: [Tore.Service.ActionShell](https://www.nuget.org/packages/Tore.Service.ActionShell/)

Dependencies: <br/>
&emsp; net5.0 <br/>
&emsp; Microsoft.AspNetCore.Mvc.NewtonsoftJson (>= 5.0.10) [Please refer to note 4 below]<br/>

## ActionShell :

A standard action filter attribute class for .Net 5 web API <br/>

Why?<br/>

* Bored of writing the same thing every time for services.
* This code contains the common behaviour of an action filter attribute.
* Having project dependent request scope data is so good.
* I can gather any data requied and process it and/or check sessions, access caches, databases, get the IP's etc.
<br/>
In the multi tenant server mode (Kestrel default) each request arriving is processed in its own thread,<br/>
the request context is built, then the target controller instance is created.<br/>
If there are action filters:<br/>
Filter OnActionExecuting methods are called before the endpoint is invoked and,<br/>
Filter OnActionExecuted methods are called after the endpoint is left.<br/>
<br/>
ActionShell class as a filter attribute:<br/>

* Constructs a developer defined object descendant of RequestScopeBase class gathering request data.
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

## RequestScopeBase :

To generate project oriented request information, developers must extend the RequestScopeBase class. 
It already collects : 

```C#
public class ExampleRequestScopeClass : RequestScopeBase {
    public string ipAddress {get; private set;} // Let's say we need IP address of requester.
    // other things needed...
    
    public ExampleRequestScopeClass(ActionExecutingContext context):base(context){
        ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        /// collect other data needed.
    }
}
```


---

**Notes:**<br/>
<br/>
1] ExceptionResponder should not raise an exception under any conditions.<br/>
&emsp; It would be like a fire extinguisher catching fire.<br/>

2] If developer exception page is required during development: <br/>
&emsp; Add <br/>
```C#
    GlobalExceptionMiddleWare.ExceptionResponder = SomeClass.AStaticMethodToRespondException;
    app.UseMiddleware<GlobalExceptionMiddleware>();
```
&emsp; Before <br/>

```C#
    app.UseDeveloperExceptionPage();
```

&emsp; That way developer exception page overrides the global exception middleware.<br/>
    <br/>
3] This setup does not handle invalid routes. <br/>
&emsp; For that, invalid routes must be re-routed to a controller endpoint, <br/>
&emsp; If that endpoint raises exception, then GlobalExceptionMiddleware is activated.<br/>
<br/>
4] Why Microsoft.AspNetCore.Mvc.NewtonsoftJson? <br/>
&emsp; Weirdly enough default http abstractions miss some methods like HttpResponse.CloseAsync().<br/>
&emsp; So it saves me from a lot of class chasings and abstractions and I use it in my API's anyway.


A controller action filter with nice features.  
