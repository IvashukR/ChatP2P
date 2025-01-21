using Godot;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class Stats : Resource
{
    [Export] public bool is_host;
    [Export] public bool is_1run;
    [Export] public string nik;
    [Export] public string ip;
    [Export] public string guid;
    [Export] public string p_avatar;
    public Dictionary<(string, string), TextBox> cp_activeChats = new Dictionary<(string, string), TextBox>();
    public Stats(bool is_host, string nik, string p_avatar, Dictionary<(string, string), TextBox> copy_Chats, string ip, string guid, bool is_1run)
    {
        this.guid = guid;
        this.is_1run = is_1run;
        this.is_host = is_host;
        this.nik = nik;
        this.p_avatar = p_avatar;
        this.ip = ip;
        cp_activeChats = copy_Chats.ToDictionary(entry => entry.Key, entry => entry.Value);

    }

}
