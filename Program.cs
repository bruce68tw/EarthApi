using Base.Enums;
using Base.Interfaces;
using Base.Models;
using Base.Services;
using EarthApi.Services;
using EarthLib.Models;
using EarthLib.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddControllers()
    .AddNewtonsoftJson(opts => opts.UseMemberCasing())
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

//http context
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//user info for base component
//services.AddSingleton<IBaseUserS, MyBaseUserS>();

//cache server for Captcha image
services.AddMemoryCache();
services.AddSingleton<ICacheSvc, CacheMemSvc>();

//ado.net for mssql
services.AddTransient<DbConnection, SqlConnection>();
services.AddTransient<DbCommand, SqlCommand>();

//appSettings "FunConfig" section -> _Fun.Config
var config = new ConfigDto();
builder.Configuration.GetSection("FunConfig").Bind(config);
_Fun.Config = config;

//7.appSettings "XpLibConfig" section -> _XpLib.Config
var xpLibConfig = new XpLibConfigDto();
builder.Configuration.GetSection("XpLibConfig").Bind(xpLibConfig);
_XpLib.Config = xpLibConfig;
_XpLib.HasEther = !string.IsNullOrEmpty(_XpLib.Config.EtherHost);

/*
//jwt驗證
services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts => {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,                //是否驗證超時  當設置exp和nbf時有效 
            ValidateIssuerSigningKey = true,        //是否驗證密鑰
            IssuerSigningKey = _Xp.GetJwtKey(),     //SecurityKey
        };
    });
*/

//cors
string[] origins = _Fun.Config.AllowOrigins.Split(',');
services.AddCors(opts => {
    opts.AddDefaultPolicy(a => {
        a.WithOrigins(origins);
        a.AllowAnyHeader();
        a.AllowAnyMethod();
        a.AllowCredentials();
    });    
});

//1.initial & set locale
var app = builder.Build();
var isDev = app.Environment.IsDevelopment();
_Fun.Init(isDev, app.Services, DbTypeEnum.MSSql);

app.UseCors(); //加上後會套用到全域
//app.UseAuthentication();    //認証
//app.UseAuthorization();     //授權

//app.UseHttpsRedirection();    //如果IIS沒有https會出現redirect error !!
app.MapControllers();
app.Run();