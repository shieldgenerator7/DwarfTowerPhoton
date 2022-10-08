using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class RuleCheckerTool : MonoBehaviour
{
    public string folderToSearch = "";

    public List<Rule> rules;

    private List<string> errorList = new List<string>();
    public List<string> ErrorList => errorList;

    private static string BASE_FOLDER = "Assets/";

    public void findRules()
    {
        //2022-10-07: copied from https://stackoverflow.com/a/56168544/2336212
        rules.Clear();
        if (String.IsNullOrWhiteSpace(folderToSearch))
        {
            folderToSearch = BASE_FOLDER;
        }
        else if (!folderToSearch.StartsWith(BASE_FOLDER))
        {
            folderToSearch = $"{BASE_FOLDER}{folderToSearch}";
        }
        string[] assetNames = AssetDatabase.FindAssets(
            "",
            new string[] { folderToSearch }
            );
        foreach (string SOName in assetNames)
        {
            string SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            if (!SOpath.EndsWith(".asset"))
            {
                continue;
            }
            Rule rule = AssetDatabase.LoadAssetAtPath<Rule>(SOpath);
            if (rule)
            {
                rules.Add(rule);
            }
        }
    }

    public void checkRules()
    {
        errorList.Clear();

        rules.ForEach(rule => errorList.AddRange(RuleChecker.checkRule(rule)));

        if (errorList.Count == 0)
        {
            errorList.Add("Everything looks good");
        }
    }
}
#endif
