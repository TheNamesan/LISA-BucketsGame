using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TUFF
{
    public class ModifyGlobalVolumeEvent : EventCommand
    {
        public ColorAdjustmentsMods colorAdjustmentsMods = new ColorAdjustmentsMods();
        public VolumeProfile volumeProfile;
        public override void Invoke()
        {
            colorAdjustmentsMods.ApplyChanges(volumeProfile);
            isFinished = true;
        }
        public override void OnInstantiate()
        {
            eventName = "Modify Global Volume";
            eventColor = new Color(1f, 1f, 0.7f, 1f);
            volumeProfile = CreateInstance<VolumeProfile>();
            colorAdjustmentsMods.Instantiate();
        }
    }
}

