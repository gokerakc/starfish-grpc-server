using Grpc.Core;
using Starfish.Web;
using RateLimiter = Starfish.Web.RateLimiter;

namespace Starfish.GrpcService.Server.Services;

public class RateLimiterService : RateLimiter.RateLimiterBase
{
    private readonly IStarfishRateLimiterService _rateLimiterService;
    private readonly ILogger<RateLimiterService> _logger;

    public RateLimiterService(IStarfishRateLimiterService rateLimiterService, ILogger<RateLimiterService> logger)
    {
        _rateLimiterService = rateLimiterService;
        _logger = logger;
    }

    public override async Task<RateLimiterResult> Acquire(RateLimiterRequest request, ServerCallContext context)
    {
        var rateLimitLease = await _rateLimiterService.AcquireAsync(request.ClientId, context.CancellationToken);

        if (rateLimitLease == null)
        {
            throw new NullReferenceException("Something wrong with the rate limiter.");
        }
        
        if (rateLimitLease.IsAcquired)
        {
            return new RateLimiterResult{IsAcquired = true, Message = ""};
        }

        var message = string.IsNullOrEmpty(request.ClientId)
            ? "Too many requests. Service is unavailable temporarily."
            : "The request limit has been exceeded.";
        
        return new RateLimiterResult{IsAcquired = false, Message = message};
    }
}