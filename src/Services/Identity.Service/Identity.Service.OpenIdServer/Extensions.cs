using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Service.OpenIdServer;

public static class Extensions
{
    public static bool MatchRouteData(this HttpContext httpContext, string controller, string action)
        => httpContext.GetRouteValue("controller").ToString().Equals(controller, StringComparison.OrdinalIgnoreCase)
           && httpContext.GetRouteValue("action").ToString().Equals(action, StringComparison.OrdinalIgnoreCase);
}