namespace SomeCatIDK.DiscordPlayerCount
{
    public sealed class DiscordPlayerCountConfig
    {
        public string BotAddress { get; set; } = "localhost";
        public ushort BotPort { get; set; } = 5000;
        public uint MaxPlayers { get; set; } = 24;
    }
}
