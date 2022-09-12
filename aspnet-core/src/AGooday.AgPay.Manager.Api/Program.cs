using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Infrastructure.Context;
using AGooday.AgPay.Manager.Api.Extensions;
using AGooday.AgPay.Manager.Api.Middlewares;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var Env = builder.Environment;


//services.AddDbContext<AgPayDbContext>(options =>
//    options.UseMySql(builder.Configuration.GetConnectionString("Default"),
//    MySqlServerVersion.LatestSupportedServerVersion));

services.AddDbContext<AgPayDbContext>();

// Automapper 注入
services.AddAutoMapperSetup();
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// Adding MediatR for Domain Events
// 领域命令、领域事件等注入
// 引用包 MediatR.Extensions.Microsoft.DependencyInjection
//services.AddMediatR(typeof(MyxxxHandler));//单单注入某一个处理程序
//或
services.AddMediatR(typeof(Program));//目的是为了扫描Handler的实现对象并添加到IOC的容器中

// .NET Core 原生依赖注入
// 单写一层用来添加依赖项，从展示层 Presentation 中隔离
NativeInjectorBootStrapper.RegisterServices(services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
