using Exiled.API.Features;
using System;
using UntitledCustomRoles.API.Interfaces;

namespace UntitledCustomRoles.Events.Args
{
    public class CustomRoleSpawnedEventArgs : EventArgs
    {
        public Player Player { get; set; }
        public ICustomRole Role { get; set; }

        public CustomRoleSpawnedEventArgs(Player player, ICustomRole role)
        {
            Player = player;
            Role = role;
        }
    }
}
