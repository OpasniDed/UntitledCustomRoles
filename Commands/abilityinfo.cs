using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using UntitledCustomRoles.API;
using UntitledCustomRoles.API.Interfaces;

namespace UntitledCustomRoles.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class abilityinfo : ICommand
    {
        public string Command { get; } = "abilityinfo";
        public string[] Aliases { get; } = { "ainfo", "infoability", "ai", "ainf" };
        public string Description { get; } = "Check ability info (UCR)";
        public bool SanitizeResponse => false;

        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender)
            {
                response = "You can use it only in game.";
                return false;
            }

            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Player does not exist.";
                return false;
            }

            if (!CustomRole.TryGetPlayerRole(player, out ICustomRole role))
            {
                response = "You need play as Custom Role.";
                return false;
            }

            ICustomRoleAbility ability = role.Ability;
            if (ability == null)
            {
                response = "Your custom role dont have ability.";
                return false;
            }

            response = @$"Ability info
                       Name: {ability.Name}
                       Description: {ability.Description}
                       Cooldown: {ability.Cooldown}
                       Max Uses: {ability.MaxUsesPerRound}";
            return true;
        }
    }
}
