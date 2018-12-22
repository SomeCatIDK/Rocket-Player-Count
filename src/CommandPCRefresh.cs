using System;
using System.Drawing;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.API.User;
using Rocket.Core.User;

namespace SomeCatIDK.DiscordPlayerCount
{
    public sealed class CommandPCRefresh : ICommand
    {
        private DiscordPlayerCount _plugin;

        public bool SupportsUser(UserType user) => true;

        public void Execute(ICommandContext context)
        {
            if (_plugin == null)
            {
                _plugin = context.Container.Resolve<IPluginLoader>().Plugins.First(x => x is DiscordPlayerCount) as DiscordPlayerCount;

                if (_plugin == null)
                    throw new InvalidOperationException("This plugin was already unloaded or it has been overriden!");
            }
            
            if (_plugin.SendServerStatus())
                context.User.SendMessage("Successfully refreshed player count!", Color.Green);
            else
                context.User.SendMessage("An exception occurred while refreshing the player count, please check the console!", Color.Red);
        }

        public string Name => "pcrefresh";
        public string[] Aliases => new string[0];
        public string Summary => "Refreshes the player count bot.";
        public string Description => Summary;
        public string Syntax => string.Empty;
        public IChildCommand[] ChildCommands => new IChildCommand[0];
    }
}
