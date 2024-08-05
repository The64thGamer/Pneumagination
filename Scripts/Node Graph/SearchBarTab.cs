using Godot;
using System;

public partial class SearchBarTab : Button
{
	[Export] NodeType nodeType;

	public enum NodeType
	{
		wave,
		round,
	}
	public void OnClick()
	{
		Control newNode = GD.Load<PackedScene>("res://Prefabs/Node Pieces/Basic Node.tscn").Instantiate() as Control;
		ulong objId = newNode.GetInstanceId();
		switch (nodeType)
		{
			case NodeType.round:
					newNode.SetScript(GD.Load<Script>("res://Scripts/Node Graph/NodeRound.cs"));
				break;
			case NodeType.wave:
					newNode.SetScript(GD.Load<Script>("res://Scripts/Node Graph/NodeWave.cs"));
				break;
			default:
			break;
		}
		newNode = InstanceFromId(objId) as Control;
		newNode.GlobalPosition = GetGlobalMousePosition();
		Random rng = new Random();
		(newNode as PneumagiNode).nodeID = rng.NextInt64();
		GetTree().Root.AddChild(newNode);
	}
}
