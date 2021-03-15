using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLegHelper : MonoBehaviour
{
    internal bool OnFloor { get; private set; }

    internal void Reset()
    {
        OnFloor = false;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Floor")
        {
            OnFloor = true;
        }
    }
}
