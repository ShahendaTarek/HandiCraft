
using HandiCraft.Application.Extentions;
using HandiCraft.Application.Interfaces;
using HandiCraft.Application.Mapping;
using HandiCraft.Infrastructure.EmailServices;
using HandiCraft.Infrastructure.Services;
using HandiCraft.Infrastructure.Services.Authentication.IdentityServices;
using HandiCraft.Infrastructure.Services.ProductList;
using HandiCraft.Infrastructure.Services.Social;
using HandiCraft.Presentation.MiddleWares;
using HandiCraft.Presistance.Identity;
using Microsoft.EntityFrameworkCore;

namespace HandiCraft.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<EmailSettings>(
                 builder.Configuration.GetSection("EmailSettings")
            );

            builder.Services.AddTransient<IEmailService, EmailServices>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDbContext<HandiCraftDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddScoped<IPostServices, PostServices>();
            builder.Services.AddScoped<IProductListServices, ProductListServices>();
            builder.Services.AddScoped<GlobalSearchServices>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleWare>();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
