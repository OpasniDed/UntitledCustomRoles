using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Scp173;
using MEC;
using System;
using System.Linq;
using UntitledCustomRoles.API;
using UntitledCustomRoles.API.Interfaces;
using UntitledCustomRoles.Commands;

namespace UntitledCustomRoles
{
    public class Handlers
    {
        private bool isApplyingRole = false;

        // ===== Registration =====
        internal void Register()
        {
            Exiled.Events.Handlers.Player.Escaping += Escaping;
            Exiled.Events.Handlers.Player.ChangingRole += ChangingRole;
            Exiled.Events.Handlers.Player.Died += Died;
            Exiled.Events.Handlers.Player.Left += Left;
            Exiled.Events.Handlers.Player.Destroying += Destroy;
            Exiled.Events.Handlers.Player.Hurting += Hurting;

            Exiled.Events.Handlers.Server.RestartingRound += RoundRestarting;
            Exiled.Events.Handlers.Server.RoundStarted += RoundStarted;

            Exiled.Events.Handlers.Scp096.AddingTarget += AddingTarget;
            Exiled.Events.Handlers.Scp173.AddingObserver += AddingObserver;
        }

        internal void Unregister()
        {
            Exiled.Events.Handlers.Player.Escaping -= Escaping;
            Exiled.Events.Handlers.Player.ChangingRole -= ChangingRole;
            Exiled.Events.Handlers.Player.Died -= Died;
            Exiled.Events.Handlers.Player.Left -= Left;
            Exiled.Events.Handlers.Player.Destroying -= Destroy;
            Exiled.Events.Handlers.Player.Hurting -= Hurting;

            Exiled.Events.Handlers.Server.RestartingRound -= RoundRestarting;
            Exiled.Events.Handlers.Server.RoundStarted -= RoundStarted;

            Exiled.Events.Handlers.Scp096.AddingTarget -= AddingTarget;
            Exiled.Events.Handlers.Scp173.AddingObserver -= AddingObserver;
        }


        // ===== Player Handlers =====
        private void Hurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null) return;
            if (CustomRole.TryGetPlayerRole(ev.Attacker, out ICustomRole role))
            {
                ev.Amount *= role.DamageMiltiplier;
            }
        }

        private void Escaping(EscapingEventArgs ev)
        {
            if (!CustomRole.TryGetPlayerRole(ev.Player, out ICustomRole role)) return;

            ev.IsAllowed = false;
            if (role.CanEscape)
            {
                CustomRole.TryRemovePlayerRole(ev.Player);
                ev.Player.Role.Set(role.RoleAfterEscape);
                useability.remainingUses.Remove(ev.Player);
                useability.cooldowns.Remove(ev.Player);
            }
        }

        private void ChangingRole(ChangingRoleEventArgs ev)
        {
            if (isApplyingRole) return;

            CustomRole.TryRemovePlayerRole(ev.Player);
            useability.remainingUses.Remove(ev.Player);
            useability.cooldowns.Remove(ev.Player);
            Timing.CallDelayed(0.1f, () => ApplyRandomRole(ev.Player));
        }

        private void Died(DiedEventArgs ev)
        {
            CustomRole.TryRemovePlayerRole(ev.Player);
            useability.remainingUses.Remove(ev.Player);
            useability.cooldowns.Remove(ev.Player);
        }
        private void Left(LeftEventArgs ev)
        {
            CustomRole.TryRemovePlayerRole(ev.Player);
            useability.remainingUses.Remove(ev.Player);
            useability.cooldowns.Remove(ev.Player);
        }
        private void Destroy(DestroyingEventArgs ev)
        {
            CustomRole.TryRemovePlayerRole(ev.Player);
            useability.remainingUses.Remove(ev.Player);
            useability.cooldowns.Remove(ev.Player);
        }


        // ===== SCP Handlers =====
        private void AddingTarget(AddingTargetEventArgs ev)
        {
            if (CustomRole.TryGetPlayerRole(ev.Player, out ICustomRole role))
                ev.IsAllowed = role.CanTrigger096;
        }

        private void AddingObserver(AddingObserverEventArgs ev)
        {
            if (CustomRole.TryGetPlayerRole(ev.Player, out ICustomRole role))
                ev.IsAllowed = role.CanTrigger173;
        }

        // ===== Server Handlers =====
        private void RoundRestarting()
        {
            CustomRole.TryClearPlayerRoles();
            useability.cooldowns.Clear();
            useability.remainingUses.Clear();
        }
        private void RoundStarted()
        {
            CustomRole.TryClearPlayerRoles();
            useability.cooldowns.Clear();
            useability.remainingUses.Clear();
        }


        // ===== Other Methods =====
        private void ApplyRandomRole(Player player)
        {
            var roleList = CustomRole.RolesList
                .Where(r => r.RoleType == player.Role.Type && !r.IgnoreGameSpawn)
                .ToList();

            if (!roleList.Any()) return;

            if (roleList.Count == 1)
            {
                var singleRole = roleList[0];
                int chance = Math.Max(singleRole.SpawnChance, 1);
                if (UnityEngine.Random.Range(0, 100) < chance)
                {
                    isApplyingRole = true;
                    CustomRole.GiveRole(player, singleRole);
                    isApplyingRole = false;
                }
                return;
            }

            int totalChance = roleList.Sum(r => r.SpawnChance);
            if (totalChance <= 0) return;

            int roll = UnityEngine.Random.Range(0, totalChance);
            int current = 0;

            foreach (var role in roleList)
            {
                current += role.SpawnChance;
                if (roll < current)
                {
                    isApplyingRole = true;
                    CustomRole.GiveRole(player, role);
                    isApplyingRole = false;
                    return;
                }
            }
        }
    }
}
