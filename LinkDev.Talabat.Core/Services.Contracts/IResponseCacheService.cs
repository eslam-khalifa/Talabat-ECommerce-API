namespace LinkDev.Talabat.Core.Services.Contracts
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string key, object response, TimeSpan timeToLive);
        Task<string?> GetCachedResponseAsync(string key);
    }
}
