using Godot;
using System;
using System.Collections.Generic;



public partial class Chat : Node
{
	private Button load_btn;
	private Button save_prof;
	private TextureButton profil_btn;
	private TextureButton krest_btn;
	private ColorRect profil;
	private FileDialog user_file;
	private TextureRect set_avatar;
	private TextureRect user_avatar;
	private LineEdit nik;
	private Label user_nik;
	private ENetMultiplayerPeer multiplayer;
	private List<Button> users_btn = new List<Button>();
	private List<User> users = new List<User>();
	private VBoxContainer vbox;
	private string ph;
	static public bool is_host;
	static public string ip;
	private void profil_click(Vector2 target_pos)
	{
			var tween = CreateTween();
        	tween.SetEase(Tween.EaseType.InOut);
        	tween.SetTrans(Tween.TransitionType.Sine);
        	tween.TweenProperty(profil, "position", target_pos, 0.9f);
	}
	private void AddOrUpdateUser(int peerID, string nik, string avatar)
	{
		User _user = users.Find(u => u.PeerId == peerID);
		if(_user != null)
		{
			_user.UpdateUserData(nik, avatar);
		}
		else
		{
			users.Add(new User(peerID, avatar, nik));
		}

	}
	private Godot.Collections.Array ConvertUsersToVariant(List<User> userList) //for RPC
    {
        Godot.Collections.Array result = new Godot.Collections.Array();

        foreach (var user in userList)
        {
            Godot.Collections.Dictionary userDict = new Godot.Collections.Dictionary();
            userDict["PeerId"] = user.PeerId;
            userDict["Nickname"] = user.Nickname;
			userDict["AvatarPath"] = user.Path_Avatar; 
            result.Add(userDict);
        }

        return result;
    }
	private void OnPlayerConnected(int peerId)
    {
		AddOrUpdateUser(peerId, "NoName", ph);
		Rpc("SyncPlayerList", ConvertUsersToVariant(users));
		UpdateInterface();
    }
	private void _FileSelected(string path)
	{
		ph = path;
        var image = Image.LoadFromFile(path);
		image.Resize(128, 128);
		var texture = ImageTexture.CreateFromImage(image);
		set_avatar.Texture = texture;
	}
	private void save_profil()
	{
		user_avatar.Texture = set_avatar.Texture;
		user_nik.Text = nik.Text;
		AddOrUpdateUser(multiplayer.GetUniqueId(), nik.Text,  ph);
		Rpc("SyncPlayerList", ConvertUsersToVariant(users));
	}
	public override void _Ready()
	{
		vbox = GetNode<VBoxContainer>("%vbox");
		save_prof = GetNode<Button>("%save_prof");
		user_nik = GetNode<Label>("%user_nik");
		nik = GetNode<LineEdit>("%nik");
		set_avatar = GetNode<TextureRect>("%set_avatar");
		user_avatar = GetNode<TextureRect>("%user_avatar");
		user_file = GetNode<FileDialog>("%file_system");
		load_btn = GetNode<Button>("%load_btn");
		profil_btn = GetNode<TextureButton>("%profil_btn");
		krest_btn = GetNode<TextureButton>("%krest_btn");
		profil = GetNode<ColorRect>("%profil");
		Vector2 start_pos = profil.Position;
		profil_btn.Pressed += () => profil_click(new Vector2(1, 2));
		krest_btn.Pressed += () => profil_click(start_pos);
		GetNode<PanelContainer>("%user_panel").ClipChildren = CanvasItem.ClipChildrenMode.AndDraw;
		GetNode<PanelContainer>("%set_panel").ClipChildren = CanvasItem.ClipChildrenMode.AndDraw;
		save_prof.Pressed += save_profil;
		set_avatar.Size = new Vector2(128, 128);
		user_avatar.Size = new Vector2(128, 128);
		user_file.Filters = new string[] { "*.png", "*.jpg", "*.jpeg" };
		nik.MaxLength = 12;
		user_file.CurrentDir = OS.GetUserDataDir();
		load_btn.Pressed += () => user_file.Popup();
		user_file.FileSelected += _FileSelected;
		if (is_host)
		{
			StartServer();
		}
		else
		{
			StartClient();
		}
	}

	private void StartServer()
	{
		GD.Print("Starting server...");
		multiplayer = new ENetMultiplayerPeer();
		multiplayer.CreateServer(12345, 3);
		Multiplayer.MultiplayerPeer = multiplayer;
		multiplayer.PeerConnected += (peerId) => OnPlayerConnected((int)peerId);
	}
	private void StartClient()
	{
		multiplayer = new ENetMultiplayerPeer();
		multiplayer.CreateClient(ip, 12345);
		Multiplayer.MultiplayerPeer = multiplayer;
		multiplayer.PeerConnected += (peerId) => OnPlayerConnected((int)peerId);

	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SyncPlayerList(Godot.Collections.Array userArray)
    {
        users.Clear();

        foreach (Godot.Collections.Dictionary userDict in userArray)
        {
            int peerId = (int)userDict["PeerId"];
            string nickname = (string)userDict["Nickname"];
			string avatarPath = (string)userDict["AvatarPath"];
            AddOrUpdateUser(peerId, nickname,avatarPath);
        }

        UpdateInterface();
    }

    

   	private void UpdateInterface()
   	{
		int index = 0;
		int myPeerId = multiplayer.GetUniqueId();
		foreach(User  user in users)
		{
			if (user.PeerId == myPeerId)
        	{
           	 	continue;
        	}
			Button btn;
			if(users_btn.Count <= index)
			{
				btn = new Button();
				btn.Text = user.Nickname;
				btn.Pressed += () => OpenChat(user.PeerId, user.Nickname, user.Path_Avatar);
				vbox.AddChild(btn);
				users_btn.Add(btn);
			}
			else
			{
				btn = (Button)users_btn[index];
				btn.Text = user.Nickname;
			}
			index++;
		}
   	}
	public class User
	{
		public int PeerId { get; set; }      
		public string Nickname { get; set; }  
		public string Path_Avatar { get; set; }

		public User(int peerId, string p_avatar, string nik)
		{
			PeerId = peerId;
			Path_Avatar = p_avatar;
			Nickname = nik;
		}
		public void UpdateUserData(string nickname, string p_avatar)
        {
            this.Nickname = nickname;
            this.Path_Avatar = p_avatar;
        }
	}
	private void OpenChat(int peerId, string avatar, string nik)
	{

	}
}
