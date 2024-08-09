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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("Toggle NodeGraph") && canInput && !pauseScreen)
		{
			ToggleNode();
		}
				
		if (Input.IsActionJustPressed("Pause"))
		{
			pauseScreen = !pauseScreen;
		}
	}

	public void ToggleNode()
	{
		Visible = !Visible;
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

	public void StopInputs()
	{
		canInput = false;
	}

	public void StartInputs()
	{		
		canInput = true;
	}
}
