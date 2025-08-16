using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using ProjectMER.Features.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledCustomRoles.API;
using UntitledCustomRoles.API.Interfaces;

namespace UntitledCustomRoles.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ucr : ICommand
    {
        public string Command { get; } = "ucr";
        public string[] Aliases { get; } = { };
        public string Description { get; } = "Команда апи UntitledCustomRoles";
        public bool SanitizeResponse => false;

        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (args.Count == 0)
            {
                response = "Использование\n" +
                    "ucr list - Посмотреть все кастом роли\n" +
                    "ucr give - Выдать кастом роль\n" +
                    "ucr check - Посмотреть роль по ID";
                return false;
            }

            switch (args.At(0).ToLower())
            {
                case "list":
                    if (CustomRole.RolesList.Count == 0)
                    {
                        response = "Нет зарегестрированых ролей";
                        return false;
                    }
                    StringBuilder sb = new();
                    foreach (var role in CustomRole.RolesList.OrderBy(s => s.UId))
                    {
                        sb.AppendLine($"<color=#FFA500>ID: {role.UId} | {role.Name}</color>");
                    }
                    response = "<color=#00FFFF>Доступные роли</color>\n" +
                        sb.ToString();
                    return true;
                case "give":
                    if (args.Count < 3)
                    {
                        response = "Использование: ucr <player id> <role id>";
                        return false;
                    }

                    if (!Int32.TryParse(args.At(1), out int id))
                    {
                        response = "Введите число";
                        return false;
                    }
                    if (!Int32.TryParse(args.At(2), out int roleId))
                    {
                        response = "Введите число";
                        return false;
                    }
                    if (!CustomRole.TryGet(roleId, out ICustomRole cRole))
                    {
                        response = "Роль не найдена";
                        return false;
                    }
                    Player target = Player.Get(id);
                    if (target is null)
                    {
                        response = "Игрок не найден";
                        return false;
                    }
                    CustomRole.GiveRole(target, cRole);
                    response = $"Роль {cRole.Name} игроку {target.Nickname} выдана";
                    return true;
                case "check":
                    if (args.Count < 2)
                    {
                        response = "Введите айди роли";
                        return false;
                    }
                    if (!Int32.TryParse(args.At(1), out int rId))
                    {
                        response = "Введите число";
                        return false;
                    }
                    if (!CustomRole.TryGet(rId, out ICustomRole cRolee))
                    {
                        response = "Роль с таким айди не найдена";
                        return false;
                    }
                    string roleItems = (cRolee.Items != null && cRolee.Items.Count > 0) ? string.Join(", ", cRolee.Items) : "Нет предметов";
                    string roleEffects = (cRolee.Effects.Keys != null && cRolee.Effects.Count > 0) ? string.Join(", ", cRolee.Effects.Keys) : "Нет эффектов";
                    string roleCustomItems = (cRolee.CustomItems != null && cRolee.CustomItems.Count > 0) ? string.Join(", ", cRolee.CustomItems.Select(cItem => CustomItem.TryGet(cItem, out var customItem) && customItem != null ? customItem.Name : "???")) : "Нет кастом предметов";

                    response = $"Информация роли\n" +
                        $"Название: {cRolee.Name}\n" +
                        $"Роль: {cRolee.RoleType}\n" +
                        $"Предметы: {roleItems}\n" +
                        $"Эффекты: {roleEffects}\n" +
                        $"Кастом предметы: {roleCustomItems}";
                    return true;
            }

            response = "Использование\n" +
                    "ucr list - Посмотреть все кастом роли\n" +
                    "ucr give - Выдать кастом роль\n" +
                    "ucr check - Посмотреть роль по ID";
            return false;
        }
    }
}
