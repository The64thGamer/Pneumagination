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
        ioClampedFloat,
        ioAudio,
        ioVideo,
    }

	public override void _Ready()
	{
        draggableArea = FindChild("DraggableArea",true) as Area2D;
        draggableArea.MouseEntered += () => isDragging = true;
        draggableArea.MouseExited += () => isDragging = false;
        draggableAreaCollision = draggableArea.GetChild(0) as CollisionShape2D;
        RefreshDraggableSize();
	}

    public override void _Process(double delta)
	{
        if(isDragging || dragDoubleCheck)
        {
            if(Input.GetCurrentCursorShape() != Input.CursorShape.Drag)
            {
                dragDoubleCheck = false;
            }
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

    public void SetOutputTabFloatValue(int index, float value)
    {
        if(nodeTabs[index].nodeOutputType == InputOutputType.ioClampedFloat || 
        nodeTabs[index].nodeOutputType == InputOutputType.ioBool ||
        nodeTabs[index].nodeOutputType == InputOutputType.none ||
        nodeTabs[index].nodeOutputType == InputOutputType.ioFloat)
        {
            nodeTabs[index].floatValue = value;
            nodeTabs[index].tabValue.Value = value;
            return;
        }
        else
        {
            GD.PrintErr("Tried to set float to a non-float node output");
        }
    }

    public float GetOutputTabFloatValue(int index)
    {
        if(nodeTabs[index].nodeOutputType == InputOutputType.ioClampedFloat || 
        nodeTabs[index].nodeOutputType == InputOutputType.ioBool ||
        nodeTabs[index].nodeOutputType == InputOutputType.none ||
        nodeTabs[index].nodeOutputType == InputOutputType.ioFloat)
        {
            return nodeTabs[index].floatValue;
        }
        else
        {
            GD.PrintErr("Tried to get float from a non-float node output");
        }
        return 0;
    }
    
    public void SliderChangedValue(int index, float value)
    {
        //later on stop this if connected to input node
        SetOutputTabFloatValue(index,value);
    }


    public void AddTab(InputOutputType input, InputOutputType output, string name, float defaultValue = 0)
    {
        Control newNode = GD.Load<PackedScene>("res://Prefabs/Node Pieces/Node Tab.tscn").Instantiate() as Control;

        NodeTabSection nodetab = new NodeTabSection
        {
            tabName = newNode.FindChild("Tab Name", true) as Label,
            leftPoint = newNode.FindChild("Left Point", true) as TextureRect,
            rightPoint = newNode.FindChild("Right Point", true) as TextureRect,
            tabValue = newNode.FindChild("Progress Bar", true) as ProgressBar,
            nodeInputType = input,
            nodeOutputType = output,
            name = name,
            slider = newNode.FindChild("HSlider", true) as HSlider,
        };

        
        newNode.GlobalPosition = GetGlobalMousePosition();
        int e = nodeTabs.Count;
        nodetab.slider.ValueChanged += (value) => SliderChangedValue(e, (float)value);
        nodetab.tabName.Text = name;
        nodetab.floatValue = defaultValue;
        nodetab.tabValue.Value = defaultValue;
        switch (input)
        {
            case InputOutputType.ioBool:
                nodetab.leftPoint.Modulate = new Color("#ffcc00");
                break;
            case InputOutputType.ioFloat:
                nodetab.leftPoint.Modulate = new Color("#0066ff");
                break;
            case InputOutputType.ioClampedFloat:
                nodetab.leftPoint.Modulate = new Color("#ffffff");
                break;
            case InputOutputType.ioAudio:
                nodetab.leftPoint.Modulate = new Color("#ff6600");
                break;
            case InputOutputType.ioVideo:
                nodetab.leftPoint.Modulate = new Color("#6600ff");
                break;
            default:
                nodetab.leftPoint.Visible = false;
                break;
        }
         switch (output)
        {
            case InputOutputType.ioBool:
                nodetab.rightPoint.Modulate = new Color("#ffcc00");
                nodetab.tabValue.Modulate = new Color("#ffcc00");
                break;
            case InputOutputType.ioFloat:
                nodetab.rightPoint.Modulate = new Color("#0066ff");
                nodetab.tabValue.Modulate = new Color("#0066ff");
                break;
            case InputOutputType.ioClampedFloat:
                nodetab.rightPoint.Modulate = new Color("#ffffff");
                nodetab.tabValue.Modulate = new Color("#ffffff");
                break;
            case InputOutputType.ioAudio:
                nodetab.rightPoint.Modulate = new Color("#ff6600");
                nodetab.tabValue.Modulate = new Color("#ff6600");
                break;
            case InputOutputType.ioVideo:
                nodetab.rightPoint.Modulate = new Color("#6600ff");
                nodetab.tabValue.Modulate = new Color("#6600ff");
                break;
            default:
                nodetab.rightPoint.Visible = false;
                break;
        }
		
        FindChild("Node Tab Holder",true).AddChild(newNode);
        nodeTabs.Add(nodetab);
    }
}

public class NodeTabSection
{
    public string name;
    public PneumagiNode.InputOutputType nodeInputType;
    public PneumagiNode.InputOutputType nodeOutputType;
    public float floatValue;
    public Label tabName;
    public TextureRect leftPoint;
    public TextureRect rightPoint;
    public ProgressBar tabValue;
    public HSlider slider;
}