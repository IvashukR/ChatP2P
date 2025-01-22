using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class Stats : Resource
{
    [Export] public bool is_host;
    [Export] public string nik;
    [Export] public string ip;
    [Export] public string guid;
    [Export] public string p_avatar;
    public Dictionary<(string, string), List<Godot.Collections.Dictionary>> cp_activeChats = new Dictionary<(string, string), List<Godot.Collections.Dictionary>>();
    //Godot can't Serealized hard object as prefab TextBox that's why I'll do it myself.
    public Stats(bool is_host, string nik, string p_avatar, string ip, string guid)
    {
        this.guid = guid;
        this.is_host = is_host;
        this.nik = nik;
        this.p_avatar = p_avatar;
        this.ip = ip;
    }
    public Stats() : this(false, null, null, null, null) {}
    //godot need void constructor for class what inhertion resorce for save and load .tres
    

}
