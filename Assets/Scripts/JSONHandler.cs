using UnityEngine;

public static class JSONHandler
{
    public static string SerializeList(UIObjectTemplateList objList)
    {
        return JsonUtility.ToJson(objList, true); // prettyPrint is set to true
    }


    public static UIObjectTemplateList DeserializeList(string json)
    {
        return JsonUtility.FromJson<UIObjectTemplateList>(json);
    }


}
