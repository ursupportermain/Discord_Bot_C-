
using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Discord.Core.Service;

public class DiscordService(
    IConfiguration configuration,
    DiscordSocketClient discordSocketClient,
    InteractionService interactionService,
    IServiceProvider serviceProvider
    ) : BackgroundService

{
    private IGuild? primaryGuild;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        {
            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), scope.ServiceProvider);
        }
        discordSocketClient.Ready += ReadyAsync;
        discordSocketClient.SlashCommandExecuted += SlashCommandExecutedAsync;
        await discordSocketClient.LoginAsync(TokenType.Bot, configuration["Discord:Token"]);

        await discordSocketClient.StartAsync();
        stoppingToken.Register(async () =>
        {
            await discordSocketClient.StopAsync();
        });
    }
    private async Task SlashCommandExecutedAsync(SocketSlashCommand command)
    {
        var socketInteractionContext = new SocketInteractionContext(discordSocketClient, command);
        await interactionService.ExecuteCommandAsync(socketInteractionContext, serviceProvider);
    }
    private async Task ReadyAsync()
    {
        if (primaryGuild != null)
        {
            return;
        }
        primaryGuild = discordSocketClient.Guilds.First(x => x.Name == "");
        if (primaryGuild == null)
        {
            return;
        }
        await interactionService.RegisterCommandsToGuildAsync(primaryGuild.Id);
    }
}
