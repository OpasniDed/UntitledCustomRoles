using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using UnityEngine;
using UntitledCustomRoles.API;
using UntitledCustomRoles.API.Interfaces;

namespace UntitledCustomRoles.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class useability : ICommand
    {
        public string Command { get; } = "abilityuse";
        public string[] Aliases { get; } = { "uability", "uab", "uaseab", "ub" };
        public string Description { get; } = "Command for ability use (UCR)";
        public bool SanitizeResponse => false;

        internal static readonly Dictionary<Player, DateTime> cooldowns = new();
        internal static readonly Dictionary<Player, int> remainingUses = new();

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

            if (cooldowns.TryGetValue(player, out DateTime last))
            {
                var timeSince = DateTime.UtcNow - last;
                if (timeSince.TotalSeconds < ability.Cooldown)
                {
                    response = $"Ability cooldown. Remaining: {(int)ability.Cooldown - (int)timeSince.TotalSeconds} seconds.";
                    return false;
                }
            }

            if (!remainingUses.ContainsKey(player))
                remainingUses[player] = ability.MaxUsesPerRound;

            if (remainingUses[player] <= 0)
            {
                response = "Ability limit.";
                return false;
            }

            Player? target = null;

            if (ability.RequiredTarget)
            {
                Ray ray = new Ray(player.CameraTransform.position, player.CameraTransform.forward);
                int layerMask = ~LayerMask.GetMask("Player", "Hitbox");

                if (!Physics.Raycast(ray, out RaycastHit hit, Plugin.Instance.Config.raycastRange, layerMask))
                {
                    response = "Target does not exist.";
                    return false;
                }

                if (target == player)
                {
                    response = "You cant use it to yourself.";
                    return false;
                }

                target = Player.Get(hit.collider.transform.root.gameObject);

                if (target == null)
                {
                    response = "Target does not exist.";
                    return false;
                }

                try
                {
                    ability.Ability(player, target);
                }
                catch (Exception ex)
                {
                    response = $"Error ability using: {ex}";
                    return false;
                }

                response = $"Ability with {target.Nickname} was used.";
                return true;
            }

            try
            {
                ability.Ability(player);
            }
            catch (Exception ex)
            {
                response = $"Error ability using: {ex}";
                return false;
            }

            cooldowns[player] = DateTime.UtcNow;
            remainingUses[player]--;

            response = $"Ability {ability.Name} was used";
            return true;
        }
    }
}
