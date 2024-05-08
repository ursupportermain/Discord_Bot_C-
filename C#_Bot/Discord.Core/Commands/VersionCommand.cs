using Discord;
using Discord.Interactions;
using Discord.Core.Service;

namespace Discord.Core.Commands;

public class VersionCommand(
    VersionService versionService
) : InteractionModuleBase
{
    private readonly VersionService versionService = versionService;

    [SlashCommand("version", "bot-version")]
    public async Task HandleVersionCommand()
    {
        var version = versionService.GetVersion();
        var builder = new EmbedBuilder()
        {
            Color = Color.Blue,
            Title = "Bot Version " + version,
        };
        await RespondAsync(embed: builder.Build());
    }
}
