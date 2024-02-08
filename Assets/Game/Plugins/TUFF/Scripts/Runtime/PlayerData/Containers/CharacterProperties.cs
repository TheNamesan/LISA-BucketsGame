using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class CharacterProperties
    {
        public Vector3 playerPosition = new Vector3();
        public FaceDirections playerFacing = FaceDirections.East;
        public bool disableRun = false;
        public bool disableRopeJump = false;
    }
}

