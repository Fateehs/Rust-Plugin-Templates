using Newtonsoft.Json;

namespace Oxide.Plugins;

[Info("TheChange", "Fatih Selvi", "0.0.1")]
class TheChange : RustPlugin
{
    #region config

    private ConfigData configData;
    class ConfigData
    {
        [JsonProperty(PropertyName = "Bool")]
        public bool reply = true;
    }

    private bool LoadConfigVariables()
    {
        try
        {
            configData = Config.ReadObject<ConfigData>();
        }
        catch
        {
            return false;
        }
        SaveConfig(configData);
        return true;
    }

    void Init()
    {
        permission.RegisterPermission("TheChange.admin", this);
        if (!LoadConfigVariables())
        {
            Puts("Config file issue detected, please delete file or check syntax and fix issue");
            return;
        }
    }

    protected override void LoadDefaultConfig()
    {
        Puts("Creating new config file.");
        configData = new ConfigData();
        SaveConfig(configData);
    }

    void SaveConfig(ConfigData config)
    {
        Config.WriteObject(config, true);
    }

    #endregion

    [ChatCommand("ConfigCheck")]
    void chatconfigcheck(BasePlayer player)
    {
        SendReply(player, $"The config value is {configData.reply}");
    }

    [ChatCommand("change")]
    void change(BasePlayer player)
    {
        if (!permission.UserHasPermission(player.userID.ToString(), "TheChange.admin"))
        {
            SendReply(player, "You do not have permission to use this command");
            return;
        }
        else
        {
            configData.reply = !configData.reply;
            SaveConfig(configData);
            SendReply(player, $"The config data was changed from {!configData.reply} to {configData.reply}");
        }
    }
}