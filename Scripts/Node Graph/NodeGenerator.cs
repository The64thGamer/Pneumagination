using Godot;
using System;

public partial class NodeGenerator : PneumagiNode
{
	float currentRandomValue;
	Godot.RandomNumberGenerator rng;

	public override void _Ready()
	{
		base._Ready();
		rng = new RandomNumberGenerator();
		Label name = FindChild("Node Title",true) as Label;
        if(name != null)
        {
            name.Text = "Generator";
        }

		AddEnumTab(new string[]{"Sine","Sawtooth","Random"});
		AddTab(InputOutputType.none, InputOutputType.ioClampedFloat, "Output");
		AddTab(InputOutputType.ioFloat, InputOutputType.none, "Size",0.5f);
		AddTab(InputOutputType.ioFloat, InputOutputType.none, "Offset");
	}

	//ALWAYS SET OUTPUT FOR ALL OUTPUTS, INPUT DOESNT AUTO EQUAL OUTPUT
	public override void _Process(double delta)
	{		
		base._Process(delta);
		switch (GetOptionSelection(0))
		{
			case 0:
				SetOutputTabFloatValue(0,(Mathf.Sin((Time.GetTicksMsec() *.04f * GetOutputTabFloatValue(1))+(GetOutputTabFloatValue(2)*6.28f))+1)/2.0f);
				break;
			case 1:
				SetOutputTabFloatValue(0,Mathf.Lerp(0,1,((Time.GetTicksMsec() *.01f * GetOutputTabFloatValue(1))+GetOutputTabFloatValue(2))/2.0f)%1);
				break;
			case 2:
				currentRandomValue = Mathf.Lerp(currentRandomValue,rng.Randf(),(float)delta * 100 * (1 - GetInputTabFloatValue(1)));
				SetOutputTabFloatValue(0,currentRandomValue);
				break;
			default:
				break;
		}
		SetOutputTabFloatValue(1,GetInputTabFloatValue(1));
		SetOutputTabFloatValue(2,GetInputTabFloatValue(2));
	}
}
