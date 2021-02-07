using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AirBattle.Core
{
    public class HealthbarManager : MonoBehaviour
    {
        private Transform cam;
        private Slider sl;
        public float CurLife { set; get; }

        public float TimeToUpdate;
        private IEnumerator corotineUpdate = null;
        private float lifeToUpdate;

        private Vector3 initPosDelta;
        [Tooltip("Which object the Health bar should follow")]
        public Transform FollowObject;

        private void Awake()
        {
            //if its not canvas then Log an error.
            if (gameObject.GetComponent<Canvas>() == null)
            {
                Debug.LogError("the HealthbarManager is not on a Canvas");
            }

            cam = Camera.main.transform;
            sl = gameObject.GetComponentInChildren<Slider>();
            if (sl == null)
            {
                Debug.LogError("the HealthbarManager deosn't have a Slider child");
            }
            initPosDelta = transform.position - FollowObject.position;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Debug.Log("look at camera: " + (cam.position + cam.forward));
            transform.position = FollowObject.position + initPosDelta;
            transform.LookAt(cam.position + cam.forward);
        }

        public void UpdateLife(float new_life)
        {
            lifeToUpdate = new_life;

            //if (corotineUpdate != null)
            //{
            //    StopCoroutine(corotineUpdate);
            //}
            corotineUpdate = UpdateLifeToValue();
            StartCoroutine(corotineUpdate);
        }

        private IEnumerator UpdateLifeToValue()
        {
            float elpasedTime = 0;
            while (elpasedTime < TimeToUpdate)
            {
                elpasedTime += Time.deltaTime;
                sl.value = Mathf.Lerp(CurLife, lifeToUpdate, elpasedTime / TimeToUpdate);
                yield return null;
            }
            sl.value = lifeToUpdate;
            CurLife = lifeToUpdate;
            Debug.Log("coroutine:");
            yield return null;
        }


    }
}
