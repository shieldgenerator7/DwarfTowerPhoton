using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuleChecker : MonoBehaviour
{
    public Rule rule;

    private List<string> errorList;
    public List<string> ErrorList => errorList;

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
        if (line.EndsWith(':'))
        {
            processTrigger(line);
        }
        if (line.EndsWith('?'))
        {
            processCondition(line);
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
}
