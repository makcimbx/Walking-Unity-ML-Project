using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    [SerializeField] private Rigidbody rigBody;
    
    internal bool OnFloor { get; private set; }
    internal Vector3 Position => rigBody.position;
    internal Quaternion Rotation => rigBody.rotation;
    internal Vector3 StartPosition => startPosition;
    
    [Space]
    [SerializeField] private List<SpiderLeg> spiderLegs;

    [Space]
    [SerializeField] private float targetUpsideDownAngle;

    private Vector3 startPosition;
    private Quaternion startQuaternion;
    
    internal List<SpiderLeg> SpiderLegs => spiderLegs;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Floor")
        {
            OnFloor = true;
        }
    }

    internal void Initialize()
    {
        startPosition = rigBody.transform.position;
        startQuaternion = rigBody.transform.rotation;

        spiderLegs.ForEach(item => item.Initialize());
    }

    internal bool CheckUpsideDown()
    {
        var sin = Mathf.Sin(Mathf.Deg2Rad * targetUpsideDownAngle);
        var upsideDownAngle = Vector3.Dot(transform.up, Vector3.down);
        return upsideDownAngle > sin;
    }

    internal void Reset()
    {
        rigBody.velocity = Vector3.zero;
        rigBody.angularVelocity = Vector3.zero;
        
        rigBody.transform.position = startPosition;
        rigBody.transform.rotation = startQuaternion;
        
        OnFloor = false;
        
        foreach (var spiderLeg in spiderLegs)
        {
            spiderLeg.Reset();
        }
    }
}
