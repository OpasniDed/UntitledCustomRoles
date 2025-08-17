using Exiled.API.Enums;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UntitledCustomRoles.API.Interfaces;
using UntitledCustomRoles.Example.Abilities;

namespace UntitledCustomRoles.Example.Roles
{
    public class FirstRole : ICustomRole
    {
        public int UId { get; set; }
        public string Name { get; set; } = "Example Role";
        public string CustomInfo { get; set; }
        public int SpawnChance { get; set; } = 20;
        public RoleTypeId RoleType { get; set; } = RoleTypeId.ClassD;
        public float Health { get; set; } = 125f;
        public RoleTypeId RoleAppearance { get; set; } = RoleTypeId.None; // None for no appearance
        public Dictionary<EffectType, byte> Effects { get; set; } = new();
        public bool CanEscape { get; set; } = true;
        public RoleTypeId RoleAfterEscape { get; set; } = RoleTypeId.Spectator;
        public Vector3 Size { get; set; } = new Vector3(1.5f, 1.2f, 1f);
        public string BroadcastText { get; set; } = "You have become an example";
        public ushort BroadcastDuration { get; set; } = 5;
        public List<ItemType> Items { get; set; } = new()
        {
            ItemType.Coin
        };
        public List<uint> CustomItems { get; set; } = new();
        public Dictionary<AmmoType, ushort> Ammo { get; set; } = new();
        public RoomType SpawnRoom { get; set; } = RoomType.Unknown; // Unknown for spawn by default
        public bool CanTrigger096 { get; set; } = true;
        public bool CanTrigger173 { get; set; } = true;
        public bool IgnoreGameSpawn { get; set; } = false;
        public float DamageMiltiplier { get; set; } = 1f;

        public ICustomRoleAbility? Ability { get; set; } = new FirstAbility();
    }
}
