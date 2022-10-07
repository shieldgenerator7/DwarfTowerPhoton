using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RuleChecker : MonoBehaviour
{
    public Rule rule;

    private List<string> errorList;
    public List<string> ErrorList => errorList;

    public void findRule()
    {
        //2022-10-07: copied from https://stackoverflow.com/a/56168544/2336212
        List<Rule> ruleList = new List<Rule>();

        string[] assetNames = AssetDatabase.FindAssets("");
        foreach (string SOName in assetNames)
        {
            string SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            Rule rule = AssetDatabase.LoadAssetAtPath<Rule>(SOpath);
            if (rule)
            {
                ruleList.Add(rule);
            }
        }
        this.rule = ruleList.FirstOrDefault();
    }

    public void checkRule()
    {
        errorList.Clear();
        Lines.ForEach(line => processLine(line));
        if (errorList.Count == 0)
        {
            errorList.Add("Everything looks good");
        }
        else
        {
            errorList.Insert(0, $"{errorList.Count} Errors Found:");
        }
    }

    private List<string> Lines
        => rule.ruleText.Trim().Split('\n').ToList()
            .ConvertAll(line => line.Trim());

    private void processLine(string line)
    {
        if (line.StartsWith("//"))
        {
            //it's a comment, don't process it
            return;
        }
        if (line.EndsWith(':'))
        {
            processTrigger(line);
        }
        else if (line.EndsWith('?'))
        {
            processCondition(line);
        }
        else
        {
            processAction(line);
        }
    }

    private void processTrigger(string line)
    {
        string triggerName = line.Split(':')[0];
        RuleTrigger trigger;
        if (!Enum.TryParse(triggerName, out trigger))
        {
            errorList.Add($"Unknown trigger: {triggerName}");
        }
    }

    private void processCondition(string line)
    {
        string conditionName = line.Split('?', ' ')[0];
        RuleCondition condition;
        if (!Enum.TryParse(conditionName, out condition))
        {
            errorList.Add($"Unknown condition: {conditionName}");
        }
    }

    private void processAction(string line)
    {
        string actionName = line.Split(' ')[0];
        RuleAction action;
        if (!Enum.TryParse(actionName, out action))
        {
            errorList.Add($"Unknown action: {actionName}");
        }
    }
}
