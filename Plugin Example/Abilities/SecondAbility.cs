using Exiled.API.Enums;
using Exiled.API.Features;
using UntitledCustomRoles.API.Enums;
using UntitledCustomRoles.API.Interfaces;

namespace UntitledCustomRoles.Example.Abilities
{
    public class SecondAbility : ICustomRoleAbility
    {
        public string Name { get; set; } = "Example Ability 2";
        public string Description { get; set; } = "This is example ability 2!";
        public float Cooldown { get; set; } = 10f; // In seconds
        public bool RequiredTarget { get; set; } = false;
        public int MaxUsesPerRound { get; set; } = 1;

        public void Ability(Player player, Player target = null)
        {
            player.RemoveHandcuffs();
            player.EnableEffect(EffectType.Invisible, duration: 5);

            // and what u want ;p
        }
    }
}
