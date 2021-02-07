using AirBattle.Core;
using Photon.Pun;
using System;
using UnityEngine;

namespace AirBattle.JetControl
{
    public class JetShooting : MonoBehaviourPun, IPunObservable
    {
        public bool IsShooting {get; set; }
        public Vector3 AimPoint { get; set; }

        [Tooltip("the distance the jet is capable of shooting")]
        public float ShootingDistance;


        // Use this for initialization
        void Start()
        {
            IsShooting = false;
        }

        // Update is called once per frame
        void Update()
        {
            //IsShooting =((photonView.IsMine ||PhotonNetwork.OfflineMode) && Input.GetAxis("Fire1") >= 1);            
            if (photonView.IsMine || PhotonNetwork.OfflineMode)
            {
                IsShooting = (Input.GetAxis("Fire1") >= 1);
            }

            //it might have updated in the previous lines or in the synchonization with the other jet.
            if (IsShooting) 
            { 
                FindShootingObject();
            }
        }

        private void FindShootingObject()
        {
            Ray JetShotRay = Camera.main.ScreenPointToRay(GameManagement.Instance.MainScreenAim.transform.position);
            JetShotRay.origin = transform.position;
            Debug.DrawRay(JetShotRay.origin, JetShotRay.direction, Color.blue, 5);
            RaycastHit hit;
            if (Physics.Raycast(JetShotRay, out hit, ShootingDistance))
            {
                AimPoint = hit.point;
                Debug.Log("aiming on target:" + AimPoint);
            }
            else
            {
                AimPoint = JetShotRay.GetPoint(ShootingDistance);
                Debug.Log("the point to shoot:" + AimPoint);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(IsShooting);
                stream.SendNext(AimPoint);
            }
            else
            {
                IsShooting = (bool)stream.ReceiveNext();
                AimPoint = (Vector3) stream.ReceiveNext();
            }
        }
    }
}