using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledCustomRoles.API.Enums;

namespace UntitledCustomRoles.API.Interfaces
{
#nullable enable
    public interface ICustomRoleAbility
    {
        string Name { get; set; }
        string Description { get; set; }
        float Cooldown { get; set; }
        bool RequiredTarget { get; set; }
        int MaxUsesPerRound { get; set; }
        AbilityType AbilityType { get; set; }


        void Ability(Player player, Player? target = null);
    }
}
