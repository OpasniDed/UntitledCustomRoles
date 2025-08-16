using Exiled.API.Interfaces;

namespace UntitledCustomRoles
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public float raycastRange { get; set; } = 5f;
    }
}
