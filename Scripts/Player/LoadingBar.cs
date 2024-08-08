using Godot;
using System;

public partial class LoadingBar : ProgressBar
{

    public override void _Ready()
    {    			
		//worldGen = GetTree().Root.GetNode("World") as WorldGen;
    }
	public override void _Process(double delta)
	{
		if(!PhotoMode.photoModeEnabled)
		{
			QueueFree();
		}

		Value = 1;//worldGen.GetChunksLoadedToLoadingRatio();
	}
}
