using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledCustomRoles.API.Enums;
using UntitledCustomRoles.API.Interfaces;

namespace UntitledCustomRoles.Example.Abilities
{
    public class FirstAbility : ICustomRoleAbility
    {
        public string Name { get; set; } = "Example Ability";
        public string Description { get; set; } = "This is example ability!";
        public float Cooldown { get; set; } = 60f; // In seconds
        public bool RequiredTarget { get; set; } = false;
        public int MaxUsesPerRound { get; set; } = 2;
        public AbilityType AbilityType { get; set; } = AbilityType.Passive;

        public void Ability(Player player, Player target = null)
        {
            player.EnableEffect(EffectType.Invisible, duration: 6);
            player.EnableEffect(EffectType.MovementBoost, duration: 2, intensity: 10);

            // and what u want ;p
        }
    }
}
