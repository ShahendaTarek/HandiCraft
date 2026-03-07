
using HandiCraft.Application.Extentions;
using HandiCraft.Application.Hubs;
using HandiCraft.Application.Interfaces;
using HandiCraft.Application.Mapping;
using HandiCraft.Domain.UserAccount;
using HandiCraft.Infrastructure.EmailServices;
using HandiCraft.Infrastructure.Services;
using HandiCraft.Infrastructure.Services.Authentication.IdentityServices;
using HandiCraft.Infrastructure.Services.Order;
using HandiCraft.Infrastructure.Services.ProductList;
using HandiCraft.Infrastructure.Services.Social;
using HandiCraft.Infrastructure.Services.UserConnections;
using HandiCraft.Presentation.MiddleWares;
using HandiCraft.Presistance.context;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

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
            builder.Services.AddSignalR();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDbContext<HandiCraftDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(connection);

            });
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddScoped<IPostServices, PostServices>();
            builder.Services.AddScoped<IProductListServices, ProductListServices>();
            builder.Services.AddScoped<GlobalSearchServices>();
            builder.Services.AddScoped<INotificationService, NotificationServices>();
            builder.Services.AddScoped<IUserConnectionsServices, UserconnectionServices>();
            builder.Services.AddScoped<ISettingsServices, SettingsServices>();
            builder.Services.AddScoped<IFavoriteServices, FavoriteServices>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<ICartServices, CartServices>();
            builder.Services.AddScoped<IOrderServices,OrderServices>();
            builder.Services.AddScoped<IPaymentServices, PaymentServices>();
            builder.Services.AddScoped<IPaymobServices, PaymobServices>();
            builder.Services.AddHttpClient<IPaymobServices, PaymobServices>();



            var app = builder.Build();

           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleWare>();
               
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapHub<ChatHub>("/hubs/chat");
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
