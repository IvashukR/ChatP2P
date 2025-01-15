using Godot;
using System;

public partial class TextBox : Control
{
	private VBoxContainer vbox;
	private ColorRect phone;
	public override void _Ready()
	{
		vbox = GetNode<VBoxContainer>("%vbox");
		phone = GetNode<ColorRect>("%phone");
		vbox.Resized += () => phone.Scale = new Vector2(500, vbox.Scale.Y);
	}

	
	public override void _Process(double delta)
	{
	}
}
