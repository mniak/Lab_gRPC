using Bogus;
using Grpc.Core;
using Grpc.Net.Client;
using Lab_gRPC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gRPCClient.Features.TestPayment
{
    [ApiController]
    [Route("TestPayment")]
    public class TestPaymentController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly TestPaymentOptions options;

        public TestPaymentController(
            ILogger<TestPaymentController> logger,
            IOptions<TestPaymentOptions> options)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options.Value;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] IDictionary<string, string> form)
        {
            var faker = new Faker();
            if (options.EnableHttp)
            {
                logger.LogDebug("Enabling switch {SwitchName}", "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport");
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }

            logger.LogDebug("TestPaymentOptions: {@Options}", options);

            using var channel = GrpcChannel.ForAddress(options.ServiceUrl);
            var client = new Payment.PaymentClient(channel);

            var headers = new Metadata(){
                {"Authorization", $"Bearer {form["Token-gRPC"]}"}
            };

            var reply = await client.PayAsync(new PaymentRequest
            {
                Amount = faker.Random.UInt(10000),
                CardNumber = faker.Finance.CreditCardNumber(),
                SecurityCode = faker.Finance.CreditCardCvv(),
            }, headers);
            return Ok(new
            {
                Message = "It worked!",
                Response = reply,
            });
        }
    }
}
