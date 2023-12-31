using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Combo_Part;

public class Combo_Part : MonoBehaviour
{
    public string partName;
    public uint price;
    public List<BendablePart> bendableParts;
    public ComboTag partTag;
    public string connectingBone;
    public enum ComboTag
    {
        none,
        body,
        leftArm,
        rightArm,
        head,
    }

    public void SetBend(int index, float pos)
    {
        if(index > bendableParts.Count || index < 0)
        {
            return;
        }

        Transform t = RecursiveFindChild(transform, bendableParts[index].boneName);
        if(t != null)
        {
            Quaternion min = new Quaternion(bendableParts[index].minPosition.x, bendableParts[index].minPosition.y, bendableParts[index].minPosition.z, bendableParts[index].minPosition.w);
            Quaternion max = new Quaternion(bendableParts[index].maxPosition.x, bendableParts[index].maxPosition.y, bendableParts[index].maxPosition.z, bendableParts[index].maxPosition.w);
            t.localRotation = Quaternion.Lerp(min, max, pos);
        }
    }

    Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }
}
[System.Serializable]
public class BendablePart
{
    public string boneName;
    public Vector4 minPosition;
    public Vector4 maxPosition;
}

[System.Serializable]
public class Combo_Part_SaveFile
{
    public uint id;
    public List<int> actuatorDTUIndexes;
    [Range(0,1)]
    public List<float> bendableSections;
    public ComboTag tag;

    public override bool Equals(object obj)
    {
        return Equals(obj as Combo_Part_SaveFile);
    }
    public bool Equals(Combo_Part_SaveFile other)
    {
        if (other == null)
            return false;

        // Check for equality based on specific properties
        return id == other.id;
    }
    public override int GetHashCode()
    {
        // Generate a hash code based on the properties used in Equals method
        return HashCode.Combine(id);
    }
}
