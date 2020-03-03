using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using IslaBot;

public class Admin  : ModuleBase<SocketCommandContext>
{
    [Command("Purge")]
    [RequireRole("Officer")]
    [Summary("Deletes x Messages")]
    [Alias("delete")]
    public async Task SayAsync(uint amount)
    {
        var messages = await Context.Channel.GetMessagesAsync((int)amount + 1).FlattenAsync();
        
        await Context.Guild.GetTextChannel(Context.Channel.Id).DeleteMessagesAsync(messages);
        await ReplyAsync("Purge completed. Pressed Delet Dis on "+amount+" Messages.");

    }

    [Command("say")]
    [Summary("Repeats what you say")]
    [RequireRole("Sovereign")]
    [Alias("repeat", "copy", "mimic")]
    public async Task SayAsync([Remainder] string echo)
    {
        // ReplyAsync is a method on ModuleBase
        await ReplyAsync(echo);
    }
}

public class Memes : ModuleBase<SocketCommandContext>
{
    [Command("hanginthere")]
    [Summary("Sayori Meme")]
    [Alias("dontleavemehanging", "justhangingaround", "hangingout")]
    public async Task Hang()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"Pictures\", "hanginthere.png");
        await Context.Guild.GetTextChannel(Context.Channel.Id).SendFileAsync(path, "");
    }
    [Command("die")]
    [Summary("Commit Die")]
    public async Task Dia()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"Pictures\", "islacry.gif");
        await Context.Guild.GetTextChannel(Context.Channel.Id).SendFileAsync(path, "");
    }
    [Command("capn")]
    [Summary("Infinity War Meme")]
    [Alias("thanos")]
    public async Task Dontfeelsogud()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"Pictures\", "infwar.png");
        await Context.Guild.GetTextChannel(Context.Channel.Id).SendFileAsync(path, "");
    }
    [Command("wave")]
    [RequireRole("Sovereign")]
    public async Task SayAsync()
    {
        var _user = Context.User.Username;
        // ReplyAsync is a method on ModuleBase
        await ReplyAsync("Waves at " + $"{_user}");
    }
    [Command("test")]
    [RequireRole("Sovereign")]
    public async Task TestAsync()
    {
        var builder = new EmbedBuilder()
        .WithTitle("Welcome to the Aesir Discord!")
        .WithDescription("Make sure to submit a request to both the Aesir AND ADF groups.\n Once that is done process yourself in the discord by saying 'ODIN Verify' in your Roblox description and then using ''/verify (Roblox UserID)'' in the Processing chat.")
        .WithUrl("https://discordapp.com/%22")
        .WithColor(new Color(0x0ffff))
        .WithTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(1531870227499))
        //.WithImageUrl("https://pre00.deviantart.net/fe74/th/pre/f/2018/229/8/7/download_by_doomforger-dckduoq.png")
        .WithThumbnailUrl("https://t2.rbxcdn.com/bc1ec93f57419f829f9247069cf8ca3a");

        await ReplyAsync("", false, builder.Build());

    }
    [Command("GetPP")]
    
    public async Task GetPP(SocketGuildUser x)
    {
        await ReplyAsync(x.GetAvatarUrl());
    }
 
}

