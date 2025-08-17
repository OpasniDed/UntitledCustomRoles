using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledCustomRoles.API;
using UntitledCustomRoles.Events;
using UntitledCustomRoles.Events.Args;

namespace Example
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "OpasniDed";
        public override string Name => "Example";
        public override string Prefix => "Example";
        public static Plugin Instance;

        public override void OnEnabled()
        {
            Instance = this;
            Register();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Unregister();
            Instance = null;
            base.OnDisabled();
        }

        private void Register()
        {
            CustomRoleEvents.CustomRoleSpawned += CustomRoleSpawned;
            CustomRole.RegisterAll();
        }

        private void Unregister()
        {
            CustomRoleEvents.CustomRoleSpawned -= CustomRoleSpawned;
            CustomRole.Unregister();
        }

        private void CustomRoleSpawned(CustomRoleSpawnedEventArgs ev)
        {
            // smth
            Log.Info(ev.Role.Name);
        }

    }
}
