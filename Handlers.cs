using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Scp173;
using MEC;
using PlayerRoles;
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
            Exiled.Events.Handlers.Player.Spawned += Spawned;
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
            Exiled.Events.Handlers.Player.Spawned -= Spawned;
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

        private void Spawned(SpawnedEventArgs ev)
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
            CustomRole.spawning.Clear();
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
                .Where(r =>
                    !r.IgnoreGameSpawn &&
                    (
                        (!r.IgnoreRole && r.RoleType == player.Role.Type) ||
                        (r.IgnoreRole && player.Role.Type is not RoleTypeId.Spectator and not RoleTypeId.Overwatch)
                    )
                )
                .ToList();

            if (!roleList.Any())
                return;

            foreach (var role in roleList)
            {
                int chance = Math.Max(role.SpawnChance, 1);
                if (UnityEngine.Random.Range(0, 100) < chance)
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
