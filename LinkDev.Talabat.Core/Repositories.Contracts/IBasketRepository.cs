using LinkDev.Talabat.Core.Entities.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Repositories.Contracts
{
    public interface IBasketRepository
    {
        Task<bool> DeleteBasketAsync(string basketId);
        Task<CustomerBasket?> GetBasketAsync(string basketId);
        Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket CustomerBasket);
    }
}
