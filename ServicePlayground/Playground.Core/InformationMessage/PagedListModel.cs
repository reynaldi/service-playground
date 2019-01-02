using System.Collections.Generic;

namespace Playground.Core.InformationMessage
{
    /// <summary>
    /// Defines the <see cref="PagedListModel{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedListModel<T>
    {
        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the index from.
        /// </summary>
        public int IndexFrom { get; set; }

        /// <summary>
        /// Gets or sets the Items
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HasPreviousPage
        /// Gets the has previous page.
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HasNextPage
        /// Gets the has next page.
        /// </summary>
        public bool HasNextPage { get; set; }
    }
}
