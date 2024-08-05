using Godot;
using System;

public partial class NodeRound : PneumagiNode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Label name = FindChild("Node Title",true) as Label;
        if(name != null)
        {
            name.Text = "Round";
        }

		AddTab(InputOutputType.ioFloat, InputOutputType.ioBool, "Output");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{		
		base._Process(delta);
		SetOutputTabFloatValue(0,0);
	}
}
