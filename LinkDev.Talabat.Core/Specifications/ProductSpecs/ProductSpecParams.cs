using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Specifications.ProductSpecs
{
    // When we have multiple parameters (more than 3) to filter or sort products, we create a class to hold these parameters.
    public class ProductSpecParams
    {
        private const int MaxPageSize = 10;
        private int pageSize = 5;
        private string? search;

        public string? Search {
            get => search;
            set => search = value?.ToLower();
        }
        public string? Sort { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public int PageSize {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
        public int PageIndex { get; set; } = 1;
    }
}
