using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public enum MoveRouteInstruction
    {
        WaitForSeconds = 0,
        MoveHorizontal = 1,
        ChangeFacing = 2
    }
    [System.Serializable]
    public class MoveRoute
    {
        public List<MoveRouteElement> elements = new List<MoveRouteElement>();
    }
    [System.Serializable]
    public class MoveRouteElement
    {
        public MoveRouteInstruction instruction = MoveRouteInstruction.WaitForSeconds;
        public float duration = 0;
        public CharacterHorizontalDirection moveDirectionH = CharacterHorizontalDirection.Right;
        public FaceDirections facing = FaceDirections.East;

        public IEnumerator PlayElement(OverworldCharacterController controller)
        {
            if (controller == null) yield break;
            switch (instruction)
            {
                case MoveRouteInstruction.WaitForSeconds:
                    yield return new WaitForSeconds(duration);
                    break;
                case MoveRouteInstruction.MoveHorizontal:
                    controller.nextInput.horizontalInput = (moveDirectionH == CharacterHorizontalDirection.Right ? 1 : -1);
                    yield return new WaitForSeconds(duration);
                    controller.nextInput.horizontalInput = 0;
                    yield return new WaitForFixedUpdate();
                    yield return new WaitForFixedUpdate();
                    break;
                case MoveRouteInstruction.ChangeFacing:
                    controller.ChangeFaceDirection(facing);
                    break;
            }
            yield break;
        }
    }
}