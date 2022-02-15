using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct RuleSettings
{
    public List<RuleSetting> settings;

    public RuleSet targetRuleSet;

    public RuleSetting Get(RuleSetting.Option setting)
        => settings.Find(s => s.setting == setting);

    public RuleSetting? Try(RuleSetting.Option setting)
    {
        if (settings.Any(s => s.setting == setting))
        {
            return settings.Find(s => s.setting == setting);
        }
        else
        {
            return null;
        }
    }
}
