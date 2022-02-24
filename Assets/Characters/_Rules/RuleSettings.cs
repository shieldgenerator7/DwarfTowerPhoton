using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct RuleSettings
{
    public RuleSet targetRuleSet;
    //public List<ObjectSpawnInfo> objectsToSpawn;
    public StatLayer statLayer;
    public StatusLayer statusLayer;

    public List<RuleSetting> settings;

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

    public int AbilityID(int viewID)
        => viewID * 10
        + (Try(RuleSetting.Option.ABILITY_ID) ?? 0);
}
