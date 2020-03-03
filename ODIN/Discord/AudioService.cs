using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using SharpLink;
using Discord;

namespace IslaBot.Discord
{
    class AudioService
    {
        private static ConcurrentQueue<LavalinkTrack> Queue = new ConcurrentQueue<LavalinkTrack>();
        private readonly LavalinkManager lavalinkManager;
        private static LavalinkPlayer player;
        private static LavalinkTrack CurrentSong;
        private static ICommandContext CachedContext;
        private static bool playing;
        
        public AudioService(LavalinkManager service)
        { 
            lavalinkManager = service;
        }

        public async Task ListQueue(ICommandContext Context) //Print out song queue to discord channel
        {
            var songEmbed = new EmbedBuilder()
                    .WithTitle("Current Playlist")
                    .WithThumbnailUrl(@"https://www.bochumer-weihnacht.de/wp-content/uploads/2016/11/playbutton.png")
                    .WithColor(Color.DarkerGrey);

            string Desc = " ";

            int count = 0;
            LavalinkTrack[] array = Queue.ToArray();
            foreach (LavalinkTrack i in array)
            {
                count++;
                Desc = Desc + $"{count}: {i.Title} \n";
            };

            songEmbed.WithDescription(Desc);
            await Context.Channel.SendMessageAsync("", false, songEmbed.Build());
        }

        public async Task SongInfo(ICommandContext Context) //Print out song data to discord channel
        {
           if (CurrentSong != null)
            {
                var songEmbed = new EmbedBuilder()
                   .WithThumbnailUrl(@"https://www.bochumer-weihnacht.de/wp-content/uploads/2016/11/playbutton.png")
                   .WithDescription($"Now playing: \n Song: {CurrentSong.Title} \n Length: {CurrentSong.Length} \n Uploader: {CurrentSong.Author} \n Link: {CurrentSong.Url}")
                   .WithColor(Color.DarkerGrey)
                   .Build();
                await Context.Channel.SendMessageAsync("", false, songEmbed);
            }
        }

        public async Task Seek(ICommandContext Context,int pos)
        {
            if (CurrentSong != null && playing == true)
            {
                await player.SeekAsync(pos);
            }
        }

        public async Task Play(ICommandContext Context, string song) //Play song and join voice channel
        {
            bool isUri = Uri.IsWellFormedUriString(song, UriKind.RelativeOrAbsolute);
            CachedContext = Context;
            if (playing == false)
            {
                playing = true;
                player = lavalinkManager.GetPlayer(Context.Guild.Id) ?? await lavalinkManager.JoinAsync((Context.User as IVoiceState).VoiceChannel);
                LoadTracksResponse response;
                if (isUri == true)
                {
                    response = await lavalinkManager.GetTracksAsync(song);
                }
                else
                {
                    response = await lavalinkManager.GetTracksAsync($"ytsearch:{song}");
                };
                
                LavalinkTrack track = response.Tracks.First();
                await player.PlayAsync(track);
                var songEmbed = new EmbedBuilder()
                    .WithThumbnailUrl(@"https://www.bochumer-weihnacht.de/wp-content/uploads/2016/11/playbutton.png")
                    .WithDescription($"Now playing: \n Song: { track.Title} \n Length: {track.Length} \n Uploader: {track.Author} \n Link: {track.Url}")
                    .WithColor(Color.DarkerGrey)
                    .Build();
                await Context.Channel.SendMessageAsync("", false,songEmbed);
                await Context.Message.DeleteAsync();
                CurrentSong = track;
            }
            else if (playing == true)
            {
                LoadTracksResponse response;
                if (isUri == true)
                {
                    response = await lavalinkManager.GetTracksAsync(song);
                }
                else
                {
                    response = await lavalinkManager.GetTracksAsync($"ytsearch:{song}");
                };
                LavalinkTrack track = response.Tracks.First();
                Queue.Enqueue(track);
                var songEmbed = new EmbedBuilder()
                    .WithDescription($"Queued: \n Song: { track.Title} \n Length: {track.Length} \n Uploader: {track.Author} \n Link: {track.Url}")
                    .WithThumbnailUrl(@"https://www.bochumer-weihnacht.de/wp-content/uploads/2016/11/playbutton.png")
                    .WithColor(Color.DarkerGrey)
                    .Build();
                await Context.Channel.SendMessageAsync("", false, songEmbed);
                await Context.Message.DeleteAsync();
            }
        }

        public async Task SetVolumeAsync(uint volume) //Volume modification (Incomplete)
        {
            await player.SetVolumeAsync(volume);
        }

        public async Task Skip() //Skip song in queue
        {
            if (Queue.TryPeek(out LavalinkTrack NextTrack))
            {
                await player.StopAsync();
            }
            else
            {
                playing = false;
                CurrentSong = null;
                Queue = new ConcurrentQueue<LavalinkTrack>();
                await player.DisconnectAsync();
            }
        }

        public async Task Clear() //Destroy current queue
        {
            if (Queue.TryDequeue(out LavalinkTrack NextTrack))
            {
                Queue = new ConcurrentQueue<LavalinkTrack>();
            }
            await Task.Delay(0);
        }

        public async Task Leave() //Disconnect from voice channel
        {
            playing = false;
            CurrentSong = null;
            Queue = new ConcurrentQueue<LavalinkTrack>();
            await player.DisconnectAsync();
        }

        public async Task Ended(LavalinkPlayer _player, LavalinkTrack track, string id) //Song track handling
        {
            if (Queue.TryDequeue(out LavalinkTrack NextTrack))
            {
                await player.PlayAsync(NextTrack);

                var songEmbed = new EmbedBuilder()
                    .WithThumbnailUrl(@"https://www.bochumer-weihnacht.de/wp-content/uploads/2016/11/playbutton.png")
                    .WithDescription($"Now playing: \n Song: { track.Title} \n Length: {track.Length} \n Uploader: {track.Author} \n Link: {track.Url}")
                    .WithColor(Color.DarkerGrey)
                    .Build();
                await CachedContext.Channel.SendMessageAsync("", false, songEmbed);
                CurrentSong = NextTrack;
            }
            else
            {
                playing = false;
                CurrentSong = null;
                await player.DisconnectAsync();
            }
        }

    }
}
