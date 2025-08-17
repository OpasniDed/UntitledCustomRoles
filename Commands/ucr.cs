using CommandSystem;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using System;
using System.Linq;
using System.Text;
using UntitledCustomRoles.API;
using UntitledCustomRoles.API.Interfaces;

namespace UntitledCustomRoles.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ucr : ICommand
    {
        public string Command { get; } = "ucr";
        public string[] Aliases { get; } = { };
        public string Description { get; } = "Command API UntitledCustomRoles (UCR)";
        public bool SanitizeResponse => false;

        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (!sender.CheckPermission("ucr"))
            {
                response = "You dont have permission to run this command. Premission: ucr";
                return false;
            }

            if (args.Count == 0)
            {
                response = @"Use
                    ucr list - List Registered Custom Roles
                    ucr spawn - Spawn Custom Role
                    ucr check - Check Role info by ID";
                return false;
            }

            switch (args.At(0).ToLower())
            {
                case "list":
                    if (CustomRole.RolesList.Count == 0)
                    {
                        response = "No registered roles.";
                        return false;
                    }
                    StringBuilder sb = new();
                    foreach (var role in CustomRole.RolesList.OrderBy(s => s.UId))
                    {
                        sb.AppendLine($"<color=#FFA500>ID: {role.UId} | {role.Name}</color>");
                    }
                    response = "<color=#00FFFF>Avilable roles</color>\n" +
                        sb.ToString();
                    return true;
                case "spawn":
                    if (args.Count < 3)
                    {
                        response = "Use: ucr spawn <player id> <role id>";
                        return false;
                    }

                    if (!Int32.TryParse(args.At(1), out int id))
                    {
                        response = "Enter number.";
                        return false;
                    }
                    if (!Int32.TryParse(args.At(2), out int roleId))
                    {
                        response = "Enter number.";
                        return false;
                    }
                    if (!CustomRole.TryGet(roleId, out ICustomRole cRole))
                    {
                        response = "Role does not exist.";
                        return false;
                    }
                    Player target = Player.Get(id);
                    if (target is null)
                    {
                        response = "Player does not exist.";
                        return false;
                    }
                    CustomRole.GiveRole(target, cRole);
                    response = $"Role {cRole.Name} for player {target.Nickname} spawned.";
                    return true;
                case "check":
                    if (args.Count < 2)
                    {
                        response = "Enter Role ID.";
                        return false;
                    }
                    if (!Int32.TryParse(args.At(1), out int rId))
                    {
                        response = "Enter number.";
                        return false;
                    }
                    if (!CustomRole.TryGet(rId, out ICustomRole cRolee))
                    {
                        response = "Role does not exist.";
                        return false;
                    }
                    string roleItems = (cRolee.Items != null && cRolee.Items.Count > 0) ? string.Join(", ", cRolee.Items) : "No items.";
                    string roleEffects = (cRolee.Effects.Keys != null && cRolee.Effects.Count > 0) ? string.Join(", ", cRolee.Effects.Keys) : "No effects.";
                    string roleCustomItems = (cRolee.CustomItems != null && cRolee.CustomItems.Count > 0) ? string.Join(", ", cRolee.CustomItems.Select(cItem => CustomItem.TryGet(cItem, out var customItem) && customItem != null ? customItem.Name : "???")) : "No Custom Items.";
                    int activePlayers = CustomRole.PlayerRoles.Where(r => r.Value.UId == cRolee.UId).Count();

                    response = $@"Role info:
                        - Name: {cRolee.Name}
                        - Role: {cRolee.RoleType}
                        - Health: {cRolee.Health}
                        - Spawn Chance: {cRolee.SpawnChance}%
                        - Items: {roleItems}
                        - Effects: {roleEffects}
                        - Custom Items: {roleCustomItems}
                        - Can Escape: {cRolee.CanEscape}
                        - Role after escape: {cRolee.RoleAfterEscape}
                        - Role appearance: {cRolee.RoleAppearance}
                        - Active Players: {activePlayers}";
                    return true;
            }

            response = @"Use
                    ucr list - List Registered Custom Roles
                    ucr spawn - Spawn Custom Role
                    ucr check - Check Role info by ID";
            return false;
        }
    }
}
