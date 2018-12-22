using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Logging;
using Rocket.Core.Player.Events;
using Rocket.Core.Plugins;

namespace SomeCatIDK.DiscordPlayerCount
{
    public sealed class DiscordPlayerCount : Plugin<DiscordPlayerCountConfig>, IEventListener<PlayerConnectedEvent>, IEventListener<PlayerDisconnectedEvent>
    {
        private readonly IPlayerManager _playerManager;
        private readonly WebClient _webClient;

        private string Address => $"http://{ConfigurationInstance.BotAddress}:{ConfigurationInstance.BotPort}/";

        public DiscordPlayerCount(IDependencyContainer container) : base(container)
        {
            _playerManager = container.Resolve<IPlayerManager>();

            _webClient = new WebClient();
        }

        ~DiscordPlayerCount()
        {
            _webClient.Dispose();
        }

        void IEventListener<PlayerConnectedEvent>.HandleEvent(IEventEmitter emitter, PlayerConnectedEvent @event)
            => SendServerStatus();

        void IEventListener<PlayerDisconnectedEvent>.HandleEvent(IEventEmitter emitter, PlayerDisconnectedEvent @event)
            => SendServerStatus();

        internal bool SendServerStatus()
        {
            var currentPlayers = (uint)_playerManager.Players.Count(x => x.IsOnline);
            var maxPlayers = ConfigurationInstance.MaxPlayers;
            
            var reqBody = new PostObject(currentPlayers, maxPlayers).Serialize();

            _webClient.Headers["Content-type"] = "application/json";

            try
            {
                _webClient.UploadString(Address, "POST", reqBody);
                return true;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ConnectFailure)
                {
                    Logger.LogError(
                        $"Failed to connect to the bot @ {Address}! Please make sure the connection info in the config is correct.");
                }
                else
                {
                    if (!(e.Response is HttpWebResponse response))
                    {
                        Logger.LogError(
                            $"An unknown error was encountered!", e);
                        return false;
                    }

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            Logger.LogError(
                                $"An invalid request was sent! If there as an update for the bot, you must also update the plugin.");
                            break;
                        case HttpStatusCode.InternalServerError:
                            Logger.LogError(
                                $"The bot encountered an internal error while completing the request.");
                            break;
                        default:
                            Logger.LogError(
                                $"An unknown error was encountered!", e);
                            break;
                    }
                }
            }

            return false;
        }
    }

    internal sealed class PostObject
    {
        [JsonProperty("currentPlayers")]
        private readonly uint _currentPlayers;

        [JsonProperty("maxPlayers")]
        private readonly uint _maxPlayers;

        public PostObject(uint currentPlayers, uint maxPlayers)
        {
            _currentPlayers = currentPlayers;
            _maxPlayers = maxPlayers;
        }

        public string Serialize() => JsonConvert.SerializeObject(this);
    }
}
