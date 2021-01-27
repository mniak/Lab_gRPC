using Bogus;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
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

        [Authorize]
        public override Task<PaymentReponse> Pay(PaymentRequest request, ServerCallContext context)
        {
            var faker = new Faker();
            var user = context.GetHttpContext().User;
            var response = new PaymentReponse
            {
                Authorized = faker.Random.Bool(),
                PaymentId = Convert.ToBase64String(faker.Random.Bytes(16)),
                ClientId = user.FindFirst("client_id").Value,
            };
            return Task.FromResult(response);
        }
    }
}
