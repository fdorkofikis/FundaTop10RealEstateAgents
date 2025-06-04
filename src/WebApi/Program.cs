using Scalar.AspNetCore;
using Service.Client;
using Service.Config;
using Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.Configure<FundaPartnerApiConfig>(builder.Configuration.GetSection("FundaPartnerApi"));
builder.Services.Configure<CacheConfig>(builder.Configuration.GetSection("Cache"));

builder.Services.AddSingleton<IFundaClient, FundaClient>();

builder.Services.AddScoped<IRealEstateAgentsService, RealEstateAgentsService>();
builder.Services.AddScoped<ICacheService, InMemoryCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();