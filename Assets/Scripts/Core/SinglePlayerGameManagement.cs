using AirBattle.Core.UI;
using Photon.Pun;
using UnityEngine;

namespace AirBattle.Core
{
    [RequireComponent(typeof(GameManagement))]
    public class SinglePlayerGameManagement : MonoBehaviour
    {

        //for initialization of the game from Menu:
        public static string GameType = "";
        public static int NumOfTargets = -1;

        public Transform TargetPrefab;
        public Transform TargetsGroupingObject;


        private Transform ExplosionEffectPrefab;
        private float TerrainLength = 500;
        private float WorldHeight = 150;

        //next function is delegate:
        //public void MakeTargetExplodeAffect(Transform t)
        //{
        //    //make explotion effect:
        //    Transform expl = Instantiate(ExplosionEffectPrefab, t.transform.position, Quaternion.identity);
        //    Destroy(expl.gameObject, 10);
        //}

        private void Start()
        {
            PhotonNetwork.OfflineMode = true;

            ExplosionEffectPrefab = GetComponent<GameManagement>().ExplosionEffectPrefab;
            TerrainLength = GetComponent<GameManagement>().TerrainLength;
            WorldHeight = GetComponent<GameManagement>().WorldHeight;

            GameManagement.Instance.LocalJetInstance = GameObject.FindGameObjectWithTag("Player");
            //deactivate the healthbar on the single jet:
            GameManagement.Instance.LocalJetInstance.GetComponentInChildren<HealthbarManager>().gameObject.SetActive(false);

            //if the type of game is KillSpree training, then we should set the targets properly first.
            if (GameType == "KillSpree" && NumOfTargets > 0)
            {
                InitGameSpreeKill(NumOfTargets);
            }
        }

        private void InitGameSpreeKill(int NumOfTargets)
        {
            //GameManagement.Instance.Targets = new Transform[NumOfTargets];
            for (int i = 0; i < NumOfTargets; i++)
            {
                float x1 = Random.Range(-TerrainLength, TerrainLength);
                float y1 = Random.Range(20, TerrainLength);
                float z1 = Random.Range(-TerrainLength, TerrainLength);
                Vector3 pos = GameManagement.Instance.LocalJetInstance.transform.position + new Vector3(x1, y1, z1);
                Transform target = Instantiate(TargetPrefab, pos, Quaternion.identity, TargetsGroupingObject);
                //add delegate for destruction to the target:
                //target.GetComponent<Health>().OnObjectDestroy += GameManagement.Instance.MakeTargetExplodeAffect;
                GameManagement.Instance.Targets.Add(target);
            }

            //Vector3 pos2 = GameManagement.Instance.LocalJetInstance.transform.position + new Vector3(200, 10, 10);
            //Transform target2 = Instantiate(TargetPrefab, pos2, Quaternion.identity, TargetsGroupingObject);
            //target2.GetComponent<Health>().OnObjectDestroy += MakeTargetExplodeAffect;
            //GameManagement.Instance.Targets.Add(target2);
        }
    }
}