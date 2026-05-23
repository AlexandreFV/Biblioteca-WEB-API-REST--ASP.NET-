using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Application.Commoms.Pagination
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Page {  get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public bool HasNext => Page < TotalPages;
        public bool HasPrevious => Page > 1;
        
        public PagedResult(IEnumerable<T> items, int totalItems, int page, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            Page = page;
            PageSize = pageSize;

            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }
    }
}
