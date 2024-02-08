using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class AdjustToCameraZPosition : MonoBehaviour
    {
        void Start()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        }
    }
}
