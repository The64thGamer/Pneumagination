using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PneumagiNode : Control
{
    List<NodeTabSection> nodeTabs = new List<NodeTabSection>();
    Area2D draggableArea;
    CollisionShape2D draggableAreaCollision;

    bool isDragging = false;
    bool dragDoubleCheck = false;
    Vector2 dragOffset;

    public enum InputOutputType
    {
        none,
        ioFloat,
        ioBool,
        ioClampedBool,
        ioAudio,
    }

	public override void _Ready()
	{
        draggableArea = FindChild("DraggableArea",true) as Area2D;
        draggableArea.MouseEntered += () => isDragging = true;
        draggableArea.MouseExited += () => isDragging = false;
        draggableAreaCollision = draggableArea.GetChild(0) as CollisionShape2D;
		Label name = FindChild("Node Title",true) as Label;
        if(name != null)
        {
            name.Text = Name;
        }
        RefreshDraggableSize();
	}

    public override void _Process(double delta)
	{
        if(isDragging || dragDoubleCheck)
        {
            if(Input.IsActionJustPressed("Click"))
            {
                dragOffset = GetGlobalMousePosition() - GlobalPosition;
                dragDoubleCheck = true;
            }
            if(Input.IsActionPressed("Click") && dragDoubleCheck)
            {
                GlobalPosition = GetGlobalMousePosition() - dragOffset;
            }
            if(!Input.IsActionPressed("Click"))
            {
                dragDoubleCheck = false;
            }
        }
        
    }

    void RefreshDraggableSize()
    {
        RectangleShape2D shape = new RectangleShape2D();
        shape.Size = GetGlobalRect().Size;
        draggableAreaCollision.Shape = shape;
        draggableAreaCollision.Position = GetGlobalRect().Size * .5f;
    }

    public void AddTab(InputOutputType input, InputOutputType output, string name)
    {
        Control newNode = GD.Load<PackedScene>("res://Prefabs/Node Pieces/Node Tab.tscn").Instantiate() as Control;
		newNode.GlobalPosition = GetGlobalMousePosition();
		FindChild("Node Tab Holder",true).AddChild(newNode);
        (newNode.FindChild("Tab Name",true) as Label).Text = name;
    }
}

public class NodeTabSection
{
    public Label tabName;
    public Texture2D leftPoint;
    public Texture2D rightPoint;
    public ProgressBar tabValue;
}