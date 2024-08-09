using Godot;
using System;

public partial class Inventory : Control
{	
    public static int[] inventory;

	bool canInput = true;
	public static bool inventoryEnabled;
	bool pauseScreen = false;
	bool nodeScreen = false;
	Input.MouseModeEnum oldMouse;

	public override void _Ready()
	{
		Visible = false;
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("Inventory") && canInput && !pauseScreen && !nodeScreen)
		{
			ToggleInventory();
		}
		if (Input.IsActionJustPressed("Pause"))
		{
			pauseScreen = !pauseScreen;
			ToggleAllInventoryUI();
		}
		if (Input.IsActionJustPressed("Toggle NodeGraph"))
		{
			nodeScreen = !nodeScreen;
			ToggleAllInventoryUI();
		}
	}

	public void ToggleAllInventoryUI()
	{
		if(pauseScreen || nodeScreen)
		{
			(GetParent() as Control).Visible = false;
		}
		else
		{
			(GetParent() as Control).Visible = true;
		}
	}

	public void ToggleInventory()
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
		inventoryEnabled = Visible;
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

