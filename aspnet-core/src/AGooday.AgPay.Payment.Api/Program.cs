using AGooday.AgPay.Payment.Api.FilterAttributes;
using AGooday.AgPay.Payment.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        //https://blog.poychang.net/using-newtonsoft-json-in-asp-net-core-projects/
        //options.SerializerSettings.Formatting = Formatting.Indented;
        //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();//Json key 首字符小写（大驼峰转小驼峰）
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // 禁用默认模型验证过滤器
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.Configure<MvcOptions>(options =>
{
    // 全局添加自定义模型验证过滤器
    options.Filters.Add<ValidateModelAttribute>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
