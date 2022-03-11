using Basket.API.GrpcServices;
using Basket.API.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using static Discount.Grpc.Protos.DiscountProtoService;

namespace Basket.API
{
    public class Startup
    {
        public IConfiguration _config { get; set; }
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Redis Configuration
            services.AddStackExchangeRedisCache(option =>
            {                
                option.Configuration = _config.GetValue<string>("CacheSettings:ConnectionString");               

            });

            //General Configuration
            services.AddScoped<IBasketRepository, BasketRepository>();
            //placing the type of same assembly
            services.AddAutoMapper(typeof(Startup));

            //Grpc configuration
            services.AddGrpcClient<DiscountProtoServiceClient>
                (o => o.Address = new Uri(_config["GrpcSettings:DiscountUrl"]));
            services.AddScoped<DiscountGrpcServices>();

            //MassTransit RabbitMq Configuration for publishing
            services.AddMassTransit(config => {
            config.UsingRabbitMq((context, cfg) => {
                cfg.Host(_config["EventBusSettings:HostAddress"]);
             });
            });
            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
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
