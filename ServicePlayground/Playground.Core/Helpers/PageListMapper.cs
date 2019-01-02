namespace Playground.Core.Helpers
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Playground.Core.InformationMessage;

    /// <summary>
    /// Defines the <see cref="PageListMapper" />
    /// </summary>
    public static class PageListMapper
    {
        /// <summary>
        /// The Map
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="items">The items<see cref="IEnumerable{T}"/></param>
        /// <param name="pagedList">The pagedList<see cref="IPagedList{TU}"/></param>
        /// <returns>The <see cref="PagedListModel{T}"/></returns>
        public static PagedListModel<T> Map<T, TU>(this IEnumerable<T> items, IPagedList<TU> pagedList)
        {
            return new PagedListModel<T>()
            {
                Items = items,
                TotalCount = pagedList.TotalCount,
                PageIndex = pagedList.PageIndex,
                IndexFrom = pagedList.IndexFrom,
                PageSize = pagedList.PageSize,
                HasNextPage = pagedList.HasNextPage,
                HasPreviousPage = pagedList.HasPreviousPage,
                TotalPages = pagedList.TotalPages
            };
        }

        /// <summary>
        /// The Map
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item<see cref="IPagedList{T}"/></param>
        /// <returns>The <see cref="PagedListModel{T}"/></returns>
        public static PagedListModel<T> Map<T>(this IPagedList<T> item)
        {
            return new PagedListModel<T>
            {
                Items = item.Items,
                TotalCount = item.TotalCount,
                PageIndex = item.PageIndex,
                IndexFrom = item.IndexFrom,
                PageSize = item.PageSize,
                HasNextPage = item.HasNextPage,
                HasPreviousPage = item.HasPreviousPage,
                TotalPages = item.TotalPages
            };
        }
    }
}
