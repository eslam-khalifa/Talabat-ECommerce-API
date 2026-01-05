using LinkDev.Talabat.Core.Entities.Basket;
using LinkDev.Talabat.Core.Repositories.Contracts;
using StackExchange.Redis;
using System.Text.Json;

namespace LinkDev.Talabat.Infrastructure.BasketRepository
{
    public class BasketRepository(IConnectionMultiplexer connectionMultiplexer) : IBasketRepository
    {
        private readonly IDatabase database = connectionMultiplexer.GetDatabase();

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var basket = await database.StringGetAsync(basketId);
            if (basket.IsNullOrEmpty) return null;
            return JsonSerializer.Deserialize<CustomerBasket>(basket);
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket customerBasket)
        {
            var createdOrUpdated = await database.StringSetAsync(customerBasket.Id, JsonSerializer.Serialize(customerBasket), TimeSpan.FromDays(30));
            if (!createdOrUpdated) return null;
            return await GetBasketAsync(customerBasket.Id);
        }
    }
}
