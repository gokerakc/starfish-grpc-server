using System.Text.Json;
using Grpc.Core;
using Starfish.Web;

namespace Starfish.GrpcService.Server.Services;

public class FraudCheckerService : FraudChecker.FraudCheckerBase
{
    private readonly ILogger<FraudCheckerService> _logger;

    public FraudCheckerService(ILogger<FraudCheckerService> logger)
    {
        _logger = logger;
    }

    public override Task<FraudReport> Check(TransactionDetailsRequest request, ServerCallContext context)
    {
        _logger.LogInformation(JsonSerializer.Serialize(request));

        Thread.Sleep(TimeSpan.FromMilliseconds(130));
        
        return Task.FromResult(new FraudReport{TransactionStatus = FraudReport.Types.StatusTypes.Valid});
    }
}