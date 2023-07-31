using System.Text;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            });


            services.AddScoped<TokenServices>(); //建立一個製作Token的空間，有HTTP請求會重新做一個Scoped

            return services;
        }
    }
}