using Scalar.AspNetCore;
using Service.Client;
using Service.Config;
using Service.Services;
using WeApi.Handler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.Configure<FundaPartnerApiConfig>(builder.Configuration.GetSection("FundaPartnerApi"));
builder.Services.Configure<CacheConfig>(builder.Configuration.GetSection("Cache"));

builder.Services.AddSingleton<IFundaClient, FundaClient>();

builder.Services.AddScoped<ICacheService, InMemoryCacheService>();
builder.Services.AddScoped<RealEstateAgentsService>();
builder.Services.AddScoped<IRealEstateAgentsService>(sp =>
{
    var cache = sp.GetRequiredService<ICacheService>();
    var realEstateAgentsService = sp.GetRequiredService<RealEstateAgentsService>();

    return new RealEstateAgentsCachedService(cache, realEstateAgentsService);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();