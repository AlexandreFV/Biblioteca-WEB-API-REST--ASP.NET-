using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Application.Commoms.Pagination
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        private const int MinPage = 1;
        private const int MinPageSize = 1;

        private int _page = 1;
        public int Page
        {
            get => _page;
            set => _page = value < MinPage ? MinPage : value;
        }

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value < MinPageSize)
                    _pageSize = MinPageSize;
                else if (value > MaxPageSize)
                    _pageSize = MaxPageSize;
                else
                    _pageSize = value;
            }
        }
    }
}
