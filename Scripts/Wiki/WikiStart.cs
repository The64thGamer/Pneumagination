using Godot;
using System;

public partial class WikiStart : CanvasLayer
{
	bool canInput = true;
	public static bool wikiEnabled;

	Input.MouseModeEnum oldMouse;
	// Called when the node enters the scene tree for the first time.
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
		wikiEnabled = Visible;
    }

    public override void _Ready()
    {      
		Visible = false;
        (GetTree().CurrentScene as InputManager).GetMenuClass("Wiki").ChangedMenu += (on) => OnToggleMenu(on);
	}
}
