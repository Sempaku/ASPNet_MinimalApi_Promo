using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.Json;
using Promo_2022_API.Models;
using Promo_2022_API.APIs;

namespace Promo_2022_API
{
    public class Program
    {
        public static List<Promo> promoList = new List<Promo>
        {
            new Promo() {Name = "Hola", Description="ka", Prizes=new List<Prize>{new Prize{Description="1"} } },
            new Promo() {Name = "Jin"},
            new Promo() {Name = "Emme"}
        };

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            

            RegisterServices(builder.Services);
            IApi promoApi = new PromoAPI(promoList);
            builder.Services.AddTransient<List<Promo>>();
            builder.Services.AddTransient<IApi, PromoAPI>();
            var app = builder.Build();
            Configure(app);

            promoApi.RegisterAPI(app);
            app.Run();



            void Configure(WebApplication app)
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseHttpsRedirection();
            }

            void RegisterServices(IServiceCollection service)
            {
                service.AddEndpointsApiExplorer();
                service.AddEndpointsApiExplorer();
                service.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SystemTSIgnoreNullValues", Version = "v1" });
                });
            }
        }

        
        
    }

}
