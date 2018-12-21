using Rocket.API;

namespace SomeCatIDK.DiscordPlayerCount
{
    public sealed class DiscordPlayerCountConfig : IRocketPluginConfiguration
    {
        public string BotAddress { get; private set; }
        public ushort BotPort { get; private set; }

        public void LoadDefaults()
        {
            BotAddress = "localhost";
            BotPort = 5000;
        }
    }
}
