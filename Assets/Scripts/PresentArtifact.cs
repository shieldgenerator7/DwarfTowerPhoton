using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentArtifact : Artifact
{
    public Sprite shotSprite;

    public override void Activate(TeamTokenCaptain teamCaptain, bool activate = true)
    {
        base.Activate(teamCaptain, activate);
        //Change all shots to presents
        if (activate)
        {
            FindObjectsToEffect<ObjectSpawner>(teamCaptain)
                .ForEach(os => os.onObjectSpawned += ChangeToPresent);
        }
        else
        {
            FindObjectsToEffect<ObjectSpawner>(teamCaptain)
                .ForEach(os => os.onObjectSpawned -= ChangeToPresent);
        }
    }

    private void ChangeToPresent(GameObject shot, Vector2 pos, Vector2 dir)
    {
        //Only change the shots, not the constructs
        HealthPool healthPool = shot.FindComponent<HealthPool>();
        if (healthPool && healthPool.entityType == EntityType.SHOT)
        {
            shot.FindComponent<SpriteRenderer>().sprite = shotSprite;
        }
        //Propagate spawning presents even further
        ObjectSpawner os = shot.FindComponent<ObjectSpawner>();
        if (os)
        {
            os.onObjectSpawned += ChangeToPresent;
        }
    }
}
