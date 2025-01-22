using Godot;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class Stats : Resource
{
    [Export] public bool is_host;
    [Export] public string nik;
    [Export] public string ip;
    [Export] public string guid;
    [Export] public string p_avatar;
    public Dictionary<(string, string), List<Godot.Collections.Dictionary>> cp_activeChats = new Dictionary<(string, string), List<Godot.Collections.Dictionary>>();
    public Stats(bool is_host, string nik, string p_avatar, string ip, string guid)
    {
        this.guid = guid;
        this.is_host = is_host;
        this.nik = nik;
        this.p_avatar = p_avatar;
        this.ip = ip;
    }
    public Stats() : this(false, null, null, null, null) {}
    //Godot can't Serealized hard object as prefab TextBox that's why I'll do it myself.

}
