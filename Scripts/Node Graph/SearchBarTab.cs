using Godot;
using System;

public partial class SearchBarTab : Button
{
	public void OnClick()
	{
		Control newNode = GD.Load<PackedScene>("res://Prefabs/Node Pieces/Basic Node.tscn").Instantiate() as Control;
		ulong objId = newNode.GetInstanceId();
		newNode.SetScript(GD.Load<Script>("res://Scripts/Node Graph/NodeWave.cs"));
		newNode = InstanceFromId(objId) as Control;
		newNode.GlobalPosition = GetGlobalMousePosition();
		GetTree().Root.AddChild(newNode);
	}
}
