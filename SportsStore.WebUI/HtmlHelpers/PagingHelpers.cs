using System;
using System.Text;
using System.Web.Mvc;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.HtmlHelpers
{
    /// <summary>
    /// Helpers for paging.
    /// </summary>
    public static class PagingHelpers
    {
        /// <summary>
        /// Static extension method for page links.
        /// </summary>
        /// <param name="html">this<c>HtmlHelper</c>.</param>
        /// <param name="pagingInfo"><c>PagingInfo</c> info about page.</param>
        /// <param name="pageUrl">Url of page.</param>
        /// <returns>Created list of pages.</returns>
        public static MvcHtmlString PageLinks(this HtmlHelper html,
                                              PagingInfo pagingInfo,
                                              Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
                tag.AddCssClass("border");
                tag.AddCssClass("border-primary");

                if (i == pagingInfo.CurrentPage)
                {
                    tag.AddCssClass("selected");
                    tag.AddCssClass("btn-primary");
                }

                tag.AddCssClass("btn");
                result.Append(tag);
            }

            return MvcHtmlString.Create(result.ToString());
        }
    }
}