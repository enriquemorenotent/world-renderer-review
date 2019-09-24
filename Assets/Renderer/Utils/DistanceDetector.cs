using UnityEngine;

namespace Extinction.Utils
{
    public class DistanceDetector : MonoBehaviour
    {
        #region Attributes

        [SerializeField]
        GameObject trackingTarget;

        [SerializeField]
        [Range(10, 100)]
        public float tooFarLimit = 50;

        [SerializeField]
        string tagToTrack = "Player";

        #endregion

        void Update()
        {
            if (trackingTarget == null)
                FindTarget();
            else
                Vector3.Distance(transform.position, trackingTarget.transform.position);
        }

        #region Helper methods

        void FindTarget()
        {
            trackingTarget = GameObject.FindGameObjectWithTag(tagToTrack);
        }

        float GetDistance()
        {
            if (trackingTarget == null) return 0;

            Vector3 position = trackingTarget.transform.position;
            position.y = 0;
            return Vector3.Distance(transform.position, position);
        }

        #endregion

        #region Public methods

        public bool IsTargetTooFar()
        {
            return GetDistance() > tooFarLimit;
        }

        public Vector3 TargetPosition()
        {
            return trackingTarget.transform.position;
        }

        #endregion
    }
}