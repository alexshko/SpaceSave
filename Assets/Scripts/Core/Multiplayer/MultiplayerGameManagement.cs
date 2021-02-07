using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

namespace AirBattle.Core.Multiplayer
{
    [RequireComponent(typeof(GameManagement))]
    public class MultiplayerGameManagement : MonoBehaviourPunCallbacks
    {

        public GameObject JetPrefab;
        public Transform PositionPlayer1;
        public Transform PositionPlayer2;


        private int numOfPlayers = 2;

        private void Start()
        {
            if (!GameManagement.Instance.LocalJetInstance)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    GameManagement.Instance.LocalJetInstance = PhotonNetwork.Instantiate(JetPrefab.name, PositionPlayer1.position, PositionPlayer1.rotation, 0);
                }
                else
                {
                    GameManagement.Instance.LocalJetInstance = PhotonNetwork.Instantiate(JetPrefab.name, PositionPlayer2.position, PositionPlayer2.rotation, 0);
                }
                SetCameraFollow();

                //if its the current player's jet then deactivate its health bar.
                GameManagement.Instance.LocalJetInstance.GetComponentInChildren<HealthbarManager>().gameObject.SetActive(false);
            }
        }


        private void SetCameraFollow()
        {
            MyCameraScripts cam = GetComponentInChildren<MyCameraScripts>();
            if (cam == null)
            {
                Debug.Log("No camera found");
                return;
            }
            cam.ObjectToFollow = GameManagement.Instance.LocalJetInstance?.GetComponent<Transform>();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Debug.LogFormat("player {0} joined the room", newPlayer.NickName);
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.LogFormat("player {0} left the room", otherPlayer.NickName);
        }
    }
}
