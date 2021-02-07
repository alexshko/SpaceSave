using AirBattle.Core;
using Photon.Pun;

namespace AirBattle.JetControl
{
    public class JetMultiplayerRadarVisability : MonoBehaviourPun, IPunObservable
    {
        public bool Visable = true;

        private void Start()
        {

            if (PhotonNetwork.OfflineMode || !photonView.IsMine)
            {
                if (Visable)
                {
                    //used to be unvisable and now visiable
                    GameManagement.Instance.Targets.Add(transform);
                }
            }
        }

        private void SetVis(bool isVisable)
        {
            if (PhotonNetwork.OfflineMode || !photonView.IsMine)
            {
                if (!Visable && isVisable)
                {
                    //used to be unvisable and now visiable
                    GameManagement.Instance.Targets.Add(transform);
                }
                else if (Visable && !isVisable)
                {
                    //used to be visable and now unvisable.
                    GameManagement.Instance.Targets.Remove(transform);
                }
            }
            Visable = isVisable;
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(Visable);
            }
            else
            {
                SetVis((bool)stream.ReceiveNext());
            }
        }
    }
}