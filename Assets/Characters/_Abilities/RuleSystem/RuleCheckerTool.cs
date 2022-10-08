using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class RuleCheckerTool : MonoBehaviour
{
    public List<Rule> rules;

    private List<string> errorList = new List<string>();
    public List<string> ErrorList => errorList;

    public void findRules()
    {
        //2022-10-07: copied from https://stackoverflow.com/a/56168544/2336212
        rules.Clear();

        string[] assetNames = AssetDatabase.FindAssets("");
        foreach (string SOName in assetNames)
        {
            string SOpath = AssetDatabase.GUIDToAssetPath(SOName);
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
