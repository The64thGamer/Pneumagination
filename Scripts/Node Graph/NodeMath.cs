using Godot;
using System;

public partial class NodeMath : PneumagiNode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Label name = FindChild("Node Title",true) as Label;
        if(name != null)
        {
            name.Text = "Math";
        }

		AddEnumTab(new string[]{"Add","Subtract","Multiply","Divide"});
		AddTab(InputOutputType.ioFloat, InputOutputType.ioFloat, "Output");
		AddTab(InputOutputType.ioFloat, InputOutputType.none, "Value",0.5f);
	}

	//ALWAYS SET OUTPUT FOR ALL OUTPUTS, INPUT DOESNT AUTO EQUAL OUTPUT
	public override void _Process(double delta)
	{		
		base._Process(delta);
		switch (GetOptionSelection(0))
		{
			case 0:
				SetOutputTabFloatValue(0,GetInputTabFloatValue(0) + GetInputTabFloatValue(1));
				break;
			case 1:
				SetOutputTabFloatValue(0,GetInputTabFloatValue(0) - GetInputTabFloatValue(1));
				break;
			case 2:
				SetOutputTabFloatValue(0,GetInputTabFloatValue(0) * GetInputTabFloatValue(1));
				break;
			case 3:
				if(GetInputTabFloatValue(1) != 0)
				{
					SetOutputTabFloatValue(0,GetInputTabFloatValue(0) / GetInputTabFloatValue(1));
				}
				else
				{
					SetOutputTabFloatValue(0,0);
				}
				break;
			default:
				break;
		}
		SetOutputTabFloatValue(1,GetInputTabFloatValue(1));
	}
}
