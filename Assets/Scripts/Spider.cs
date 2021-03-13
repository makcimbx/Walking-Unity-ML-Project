using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    [SerializeField] private Rigidbody rigBody;
    
    internal Vector3 Position => rigBody.position;
    internal Quaternion Rotation => rigBody.rotation;
    internal Vector3 StartPosition => startPosition;
    
    [Space]
    [SerializeField] private List<SpiderLeg> spiderLegs;

    private Vector3 startPosition;
    private Quaternion startQuaternion;
    
    internal List<SpiderLeg> SpiderLegs => spiderLegs;

    internal void Initialize()
    {
        startPosition = rigBody.transform.position;
        startQuaternion = rigBody.transform.rotation;

        spiderLegs.ForEach(item => item.Initialize());
    }
    
    internal void Reset()
    {
        rigBody.velocity = Vector3.zero;
        rigBody.angularVelocity = Vector3.zero;
        
        rigBody.transform.position = startPosition;
        rigBody.transform.rotation = startQuaternion;
        
        foreach (var spiderLeg in spiderLegs)
        {
            spiderLeg.Reset();
        }
    }
}
