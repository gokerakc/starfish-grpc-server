using System.Threading.RateLimiting;

namespace Starfish.GrpcService.Server;

public interface IStarfishRateLimiterService
{
    public Task<RateLimitLease?> AcquireAsync(string clientId, CancellationToken ctx);
}