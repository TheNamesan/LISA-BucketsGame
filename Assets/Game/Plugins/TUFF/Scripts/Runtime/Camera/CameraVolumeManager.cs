using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TUFF
{
    public class CameraVolumeManager : MonoBehaviour
    {
        public Volume volume;
        public static VolumeProfile runtimeGlobalProfile;
        public void Awake()
        {
            if (volume == null) TryGetComponent(out volume);
            SetGlobalVolume();
        }
        public void SetGlobalVolume()
        {
            if (volume == null) return;
            if (runtimeGlobalProfile == null)
            {
                var copy = Instantiate(volume.profile);
                copy.name = "Runtime " + volume.profile.name;
                runtimeGlobalProfile = copy;
            }
            volume.profile = runtimeGlobalProfile;
        }
    }
}

