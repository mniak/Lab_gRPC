using System;

namespace gRPCClient.Features.TestPayment
{
    public class TestPaymentOptions
    {
        public Uri ServiceUrl { get; set; }
        public bool EnableHttp { get; set; }
    }
}
