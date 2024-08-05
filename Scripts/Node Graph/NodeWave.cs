using Godot;
using System;

public partial class NodeWave : PneumagiNode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Label name = FindChild("Node Title",true) as Label;
        if(name != null)
        {
            name.Text = "Wave";
        }

		AddTab(InputOutputType.none, InputOutputType.ioClampedFloat, "Output");
		AddTab(InputOutputType.ioFloat, InputOutputType.none, "Size",0.5f);
		AddTab(InputOutputType.ioFloat, InputOutputType.none, "Offset");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{		
		base._Process(delta);
		SetOutputTabFloatValue(0,Mathf.Sin((Time.GetTicksMsec() *.04f * GetOutputTabFloatValue(1))+(GetOutputTabFloatValue(2)*4)));
	}
}
