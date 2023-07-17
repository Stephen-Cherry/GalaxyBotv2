using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GalaxyBot.Data;
using GalaxyBot.Handlers;
using GalaxyBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GalaxyBot.Extensions;

public static class HostBuilderExtensions
{
    private static readonly string _connString = "GalaxyBotDatabase";
    public static IHostBuilder SetAppConfiguration(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureAppConfiguration((hostingContext, configuration) =>
        {
            if (hostingContext.HostingEnvironment.IsDevelopment())
            {
                configuration.AddUserSecrets<Program>();
            }
            else
            {
                configuration.AddEnvironmentVariables();
            }
        });
    }

    public static IHostBuilder SetAppServices(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices((hostingContext, services) =>
        {
            if (hostingContext.Configuration.GetConnectionString(_connString) == null)
            {
                throw new Exception("Missing a database connection string");
            }
            services
            .AddDbContextFactory<GalaxyBotContext>()
            .AddSingleton(new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            })
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(new InteractionServiceConfig())
            .AddSingleton<InteractionService>()
            .AddSingleton<LoggingService>()
            .AddSingleton<BuffReminderService>()
            .AddSingleton<InteractionHandler>();
        });
    }
}