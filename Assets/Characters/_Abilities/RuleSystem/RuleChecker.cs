using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RuleChecker
{

    public static List<string> checkRule(Rule rule)
    {
        List<string> errorList = new List<string>();
        getLines(rule).ForEach(line => errorList.Add(processLine(line)));
        errorList.RemoveAll(err => String.IsNullOrWhiteSpace(err));
        if (errorList.Count > 0)
        {
            errorList.Insert(0, $"Rule {rule.name}: {errorList.Count} Errors Found:");
        }
        return errorList;
    }

    private static List<string> getLines(Rule rule)
        => rule.ruleText.Trim().Split('\n').ToList()
            .ConvertAll(line => line.Trim())
            .FindAll(line => !String.IsNullOrWhiteSpace(line));

    private static string processLine(string line)
    {
        if (line.StartsWith("//"))
        {
            //it's a comment, don't process it
            return "";
        }
        if (line.EndsWith(':'))
        {
            return processTrigger(line);
        }
        else if (line.EndsWith('?'))
        {
            return processCondition(line);
        }
        else
        {
            return processAction(line);
        }
    }

    private static string processTrigger(string line)
    {
        string triggerName = line.Split(':')[0];
        RuleTrigger trigger;
        if (!Enum.TryParse(triggerName, out trigger))
        {
            return $"Unknown trigger: {triggerName}";
        }
        return "";
    }

    private static string processCondition(string line)
    {
        string conditionName = line.Split('?', ' ')[0];
        RuleCondition condition;
        if (!Enum.TryParse(conditionName, out condition))
        {
            return $"Unknown condition: {conditionName}";
        }
        return "";
    }

    private static string processAction(string line)
    {
        string actionName = line.Split(' ')[0];
        RuleAction action;
        if (!Enum.TryParse(actionName, out action))
        {
            return $"Unknown action: {actionName}";
        }
        return "";
    }
}
