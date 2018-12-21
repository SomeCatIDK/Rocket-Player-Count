using System.Collections.Generic;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace SomeCatIDK.DiscordPlayerCount.src
{
    public sealed class CommandPCRefresh : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "pcrefresh";

        public string Help => "Forces the server to send a POST request to the bot.";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            DiscordPlayerCount.Instance.SendServerStatus();

            string message = "Player count has been updated!";

            if (caller is UnturnedPlayer player)
                UnturnedChat.Say(player, message);
            else
                Logger.Log(message);
        }
    }
}
