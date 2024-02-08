using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TUFF
{
    [System.Serializable]
    public class ModifyGlobalVolumeAction : EventAction
    {
        public ColorAdjustmentsMods colorAdjustmentsMods = new ColorAdjustmentsMods();
        public VolumeProfile volumeProfile;
        public ModifyGlobalVolumeAction()
        {
            eventName = "Modify Global Volume";
            eventColor = new Color(1f, 1f, 0.7f, 1f);
            colorAdjustmentsMods.Instantiate();
        }
        public override void Invoke()
        {
            colorAdjustmentsMods.ApplyChanges(volumeProfile);
            isFinished = true;
        }
    }
}

