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
using UnityEngine;
using UntitledCustomRoles.API.Interfaces;
using UntitledCustomRoles.Events;

namespace UntitledCustomRoles.API
{
    public class CustomRole
    {

        private static readonly Dictionary<int, ICustomRole> CustomRoles = new();
        internal static readonly Dictionary<Player, ICustomRole> PlayerRoles = new();
        internal static readonly List<Player> spawning = new();
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



            Timing.CallDelayed(0.1f, () =>
            {
                ApplyRole(player, role);
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
            if (spawning.Contains(player)) return;
            spawning.Add(player);
            Timing.CallDelayed(0f, () =>
            {
                if (player.Role.Type != role.RoleType)
                    player.Role.Set(role.RoleType);
                player.CustomInfo = role.CustomInfo;
                player.Health = role.Health;
                player.MaxHealth = role.Health;
                player.ClearInventory();
                player.ClearAmmo();
                player.Scale = role.Size;
                PlayerRoles[player] = role;

                if (role.RoleAppearance != RoleTypeId.None)
                    player.ChangeAppearance(role.RoleAppearance);

                foreach (var effect in role.Effects)
                    player.EnableEffect(effect.Key, effect.Value);

                foreach (ItemType item in role.Items)
                    player.AddItem(item);

                foreach (uint item in role.CustomItems)
                    CustomItem.TryGive(player, item);

                foreach (var ammo in role.Ammo)
                    player.AddAmmo(ammo.Key, ammo.Value);

                if (role.SpawnRoom != RoomType.Unknown)
                    player.Position = GetValidPosition(Room.Get(role.SpawnRoom));

                player.Broadcast(role.BroadcastDuration, role.BroadcastText);
                CustomRoleEvents.OnCustomRoleSpawned(new Events.Args.CustomRoleSpawnedEventArgs(player, role));
                Timing.CallDelayed(0.1f, () =>
                {
                    if (spawning.Contains(player))
                        spawning.Remove(player);
                });
            });
                 
        }


        private static Vector3 GetValidPosition(Room room)
        {
            Vector3 offset = Vector3.zero;
            switch (room.Type)
            {
                case RoomType.HczStraightPipeRoom:
                    offset = new Vector3(2.96f, 1f, -6.19f);
                    break;
                case RoomType.Hcz127:
                    offset = new Vector3(2.37f, 1f, 0.82f);
                    break;
                case RoomType.LczCheckpointA:
                case RoomType.LczCheckpointB:
                case RoomType.HczTesla:
                    offset = new Vector3(5.34f, 1f, 0.12f);
                    break;
                case RoomType.HczTestRoom:
                    offset = new Vector3(6.53f, 1f, 5.48f);
                    break;
                case RoomType.HczCrossRoomWater:
                    offset = new Vector3(-5f, 1f, 0f);
                    break;
                case RoomType.HczNuke:
                    offset = new Vector3(-3.14f, 1f, -0.12f);
                    break;
                case RoomType.HczArmory:
                    offset = new Vector3(-2.58f, 1f, 0f);
                    break;
                case RoomType.Hcz939:
                    offset = new Vector3(2.04f, 1f, -0.45f);
                    break;
                case RoomType.Lcz330:
                    offset = new Vector3(-4.50f, 1f, 0f);
                    break;
                case RoomType.Lcz173:
                    offset = new Vector3(-4.28f, 1f, 0f);
                    break;
                case RoomType.EzCollapsedTunnel:
                case RoomType.EzShelter:
                    offset = new Vector3(0f, 1f, 4.26f);
                    break;

            }
            return room.WorldPosition(offset) + Vector3.up;
        }
    }
}
