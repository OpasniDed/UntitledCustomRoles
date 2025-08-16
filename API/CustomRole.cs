using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UntitledCustomRoles.API.Interfaces;
using UntitledCustomRoles.Events;

namespace UntitledCustomRoles.API
{
    public class CustomRole
    {

        private static readonly Dictionary<int, ICustomRole> CustomRoles = new();
        internal static readonly Dictionary<Player, ICustomRole> PlayerRoles = new();
        public static IReadOnlyList<ICustomRole> RolesList => CustomRoles.Values.ToList();


        public static bool TryGet(int id, out ICustomRole role) => CustomRoles.TryGetValue(id, out role);

        public static ICustomRole Get(int id) => TryGet(id, out var role) ? role : null;

        public static bool TryGetPlayerRole(Player player, out ICustomRole role) => PlayerRoles.TryGetValue(player, out role);

        internal static void TryClearPlayerRoles() => PlayerRoles.Clear();

        public static bool TryRemovePlayerRole(Player player)
        {
            return PlayerRoles.Remove(player);
        }

        public static bool GiveRole(Player player, ICustomRole role)
        {
            if (player == null || role == null) return false;


            player.Role.Set(role.RoleType);

            Timing.CallDelayed(0.1f, () =>
            {
                ApplyRole(player, role);
                PlayerRoles[player] = role;
            });

            return true;
        }


        public static void RegisterAll()
        {
            var roleTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        return e.Types.Where(t => t != null);
                    }
                })
                .Where(t => typeof(ICustomRole).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

         
            foreach (var type in roleTypes)
            {
                try
                {
                    if (Activator.CreateInstance(type) is ICustomRole roleInstance)
                    {
                        Register(roleInstance);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error create example {type.FullName}: {ex}");
                }
            }
        }


        public static bool Register(ICustomRole role)
        {
            if (role == null)
            {
                Log.Error("Trying register null role.");
                return false;
            }

            if (CustomRoles.ContainsKey(role.UId))
            {
                Log.Error($"Role with ID {role.UId} already exist.");
                return false;
            }

            CustomRoles[role.UId] = role;
            return true;
        }

        

        public static void Unregister()
        {
            CustomRoles.Clear();
            PlayerRoles.Clear();
        }

        internal static void ApplyRole(Player player, ICustomRole role)
        {
            player.CustomInfo = role.CustomInfo;
            player.Health = role.Health;
            player.ClearInventory();
            player.ClearAmmo();
            player.Scale = role.Size;
            

            if (role.RoleAppearance != RoleTypeId.None)
                player.ChangeAppearance(role.RoleAppearance);

            if (role.Effects.Count > 0)
            {
                foreach (var effect in role.Effects)
                    player.EnableEffect(effect.Key, effect.Value);
            }

            if (role.Items.Count > 0)
            {
                foreach (ItemType item in role.Items)
                    player.AddItem(item);
            }

            if (role.CustomItems.Count > 0)
            {
                foreach (uint item in role.CustomItems)
                    CustomItem.TryGive(player, item);
            }

            if (role.Ammo.Count > 0)
            {
                foreach (var ammo in role.Ammo)
                    player.AddAmmo(ammo.Key, ammo.Value);
            }

            if (role.SpawnRoom != RoomType.Unknown)
                player.Position = Room.Get(role.SpawnRoom).Position;

            player.Broadcast(role.BroadcastDuration, role.BroadcastText);
            CustomRoleEvents.OnCustomRoleSpawned(new Events.Args.CustomRoleSpawnedEventArgs(player, role));
        }
    }
}
