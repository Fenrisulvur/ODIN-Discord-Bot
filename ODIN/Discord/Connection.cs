using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using IslaBot.Discord.Entities;
using Microsoft.Extensions.DependencyInjection;
using SharpLink;
using SharpLink.Stats;

namespace IslaBot.Discord
{
    public class Connection   
    {
        public static DiscordSocketClient _client;
        private DiscordLogger _logger;
        private CommandService _commands;
        private IServiceProvider _services;
        private AudioService _audio;
        private LavalinkManager lavalinkManager;
        public static ulong _chan = 0;

        public Connection(DiscordLogger logger)
        {
            _logger = logger;
        }

        internal async Task ConnectAsync(IslaConfig config)
        {
            
            _commands = new CommandService();
            _client = new DiscordSocketClient(config.SocketConfig);
            lavalinkManager = new LavalinkManager(_client, new LavalinkManagerConfig
            {
                RESTHost = "localhost",
                RESTPort = 2333,
                WebSocketHost = "localhost",
                WebSocketPort = 80,
                Authorization = "Isla",
                TotalShards = 1
            });
            _audio = new AudioService(lavalinkManager);
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_audio)
                .AddSingleton(lavalinkManager)
                .BuildServiceProvider();

            lavalinkManager.Log += _logger.Log;
           // lavalinkManager.Stats += StatLog;
            lavalinkManager.TrackEnd += _audio.Ended;

            _client.Log += _logger.Log; 
            _client.MessageReceived += HandleCommand;
            _client.MessageReceived += MessageRecieved;
            _client.UserJoined += MemberJoined;
            _client.LatencyUpdated += Update;

            /*
            _client.Ready += async () =>
            {
                await lavalinkManager.StartAsync();
            };*/


            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _client.LoginAsync(TokenType.Bot, config.Token );
            await _client.StartAsync();
            await _client.SetGameAsync("Testing", "", ActivityType.Listening);

        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) { return; }
            int argPos = 0;
            if (!(message.HasCharPrefix('/', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) { return; }
            var context = new SocketCommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        public async Task Update(int e, int e2)
        {
            MainWindow.SetPop(_client.GetGuild(485775243051401227).Users.Count.ToString());
            MainWindow.SetLat(_client.Latency.ToString());
            await Task.Delay(1);

        }


        public async Task MemberJoined(SocketGuildUser user)
        {
            var builder = new EmbedBuilder()
            .WithTitle("Welcome to the Aesir Discord!")
            .WithDescription("This bot is currently under development.")
            .WithColor(new Color(0xAFA755))
            .WithTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(1531870227499))
            .WithImageUrl("https://pre00.deviantart.net/fe74/th/pre/f/2018/229/8/7/download_by_doomforger-dckduoq.png")
            .WithThumbnailUrl("https://t3.rbxcdn.com/14185a4e9ce1448c0b1b9e7d8279b770");

            await user.SendMessageAsync("", false, builder.Build());
        }
        
        public static async Task SendRemoteMsg(string message)
        {
            var Channel = _client.GetGuild(485775243051401227).GetTextChannel(_chan);
            await Channel.SendMessageAsync(message);

        }
        public async Task MessageRecieved(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            /*
            SocketGuildUser Author = _client.GetGuild(255859557711085578).GetUser(message.Author.Id);
            var role = _client.GetGuild(255859557711085578).Roles.FirstOrDefault(x => x.Name == "Mutey Boi");
            if (Author.Roles.Contains(role))
            { await message.DeleteAsync(); };*/

            if (message.Channel.Id == 557061019848015882)
            {
                var Channel = _client.GetGuild(485775243051401227).GetTextChannel(612908224097550337);
                await Channel.SendMessageAsync(DateTime.Now + " | " + message.Author.Username + ": " + message.Content);
                await Task.Delay(20000);
                await message.DeleteAsync();

            };

            if (message.Channel.Id == 485775474841223182)
            {
                MainWindow.ChatLogUpd(DateTime.Now + " | " + message.Author.Username + ": " + message.Content);
            };
            
           
        }
    }


}

