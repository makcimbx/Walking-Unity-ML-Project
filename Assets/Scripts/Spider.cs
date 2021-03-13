using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    
    internal Vector3 Position => rigidbody.position;
    internal Quaternion Rotation => rigidbody.rotation;
    
    [Space]
    [SerializeField] private List<SpiderLeg> spiderLegs;

    private Vector3 startPosition;
    
    internal List<SpiderLeg> SpiderLegs => spiderLegs;

    private void Awake()
    {
        startPosition = rigidbody.position;
    }
    
    internal void Reset()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        
        rigidbody.position = startPosition;
        
        foreach (var spiderLeg in spiderLegs)
        {
            spiderLeg.Reset();
        }
    }
}
