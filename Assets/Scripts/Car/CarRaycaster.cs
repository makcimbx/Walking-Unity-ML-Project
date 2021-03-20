using UnityEngine;

public class CarRaycaster : MonoBehaviour
{
    [SerializeField] private float rayCastDistance;
    
    [Space]
    [SerializeField] private RayCastElement[] rayCastElements;

    private void Update()
    {
        foreach (var rayCast in rayCastElements)
        {
            var hit = Physics.Raycast(rayCast.StartPos, rayCast.Direction, out var rayCastHit, rayCastDistance);
            
            rayCast.UpdateHit(hit, rayCastHit.point);
        }
    }

    private void OnDrawGizmos()
    {
        if (rayCastElements == null) return;

        foreach (var rayCast in rayCastElements)
        {
            if (!rayCast.CanUse) continue;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(rayCast.StartPos, rayCast.StartPos + rayCast.Direction * rayCastDistance);

            if (rayCast.Hit)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(rayCast.HitPosition, 0.1f);
            }
        }

        Gizmos.color = Color.white;
    }

    [System.Serializable]
    public class RayCastElement
    {
        [SerializeField] private Transform root;
        //[SerializeField] private float distance;
        
        private bool hit;
        private Vector3 hitPosition;

        public bool CanUse => root != null;
        
        public Vector3 StartPos => root.position;
        public Vector3 Direction => root.forward;
        //public float Distance => distance;
        
        public bool Hit => hit;
        public Vector3 HitPosition => hitPosition;

        public void UpdateHit(bool hit, Vector3 hitPosition)
        {
            this.hit = hit;
            this.hitPosition = hitPosition;
        }
    }
}
