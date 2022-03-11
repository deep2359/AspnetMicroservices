using EventBus.Messages.Common;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ordering.API.EventBusConsumer;
using Ordering.Application;
using Ordering.Infrastructure;

namespace Ordering.API
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationService();
            services.AddInfrastructureService(_config);

            //placing the type of same assembly
            services.AddAutoMapper(typeof(Startup));

            //MassTransit RabbitMq Configuration for consuming from rabbitmq and subscribing it
            //1st need to consume
            //2nd need to receive the endpoint here
            services.AddMassTransit(config => {

                //for consuming from rabbitmq
                //type of consumer class is BasketCheckoutConsumer
                config.AddConsumer<BasketCheckoutConsumer>();

                config.UsingRabbitMq((context, cfg) => {
                    cfg.Host(_config["EventBusSettings:HostAddress"]);

                    //receive endpoint from here
                    //1st provide the queue name of which will be generated in rabbitmq
                    //configure the consumer
                    cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
                    {
                        //configure the consumer
                        //type of consumer class is BasketCheckoutConsumer
                        c.ConfigureConsumer<BasketCheckoutConsumer>(context);
                    });
                });
            });
            services.AddMassTransitHostedService();

            services.AddScoped<BasketCheckoutConsumer>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
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
