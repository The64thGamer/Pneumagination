using Godot;
using System;

public partial class SearchBarTab : Button
{
	[Export] NodeType nodeType;
	[Export] Control parentNode;

	public enum NodeType
	{
		generator,
		compare,
		math,
		blend,
		bitwise,
	}
	public void OnClick()
	{
		Control newNode = GD.Load<PackedScene>("res://Prefabs/Node Pieces/Basic Node.tscn").Instantiate() as Control;
		ulong objId = newNode.GetInstanceId();
		switch (nodeType)
		{
			case NodeType.generator:
					newNode.SetScript(GD.Load<Script>("res://Scripts/Node Graph/NodeGenerator.cs"));
				break;
			case NodeType.compare:
					newNode.SetScript(GD.Load<Script>("res://Scripts/Node Graph/NodeCompare.cs"));
				break;
			case NodeType.math:
					newNode.SetScript(GD.Load<Script>("res://Scripts/Node Graph/NodeMath.cs"));
				break;
			case NodeType.blend:
					newNode.SetScript(GD.Load<Script>("res://Scripts/Node Graph/NodeBlend.cs"));
				break;
			case NodeType.bitwise:
					newNode.SetScript(GD.Load<Script>("res://Scripts/Node Graph/NodeBitwise.cs"));
				break;
			default:
			break;
		}
		newNode = InstanceFromId(objId) as Control;
		newNode.GlobalPosition = GetGlobalMousePosition();
		Random rng = new Random();
		(newNode as PneumagiNode).nodeID = rng.NextInt64();
		parentNode.GetParent().AddChild(newNode);
	}
}
