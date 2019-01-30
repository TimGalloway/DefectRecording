using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DefectServer
{
    //http://blog.slaks.net/2011/09/using-default-controller-in-aspnet-mvc.html
    public static class RoutingExtensions
    {
        #region GetCustomAttributes
        ///<summary>Gets a custom attribute defined on a member.</summary>
        ///<typeparam name="TAttribute">The type of attribute to return.</typeparam>
        ///<param name="provider">The object to get the attribute for.</param>
        ///<returns>The first attribute of the type defined on the member, or null if there aren't any</returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider provider) where TAttribute : Attribute
        {
            return provider.GetCustomAttribute<TAttribute>(false);
        }
        ///<summary>Gets the first custom attribute defined on a member, or null if there aren't any.</summary>
        ///<typeparam name="TAttribute">The type of attribute to return.</typeparam>
        ///<param name="provider">The object to get the attribute for.</param>
        ///<param name="inherit">Whether to look up the hierarchy chain for attributes.</param>
        ///<returns>The first attribute of the type defined on the member, or null if there aren't any</returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider provider, bool inherit) where TAttribute : Attribute
        {
            return provider.GetCustomAttributes<TAttribute>(inherit).FirstOrDefault();
        }
        ///<summary>Gets the custom attributes defined on a member.</summary>
        ///<typeparam name="TAttribute">The type of attribute to return.</typeparam>
        ///<param name="provider">The object to get the attribute for.</param>
        public static TAttribute[] GetCustomAttributes<TAttribute>(this ICustomAttributeProvider provider) where TAttribute : Attribute
        {
            return provider.GetCustomAttributes<TAttribute>(false);
        }
        ///<summary>Gets the custom attributes defined on a member.</summary>
        ///<typeparam name="TAttribute">The type of attribute to return.</typeparam>
        ///<param name="provider">The object to get the attribute for.</param>
        ///<param name="inherit">Whether to look up the hierarchy chain for attributes.</param>
        public static TAttribute[] GetCustomAttributes<TAttribute>(this ICustomAttributeProvider provider, bool inherit) where TAttribute : Attribute
        {
            if (provider == null) throw new ArgumentNullException("provider");

            return (TAttribute[])provider.GetCustomAttributes(typeof(TAttribute), inherit);
        }
        #endregion


        ///<summary>Creates a route that maps URLs without a controller to action methods in the specified controller</summary>
        ///<typeparam name="TController">The controller type to map the URLs to.</typeparam>
        public static void MapDefaultController<TController>(this RouteCollection routes) where TController : ControllerBase
        {
            routes.MapControllerActions<TController>(typeof(TController).Name, "{action}/{id}", new { action = "Index", id = UrlParameter.Optional });
        }
        ///<summary>Creates a route that only matches actions from the given controller.</summary>
        ///<typeparam name="TController">The controller type to map the URLs to.</typeparam>
        public static void MapControllerActions<TController>(this RouteCollection routes, string name, string url, object defaults) where TController : ControllerBase
        {
            var methods = typeof(TController).GetMethods()
                                             .Where(m => !m.ContainsGenericParameters)
                                             .Where(m => !m.IsDefined(typeof(ChildActionOnlyAttribute), true))
                                             .Where(m => !m.IsDefined(typeof(NonActionAttribute), true))
                                             .Where(m => !m.GetParameters().Any(p => p.IsOut || p.ParameterType.IsByRef))
                                             .Select(m => m.GetActionName());

            routes.Add(name, new Route(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults) { { "controller", typeof(TController).Name.Replace("Controller", "") } },
                Constraints = new RouteValueDictionary { { "action", new StringListConstraint(methods) } }
            });
        }

        private static string GetActionName(this MethodInfo method)
        {
            var attr = method.GetCustomAttribute<ActionNameAttribute>();
            if (attr != null)
                return attr.Name;
            return method.Name;
        }

        class StringListConstraint : IRouteConstraint
        {
            readonly HashSet<string> validValues;
            public StringListConstraint(IEnumerable<string> values) { validValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase); }

            public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
            {
                //Call HashSet<string>.Contains(string), not the covariant Enumerable.Contains(IEnumerable<object>, object)
                //http://blog.slaks.net/2011/12/dark-side-of-covariance.html
                return validValues.Contains((string)values[parameterName]);

            }
        }
    }
}