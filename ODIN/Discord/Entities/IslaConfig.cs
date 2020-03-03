using Discord.WebSocket;

namespace IslaBot.Discord.Entities
{
    class IslaConfig
    {
        public string Token { get; set; }
        public DiscordSocketConfig SocketConfig { get; set; }

    }

}
