namespace IslaBot.Discord.Entities
{
    class UserProfile
    {
        public long DiscId { get; set; }
        public long RobloxId { get; set; }
        public int Exp { get; set; }
        public int Rank { get; set; }
        public bool Exists { get; set; }
    }

}
