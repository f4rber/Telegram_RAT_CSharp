using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramRAT
{
    class BotCommand
    {
        public string Command { get; set; }
        public string Example { get; set; }
        public Action<BotCommandModel, Update> Execute { get; set; }
        public Action<BotCommandModel, Update> OnError { get; set; }
        public int CountArgs;

        public static BotCommandModel Parse(string text)
        {
            if (text.StartsWith("/"))
            {
                var splits = text.Split(' ');
                var name = splits?.FirstOrDefault();
                var args = splits.Skip(1).Take(splits.Count()).ToArray();

                return new BotCommandModel
                {
                    Command = name,
                    Args = args,
                };
            }
            return null;
        }
    }
}
