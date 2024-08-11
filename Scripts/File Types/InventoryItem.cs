using Godot;
using System;

public partial class InventoryItem : Node
{
	[Export] public string name;
	[Export] public int count;
	[Export] public string[] tags;
}
