using API.Extensions;
using API.Middleware;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt => //註冊各項controller 
    {  
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        opt.Filters.Add(new AuthorizeFilter(policy));
    }
); 

builder.Services.AddApplicationServices(builder.Configuration); //註冊我們要的額外服務，傳入builder.Configuration給裡面需要Configuration的服務

builder.Services.AddIdentityServices(builder.Configuration); //註冊身分驗證的額外服務，Extensions

var app = builder.Build(); //內建生成

app.UseMiddleware<ExceptionMiddleware>(); //使用中介器<我們做的例外中介器>

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) //若在開發模式，則用swagger
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy"); 

app.UseAuthentication(); //身分驗證
app.UseAuthorization(); //授權

app.MapControllers();

using var scope = app.Services.CreateScope(); 
var services = scope.ServiceProvider;  

try //Migrate(移民)我們做好的Seed並建構DB
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context , userManager);
}
catch (Exception e)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(e, "An error occured during migrations");
}

app.Run();
