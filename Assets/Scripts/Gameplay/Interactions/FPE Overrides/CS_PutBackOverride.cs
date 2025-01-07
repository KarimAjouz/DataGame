using System.Collections.Generic;
using NInteractionTypes;
using UnityEngine;

namespace Gameplay.Interactions.FPE_Overrides
{
    public class CS_PutBackOverride : Whilefun.FPEKit.FPEPutBackScript
    {
        [SerializeField]
        private List<EPickupObjectTypes> m_AllowedSocketedObjects = new List<EPickupObjectTypes>();

        /// <summary>
        /// Checks for a match between this put back location and the provided game object.
        /// </summary>
        /// <param name="go">The GameObject to test for a match against</param>
        /// <returns>True if there is a match, false if there is not.</returns>
        public override bool putBackMatchesGameObject(GameObject go)
        {
            CS_PickupType type = go.GetComponent<CS_PickupType>();
            return type && m_AllowedSocketedObjects.Contains(type.PickupType);
        }
    }
}
