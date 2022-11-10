using Cinemachine;
using UnityEngine;

namespace Content.Scripts.Camera
{
    [ExecuteAlways]
    public class ArenaCameraSet : MonoBehaviour
    {
        [SerializeField] [Tooltip("Track start")] private Transform pointA;
        [SerializeField][Tooltip("Track end")] private Transform pointB;
        [SerializeField] private float offset = 15;

        private CinemachineVirtualCamera _cvm;
        private Transform _targetGroup;
        private Vector3 _lineDir;

        private void Start()
        {
            _cvm = GetComponent<CinemachineVirtualCamera>();
            _targetGroup = _cvm.LookAt;
            _lineDir = (pointB.position - pointA.position).normalized;
        }

        private void Update()
        {
            var positionA = pointA.position;
            var distanceVector = _targetGroup.position - positionA;
            var dot = Vector3.Dot(distanceVector, _lineDir) - offset;
            transform.position = positionA + _lineDir * dot;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}