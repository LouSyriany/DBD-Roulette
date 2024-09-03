using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ConvertString : MonoBehaviour
{
    [System.Serializable]
    class Icons
    {
        public string WordRelated = "";
        [Range(0, 64)]public int spriteId = 0;
        public bool FilterOn = true;
    }

    [SerializeField] string[] ToYellow;
    [SerializeField] string[] ToGreen;
    [SerializeField] string[] ToPurple;
    [SerializeField] string[] ToOrange;
    [SerializeField] string[] ToRed;

    [SerializeField] Icons[] IconList;

    [TextArea(3, 20), SerializeField] string InputField;

    [TextArea(3, 20), SerializeField] string OutputField;

    [SerializeField] string[] boldWords = new string[]
    {
        "Increases",
        "Decreases",
        "Status Effect",
        "The Trapper",
        "Bear Traps",
    };

    [SerializeField] string[] paleYellowWords = new string[]
    {
        "Terror Radius",
        "Generators",
        "Breakable Walls",
        "Pallets",
        "Skill Check",
        "Survivors",
        "Bloodpoints",
        "Haste",
        "Deep Wound",
        "Haemorrhage",
        "Mangled",
        "Dying State",
        "Aura",
        "Injured State"
    };

    string paleYellow = "<#e2ce97>";
    string yellow     = "<#e8c252>";
    string green      = "<#199b1e>";
    string purple     = "<#ac3ee3>";
    string red        = "<#d41c1c>";
    string orange     = "<#ff8800>";

    //# e2ce97 -> pale Yellow
    //# e8c252 -> yellow
    //# 199b1e -> green
    //# ac3ee3 -> purple
    //# d41c1c -> red
    //# ff8800 -> orange


    [ContextMenu("Convert String")]
    void GetOutputField()
    {
        string tmp = InputField;

        string backspace = "\\" + "n";
        string tab = "\\" + "t";

        tmp = tmp.Replace(System.Environment.NewLine, backspace);
        tmp = tmp.Replace("    ", tab);

        string output = "";

        bool indentOn = false;
        bool quoteFound = false;

        for (int i = 0; i < tmp.Length; i++)
        {
            if (indentOn == false)
            {
                if (tmp[i] == '\\' && i + 1 < tmp.Length)
                {
                    if (tmp[i + 1] == 't')
                    {
                        indentOn = true;
                        output += "<indent=10%>- ";
                        i++;
                        continue;
                    }
                }
            }
            else
            {
                if (tmp[i] == '\\' && i + 1 < tmp.Length)
                {
                    if (tmp[i + 1] == 'n')
                    {
                        indentOn = false;
                        output += "</indent>";
                        i++;
                        continue;
                    }
                }
            }

            if (quoteFound == false)
            {
                if (tmp[i] == '\"')
                {
                    quoteFound = true;
                    output += paleYellow + "<i>";
                    output += tmp[i];
                    continue;
                }
            }
            else
            {
                if (tmp[i] == '\"')
                {
                    quoteFound = false;
                    output += tmp[i];
                    output += "</i>" + "</color>";
                    continue;
                }
            }

            output += tmp[i];
        }

        output = FilterWordsColor(output, ToYellow, yellow);
        output = FilterWordsColor(output, paleYellowWords, paleYellow);
        output = FilterWordsColor(output, ToGreen, green);
        output = FilterWordsColor(output, ToPurple, purple);
        output = FilterWordsColor(output, ToOrange, orange);
        output = FilterWordsColor(output, ToRed, red);

        foreach (string filter in boldWords)
        {
            if (output.Contains(filter))
            {
                output = output.Replace(filter, "<b>" + filter + "</b>");
            }
        }

        foreach (Icons icon in IconList)
        {
            if (icon.FilterOn)
            {
                if (output.Contains(icon.WordRelated))
                {
                    output = output.Replace(icon.WordRelated, icon.WordRelated + "<sprite=" + icon.spriteId.ToString() + ">");
                }
            }
        }

        OutputField = output;
    }

    string FilterWordsColor(string output, string[] filters, string color)
    {
        foreach (string filter in filters)
        {
            if (output.Contains(filter))
            {
                output = output.Replace(filter, color + filter + "</color>");
            }
        }

        if (output.Contains("<indent=<#e8c252>10</color>%>"))
        {
            output = output.Replace("<indent=<#e8c252>10</color>%>", "<indent=10%>");
        }

        return output;
    }
}
