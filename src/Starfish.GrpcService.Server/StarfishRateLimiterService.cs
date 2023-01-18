using System.Collections.Concurrent;
using System.Threading.RateLimiting;

namespace Starfish.GrpcService.Server;

public class StarfishRateLimiterService : IStarfishRateLimiterService
{
    private readonly ConcurrentDictionary<string, TokenBucketRateLimiter> _limiters = new();

    public StarfishRateLimiterService()
    {
        InitialiseLimiters();
        _ = new Timer(ReplenishLimits, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    public async Task<RateLimitLease?> AcquireAsync(string clientId, CancellationToken ctx)
    {
        clientId = string.IsNullOrEmpty(clientId) ? "GuestBucket000" : clientId;

        if (_limiters.ContainsKey(clientId) == false)
        {
            // WarningLog: clientId should be added to the clients. It's using the guest bucket for now. 
            await _limiters["GuestBucket000"].AcquireAsync(1, ctx);
        }

        return await _limiters[clientId].AcquireAsync(1, ctx);
    }
    
    private void InitialiseLimiters()
    {
        var clients = new[] {"GuestBucket000", "Meta", "Google", "Twitter", "Dragon" };
        foreach (var client in clients)
        {
            _limiters.TryAdd(client, new TokenBucketRateLimiter(TokenBucketOptions));
        }
    }
    
    private void ReplenishLimits(object? state)
    {
        Parallel.ForEach(_limiters, (limiter) =>
        {
            limiter.Value.TryReplenish();
        });
    }

    private static TokenBucketRateLimiterOptions TokenBucketOptions => new() 
    {
        TokenLimit = 30,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 0,
        TokensPerPeriod = 30,
        ReplenishmentPeriod = TimeSpan.FromSeconds(1),
        AutoReplenishment = false
    };
}