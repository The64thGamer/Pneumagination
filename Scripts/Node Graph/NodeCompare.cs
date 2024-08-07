using Godot;
using System;

public partial class NodeCompare : PneumagiNode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Label name = FindChild("Node Title",true) as Label;
        if(name != null)
        {
            name.Text = "Compare";
        }

		AddEnumTab(new string[]{"Greater Than","Lesser Than","Equal To"});
		AddTab(InputOutputType.none, InputOutputType.ioBool, "Output");
		AddTab(InputOutputType.ioFloat, InputOutputType.none, "Value A",0.5f);
		AddTab(InputOutputType.ioFloat, InputOutputType.none, "Value B",0.5f);
	}

	//ALWAYS SET OUTPUT FOR ALL OUTPUTS, INPUT DOESNT AUTO EQUAL OUTPUT
	public override void _Process(double delta)
	{		
		base._Process(delta);
		switch (GetOptionSelection(0))
		{
			case 0:
				SetOutputTabFloatValue(0,GetInputTabFloatValue(1) >= GetInputTabFloatValue(2) ? 1 : 0);
				break;
			case 1:
				SetOutputTabFloatValue(0,GetInputTabFloatValue(1) <= GetInputTabFloatValue(2) ? 1 : 0);
				break;
			case 2:
				SetOutputTabFloatValue(0,GetInputTabFloatValue(1).Equals(GetInputTabFloatValue(2)) ? 1 : 0);
				break;
			default:
				break;
		}
		SetOutputTabFloatValue(1,GetInputTabFloatValue(1));
		SetOutputTabFloatValue(2,GetInputTabFloatValue(2));
	}
}
