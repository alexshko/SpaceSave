using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

namespace AirBattle.UI.Menu 
{
    public class MenuMultiplayerUI : MonoBehaviourPunCallbacks
    {
        public static float GameVersion = 1f;
        public static string LoginNamePref = "loginname_pref";

        public GameObject MenuInitiaLogin;
        public GameObject MenuCreateRoom;
        public GameObject MenuJoinRoom;
        public Button btnJoinRoom;

        private bool showCreateScreen;
        
        public GameObject LoginNameOBJ;
        public Slider NumOfPlayersSlider;


        #region attributes of rooms and Multiplayer

        private string LoginName;
        //for create:
        private bool AllowedToJoinRandom = false;
        private string Password = "";
        private int NumOfPlayers = 2;
        //for join:
        private string InviteCode;

        #endregion

        private void Awake()
        {
            InviteCode = "";

            if (!showCreateScreen)
            {
                btnJoinRoom.Select();
            }
            showRoomLoby();
        }

        #region functions called from UI:
        public void CreateGame()
        {
            NumOfPlayers = (int)NumOfPlayersSlider.value;

            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = (byte)NumOfPlayers;
            ro.IsVisible = AllowedToJoinRandom;

            PhotonNetwork.CreateRoom(Password,ro);
        }

        public void setCreateScreenVal(bool val)
        {
            showCreateScreen = val;
            showRoomLoby();
        }

        public void setAllowedJoinRandom(bool allowed)
        {
            AllowedToJoinRandom = allowed;
        }

        public void setInviteCode(string inv)
        {
            InviteCode = inv;
            if (inv.Length > 0)
            {
                btnJoinRoom.GetComponentInChildren<TMP_Text>().text = "Join By Invite";
            }
            else
            {
                btnJoinRoom.GetComponentInChildren<TMP_Text>().text = "Join Random Room";
            }
        }

        public void setPassword(string pass)
        {
            Password = pass;
        }

        public void JoinRoom()
        {
            if (InviteCode.Length > 0)
            {
                PhotonNetwork.JoinRoom(InviteCode);
            }
            else
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public void Connect()
        {
            LoginName = LoginNameOBJ.GetComponent<TMP_InputField>().text;
            SaveLoginInPrefs();

            //connect to the server with the login:
            PhotonNetwork.GameVersion = MenuMultiplayerUI.GameVersion.ToString();
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = LoginName;     
            if (!(PhotonNetwork.ConnectUsingSettings()))
            {
                Debug.LogFormat("Failed to Connect to server");
            }

        }
        #endregion

        private void SaveLoginInPrefs()
        {
            if (LoginName.Length > 0 && LoginName != null)
            {
                PlayerPrefs.SetString(LoginNamePref, LoginName);
            }
        }

        private void showRoomLoby()
        {
            if (MenuInitiaLogin != null)
            {
                MenuInitiaLogin.SetActive(!PhotonNetwork.IsConnected);
            }
            if (MenuJoinRoom != null)
            {
                MenuJoinRoom.SetActive(PhotonNetwork.IsConnected && !showCreateScreen);
            }
            if (MenuCreateRoom != null)
            {
                MenuCreateRoom.SetActive(PhotonNetwork.IsConnected && showCreateScreen);
            }
        }

        #region PUN callbacks

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            showRoomLoby();
            Debug.LogFormat("Connected to server");
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.LogFormat("Room created successfuly");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.LogFormat("Room Joined. Joined As master client: {0}", PhotonNetwork.IsMasterClient);
            PhotonNetwork.LoadLevel(2);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.LogFormat("room creation failed: {0}. ", returnCode);
            Debug.LogFormat(message);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            Debug.LogFormat("failed to join a random room: {0}. ", returnCode);
            Debug.Log(message);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.LogFormat("failed to join a room: {0}. ", returnCode);
            Debug.Log(message);
        }

        #endregion
    }
}
