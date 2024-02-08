using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class SetMoveRouteAction : EventAction
    {
        public FieldOriginType originType;
        public MoveRoute moveRoute = new MoveRoute();
        public MoveRouteHandler sceneMoveRouteHandler;
        public bool waitForCompletion = true;
        public SetMoveRouteAction()
        {
            eventName = "Set Move Route";
            eventColor = EventGUIColors.movement;
        }

        public override void Invoke()
        {
            var handler = sceneMoveRouteHandler;
            if (originType == FieldOriginType.FromPersistentInstance)
            {
                if (FollowerInstance.player == null) { isFinished = true; return; }
                if (FollowerInstance.player.moveRouteHandler == null) { isFinished = true; return; }
                handler = FollowerInstance.player.moveRouteHandler;
            }
            if (handler) handler.PlayMoveRoute(moveRoute, (waitForCompletion ? this : null));
            else isFinished = true;
            if (!waitForCompletion) isFinished = true;
        }
    }
}
