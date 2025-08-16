using Exiled.API.Enums;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;
using UntitledCustomRoles.API.Interfaces;
using UntitledCustomRoles.Example.Abilities;

namespace UntitledCustomRoles.Example.Roles
{
    public class SecondRole : ICustomRole
    {
        public int UId { get; set; }
        public string Name { get; set; } = "Example Role 2";
        public string CustomInfo { get; set; }
        public int SpawnChance { get; set; } = 10;
        public RoleTypeId RoleType { get; set; } = RoleTypeId.NtfCaptain;
        public float Health { get; set; } = 500f;
        public RoleTypeId RoleAppearance { get; set; } = RoleTypeId.ChaosConscript; // None for no appearance
        public Dictionary<EffectType, byte> Effects { get; set; } = new()
        {
            { EffectType.Scp1853, 25 }
        };
        public bool CanEscape { get; set; } = true;
        public RoleTypeId RoleAfterEscape { get; set; } = RoleTypeId.Spectator;
        public Vector3 Size { get; set; } = new Vector3(1f, 1f, 1f);
        public string BroadcastText { get; set; } = "You have become an example 2";
        public ushort BroadcastDuration { get; set; } = 5;
        public List<ItemType> Items { get; set; } = new()
        {
            ItemType.GunE11SR,
            ItemType.KeycardMTFCaptain,
            ItemType.SCP500
        };
        public List<uint> CustomItems { get; set; } = new();
        public Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            { AmmoType.Nato556, 120 }
        };
        public RoomType SpawnRoom { get; set; } = RoomType.Unknown; // Unknown for spawn by default
        public bool CanTrigger096 { get; set; } = false;
        public bool CanTrigger173 { get; set; } = false;
        public bool IgnoreGameSpawn { get; set; } = false;
        public float DamageMiltiplier { get; set; } = 5f;

        public ICustomRoleAbility? Ability { get; set; } = new SecondAbility();
        // Or you can use same Ability
        // public ICustomRoleAbility? Ability { get; set; } = new FirstAbility();
    }
}
