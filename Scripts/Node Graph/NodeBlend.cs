using Godot;
using System;

public partial class NodeBlend : PneumagiNode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Label name = FindChild("Node Title",true) as Label;
        if(name != null)
        {
            name.Text = "Blend";
        }

		AddTab(InputOutputType.none, InputOutputType.ioFloat, "Output");
		AddTab(InputOutputType.ioClampedFloat, InputOutputType.none, "Blend",0.5f);
		AddTab(InputOutputType.ioFloat, InputOutputType.none, "Value A",0.25f);
		AddTab(InputOutputType.ioFloat, InputOutputType.none, "Value B",0.75f);
	}

	//ALWAYS SET OUTPUT FOR ALL OUTPUTS, INPUT DOESNT AUTO EQUAL OUTPUT
	public override void _Process(double delta)
	{		
		base._Process(delta);
		SetOutputTabFloatValue(0,Mathf.Lerp(GetInputTabFloatValue(2),GetInputTabFloatValue(3),GetInputTabFloatValue(1)));
		SetOutputTabFloatValue(1,GetInputTabFloatValue(1));
		SetOutputTabFloatValue(2,GetInputTabFloatValue(2));
		SetOutputTabFloatValue(3,GetInputTabFloatValue(3));

	}
}
