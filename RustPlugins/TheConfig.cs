using Newtonsoft.Json;

namespace Oxide.Plugins;

[Info("TheConfig", "Fatih Selvi", "0.0.1")]
class TheConfig : RustPlugin
{
    private ConfigData configData;
    class ConfigData
    {
        [JsonProperty(PropertyName = "Reply Message")]
        public string reply = "This is the reply that is set in the config";
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

    [ConsoleCommand("configtest")]
    void configtest(ConsoleSystem.Arg args)
    {
        Puts(configData.reply);
    }
}
