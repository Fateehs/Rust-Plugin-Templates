using Newtonsoft.Json;
using UnityEngine;
using Oxide.Core;

namespace Oxide.Plugins;

[Info("TheData", "Fatih Selvi", "0.0.1")]
class TheData : RustPlugin
{
    private ConfigData configData;

    class ConfigData
    {
        [JsonProperty(PropertyName = "Door Net Id")]
        public NetworkableId door { get; set; }
        [JsonProperty(PropertyName = "Knock Message")]
        public string knock = "poo poo";
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
        permission.RegisterPermission("TheData.admin", this);

        if (!LoadConfigVariables())
        {
            Puts("Config file issue detected, please delete file or check syntax and fix");
            return;
        }
    }

    protected override void LoadDefaultConfig()
    {
        Puts("Creating new config files");
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
        public List<ulong> Knocked = new List<ulong>();
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

    [ChatCommand("myDoor")]
    void mydoor(BasePlayer player)
    {
        if (!permission.UserHasPermission(player.userID.ToString(), "TheData.admin"))
        {
            SendReply(player, "No permission");
            return;
        }
        else
        {
            Door door;
            if (!DOORLOOK(player, out door))
            {
                SendReply(player, "no door found");
                return;
            }
            else
            {
                SendReply(player, $"found door {door}");
                configData.door = door.net.ID;
                SaveConfig(configData);
            }
        }
    }

    void OnDoorKnocked(Door door, BasePlayer player)
    {
        if (door.net.ID == configData.door)
        {
            if (storedData.Knocked.Contains(player.userID)) return;
            SendReply(player, configData.knock);
            storedData.Knocked.Add(player.userID);
            SaveData();
        }
        else
        {
            return;
        }
    }

    [ChatCommand("clear")]
    void clear(BasePlayer player)
    {
        if (permission.UserHasPermission(player.userID.ToString(), "TheData.admin"))
        {
            storedData.Knocked.Clear();
            SaveData();
            SendReply(player, "Data has been cleared");
        }

    }

    private void OnNewSave(string filename)
    {
        storedData.Knocked.Clear();
        SaveData();
        Puts("Wipe Detected, Knocked Data Cleared");
    }

    private bool DOORLOOK(BasePlayer player, out Door door)
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
