using Godot;
using Console = media.Laura.SofiaConsole.Console;

public partial class MenuToggle : CanvasLayer
{
	Input.MouseModeEnum oldMouse;
	public static bool pauseMenuEnabled;

    void OnToggleMenu(bool menu)
    {
		if(GetTree().CurrentScene.Name == "Menu")
		{
			return;
		}
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
		pauseMenuEnabled = Visible;
    }

	void SetConsole(bool on)
	{
		Console.Instance.SetConsole(on);
	}

    public override void _Ready()
    {      
        (GetTree().CurrentScene as InputManager).GetMenuClass("Pause Screen").ChangedMenu += (on) => OnToggleMenu(on);
		(GetTree().CurrentScene as InputManager).GetMenuClass("Console").ChangedMenu += (on) => SetConsole(on);		
	}
}
