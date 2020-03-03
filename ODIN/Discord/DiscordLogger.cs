using System.Threading.Tasks;
using Discord;
namespace IslaBot.Discord
{
    public class DiscordLogger
    {
        ILogger _logger;

        public DiscordLogger(ILogger logger)
        {
            _logger = logger;
        }
        public Task Log(LogMessage LogMsg)
        {
            _logger.Log(LogMsg.Message);
            return Task.CompletedTask;
        }
    }
}
