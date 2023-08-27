using Application.Activities;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using FluentValidation.AspNetCore;
using FluentValidation;
using Application.Interfaces;
using Infrastructure.Security;
using Infrastructure.Photos;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services ,
         IConfiguration config)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            
            services.AddDbContext<DataContext>(options =>
                {
                    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"); 
                    //獲取當前為開發者或客戶模式

                    string connStr; //宣告字串變數

                    if (env == "Development") //若在開發者模式
                    {
                        connStr = config.GetConnectionString("DefaultConnection");
                        //使用application.json中的連線字串
                    }
                    else //若不再開發模式(就是在客戶模式中)
                    {
                        var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
                        //連線字串由 fly secret list 中的 Key 為"DATABASE_URL"中提取值

                        // 切割字串中需要的資料
                        connUrl = connUrl.Replace("postgres://", string.Empty); //以下做一些字串置換、切割...
                        var pgUserPass = connUrl.Split("@")[0];
                        var pgHostPortDb = connUrl.Split("@")[1];
                        var pgHostPort = pgHostPortDb.Split("/")[0];
                        var pgDb = pgHostPortDb.Split("/")[1];
                        var pgUser = pgUserPass.Split(":")[0];
                        var pgPass = pgUserPass.Split(":")[1];
                        var pgHost = pgHostPort.Split(":")[0];
                        var pgPort = pgHostPort.Split(":")[1];
                        var updatedHost = pgHost.Replace("flycast", "internal"); //以上做一些字串置換、切割...

                        connStr = $"Server={updatedHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
                        //重組連線字串
                    }
                    options.UseNpgsql(connStr);  //使用連接字串
                });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy => 
                {
                    policy
                        .AllowAnyMethod()   //允許任意的請求方法(GET、POST..)
                        .AllowAnyHeader()   //允許任意的請求標頭
                        .AllowCredentials() //跨域請求中使用身份驗證憑證(SignalR)
                        .WithOrigins("http://localhost:3000"); //首頁
                });

            });
            
            services.AddMediatR(typeof(List.Handler)); //註冊Handler(同namespace註冊一次即可)
            services.AddAutoMapper(typeof(MappingProfiles).Assembly); //AutoMapper
            services.AddFluentValidationAutoValidation();
                //當需要進行模型驗證時使用FluentValidation的驗證功能
            services.AddValidatorsFromAssemblyContaining<Create>(); 
                //自動將所有繼承自 AbstractValidator 的類型註冊為驗證器。(同namespace註冊一次即可)
            services.AddHttpContextAccessor(); //http請求取得服務
            services.AddScoped<IUserAccessor, UserAccessor>(); //取得當前用戶
            services.AddScoped<IPhotoAccessor, PhotoAccessor>(); //取得用戶圖片
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary")); //設定雲端
            services.AddSignalR(); //註冊SignalR服務

            return services;

        }    
    }
}