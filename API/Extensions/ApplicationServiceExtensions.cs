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
            services.AddDbContext<DataContext>(opt => 
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection")); //資料庫來源
            });
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy => 
                {
                    policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000"); //首頁
                });
            });
            services.AddMediatR(typeof(List.Handler)); 
            services.AddAutoMapper(typeof(MappingProfiles).Assembly); //AutoMapper
            services.AddFluentValidationAutoValidation();
                //當需要進行模型驗證時使用FluentValidation的驗證功能
            services.AddValidatorsFromAssemblyContaining<Create>(); 
                //自動將所有繼承自 AbstractValidator 的類型註冊為驗證器。(同namespace註冊一次即可)
            services.AddHttpContextAccessor(); //http請求取得服務
            services.AddScoped<IUserAccessor, UserAccessor>(); //取得當前用戶
            services.AddScoped<IPhotoAccessor, PhotoAccessor>(); //取得用戶圖片
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary")); //設定雲端

            return services;

        }    
    }
}