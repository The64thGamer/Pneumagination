using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PneumagiNode : Control
{
    //Savable Data
    public long nodeID;
    List<NodeTabSection> nodeTabs = new List<NodeTabSection>();

    //Locals
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
        
        MouseDragProcess();
    }

    void MouseDragProcess()
    {
        for (int i = 0; i < nodeTabs.Count; i++)
        {
            if(nodeTabs[i].isOutputDraggedByMouse)
            {
                if(!Input.IsActionPressed("Click"))
                {
                    nodeTabs[i].isOutputDraggedByMouse = false;
                    if(nodeTabs[i].outputLines.Count >= nodeTabs[i].outputNodes.Count)
                    {                    
                        int lineIndex = nodeTabs[i].outputLines.Count-1;
                        nodeTabs[i].outputLines[lineIndex].QueueFree();
                        nodeTabs[i].outputLines.RemoveAt(lineIndex);
                    }
                    return;
                }

                if(nodeTabs[i].outputLines.Count == nodeTabs[i].outputNodes.Count)
                {
                    Line2D newLine = new Line2D
                    {
                        Width = 10,
                        DefaultColor = new Color(1,1,1,1),
                        BeginCapMode = Line2D.LineCapMode.Round,
                        EndCapMode = Line2D.LineCapMode.Round,
                    };
                    nodeTabs[i].rightPoint.AddChild(newLine);
                    nodeTabs[i].outputLines.Add(newLine);
                    newLine.Position = Vector2.Zero;
                    newLine.Points = new Vector2[2]{new Vector2(10,10),GetGlobalMousePosition() - nodeTabs[i].outputLines[nodeTabs[i].outputLines.Count-1].GlobalPosition};
                }
                else
                {
                    int lineIndex = nodeTabs[i].outputLines.Count-1;
                    Vector2 linePos = nodeTabs[i].outputLines[lineIndex].GlobalPosition;
                    nodeTabs[i].outputLines[lineIndex].Points = new Vector2[2]{new Vector2(10,10),GetGlobalMousePosition() - linePos};
                }
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
            nodeTabs[index].floatOutputValue = value;
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
            return nodeTabs[index].floatOutputValue;
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

    public void MouseDraggingOutput(int index)
    {
        nodeTabs[index].isOutputDraggedByMouse = true;
    }


    public void AddTab(InputOutputType input, InputOutputType output, string name, float defaultValue = 0)
    {
        Control newNode = GD.Load<PackedScene>("res://Prefabs/Node Pieces/Node Tab.tscn").Instantiate() as Control;

        NodeTabSection nodetab = new NodeTabSection
        {
            tabName = newNode.FindChild("Tab Name", true) as Label,
            leftPoint = newNode.FindChild("Left Button", true) as TextureButton,
            rightPoint = newNode.FindChild("Right Button", true) as TextureButton,
            tabValue = newNode.FindChild("Progress Bar", true) as ProgressBar,
            nodeInputType = input,
            nodeOutputType = output,
            name = name,
            slider = newNode.FindChild("HSlider", true) as HSlider,
            outputLines = new Godot.Collections.Array<Line2D>(),
            outputNodes = new Godot.Collections.Array<PneumagiNode>(),
        };

        
        newNode.GlobalPosition = GetGlobalMousePosition();
        int e = nodeTabs.Count;
        nodetab.rightPoint.ButtonDown += () => MouseDraggingOutput(e);
        nodetab.slider.ValueChanged += (value) => SliderChangedValue(e, (float)value);
        nodetab.tabName.Text = name;
        nodetab.floatOutputValue = defaultValue;
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
    //Info
    public string name;
    public PneumagiNode.InputOutputType nodeInputType;
    public PneumagiNode.InputOutputType nodeOutputType;
    //State
    public float floatOutputValue;
    public float floatInputValue;
    public bool isOutputDraggedByMouse;
    //Connected Nodes
    public Godot.Collections.Array<PneumagiNode> outputNodes;
    public Godot.Collections.Array<Line2D> outputLines;
    public PneumagiNode inputNode;
    //Objects
    public Label tabName;
    public TextureButton leftPoint;
    public TextureButton rightPoint;
    public ProgressBar tabValue;
    public HSlider slider;
}