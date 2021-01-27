using Bogus;
using Grpc.Core;
using Lab_gRPC;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gRPCClient.Features.TestPayment
{
    [ApiController]
    [Route("TestPayment")]
    public class TestPaymentController : ControllerBase
    {
        private readonly Payment.PaymentClient client;

        public TestPaymentController(Payment.PaymentClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] IDictionary<string, string> form)
        {
            var faker = new Faker();

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
