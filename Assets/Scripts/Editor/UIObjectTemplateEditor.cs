using UnityEngine;
using UnityEditor;
using System.Collections.Generic; // For Dictionary
using System.IO;
using System;

public class UIObjectTemplateEditor : EditorWindow
{
    string jsonString = "";
    private Dictionary<string, GameObject> instantiatedObjects = new Dictionary<string, GameObject>();
    private int selectedIndex = 0;
    private int parentIndex = 0;
    private List<string> objectNames;

    [MenuItem("Window/UI Object Template Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UIObjectTemplateEditor));
    }

    private UIObjectTemplate currentTemplate = new UIObjectTemplate();
    private UIObjectTemplateList currentTemplateList = new UIObjectTemplateList();

    private void OnGUI()
    {
        currentTemplate.name = EditorGUILayout.TextField("Name:", currentTemplate.name);
        currentTemplate.position = EditorGUILayout.Vector3Field("Position:", currentTemplate.position);
        currentTemplate.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation:", currentTemplate.rotation.eulerAngles));
        currentTemplate.scale = EditorGUILayout.Vector3Field("Scale:", currentTemplate.scale);
        currentTemplate.color = EditorGUILayout.ColorField("Color:", currentTemplate.color);

        if (GUILayout.Button("Load JSON"))
        {
            LoadJSON();
        }

        if (GUILayout.Button("Save JSON"))
        {
            SaveJSON();
        }

        if (GUILayout.Button("Save All Templates"))
        {
            SaveAllTemplates();
        }

        if (GUILayout.Button("Save Changes to JSON"))
        {
            SaveCurrentToJSON();
        }

        if (GUILayout.Button("Instantiate"))
        {
            InstantiateObject();
        }

        objectNames = new List<string>(instantiatedObjects.Keys);
        selectedIndex = EditorGUILayout.Popup("Select Object to Modify", selectedIndex, objectNames.ToArray());

        if (objectNames.Count > 0)
        {
            string selectedName = objectNames[selectedIndex];
            UIObjectTemplate selectedTemplate = currentTemplateList.templates.Find(t => t.name == selectedName);

            if (selectedTemplate != null)
            {
                string newName = EditorGUILayout.TextField("Modify Name:", selectedTemplate.name);
                selectedTemplate.position = EditorGUILayout.Vector3Field("Modify Position:", selectedTemplate.position);
                selectedTemplate.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Modify Rotation:", selectedTemplate.rotation.eulerAngles));
                selectedTemplate.scale = EditorGUILayout.Vector3Field("Modify Scale:", selectedTemplate.scale);
                selectedTemplate.color = EditorGUILayout.ColorField("Modify Color:", selectedTemplate.color);

                if (GUILayout.Button("Modify"))
                {
                    if (newName != selectedName)
                    {
                        GameObject go = instantiatedObjects[selectedName];
                        instantiatedObjects.Remove(selectedName);
                        go.name = newName;
                        instantiatedObjects[newName] = go;

                        selectedTemplate.name = newName;
                        int index = currentTemplateList.templates.FindIndex(t => t.name == selectedName);
                        currentTemplateList.templates[index].name = newName;
                    }

                    GameObject got = instantiatedObjects[newName];
                    got.transform.position = selectedTemplate.position;
                    got.transform.rotation = selectedTemplate.rotation;
                    got.transform.localScale = selectedTemplate.scale;

                    // Assuming the GameObject has a Renderer component to set color
                    if (got.GetComponent<Renderer>() != null)
                    {
                        got.GetComponent<Renderer>().material.color = selectedTemplate.color;
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("Template not found");
            }

            parentIndex = EditorGUILayout.Popup("Select Parent Object", parentIndex, objectNames.ToArray());

            if (GUILayout.Button("Add as Child"))
            {
                string parentName = objectNames[parentIndex];
                GameObject parentObject = instantiatedObjects[parentName];
                GameObject childObject = instantiatedObjects[selectedName];
                childObject.transform.SetParent(parentObject.transform);
            }
        }
    }

    private void LoadJSON()
    {
        string path = EditorUtility.OpenFilePanel("Select JSON file", "", "json");
        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                jsonString = File.ReadAllText(path);
                currentTemplateList = JSONHandler.DeserializeList(jsonString);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", "Failed to load JSON: " + e.Message, "OK");
            }
        }
    }

    private void SaveJSON()
    {
        string path = EditorUtility.SaveFilePanel("Save JSON file", "", "template", "json");
        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                File.WriteAllText(path, jsonString);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", "Failed to save JSON: " + e.Message, "OK");
            }
        }
    }

    private void SaveAllTemplates()
    {
        try
        {
            jsonString = JSONHandler.SerializeList(currentTemplateList);
            SaveJSON();
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Error", "Failed to save templates: " + e.Message, "OK");
        }
    }

    private void SaveCurrentToJSON()
    {
        try
        {
            Debug.Log("Before saving JSON: " + jsonString);

            // Find the template in the list with the same name as currentTemplate.
            int index = currentTemplateList.templates.FindIndex(t => t.name == currentTemplate.name);

            if (index != -1) // If the template is found
            {
                // Update the existing template in the list
                currentTemplateList.templates[index] = currentTemplate;
            }
            else
            {
                // Otherwise, add the current template to the list
                currentTemplateList.templates.Add(currentTemplate);
            }

            jsonString = JSONHandler.SerializeList(currentTemplateList);
            Debug.Log("After saving JSON: " + jsonString);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Error", "Failed to save JSON: " + e.Message, "OK");
        }
    }

    private void InstantiateObject()
    {
        try
        {
            foreach (UIObjectTemplate template in currentTemplateList.templates)
            {
                GameObject go = new GameObject(template.name);
                go.transform.position = template.position;
                go.transform.rotation = template.rotation;
                go.transform.localScale = template.scale;

                if (instantiatedObjects.ContainsKey(template.name))
                {
                    DestroyImmediate(instantiatedObjects[template.name]); // Destroy the old GameObject
                }

                instantiatedObjects[template.name] = go; // Update the dictionary
            }
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Error", "Failed to instantiate object: " + e.Message, "OK");
        }
    }
}
