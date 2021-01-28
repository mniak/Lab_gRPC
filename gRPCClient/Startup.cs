using gRPCClient.Features.TestPayment;
using gRPCClient.Infrastructure.BpAuth;
using Lab_gRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var testPaymentOptions = Configuration.GetSection("TestPayment").Get<TestPaymentOptions>();
            if (testPaymentOptions.EnableHttp)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
            services.AddGrpcClient<Payment.PaymentClient>(o =>
            {
                o.Address = testPaymentOptions.ServiceUrl;
            }).AddHttpMessageHandler<BpAuthTokenMessageHandler>();

            services.AddSingleton<IHostedService, BpAuthTokenHostedService>();
            services.AddPolicyRegistry()
                .Add("exponential-2-60", Policy.HandleResult(false)
                    .WaitAndRetryForeverAsync(n => n > 6 /* 2^6 = 64 (1 minute max) */
                        ? TimeSpan.FromMinutes(1)
                        : TimeSpan.FromSeconds(Math.Pow(2, n))
                    )
                );
            services.AddHttpClient<IBpAuthTokenClient, BpAuthTokenClient>()
                .AddPolicyHandlerFromRegistry("exponential-2-60");
            services.AddSingleton<IBpAuthTokenHolder, BpAuthTokenHolder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
