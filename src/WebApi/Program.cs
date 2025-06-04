using Scalar.AspNetCore;
using Service.Client;
using Service.Config;
using Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IFundaClient, FundaClient>();
builder.Services.AddScoped<IRealEstateAgentsService, RealEstateAgentsService>();
builder.Services.AddHttpClient();

builder.Services.Configure<FundaPartnerApiConfig>(builder.Configuration.GetSection("FundaPartnerApi"));

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