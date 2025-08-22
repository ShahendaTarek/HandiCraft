using HandiCraft.Domain.ProductList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Specificatoins
{
    public  class ProductSpecParams
    {
        private const int MaxPageSize = 50;

        private int _pageSize = 10;
        //public int PageIndex { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : (value <= 0 ? 10 : value);
        }
        private int _pageIndex = 1;
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = (value <= 0 ? 1 : value);
        }


        public string? Search { get; set; }
        public string? Sort { get; set; }

        public string ? CategoryName { get; set; }
        public string? Material { get; set; }
        public string? Type { get; set; }
    }
}
