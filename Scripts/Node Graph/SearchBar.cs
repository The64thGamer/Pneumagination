using Godot;
using System;

public partial class SearchBar : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("Alt Click") && !Visible)
		{
			Visible = true;
			GlobalPosition = GetGlobalMousePosition();
		}
		else if((Input.IsActionJustPressed("Alt Click")||Input.IsActionJustPressed("Click")) && Visible)
		{
			Visible = false;
		}
	}
}
