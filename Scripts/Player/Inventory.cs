using Godot;
using System;

public partial class Inventory : Control
{	
    public static int[] inventory;

	bool canInput = true;
	public static bool inventoryEnabled;
	bool pauseScreen = false;
	Input.MouseModeEnum oldMouse;

	public override void _Ready()
	{
		Visible = false;
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("Inventory") && canInput && !pauseScreen)
		{
			ToggleInventory();
		}
		if (Input.IsActionJustPressed("Pause"))
		{
			pauseScreen = !pauseScreen;
			ToggleAllInventoryUI();
		}
	}

	public void ToggleAllInventoryUI()
	{
		(GetParent() as Control).Visible = !(GetParent() as Control).Visible;
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

