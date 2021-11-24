using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AbilityContext : MonoBehaviour
{
    public List<PlayerAbility> abilities = new List<PlayerAbility>();

    private void Start()
    {
        //Check to make sure all abilities have unique abilityIDs
        List<int> knownIDs = new List<int>();
        foreach(PlayerAbility pa in abilities)
        {
            if (knownIDs.Contains(pa.abilityID))
            {
                Debug.LogError(
                    "Player ability has duplicate abilityID! abilityID: " + pa.abilityID
                    + ", ability: " + pa.name,
                    pa.gameObject
                    );
            }
            knownIDs.Add(pa.abilityID);
        }
    }
}
