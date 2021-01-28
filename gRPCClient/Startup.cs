using gRPCClient.Features.TestPayment;
using gRPCClient.Infrastructure.BpAuth;
using Lab_gRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;
using System;

namespace gRPCClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<BpAuthOptions>(Configuration.GetSection("BpAuth"));


            var testPaymentOptions = Configuration.GetSection("TestPayment").Get<TestPaymentOptions>();
            if (testPaymentOptions.EnableHttp)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
            services.AddGrpcClient<Payment.PaymentClient>(o =>
            {
                o.Address = testPaymentOptions.ServiceUrl;
            }).AddHttpMessageHandler<BpAuthTokenMessageHandler>();
            services.AddSingleton<BpAuthTokenMessageHandler>();

            services.AddSingleton<IHostedService, BpAuthTokenHostedService>();
            services.AddPolicyRegistry()
                .Add(nameof(BpAuthTokenHostedService) + "." + nameof(AsyncRetryPolicy<bool>), Policy.HandleResult(false)
                    .WaitAndRetryForeverAsync(n => n > 6 /* 2^6 = 64 (1 minute max) */
                        ? TimeSpan.FromMinutes(1)
                        : TimeSpan.FromSeconds(Math.Pow(2, n))
                    )
                );
            services.AddHttpClient<IBpAuthTokenClient, BpAuthTokenClient>();
            services.AddSingleton<IBpAuthTokenHolder, BpAuthTokenHolder>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
