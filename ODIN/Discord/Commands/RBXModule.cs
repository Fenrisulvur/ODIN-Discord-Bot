using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using HtmlAgilityPack;
using IslaBot.Storage.Implementations;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using IslaBot.Discord.Entities;

namespace IslaBot
{

    public class Roblox : ModuleBase<SocketCommandContext>
    {
        public dynamic _Storage = Unity.Resolve<InMemoryStorage>();

        private async Task<UserProfile> GetProfile(string RobloxId)
        {
            UserProfile _profile = new UserProfile();
            string query = $"SELECT TOP 1 * FROM Profiles WHERE RobloxId = {RobloxId}";

            using (SqlConnection connection = new SqlConnection(MainWindow.connectionString)) {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int DiscordIdOrdinal = reader.GetOrdinal("DiscId");
                    int RobloxIdOrdinal = reader.GetOrdinal("RobloxId");
                    int ExpOrdinal = reader.GetOrdinal("Exp");
                    int RankOrdinal = reader.GetOrdinal("Rank");

                    while (reader.Read())
                    {
                        _profile.DiscId = reader.GetInt64(DiscordIdOrdinal);
                        _profile.RobloxId = reader.GetInt64(RobloxIdOrdinal);
                        _profile.Exp = reader.GetInt32(ExpOrdinal);
                        _profile.Rank = reader.GetInt32(RankOrdinal);
                        _profile.Exists = true;
                    }

                }
            }

            return _profile;
        }

        private async Task<UserProfile> GetProfileFromDiscId(string DiscID)
        {
            UserProfile _profile = new UserProfile();
            string query = $"SELECT TOP 1 * FROM Profiles WHERE DiscId = {DiscID}";

            using (SqlConnection connection = new SqlConnection(MainWindow.connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int DiscordIdOrdinal = reader.GetOrdinal("DiscId");
                    int RobloxIdOrdinal = reader.GetOrdinal("RobloxId");
                    int ExpOrdinal = reader.GetOrdinal("Exp");
                    int RankOrdinal = reader.GetOrdinal("Rank");

                    while (reader.Read())
                    {
                        _profile.DiscId = reader.GetInt64(DiscordIdOrdinal);
                        _profile.RobloxId = reader.GetInt64(RobloxIdOrdinal);
                        _profile.Exp = reader.GetInt32(ExpOrdinal);
                        _profile.Rank = reader.GetInt32(RankOrdinal);
                        _profile.Exists = true;
                    }

                }
            }

            return _profile;
        }

        private async Task CreateProfile(string DiscordId, string RobloxId)
        {
            string query = $"INSERT INTO Profiles VALUES ({DiscordId}, {RobloxId}, 0, 0)";

            using (SqlConnection connection = new SqlConnection(MainWindow.connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
           
        }




        [Command("OTest")]
        [Summary(" ")]
        public async Task OTest(string RobloxId)
        {
            UserProfile _profile = await GetProfile(RobloxId);

            
            if (_profile.Exists)
            {
                await ReplyAsync("Record Exists.");
                if (""+Context.Message.Author.Id == ""+_profile.DiscId)
                {
                    await ReplyAsync($"Record Found: USERID - {_profile.RobloxId} DISCORDID - {_profile.DiscId} EXP - {_profile.Exp} RANK - {_profile.Rank}");
                    //await HandleRoles(id);
                    
                }else{
                    await ReplyAsync("Roblox ID is already claimed.");
                }

            }
            else
            {
                await ReplyAsync("Record does not exist.");
            };
        }

        [Command("OCreate")]
        [Summary(" ")]
        public async Task OCreate(string DiscId, string RobloxId)
        {
            UserProfile _profile = await GetProfile(RobloxId);

            if (_profile.Exists)
            {
                await ReplyAsync($"COMMANDED OVERRIDE - REASON: Record Found: USERID - {_profile.RobloxId} DISCORDID - {_profile.DiscId} EXP - {_profile.Exp} RANK - {_profile.Rank}");
            }
            else
            {
                await ReplyAsync("Creating record.");
                await CreateProfile(DiscId, RobloxId);
            };
            
        }

        [Command("Verify")]
        [Summary("Verifies and roles you based on roblox id")]
        [Alias("getrole", "getroles", "v", "update")]
        public async Task VerifyAsync(string id)
        {
            
            if (id == "userid") { await ReplyAsync("userid is an invalid parameter, use your roblox ID get it from your profile here => http://www.roblox.com/user.aspx "); return; }
            string check = await CheckVerification(id);
            if (check == "Verified") { await HandleRoles(id, Context); return; }
            if (check == "Claimed") { return; }
            var html = @"https://www.roblox.com/users/" + id + "/profile";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var node = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='profile-about-content-text linkify']");

            if (node.InnerText.Contains("Isla Verify"))
            {
                await ReplyAsync("Verified - Checking groups");
                await HandleRoles(id, Context);
            }
            else
            {
                await ReplyAsync("Add: Isla Verify to your Profile Description (in profile settings).");
            };
        }

        [Command("getnick")]
        [Summary("Gets data without roblox ID if previously verified.")]
        [Alias("gn", "get")]
        public async Task GetNickAsync()
        {
            UserProfile _profile = await GetProfileFromDiscId("" + Context.Message.Author.Id);

            if (_profile.Exists)
            {
                await ReplyAsync("You are in the database.");
                await HandleRoles("" + _profile.RobloxId, Context);
            }
            else
            {
                await ReplyAsync("You are not in the database.");
            }
        }

        [Command("assign")]
        [Summary("Sets IDs.")]
        [RequireRole("Officio Prefectus")]
        public async Task SetDataAsync(string id, string discordid)
        {
            string check = await CheckVerification(id);
            if (check == "Verified")
            {
                await ReplyAsync("ID Already set.");
            }
            else
            {
                _Storage.StoreObject(id, discordid);
                await ReplyAsync("IDs paired.");
            }
        }

        public async Task<string> CheckVerificationOld(string uid)
        {
            var _DUSER = "" + Context.User.Id;
            var _RUSER = "" + uid;
            var _RAMDATA = _Storage.GrabObject(_RUSER);
            if (_RAMDATA != null && _RAMDATA == _DUSER)
            {
                await ReplyAsync("UserId Previously Verified, adding roles.");
                return "Verified";
            }
            if (_RAMDATA != null && _RAMDATA != _DUSER)
            {
                await ReplyAsync("UserId claimed by => " + _RAMDATA + " Cannot claim. ");
                return "Claimed";
            }
            return "";

        }

        public async Task<string> CheckVerification(string uid)
        {
            ulong _DUSER = Context.User.Id;
            ulong _RUSER = (ulong)Convert.ToInt32(uid);

            UserProfile _profile = new UserProfile();
            string query = $"SELECT TOP 1 * FROM Profiles WHERE RobloxId = {_RUSER}";

            using (SqlConnection connection = new SqlConnection(MainWindow.connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int DiscordIdOrdinal = reader.GetOrdinal("DiscId");
                    int RobloxIdOrdinal = reader.GetOrdinal("RobloxId");
                    int ExpOrdinal = reader.GetOrdinal("Exp");
                    int RankOrdinal = reader.GetOrdinal("Rank");

                    while (reader.Read())
                    {
                        _profile.DiscId = reader.GetInt64(DiscordIdOrdinal);
                        _profile.RobloxId = reader.GetInt64(RobloxIdOrdinal);
                        _profile.Exp = reader.GetInt32(ExpOrdinal);
                        _profile.Rank = reader.GetInt32(RankOrdinal);
                        _profile.Exists = true;
                    }

                }
            }

            if (_profile.Exists && _profile.DiscId == (long)_DUSER)
            {
                await ReplyAsync("UserId Previously Verified, adding roles.");
                return "Verified";
            }
            if (_profile.Exists && _profile.DiscId != (long)_DUSER)
            {
                await ReplyAsync("UserId claimed by => " + _profile.DiscId + " Cannot claim. ");
                return "Claimed";
            }
            return "";

        }


        async Task HandleRoles(string uid, SocketCommandContext Context)
        {
            bool InGroup = false;
            var user = Context.Guild.Users.FirstOrDefault(x => x.Id == Context.User.Id);
            var w = new WebClient();
            var url = "https://api.roblox.com/users/" + uid + "/groups";
            string json_data = w.DownloadString(url);
            JArray Groups = JArray.Parse(json_data);
            string rolesgiven = "";
            string rolename = "[U/A]";
            int glvl = 0;
            await ReplyAsync("Test1");
            foreach (JObject item in Groups)
            {
                var _name = item.GetValue("Name");
                var _id = item.GetValue("Id");
                var _rank = item.GetValue("Rank");
                var _role = item.GetValue("Role");
                if (_id.ToString() == "3063276")
                {
                    InGroup = true;
                    if (glvl < 3) { string input = "" + _role; String SID = input.Remove(input.IndexOf(']')).Substring(input.IndexOf('[') + 1); rolename = "[" + SID + "]"; glvl = 3; ; }
                    SocketRole role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToString() == "Guardsman");
                    //await user.AddRoleAsync(role);
                    rolesgiven += " Guardsman";
                }
            }
            if (InGroup == true)
            {
                if ("" + Context.User.Id != "210266331541536768")
                {
                    _Storage.StoreObject(uid, "" + Context.User.Id);
                    var url2 = "https://api.roblox.com/users/" + uid;
                    string udata = w.DownloadString(url2);
                    var _uinfo = JObject.Parse(udata);
                    await user.ModifyAsync(x => x.Nickname = "" + rolename + _uinfo["Username"]);
                }
                await ReplyAsync("<" + rolesgiven + " > Roles were given.");
            }
            else
            {
                await ReplyAsync("You are not in IoM or and Allied group, to process join this: https://www.roblox.com/groups/group.aspx?gid=3004523");
            }
        }

    }

}
