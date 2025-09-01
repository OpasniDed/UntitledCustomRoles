using Exiled.API.Features;
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


        void Ability(Player player, Player? target = null);


    }
}
