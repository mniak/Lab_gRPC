using Bogus;
using Grpc.Net.Client;
using Lab_gRPC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace gRPCClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestPaymentController : ControllerBase
    {
        private readonly ILogger _logger;

        public TestPaymentController(ILogger<TestPaymentController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            var faker = new Faker();
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            using var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new Payment.PaymentClient(channel);
            var reply = await client.PayAsync(new PaymentRequest
            {
                Amount = faker.Random.UInt(10000),
                CardNumber = faker.Finance.CreditCardNumber(),
                SecurityCode = faker.Finance.CreditCardCvv(),
            });
            return Ok(new
            {
                Message = "It worked!",
                Response = reply,
            });
        }
    }
}
