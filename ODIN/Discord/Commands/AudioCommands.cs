using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using IslaBot.Discord;
using SharpLink;

public class Audio : ModuleBase<ICommandContext>
{


    private static AudioService _service;

    private Audio(AudioService AudioSer)
    {
        _service = AudioSer;
    }

    [Command("skip", RunMode = RunMode.Async)]
    [Summary("Skip current song.")]
    [RequireRole("High Lords of Terra")]
    public async Task Skip()
    {
        await _service.Skip();
    }

    [Command("clear", RunMode = RunMode.Async)]
    [Summary("Clears current queue.")]
    [RequireRole("High Lords of Terra")]
    public async Task clr()
    {
        await _service.Clear();
    }

    [Command("leave", RunMode = RunMode.Async)]
    [Summary("Make the bot leave the VC.")]
    [RequireRole("High Lords of Terra")]
    public async Task LeaveCmd()
    {
        await _service.Leave();
    }

    [Command("lq")]
    [Summary("List the current song queue.")]
    public async Task ListQueue()
    {
        await _service.ListQueue(Context);
    }

    [Command("np")]
    [Summary("Post song info.")]
    public async Task NowPlaying()
    {
        await _service.SongInfo(Context);
    }

    [Command("volume", RunMode = RunMode.Async)]
    [Summary("Set volume (0-100).")]
    [RequireRole("Emperor of Mankind")]
    public async Task Volume(uint volume)
    {
        await _service.SetVolumeAsync(volume);
    }

    [Command("seek", RunMode = RunMode.Async)]
    [Summary("Change time pos of song.")]
    [RequireRole("Emperor of Mankind")]
    public async Task Seek(int pos)
    {
        await _service.Seek(Context, pos);
    }

    [Command("play", RunMode = RunMode.Async)]
    [Summary("Play/Queue a song.")]
    [RequireRole("High Lords of Terra")]
    public async Task PlayCmd([Remainder] string song)
    {
        await _service.Play(Context, song);
    }
}