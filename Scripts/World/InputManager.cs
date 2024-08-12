using Godot;
using System;

public partial class InputManager : Node
{
	Godot.Collections.Array<ToggleableIngameMenu> menus;

   	public override void _Ready()
    {     
		if(menus == null)
		{
			SetupMenus();
		}

		for (int i = 0; i < menus.Count; i++)
		{
			AddChild(menus[i]);
		}
	}

	void SetupMenus()
	{
		menus = new Godot.Collections.Array<ToggleableIngameMenu>()
		{
			new ToggleableIngameMenu()
			{
				menuName = "Console",
				menu = false,
			},
			new ToggleableIngameMenu()
			{
				menuName = "Wiki",
				menu = false,
			},
			new ToggleableIngameMenu()
			{
				menuName = "Pause Screen",
				menu = false,
			},
			new ToggleableIngameMenu()
			{
				menuName = "Photo Mode",
				menu = false,
			},
			new ToggleableIngameMenu()
			{
				menuName = "Node Graph",
				menu = false,
			},
			new ToggleableIngameMenu()
			{
				menuName = "Inventory",
				menu = false,
			},
			new ToggleableIngameMenu()
			{
				menuName = "Hotbar",
				menu = true,
			},
		};
	}
	
	public override void _Process(double delta)
	{        

		if (Input.IsActionJustPressed("Console"))
        {
			ToggleAMenu("Console");
		}
		if(!GetMenu("Console"))
		{
			if (Input.IsActionJustPressed("Photo Mode"))
			{
				ToggleAMenu("Photo Mode");
			}
			if (Input.IsActionJustPressed("Wiki"))
			{
				ToggleAMenu("Wiki");
			}
			if (Input.IsActionJustPressed("Pause"))
			{
				ToggleAMenu("Pause Screen");
			}
			if (Input.IsActionJustPressed("Inventory"))
			{
				ToggleAMenu("Inventory");
			}
			if (Input.IsActionJustPressed("Node Graph"))
			{
				ToggleAMenu("Node Graph");
			}
		}
	}
	
	void SetMenu(string menuString,bool set)
	{
		for (int i = 0; i < menus.Count; i++)
		{
			if(menus[i].menuName == menuString)
			{
				menus[i].menu = set;
				menus[i].EmitSignal("ChangedMenu",menus[i].menu);
				return;
			}
			if(menus[i].menu)
			{
				return;
			}
		}
	}

	public void ToggleAMenu(string menuString)
	{			
		for (int i = 0; i < menus.Count; i++)
		{
			if(menus[i].menuName == menuString)
			{
				menus[i].menu = !menus[i].menu;
				menus[i].EmitSignal("ChangedMenu",menus[i].menu);
				return;
			}
			if(menus[i].menu)
			{
				return;
			}
		}
	}

	public bool GetMenu(string menuString)
	{
		if(menus == null)
		{
			SetupMenus();
		}
		for (int i = 0; i < menus.Count; i++)
		{
			if(menus[i].menuName == menuString)
			{
				return menus[i].menu;
			}
		}

		return false;
	}

	public ToggleableIngameMenu GetMenuClass(string name)
	{
		if(menus == null)
		{
			SetupMenus();
		}
		for (int i = 0; i < menus.Count; i++)
		{
			if(menus[i].menuName == name)
			{
				return menus[i];
			}
		}
		return null;
	}

	public partial class ToggleableIngameMenu : Node
	{
		[Signal] public delegate void ChangedMenuEventHandler(bool on);
		public string menuName;
		public bool menu;
	}

}
