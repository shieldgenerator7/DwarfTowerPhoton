using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShotController), editorForChildClasses: true)]
[CanEditMultipleObjects]
public class ShotControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Convert to StatLayer"))
        {
            foreach (object target in targets)
            {
                convertToStatLayer((ShotController)target);
                if (target is ChargedShotController)
                {
                    convertToChargedStatLayer((ChargedShotController)target);
                }
                EditorUtility.SetDirty((ShotController)target);
            }
        }
    }

    private void convertToStatLayer(ShotController sc)
    {
        sc.statBase.moveSpeed = sc.speed;
        sc.statBase.damage = sc.initialDamage;
        sc.statBase.maxHits = sc.maxHealth;
        sc.statBase.fireRate = -1;
        sc.statBase.size = 1;
    }
    private void convertToChargedStatLayer(ChargedShotController csc)
    {
        csc.multiplierLayer.moveSpeed = convertChargedValue(csc.speedChargeMultiplier);
        csc.multiplierLayer.damage = convertChargedValue(csc.initialDamageChargeMultiplier);
        csc.multiplierLayer.maxHits = convertChargedValue(csc.maxHealthChargeMultiplier);
        csc.multiplierLayer.fireRate = -1;
        csc.multiplierLayer.size = 1;
    }

    private float convertChargedValue(float multiplier)
    {
        if (multiplier > 0)
        {
            return multiplier;
        }
        return -1;
    }

}
