using Godot;
using System;
using media.Laura.SofiaConsole;
using Console = media.Laura.SofiaConsole.Console;

public partial class EnvironmentController : WorldEnvironment
{
    [Export] Gradient fogDepthColor;
    [Export] Gradient skyDepthColor;
    [Export] Gradient sunColor;
    [Export] Gradient skyColor;
    [Export] Gradient fogColor;
    [Export] Curve maxFogDistance;
    [Export] Curve minFogDistance;
    [Export] DirectionalLight3D sun;
    ShaderMaterial fogMat;
    
    ServerClient server;

    //Statics
    public static float timeOfDay;
    public static float lengthOfDay = 1500;

    //Locals
    float exactTimeOfDay = 400;
    float maxFogRange = (WorldGen.chunkLoadingDistance * WorldGen.chunkSize) - WorldGen.chunkSize;

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

        if(fogMat == null)
        {
            if(server.GetMainPlayer() == null)
            {
                return;
            }
            fogMat = (ShaderMaterial)(server.GetMainPlayer().GetNode("Player Head/Player Camera/Fog Mesh") as MeshInstance3D).MaterialOverride;
        }

        exactTimeOfDay = (exactTimeOfDay + (float)delta) % lengthOfDay;
        timeOfDay = exactTimeOfDay / lengthOfDay;
        sun.LightColor = sunColor.Sample(timeOfDay);
        Environment.FogLightColor = 
        Environment.BackgroundColor = skyColor.Sample(timeOfDay) * skyDepthColor.Sample(range);
        fogMat.SetShaderParameter("fog_color", fogColor.Sample(timeOfDay) * fogDepthColor.Sample(range));
        fogMat.SetShaderParameter("fogCenterWorldPos", server.GetMainPlayer().GlobalPosition);
        fogMat.SetShaderParameter("fogMaxRadius", maxFogRange * maxFogDistance.Sample(timeOfDay));
        fogMat.SetShaderParameter("fogMinRadius", maxFogRange * minFogDistance.Sample(timeOfDay));

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
