using Godot;
using System;

public partial class TextBox : Control
{
	public VBoxContainer vbox;
	private ColorRect phone;
	private PanelContainer panel;
	public TextureButton send_btn;
	public TextureButton closed_btn;
	public Label nik;
	public TextureRect avat;
	public LineEdit enter_msg;

	public override void _Ready()
	{
		enter_msg = GetNode<LineEdit>("%enter_msg");
		avat = GetNode<TextureRect>("%avat");
		nik = GetNode<Label>("%nik");
		send_btn = GetNode<TextureButton>("%send");
		closed_btn = GetNode<TextureButton>("%closed");
		panel = GetNode<PanelContainer>("%panel");
		vbox = GetNode<VBoxContainer>("%vbox");
		phone = GetNode<ColorRect>("%phone");
		panel.ClipChildren = CanvasItem.ClipChildrenMode.AndDraw;
	}

	
	public override void _Process(double delta)
	{
	}
}
