using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Payola.Helpers
{
    public static class HtmlExtensions
    {

        /// <summary>
        ///     Creates a link with span'ed text inside.
        /// </summary>
        /// <param name="html">Helper.</param>
        /// <param name="linkText">The text inside the span.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="controllerName">Name of the controller. If null, uses current controller.</param>
        /// <param name="routeDict">Routing information. May be null.</param>
        /// <param name="spanAttributeDict">Attributes of the span HTML element. May be null.</param>
        /// <returns>MvcHtmlString of the link tag.</returns>
        public static MvcHtmlString ActionLinkWithSpan(this HtmlHelper html,
                                                       string linkText,
                                                       string actionName,
                                                       string controllerName,
                                                       object routeDict,
                                                       object spanAttributeDict)
        {
            TagBuilder linkTag = new TagBuilder("a");
            TagBuilder spanTag = new TagBuilder("span");
            spanTag.SetInnerText(linkText);
            if (spanAttributeDict != null)
            {
                spanTag.MergeAttributes(new RouteValueDictionary(spanAttributeDict));
            }

            UrlHelper url = new UrlHelper(html.ViewContext.RequestContext);
            string actionURLString;
            if (controllerName != null){
                if (routeDict == null){
                    actionURLString = url.Action(actionName, controllerName);
                }else{
                    actionURLString = url.Action(actionName, controllerName, routeDict);
                }
            }else{
                 if (routeDict == null){
                    actionURLString = url.Action(actionName);
                }else{
                    actionURLString = url.Action(actionName, routeDict);
                }
            }
            
            linkTag.Attributes.Add("href", actionURLString);
            linkTag.InnerHtml = spanTag.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(linkTag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        ///     <see cref="ActionLinkWithSpan"/>
        /// </summary>
        /// <param name="html">Helper.</param>
        /// <param name="linkText">The text inside the span.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="controllerName">Name of the controller. If null, uses current controller.</param>
        /// <param name="routeDict">Routing information. May be null.</param>
        /// <returns>MvcHtmlString of the link tag.</returns>
        public static MvcHtmlString ActionLinkWithSpan(this HtmlHelper html,
                                                       string linkText,
                                                       string actionName,
                                                       string controllerName,
                                                       object routeDict)
        {
            return ActionLinkWithSpan(html, linkText, actionName, controllerName, routeDict, null);
        }


        /// <summary>
        ///     <see cref="ActionLinkWithSpan"/>
        /// </summary>
        /// <param name="html">Helper.</param>
        /// <param name="linkText">The text inside the span.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="controllerName">Name of the controller. If null, uses current controller.</param>
        /// <returns>MvcHtmlString of the link tag.</returns>
        public static MvcHtmlString ActionLinkWithSpan(this HtmlHelper html,
                                                       string linkText,
                                                       string actionName,
                                                       string controllerName)
        {
            return ActionLinkWithSpan(html, linkText, actionName, controllerName, null, null);
        }

        /// <summary>
        ///     <see cref="ActionLinkWithSpan"/>
        /// </summary>
        /// <param name="html">Helper.</param>
        /// <param name="linkText">The text inside the span.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>MvcHtmlString of the link tag.</returns>
        public static MvcHtmlString ActionLinkWithSpan(this HtmlHelper html,
                                                       string linkText,
                                                       string actionName)
        {
            return ActionLinkWithSpan(html, linkText, actionName, null, null, null);
        }

        /// <summary>
        ///     <see cref="ActionLinkWithSpan"/>
        /// </summary>
        /// <param name="html">Helper.</param>
        /// <param name="linkText">The text inside the span.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="routeDict">Routing information. May be null.</param>
        /// <param name="spanAttributeDict">Attributes of the span HTML element. May be null.</param>
        /// <returns>MvcHtmlString of the link tag.</returns>
        public static MvcHtmlString ActionLinkWithSpan(this HtmlHelper html,
                                                       string linkText,
                                                       string actionName,
                                                       object routeDict,
                                                       object spanAttributeDict)
        {
            return ActionLinkWithSpan(html, linkText, actionName, null, routeDict, spanAttributeDict);
        }

        /// <summary>
        ///     <see cref="ActionLinkWithSpan"/>
        /// </summary>
        /// <param name="html">Helper.</param>
        /// <param name="linkText">The text inside the span.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="routeDict">Routing information. May be null.</param>
        /// <returns>MvcHtmlString of the link tag.</returns>
        public static MvcHtmlString ActionLinkWithSpan(this HtmlHelper html,
                                                       string linkText,
                                                       string actionName,
                                                       object routeDict)
        {
            return ActionLinkWithSpan(html, linkText, actionName, null, routeDict, null);
        }

        

        
    }
}