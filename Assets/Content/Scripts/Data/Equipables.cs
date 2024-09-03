using System.Text;
using UnityEngine;

public class Equipables : ScriptableObject
{
    public Sprite Icon;
    public string Name = "";
    [TextArea(3, 15)] public string Descriton;

    void OnValidate()
    {
        if (Name == "")
        {
            if (Icon)
            {
                Name = FormatName(Icon.name.Split("_")[1]);
            }
        }
    }

    static public string FormatName(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }
}
