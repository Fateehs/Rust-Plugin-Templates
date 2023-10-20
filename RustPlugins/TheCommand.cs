namespace Oxide.Plugins;

[Info("TheCommand", "Fatih Selvi", "0.0.1")]
class TheCommand : RustPlugin
{
    [ChatCommand("chatcommand")]
    void test(BasePlayer player)
    {
        SendReply(player, "You sent the chat command.");
    }

    [ConsoleCommand("consolecommand")]
    void contest(ConsoleSystem.Arg args)
    {
        Puts("You sent the console command.");
    }
}
