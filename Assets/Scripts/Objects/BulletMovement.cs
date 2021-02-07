using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AirBattle.Objects.Bullets
{
    [RequireComponent(typeof(BulletStats))]
    public class BulletMovement : MonoBehaviour
    {
        public float ForceToShoot;
        public float InitialSpeedToShoot { get; set; }
        public Vector3 ShootTarget { set; get; }

        // Start is called before the first frame update
        void Start()
        {
            ShootTarget = Vector3.zero;
            InitialSpeedToShoot = 0;
        }

        public void Shoot()
        {
            Vector3 direction;
            if (ShootTarget != Vector3.zero)
            {
                //Debug.Log("Target: " + ShootTarget);
                direction = ShootTarget - transform.position;
                direction = direction.normalized;
            }
            else
            {
                direction = transform.forward;
            }
            //GetComponent<Rigidbody>().velocity = direction * speed;
            GetComponent<Rigidbody>().velocity = direction * InitialSpeedToShoot;
            GetComponent<Rigidbody>().AddForce(direction * ForceToShoot, ForceMode.Acceleration);
            //Debug.Log("init shot:" + direction * speed + ". speed: " + GetComponent<Rigidbody>().velocity);
            //Debug.Log("Shoot: " + GetComponent<Rigidbody>().velocity);
        }

        // Update is called once per frame
        void Update()
        {
            //transform.Translate(new Vector3(0,0,1) * speed * Time.deltaTime);
            //Debug.Log("forward:" + transform.forward);
            //Debug.Log("speed y: " + GetComponent<Rigidbody>().velocity.y);
            //Debug.Log("speed:" + GetComponent<Rigidbody>().velocity);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("bullet collided");
            BulletDestroy();
        }

        private void BulletDestroy()
        {
            //play some animation:
            Destroy(gameObject);
        }
    }
}
