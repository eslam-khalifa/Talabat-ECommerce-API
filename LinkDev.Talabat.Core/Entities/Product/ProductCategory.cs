using LinkDev.Talabat.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Entities.Products
{
    public class ProductCategory : BaseEntity
    {
        public required string Name { get; set; }
    }
}
