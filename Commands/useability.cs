using CommandSystem;
using Exiled.API.Features;
using Exiled.CreditTags.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string Description { get; } = "Команда для использования абилити (UCR)";
        public bool SanitizeResponse => false;

        internal static readonly Dictionary<Player, DateTime> cooldowns = new();
        internal static readonly Dictionary<Player, int> remainingUses = new();

        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (sender is ServerConsole)
            {
                response = "Команду можно использовать только в игре";
                return false;
            }

            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Игрока не существует";
                return false;
            }

            if (!CustomRole.TryGetPlayerRole(player, out ICustomRole role))
            {
                response = "Вы должны играть за кастомную роль";
                return false;
            }

            ICustomRoleAbility ability = role.Ability;
            if (ability == null)
            {
                response = "У вашей роли нет способностей";
                return false;
            }

            if (cooldowns.TryGetValue(player, out DateTime last))
            {
                var timeSince = DateTime.UtcNow - last;
                if (timeSince.TotalSeconds < ability.Cooldown)
                {
                    response = $"Способность на кулдауне. Осталось: {(int)ability.Cooldown - (int)timeSince.TotalSeconds} секунд";
                    return false;
                }
            }

            if (!remainingUses.ContainsKey(player))
                remainingUses[player] = ability.MaxUsesPerRound;

            if (remainingUses[player] <= 0)
            {
                response = "Вы достигли лимита способности";
                return false;
            }

            if (ability.MaxUsesPerRound <= 0)
            {
                response = "Способность больше нельзя использовать, лимит";
                return false;
            }
            Player? target = null;

            if (ability.RequiredTarget)
            {
                Ray ray = new Ray(player.CameraTransform.position, player.CameraTransform.forward);
                int layerMask = ~LayerMask.GetMask("Player", "Hitbox");

                if (!Physics.Raycast(ray, out RaycastHit hit, Plugin.Instance.Config.raycastRange, layerMask))
                {
                    response = "Таргета не найдено";
                    return false;
                }

                if (target == player)
                {
                    response = "Вы не можете использовать абилити на самом себе";
                    return false;
                }

                target = Player.Get(hit.collider.transform.root.gameObject);

                if (target == null)
                {
                    response = "Таргета не найденео";
                    return false;
                }

                try
                {
                    ability.Ability(player, target);
                }
                catch (Exception ex)
                {
                    response = $"Ошибка использования абилити: {ex}";
                    return false;
                }

                response = $"Способность с игроком {target.Nickname} применена";
                return true;
            }

            try
            {
                ability.Ability(player);
            }
            catch (Exception ex)
            {
                response = $"Ошибка использования способности: {ex}";
                return false;
            }

            cooldowns[player] = DateTime.UtcNow;
            remainingUses[player]--;

            response = $"Способность {ability.Name} использована";
            return true;
        }
    }
}
