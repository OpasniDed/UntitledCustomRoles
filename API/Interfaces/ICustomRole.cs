using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

namespace UntitledCustomRoles.API.Interfaces
{
#nullable enable
    public interface ICustomRole
    {
        int UId { get; set; }
        string Name { get; set; }
        string CustomInfo { get; set; }
        int SpawnChance { get; set; }
        RoleTypeId RoleType { get; set; }
        bool IgnoreRole { get; set; }
        float Health { get; set; }
        RoleTypeId RoleAppearance { get; set; }
        Dictionary<EffectType, byte> Effects { get; set; }
        bool CanEscape { get; set; }
        RoleTypeId RoleAfterEscape { get; set; }
        Vector3 Size { get; set; }
        string BroadcastText { get; set; }
        ushort BroadcastDuration { get; set; }
        List<ItemType> Items { get; set; }
        List<uint> CustomItems { get; set; }
        Dictionary<AmmoType, ushort> Ammo { get; set; }
        RoomType SpawnRoom { get; set; }
        bool CanTrigger096 { get; set; }
        bool CanTrigger173 { get; set; }
        bool IgnoreGameSpawn { get; set; }
        float DamageMiltiplier { get; set; }

        ICustomRoleAbility? Ability { get; set; }
    }
}
