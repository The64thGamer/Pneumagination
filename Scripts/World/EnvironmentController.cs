using Godot;
using System;
using media.Laura.SofiaConsole;
using Console = media.Laura.SofiaConsole.Console;

public partial class EnvironmentController : WorldEnvironment
{
    [Export] Gradient fogDepthColor;
    [Export] Gradient skyDepthColor;
    [Export] Gradient sunColor;
    [Export] Gradient ambientColor;
    [Export] Gradient fogColor;
    [Export] Curve maxFogDistance;
    [Export] Curve minFogDistance;
    [Export] DirectionalLight3D sun;
    
    ServerClient server;

    //Statics
    public static float timeOfDay;
    public static float lengthOfDay = 1500;

    //Locals
    float exactTimeOfDay = 800;
    const float maxFogRange =200;

    //Consts
    const float sdfgiMaxDistance = 1600;
    const float sdfgiMaxDistancePhotoMode = 2500;
    const float sdfgiProbeBias = 1.1f;

    const float sdfgiProbeBiasPhotoMode = 30;


    public override void _Ready()
    {
		server = GetTree().Root.GetNode("World/Server") as ServerClient;
    }
    public override void _Process(double delta)
    {		
        if(server.GetMainPlayer() == null)
		{
			return;
		}
        float range = GetClampedRange(-200, 0, server.GetMainPlayer().GlobalPosition.Y);
        exactTimeOfDay = (exactTimeOfDay + (float)delta) % lengthOfDay;
        timeOfDay = exactTimeOfDay / lengthOfDay;
        sun.LightColor = sunColor.Sample(timeOfDay);
        RenderingServer.GlobalShaderParameterSet("fog_color", fogColor.Sample(timeOfDay) * fogDepthColor.Sample(range));
        RenderingServer.GlobalShaderParameterSet("fogMaxRadius", maxFogRange * maxFogDistance.Sample(timeOfDay));
        RenderingServer.GlobalShaderParameterSet("fogMinRadius", maxFogRange * minFogDistance.Sample(timeOfDay));
        RenderingServer.GlobalShaderParameterSet("ambient", ambientColor.Sample(timeOfDay) * skyDepthColor.Sample(range));
    }

    float GetClampedRange(float lowerBound, float upperBound, float pos)
    {
        return Mathf.Clamp((pos - lowerBound) / (upperBound - lowerBound), 0, 1);
    }

    public void EnablePhotoMode()
    {
        Environment.SdfgiProbeBias = sdfgiProbeBiasPhotoMode;
        Environment.SdfgiMaxDistance = sdfgiMaxDistancePhotoMode;
    }

    public void DisablePhotoMode()
    {        
        Environment.SdfgiProbeBias = sdfgiProbeBias;
        Environment.SdfgiMaxDistance = sdfgiMaxDistance;
    }
	[ConsoleCommand("gettime", Description = "Prints the current in-game time.")]
	void GetTime()
	{
        if(!IsInsideTree())
		{
			Console.Instance.Print("Not currently in game");
			return;
		}
		Console.Instance.Print(DateTime.Today.Add(TimeSpan.FromDays(timeOfDay)).ToString("hhtt").TrimStart('0'));
	}

    [ConsoleCommand("getexacttime", Description = "Prints the exact in-game time of the day in seconds out of total.")]
	void GetExactTime()
	{
        if(!IsInsideTree())
		{
			Console.Instance.Print("Not currently in game");
			return;
		}
		Console.Instance.Print(exactTimeOfDay + " / " + lengthOfDay);
	}
}
