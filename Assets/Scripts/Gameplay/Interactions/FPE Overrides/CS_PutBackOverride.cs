using NInteractionTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_PutBackOverride : Whilefun.FPEKit.FPEPutBackScript
{
    [SerializeField]
    List<EPickupObjectTypes> AllowedSocketedObjects = new List<EPickupObjectTypes>();

    /// <summary>
    /// Checks for a match between this put back location and the provided game object.
    /// </summary>
    /// <param name="go">The GameObject to test for a match against</param>
    /// <returns>True if there is a match, false if there is not.</returns>
    public override bool putBackMatchesGameObject(GameObject go)
    {
        CS_PickupType type = go.GetComponent<CS_PickupType>();
        if (type == null) 
        {
            return false;
        }

        return AllowedSocketedObjects.Contains(type.PickupType);
    }
}
