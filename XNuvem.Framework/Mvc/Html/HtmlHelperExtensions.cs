using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace XNuvem.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        public static MvcForm XNBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName,
            object routeValues)
        {
            return htmlHelper.BeginForm(
                actionName,
                controllerName,
                new RouteValueDictionary(routeValues),
                FormMethod.Post,
                new Dictionary<string, object>
                {
                    {"role", "form"},
                    {"data-xn-form", null}
                });
        }

        public static MvcForm XNBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName,
            string areaName)
        {
            return htmlHelper.BeginForm(
                actionName,
                controllerName,
                new RouteValueDictionary(new {area = areaName}),
                FormMethod.Post,
                new Dictionary<string, object>
                {
                    {"role", "form"},
                    {"data-xn-form", null}
                });
        }

        public static MvcHtmlString XNAjaxDropDownFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, string lookupAction, string lookupController,
            string lookupArea)
        {
            var urlValue =
                new UrlHelper(html.ViewContext.RequestContext).Action(lookupAction, lookupController,
                    new {area = lookupArea});
            return html.EditorFor(expression, "XNAjaxDropDown", new {Url = urlValue});
        }

        public static MvcHtmlString XNAjaxDropDownFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, string lookupAction, string lookupController,
            string lookupArea, string selectedText)
        {
            var urlValue =
                new UrlHelper(html.ViewContext.RequestContext).Action(lookupAction, lookupController,
                    new {area = lookupArea});
            return html.EditorFor(expression, "XNAjaxDropDown", new {Url = urlValue, SelectedText = selectedText});
        }
    }
}