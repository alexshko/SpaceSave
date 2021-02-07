using UnityEngine;
namespace AirBattle.Core
{
    public class MyCameraScripts : MonoBehaviour
    {
        [Tooltip("constant delta between the camera and the object.\nif its ZERO then its calculated by the initial distance in the scene and will be constant through out the game.")]
        public Vector3 ConstInitDelta;
        private Vector3 InitDelta;
        public Transform ObjectToFollow;

        // Start is called before the first frame update
        void Start()
        {
            if (ConstInitDelta == Vector3.zero)
            {
                InitDelta = ObjectToFollow.position - transform.position;
            }
            else
            {
                InitDelta = ConstInitDelta;
            }
            Debug.Log("delta:" + InitDelta);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (ObjectToFollow != null)
            {
                Vector3 tmp = ObjectToFollow.position - InitDelta.magnitude * ObjectToFollow.forward;
                transform.position = tmp;
                transform.LookAt(ObjectToFollow);
                transform.position = transform.position - InitDelta.y * transform.up;
                Debug.Log("delta:" + InitDelta);
            }

        }
    }
}
