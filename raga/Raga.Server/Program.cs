using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Raga.Server;
using Raga.Server.Common.Interfaces;
using Raga.Server.Data;
using Raga.Server.Data.Models;
using Raga.Server.Data.Repositories;
using Raga.Server.Features.Gacha.Commands.PullGacha;
using Raga.Server.Features.Gacha.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Host.UseSerilog((context, loggerConfig)
    => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddDbContext<GachaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("GachaDatabase")));

builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IGachaItemRepository, GachaItemRepository>();
builder.Services.AddScoped<IClanRepository, ClanRepository>();
builder.Services.AddScoped<IFakeDataGenerator<GachaItem>, GachaItemFaker>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PullGachaHandler).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<PullGachaCommandValidator>();
builder.Services.AddLogging();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GachaContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGrpcAuthService();
app.MapGrpcChatService();
app.MapGrpcClanService();
app.MapGrpcGachaService();
app.MapGrpcInfoService();
app.MapGrpcLeaderboardService();
app.MapGrpcMatchmakingService();
app.MapGrpcNotificationService();
app.MapGrpcPlayerService();
app.MapGrpcStoreService();

app.MapGet("/", () => 
    """
    Communication with gRPC endpoints must be made through a gRPC client.
    To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909
    """);

app.Run();