using API.Extensions;
using API.Middleware;
using API.SignalR;
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

app.UseXContentTypeOptions(); //阻止網路嗅探
app.UseReferrerPolicy(opt => opt.NoReferrer()); //防止DOS/DDOS
app.UseXXssProtection(opt => opt.EnabledWithBlockMode()); //防止XXS跨網站攻擊
app.UseXfo(opt => opt.Deny()); //防止在iframe中使用
app.UseCsp(opt => opt  //防止腳本攻擊(設定白名單)，先用app.UseCspReportOnly增加白名單，增加完再改回app.UseCsp
    .BlockAllMixedContent() //防止載入http及https混和請求
    .StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com")) //程式的自我樣式設定設為安全
    .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:")) //程式的自我字體設定設為安全
    .FormActions(s => s.Self()) //程式的自我動作設為安全
    .FrameAncestors(s => s.Self()) //自我框架設定安全
    .ImageSources(s => s.Self().CustomSources("blob:", "https://res.cloudinary.com")) //自我圖片設定安全
    .ScriptSources(s => s.Self()) //自我腳本設定安全
); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) //若在開發模式，則用swagger
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.Use(async (context, next) => //字定義一個標頭(嚴格安全性)
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
        await next.Invoke();
    });
}

app.UseCors("CorsPolicy"); 

app.UseAuthentication(); //身分驗證
app.UseAuthorization(); //授權

app.UseDefaultFiles(); //使用預設文件
app.UseStaticFiles(); //使用靜態文件(wwwroot)

app.MapControllers(); //路由的控制
app.MapHub<ChatHub>("/chat"); //SignalR的路由
app.MapFallbackToController("Index", "Fallback");

using var scope = app.Services.CreateScope(); 
var services = scope.ServiceProvider;  

try //Migrate(移民)做好的Seed並建構DB
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context , userManager);
}
catch (Exception e)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(e, "An error occurred during migrations");
}

app.Run();
