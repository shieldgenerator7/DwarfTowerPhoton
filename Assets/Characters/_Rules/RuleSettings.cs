using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RuleSettings
{
    public List<RuleSetting> settings;

    public RuleSetting Get(RuleSetting.Option setting)
        => settings.Find(s => s.setting == setting);
}
