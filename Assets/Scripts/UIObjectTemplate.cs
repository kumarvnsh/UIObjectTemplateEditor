using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class UIObjectTemplate
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Color color;
    public List<UIObjectTemplate> children; // For nested templates


    


    public UIObjectTemplate()
    {
        name = "New Object";
        position = Vector3.zero;
        rotation = Quaternion.identity;
        scale = Vector3.one;
        color = Color.white;
        children = new List<UIObjectTemplate>();
    }
}

[Serializable]
public class UIObjectTemplateList
{
    public List<UIObjectTemplate> templates;

    public UIObjectTemplateList()
    {
        templates = new List<UIObjectTemplate>();
    }
}
