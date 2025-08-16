using System;
using UntitledCustomRoles.Events.Args;

namespace UntitledCustomRoles.Events
{
    public class CustomRoleEvents
    {
        public static event Action<CustomRoleSpawnedEventArgs> CustomRoleSpawned;

        public static void OnCustomRoleSpawned(CustomRoleSpawnedEventArgs ev)
        {
            CustomRoleSpawned?.Invoke(ev);
        }
    }
}
