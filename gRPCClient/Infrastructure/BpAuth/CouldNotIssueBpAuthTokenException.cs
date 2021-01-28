using System;
using System.Runtime.Serialization;

namespace gRPCClient.Infrastructure.BpAuth
{
    [Serializable]
    public class CouldNotIssueBpAuthTokenException : Exception
    {
        public CouldNotIssueBpAuthTokenException() { }
        public CouldNotIssueBpAuthTokenException(string message) : base(message) { }
        public CouldNotIssueBpAuthTokenException(string message, Exception inner) : base(message, inner) { }
        protected CouldNotIssueBpAuthTokenException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }

        public string Error { get; set; }
    }
}
