using Newtonsoft.Json;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Net;

namespace SomeCatIDK.DiscordPlayerCount
{
    public sealed class DiscordPlayerCount : RocketPlugin<DiscordPlayerCountConfig>
    {
        private WebClient _webClient;
        private DiscordPlayerCountConfig _config => Configuration.Instance;
        private string address => $"http://{_config.BotAddress}:{_config.BotPort}/";

        public static DiscordPlayerCount Instance { get; private set; }

        protected override void Load()
        {
            Instance = this;

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;

            _webClient = new WebClient();

            SendServerStatus(false);
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;

            SendServerStatus(false, 0);

            _webClient.Dispose();

            Instance = null;
        }

        private void OnPlayerConnected(UnturnedPlayer player) => SendServerStatus();
        private void OnPlayerDisconnected(UnturnedPlayer player) => SendServerStatus(true);

        internal void SendServerStatus(bool isDisconnect = false, byte? currentPlayers = null, byte? maxPlayers = null)
        {
            currentPlayers = currentPlayers == null ? (byte)Provider.clients.Count : currentPlayers;
            maxPlayers = maxPlayers == null ? Provider.maxPlayers : maxPlayers;

            if (isDisconnect)
                currentPlayers--;

            string reqBody = new POSTObject(currentPlayers.Value, maxPlayers.Value).Serialize();
            
            _webClient.Headers["Content-type"] = "application/json";

            try
            {
                _webClient.UploadString(address, "POST", reqBody);
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ConnectFailure)
                {
                    Logger.LogError($"Failed to connect to the bot @ {address}! Please make sure the connection info in the config is correct.");
                }
                else
                {
                    Logger.LogError($"Failed to post to the bot @ {address}! There may have been an update to either the plugin or the Discord bot.");
                }
            }
        }
    }

    public sealed class POSTObject
    {
        [JsonProperty("currentPlayers")]
        private readonly byte _currentPlayers;

        [JsonProperty("maxPlayers")]
        private readonly byte _maxPlayers;

        public POSTObject(byte currentPlayers, byte maxPlayers)
        {
            _currentPlayers = currentPlayers;
            _maxPlayers = maxPlayers;
        }

        public string Serialize() => JsonConvert.SerializeObject(this);
    }
}
