using System.Text;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services 
            , IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt => //設定一些內建的驗證規則
                {
                    opt.Password.RequireNonAlphanumeric = false; //關閉特殊字元請求
                    opt.User.RequireUniqueEmail = true; //開啟Email不重複註冊
                }
            ).AddEntityFrameworkStores<DataContext>();


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])); //建立檢驗金鑰

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => //驗證JWT
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                opt.Events = new JwtBearerEvents //SignalR特殊驗證方式
                {
                    OnMessageReceived = context => //委派->context變成MessageReceivedContext下面就是操作MessageReceivedContext物件
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(opt =>     //註冊驗證規則
            {
                opt.AddPolicy("IsActivityHost", policy =>  //驗證規則名稱
                {
                    policy.Requirements.Add(new IsHostRequirement());  //驗證規則的注入函式
                });
            });
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();  //注入容器，要求"IsActivityHost"策略

            services.AddScoped<TokenServices>(); //建立一個製作Token的空間，有HTTP請求會重新做一個Scoped

            return services;
        }
    }
}

