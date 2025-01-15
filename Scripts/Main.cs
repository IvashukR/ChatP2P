using Godot;
using System;
using System.Net;
using System.Net.Sockets;


public partial class Main : Node
{
	private Button create_host;
	private Window window_IPlocal;
	private Window window_e_ip;
	private Button ok_ip;
	private Button ok_ip2;
	private TextureButton copy_ip;
	private Button JoinRoom;
	private LineEdit enter_ip;
	private Label label_ip;
	private void ChangeScene(){GetTree().ChangeSceneToFile("res://chat.tscn");}
	public override void _Ready()
	{
		enter_ip = GetNode<LineEdit>("%enter_ip");
		window_e_ip = GetNode<Window>("%window_e_ip");
		copy_ip = GetNode<TextureButton>("%copy_ip");
		ok_ip = GetNode<Button>("%ok_ip");
		ok_ip2 = GetNode<Button>("%ok_ip2");
		label_ip = GetNode<Label>("%label_ip");
		window_IPlocal = GetNode<Window>("%window_ip");
		create_host = GetNode<Button>("%CreateRoom");
		JoinRoom = GetNode<Button>("%JoinRoom");
		label_ip.Text = GetLocaleIP();;
		window_IPlocal.Unresizable = true;
		window_IPlocal.CloseRequested += () => window_IPlocal.Hide();
		window_e_ip.CloseRequested += () => window_e_ip.Hide();
		create_host.Pressed += () => window_IPlocal.PopupCentered();
		JoinRoom.Pressed += () => window_e_ip.PopupCentered();
		copy_ip.Pressed += () =>
		{
			copy_ip.TextureNormal = (Texture2D)GD.Load("res://copy_ip_ok.png");
			DisplayServer.ClipboardSet(label_ip.Text);

		};
		ok_ip.Pressed += () => 
		{
			if (IsInsideTree())
            {
                Chat.is_host = true;
                CallDeferred("ChangeScene");
            }
		};
		ok_ip2.Pressed += () => 
		{
			if (IsInsideTree())
            {
                Chat.is_host = false;
				Chat.ip = enter_ip.Text;
				GD.Print(Chat.ip);
				CallDeferred("ChangeScene");
            }
		};
	}

	public override void _Process(double delta)
	{

	}
	public static string GetLocaleIP()
	{
		string localIP = "";
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            socket.Connect("8.8.8.8", 65530); 
            localIP = ((IPEndPoint)socket.LocalEndPoint).Address.ToString();
        }
        return localIP;
	}
}
