# unhandled-exception-handler

[![Build status](https://ci.appveyor.com/api/projects/status/car1psys90556srk?svg=true)](https://ci.appveyor.com/project/spektrum/unhandled-exception-handler)
[![NuGet](https://img.shields.io/nuget/dt/Spk.UnhandledExceptionHandlerUi.svg)](https://www.nuget.org/packages/Spk.UnhandledExceptionHandlerUi/)
[![NuGet](https://img.shields.io/nuget/dt/Spk.UnhandledExceptionHandlerCore.svg)](https://www.nuget.org/packages/Spk.UnhandledExceptionHandlerCore/)

An exception catcher for .Net MVC 4 and 5.

## Seting up the Unhandled Error Handler in an MVC solution

In your solution's front-end or site project (often called **MyProject.Site** or **MyProject.Web**), install the `Ui`
package:

`Install-Package Spk.UnhandledExceptionHandlerUi`

This will also install `Spk.UnhandledExceptionHandlerCore` as a dependancy.

Be cautious when accepting (or not) NuGet's requests to overwrite files. If your project or solution is brand new, you
can probably accept to overwrite the `FilterConfig.cs` file.

If not installed, you will need to install NLog:

`Install-Package NLog`

### Wire the handler to your HttpApplication

Open `Global.asax.cs`. Add a new function that will be used as a _catch all_ error handler:

```csharp
private void OnUnhandledError(object sender, EventArgs e)
{
    try
    {
        AppErrorUtils.OnApplicationError(sender, e, Server.GetLastError());
    }
    catch (Exception exception)
    {
        LoggingUtils.LogAllExceptions(exception);
    }
}
```

Then add it to your application's list of error handlers. This can be done from the constructor:

```csharp
public class Application : System.Web.HttpApplication
{
    public Application()
    {
        Error += OnUnhandledError;
    }

    /* ... */
}
```

### Global filter configuration

This library also comes with a filter that helps managing situations where `NotFoundException`s are thrown (which is
different from a controller _smoothly_ returning a 404 response). If your project already had a FilterConfig.cs, and you
asked the installer not to overwrite it, then the library's `HttpNotFoundFilter` must be manually added like this:

```csharp
public class FilterConfig
{
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
        filters.Add(new HttpNotFoundFilter { Order = 1 });
    }
}
```

You should then make sure the filters are loaded when your application starts.

Here's an example for a basic MVC application:

```csharp
public class Application : System.Web.HttpApplication
{
    /* ... */

    protected void Application_Start()
    {
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    }
}
```

Here's another example for an Umbraco application:

```csharp
public class Application : Umbraco.Web.UmbracoApplication
{
    /* ... */

    protected override void OnApplicationStarted(object sender, EventArgs e)
    {
        base.OnApplicationStarted(sender, e);
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    }
}
```