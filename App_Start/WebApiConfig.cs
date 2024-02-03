using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MariamProject
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
            name: "LocationApi",
            routeTemplate: "api/location/{action}/{id}",
            defaults: new { controller = "Location" }
        );

        }
    }
}
