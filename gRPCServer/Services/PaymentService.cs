using Bogus;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab_gRPC
{
    public class PaymentService : Payment.PaymentBase
    {
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(ILogger<PaymentService> logger)
        {
            _logger = logger;
        }

        public override Task<PaymentReponse> Pay(PaymentRequest request, ServerCallContext context)
        {
            var faker = new Faker();
            var response = new PaymentReponse
            {
                Authorized = faker.Random.Bool(),
                PaymentId = faker.Random.Guid().ToString(),
            };
            return Task.FromResult(response);
        }
    }
}
