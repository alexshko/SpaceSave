using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AirBattle.Objects.Bullets;
using AirBattle.Core;

namespace AirBattle.JetControl
{

    //[RequireComponent(typeof(JetMovement))] 
    public class ShootingMechanism : MonoBehaviour
    {
        public GameObject ShotPrefab;
        //public GameObject AimPrefab;

        //number of shots in second:
        public float FireRate;
        public float ShotDelay; //used for sync between shooting canons.
        public float ShotsLife;
        public float ShootingDistance;

        private float TimeSinceLastShot;
        private float TimeSinceStartedShooting;

        private JetMovement jet;
        // Start is called before the first frame update
        void Start()
        {
            TimeSinceLastShot = 0;
            TimeSinceStartedShooting = 0;

            //JetShooting can be on the jet object or as child of jet:
            if (!TryGetComponent<JetMovement>(out jet))
            {
                jet = GetComponentInParent<JetMovement>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (jet.GetComponent<JetShooting>().IsShooting)
            {
                if ((TimeSinceLastShot >= 1 / FireRate) && (TimeSinceStartedShooting > ShotDelay))
                {
                    //don't shoot if he's dead.
                    if (jet != null && !jet.isDead)
                    {
                        Debug.Log("fire");
                        Shoot();
                        TimeSinceLastShot = 0;
                    }
                }

                //count the time since started shooting:
                TimeSinceStartedShooting += Time.deltaTime;
            }
            else
            {
                //stopped pressing the fire button, so initialize the time since started shooting count.
                TimeSinceStartedShooting = 0;
            }
            TimeSinceLastShot += Time.deltaTime;
        }

        private void Shoot()
        {
            //Ray JetShotRay = Camera.main.ScreenPointToRay(GameManagement.Instance.Aim.transform.position);
            //JetShotRay.origin = transform.position;
            //Debug.DrawRay(JetShotRay.origin, JetShotRay.direction, Color.blue, 5);
            //RaycastHit hit;
            //Vector3 aimPoint;
            //if (Physics.Raycast(JetShotRay, out hit, ShootingDistance))
            //{
            //    aimPoint = hit.point;
            //    Debug.Log("aiming on target:" + aimPoint);
            //}
            //else
            //{
            //    aimPoint = JetShotRay.GetPoint(ShootingDistance);
            //    Debug.Log("the point to shoot:" + aimPoint);
            //}
            //add here.
            GameObject shot = Instantiate(ShotPrefab, transform.position, transform.rotation, jet.transform);
            shot.GetComponent<BulletMovement>().ShootTarget = jet.GetComponent<JetShooting>().AimPoint;
            shot.GetComponent<BulletMovement>().InitialSpeedToShoot += jet.SpeedWithGravity.magnitude;
            shot.GetComponent<BulletMovement>().Shoot();
            Destroy(shot, ShotsLife);
        }
    }
}
