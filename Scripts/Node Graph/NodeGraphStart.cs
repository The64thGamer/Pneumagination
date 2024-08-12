using Godot;
using System;

public partial class NodeGraphStart : CanvasLayer
{
	bool canInput = true;
	public static bool nodeEnabled;
	bool pauseScreen = false;
	Input.MouseModeEnum oldMouse;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {      
		Visible = false;
        (GetTree().CurrentScene as InputManager).GetMenuClass("Node Graph").ChangedMenu += (on) => OnToggleMenu(on);
	}

	public void OnToggleMenu(bool on)
	{
		Visible = on;
		if(Visible)
		{
			oldMouse = Input.MouseMode;
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else
		{
			Input.MouseMode = oldMouse;
		}
		nodeEnabled = Visible;
	}
}
