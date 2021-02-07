using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AirBattle.JetControl
{
    public class LandTile : MonoBehaviour
    {

        public float MaxYSpeedAllowedToLAnd = 5f;

        public bool AllowedToLand(Vector3 velocity)
        {
            return (Mathf.Abs(velocity.y) <= MaxYSpeedAllowedToLAnd);
        }
    }
}
