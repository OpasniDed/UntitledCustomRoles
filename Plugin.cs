using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledCustomRoles.Events;

namespace UntitledCustomRoles
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "OpasniDed";
        public override string Name => "UntitledCustomRoles";
        public override string Prefix => "UntitledCustomRoles";
        public static Plugin Instance;
        public static Handlers _handlers;
        public override void OnEnabled()
        {
            Instance = this;
            _handlers = new Handlers();
            _handlers.Register();
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            _handlers.Unregister();
            Instance = null;
            base.OnDisabled();
        }
    }
}
