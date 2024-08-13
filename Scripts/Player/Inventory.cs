using Godot;
using ProjectDMG.DMG.gameRom;
using System;

public partial class Inventory : Control
{	
    Godot.Collections.Array<InventoryItem> inventory = new Godot.Collections.Array<InventoryItem>();
	InventoryItem offHandItem;
	[Export] VBoxContainer uiInvArray;
	[Export] Control hotbar;
	[Export] Label hotbarTabText;
	Godot.Collections.Array<Node> hotbarBoxes;

	bool canInput = true;
	public static bool inventoryEnabled;
	bool pauseScreen = false;
	bool nodeScreen = false;
	Input.MouseModeEnum oldMouse;
	int hotbarIndex;
	int inventoryIndexOfHotbar;
	InputManager inputManager;

    void OnToggleMenu(bool menu)
    {
        Visible = menu;
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

    public override void _Ready()
    {     
		inputManager = GetTree().CurrentScene as InputManager; 
        inputManager.GetMenuClass("Inventory").ChangedMenu += (on) => OnToggleMenu(on);

		hotbarBoxes = hotbar.GetChild(0).GetChildren();
		Visible = false;
		inventory.Add(new InventoryItem()
		{
			name = "Lemon",
			count = 5,
		});
		for (int i = 0; i < 20; i++)
		{
			inventory.Add(null);
		}
		inventory.Add(new InventoryItem()
		{
			name = "Lemon",
			count = 4,
		});
		RefreshInventoryLayout();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Hotbar Down") && Input.IsActionPressed("Shift"))
		{
			inventoryIndexOfHotbar = inventoryIndexOfHotbar-1;
			if(inventoryIndexOfHotbar < 0)
			{
				inventoryIndexOfHotbar = Mathf.CeilToInt(inventory.Count / 9.0f);
			}
			RefreshInventoryLayout();
		}
		if (Input.IsActionJustPressed("Hotbar Down"))
		{
			inventoryIndexOfHotbar = inventoryIndexOfHotbar+1;
			if(inventoryIndexOfHotbar >= Mathf.Ceil(inventory.Count / 9.0f))
			{
				inventoryIndexOfHotbar = 0;
			}
			RefreshInventoryLayout();
		}
		if (Input.IsActionJustPressed("Scroll Up"))
		{
			hotbarIndex = hotbarIndex-1;
			if(hotbarIndex < 0)
			{
				hotbarIndex = 8;
			}
			RefreshInventoryLayout();
		}
		if (Input.IsActionJustPressed("Scroll Down"))
		{
			hotbarIndex = hotbarIndex+1;
			if(hotbarIndex > 8)
			{
				hotbarIndex = 0;
			}
			RefreshInventoryLayout();
		}
		if (Input.IsActionJustPressed("Action"))
		{
			PlaceItem();
		}
	}

	public void PlaceItem()
	{
		Camera3D camera3D = GetViewport().GetCamera3D();
		PhysicsDirectSpaceState3D spaceState = camera3D.GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(camera3D.GlobalPosition, camera3D.GlobalPosition + (-camera3D.GlobalTransform.Basis.Z * PlayerMovement.playerReach));
		query.CollisionMask = 0b00000000_00000000_00000000_00000100; //Brushes
		Godot.Collections.Dictionary result = spaceState.IntersectRay(query);
		if (result.Count > 0)
		{
			int hotbarSelection = (inventoryIndexOfHotbar*9)+hotbarIndex;
			if(hotbarSelection < inventory.Count && inventory[hotbarSelection] != null)
			{
				Node3D newNode = GD.Load<PackedScene>("res://Prefabs/Items/"+ inventory[hotbarSelection].name + ".tscn").Instantiate() as Node3D;
				newNode.Position = (Vector3)result["position"];
				GetTree().Root.AddChild(newNode);
				inventory[hotbarSelection].count -= 1;
				if(inventory[hotbarSelection].count == 0)
				{
					inventory[hotbarSelection] = null;
				}
							RefreshInventoryLayout();

			}

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
			Control bar = ResourceLoader.Load<PackedScene>("res://Prefabs/UI/Inventory Item Bar.tscn").Instantiate() as Control;

			Godot.Collections.Array<Node> childs = bar.GetChild(0).GetChildren();
			for (int e = 0; e < childs.Count; e++)
			{
				RefreshItemBox(childs[e] as Control, (i*9)+e,false);
			}
			uiInvArray.AddChild(bar);
		}

		for (int e = 0; e < hotbarBoxes.Count; e++)
		{
			RefreshItemBox(hotbarBoxes[e] as Control, (inventoryIndexOfHotbar*9)+e, hotbarIndex == e);
		}

		hotbarTabText.Text = ((char)('A' + inventoryIndexOfHotbar)).ToString();
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
	public void StopInputs()
	{
		canInput = false;
	}

	public void StartInputs()
	{		
		canInput = true;
	}
}

