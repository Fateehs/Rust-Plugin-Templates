using Newtonsoft.Json;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins;

[Info("TheFinally", "Fatih Selvi", " 0.0.1")]
class TheFinally : RustPlugin
{
    private ConfigData configData;
    class ConfigData
    {
        [JsonProperty(PropertyName = "LockDown")]
        public bool lockdown = false;
        [JsonProperty(PropertyName = "Button Ent Id")]
        public NetworkableId button { get; set; }
        [JsonProperty(PropertyName = "Kick Message")]
        public string kick = "The Lockdown is in effect";
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
        permission.RegisterPermission("TheFinally.admin", this);

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

    StoredData storedData;
    class StoredData
    {
        public List<ulong> Whitelisted = new List<ulong>();
    }
    void Loaded()
    {
        storedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>("TheData");
        Interface.Oxide.DataFileSystem.WriteObject("TheData", storedData);
    }
    void SaveData()
    {
        Interface.Oxide.DataFileSystem.WriteObject("TheData", storedData);
    }

    [ChatCommand("SetButton")]
    void set(BasePlayer player)
    {
        if (!permission.UserHasPermission(player.userID.ToString(), "TheFinally.admin"))
        {
            SendReply(player, "Go Away");
            return;
        }
        else
        {
            IOEntity button;
            if (!IOLOOKUP(player, out button))
            {
                SendReply(player, "No Button Found");
                return;
            }
            SendReply(player, $"Found button {button}");
            configData.button = button.net.ID;
            SaveConfig(configData);
        }
    }

    void OnButtonPress(PressButton button, BasePlayer player)
    {
        if (button.net.ID == configData.button)
        {
            storedData.Whitelisted.Add(player.userID);
            SaveData();
            SendReply(player, "You have been added to the whitelist");
        }
        else
        {
            return;
        }
    }

    void OnPlayerConnected(BasePlayer player)
    {
        if (configData.lockdown == false) return;
        if (!storedData.Whitelisted.Contains(player.userID))
        {
            Network.Net.sv.Kick(player.net.connection, rust.QuoteSafe(configData.kick));
        }
        else
        {
            Puts($"User {player.displayName} is on the whitelist");
            return;
        }
    }

    [ChatCommand("lockdown")]
    void lockdown(BasePlayer player)
    {
        if (!permission.UserHasPermission(player.userID.ToString(), "TheFinally.admin"){
            SendReply(player, "No Permission");
            return;
        }
        else
        {
            configData.lockdown = !configData.lockdown;
            SaveConfig(configData);
            SendReply(player, $"Lockdown is set {configData.lockdown}");
        }
    }

    [ConsoleCommand("clearlock")]
    void clearlock(ConsoleSystem.Arg arg)
    {
        if (arg.Player() == null || arg.Player()?.IsAdmin == true)
        {
            storedData.Whitelisted.Clear();
            SaveData();
            Puts("Lockdown whitelist cleared");
        }
    }

    private void OnNewSave(string filename)
    {
        storedData.Whitelisted.Clear();
        SaveData();
        Puts("New map detected whitelist data cleared.");
        configData.lockdown = false;
        SaveConfig(configData);
    }

    private bool IOLOOKUP(BasePlayer player, out Door door)
    {
        RaycastHit hit;
        door = null;
        if (Physics.Raycast(player.eyes.HeadRay(), out hit, 3))
        {
            door = hit.GetEntity() as Door;
        }
        return door != null;
    }
}
