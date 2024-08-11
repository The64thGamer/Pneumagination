using Godot;
using ProjectDMG.DMG.gameRom;
using System;

public partial class Inventory : Control
{	
    Godot.Collections.Array<InventoryItem> inventory = new Godot.Collections.Array<InventoryItem>();
	InventoryItem offHandItem;
	[Export] VBoxContainer uiInvArray;
	[Export] Control hotbar;
	Godot.Collections.Array<Node> hotbarBoxes;

	bool canInput = true;
	public static bool inventoryEnabled;
	bool pauseScreen = false;
	bool nodeScreen = false;
	Input.MouseModeEnum oldMouse;
	int hotbarIndex;

	public override void _Ready()
	{
		hotbarBoxes = hotbar.GetChild(0).GetChildren();
		Visible = false;
		inventory.Add(new InventoryItem()
		{
			name = "Lemon",
			count = 1,
		});
		RefreshInventoryLayout();
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

		if (Input.IsActionJustPressed("Scroll Up"))
		{
			hotbarIndex = Mathf.Max(hotbarIndex-1,0);
			RefreshInventoryLayout();
		}
		if (Input.IsActionJustPressed("Scroll Down"))
		{
			hotbarIndex = Mathf.Min(hotbarIndex+1,8);
			RefreshInventoryLayout();
		}
	}

	//Redo this to not delete bars and instead reuse them
	void RefreshInventoryLayout()
	{
		for (int i = 0; i < uiInvArray.GetChildCount(); i++)
		{
			uiInvArray.GetChild(i).QueueFree();
		}

		for (int i = 0; i < Mathf.CeilToInt(inventory.Count / 9.0f)+1; i++)
		{
			Control bar = (ResourceLoader.Load<PackedScene>("res://Prefabs/UI/Inventory Item Bar.tscn").Instantiate()) as Control;

			Godot.Collections.Array<Node> childs = bar.GetChild(0).GetChildren();
			for (int e = 0; e < childs.Count; e++)
			{
				RefreshItemBox(childs[e] as Control, (i*9)+e,false);
			}
			uiInvArray.AddChild(bar);
		}

		for (int e = 0; e < hotbarBoxes.Count; e++)
		{
			RefreshItemBox(hotbarBoxes[e] as Control, (0*9)+e, hotbarIndex == e);
		}
	}

	void RefreshItemBox(Control control, int index, bool highlighted)
	{
		if(index > inventory.Count-1 || inventory[index] == null)
		{
			(control.FindChild("Image",true) as TextureRect).Visible = false;
			(control.FindChild("Count",true) as Label).Visible = false;
		}
		else
		{
			TextureRect tr = control.FindChild("Image",true) as TextureRect;
			tr.Visible = true;
			tr.Texture = ResourceLoader.Load<Texture2D>("res://UI/Item Icons/"+inventory[index].name + ".png");

			Label lb = control.FindChild("Count",true) as Label;
			lb.Visible = true;
			lb.Text = inventory[index].count.ToString();
		}

		(control.FindChild("Highlight",true) as Panel).Visible = highlighted;
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

