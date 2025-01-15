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

	public override void _Ready()
	{
		avat = GetNode<TextureRect>("%avat");
		nik = GetNode<Label>("%nik");
		send_btn = GetNode<TextureButton>("%send");
		closed_btn = GetNode<TextureButton>("%closed");
		panel = GetNode<PanelContainer>("%panel");
		vbox = GetNode<VBoxContainer>("%vbox");
		phone = GetNode<ColorRect>("%phone");
		vbox.Resized += () => phone.Scale = new Vector2(683, vbox.Scale.Y);
		panel.ClipChildren = CanvasItem.ClipChildrenMode.AndDraw;
	}

	
	public override void _Process(double delta)
	{
	}
}
