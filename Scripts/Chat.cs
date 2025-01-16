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
	private Dictionary<(int, int), TextBox> activeChats = new Dictionary<(int, int), TextBox>();
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
        LoadPhoto(path, set_avatar);
	}
	private void save_profil()
	{
		user_avatar.Texture = set_avatar.Texture;
		user_nik.Text = nik.Text;
		AddOrUpdateUser(multiplayer.GetUniqueId(), nik.Text,  ph);
		Rpc("SyncPlayerList", ConvertUsersToVariant(users));
		Rpc("NotifyProfileUpdate", multiplayer.GetUniqueId(), nik.Text, ph);
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
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void NotifyProfileUpdate(int peerId, string nik, string p_a)
	{
		int myPeerId = multiplayer.GetUniqueId(); 
		if (myPeerId == peerId)
		{
			return;
		}
		var chatKey = myPeerId < peerId ? (myPeerId, peerId) : (peerId, myPeerId);
		if (activeChats.ContainsKey(chatKey))
    	{
        	var chatBox = activeChats[chatKey];
			chatBox.nik.Text = nik;
			if(string.IsNullOrEmpty(p_a))
			{
				return;
			}
			LoadPhoto(p_a, chatBox.avat);
   	 	}
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
				btn.Pressed += () => OpenChat(user.PeerId, user.Path_Avatar, user.Nickname);
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
	internal class User
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
		RpcId(peerId, "OpenPersonalChat", multiplayer.GetUniqueId(), nik, avatar);
    	RpcId(multiplayer.GetUniqueId(), "OpenPersonalChat", peerId, nik, avatar);
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void OpenPersonalChat(int peerId, string nik, string avatar)
	{
		int myPeerId = multiplayer.GetUniqueId(); 
		var chatKey = myPeerId < peerId ? (myPeerId, peerId) : (peerId, myPeerId);
		if (activeChats.ContainsKey(chatKey))
    	{
        	var existingChat = activeChats[chatKey];
        	existingChat.Visible = true;
        	return;
   	 	}
		var chatScene = GD.Load<PackedScene>("res://text_box.tscn");
    	var chatInstance = chatScene.Instantiate<TextBox>();
		AddChild(chatInstance);
		chatInstance.closed_btn.Pressed += () => LocalCloseChat(peerId);
		chatInstance.send_btn.Pressed += () => SendMessage(chatInstance.enter_msg.Text, chatInstance.vbox, chatInstance.enter_msg, new Color(1, 0, 0));
		chatInstance.send_btn.Pressed += () => MediatorMessage(chatInstance.enter_msg.Text ,chatInstance.vbox, peerId, chatInstance.enter_msg, multiplayer.GetUniqueId());
		chatInstance.nik.Text = nik;
		if (string.IsNullOrEmpty(avatar))
		{
			activeChats[chatKey] = chatInstance;
			return;
		}
		LoadPhoto(avatar, chatInstance.avat);
		activeChats[chatKey] = chatInstance;
		
	}
	private void LocalCloseChat(int peerID)
	{
		int myPeerId = multiplayer.GetUniqueId(); 
		var chatKey = myPeerId < peerID ? (myPeerId, peerID) : (peerID, myPeerId);
		TextBox _tb = (TextBox)activeChats[chatKey];
		_tb.Visible = false;

	}
	private void LoadPhoto(string path, TextureRect tr)
	{
		var image = Image.LoadFromFile(path);
		image.Resize(128, 128);
		var texture = ImageTexture.CreateFromImage(image);
		tr.Texture = texture;
	}
	private void SendMessage(string msg, VBoxContainer box_msg, LineEdit e_msg, Color color_msg)
	{
		if(string.IsNullOrEmpty(msg))
		{
			return;
		}
		var l_msg = new Label();
		l_msg.Text = msg;
		l_msg.SelfModulate = color_msg;
		e_msg.Clear();
		box_msg.AddChild(l_msg);
		
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void ReceivMessage(string msg, VBoxContainer box_msg, int peerId, LineEdit e_msg, int mId)
	{
		var chatKey = mId < peerId ? (mId, peerId) : (peerId, mId);
		if (activeChats.ContainsKey(chatKey))
    	{
        	var Chat = activeChats[chatKey];
        	SendMessage(msg, Chat.vbox , Chat.enter_msg, new Color(1, 1, 1));
   	 	}
	}
	private void MediatorMessage(string msg, VBoxContainer box_msg, int peerId, LineEdit e_msg, int mId)
	{
		RpcId(peerId, "ReceivMessage", msg, box_msg, peerId, e_msg, mId);
	}
}
