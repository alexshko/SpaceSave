using AirBattle.JetControl;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace AirBattle.Core
{
    public class GameManagement : MonoBehaviour
    {
        //singleton:
        public static GameManagement Instance;
        
        public GameObject LocalJetInstance { get; set; }
        //public GameObject AltitudeMeter;
        public GameObject MainScreenHealthBar;
        public GameObject MainScreenAim;

        public Transform ExplosionEffectPrefab;
        public float TerrainLength = 2500;
        public float WorldHeight = 150;

        public ObservableCollection<Transform> Targets { get; set; }

        //next function is delegate:
        public void MakeTargetExplodeAffect(Transform t)
        {
            //make explotion effect:
            Transform expl = Instantiate(ExplosionEffectPrefab, t.transform.position, Quaternion.identity);
            Destroy(expl.gameObject, 10);
        }


        //next function is delegate:
        public void OnJetExplodeFunc()
        {
            Debug.Log("Jet Exploded");
        }

        public void OnJetChangeAltitude(float CurAltitude)
        {
        }
        private void Awake()
        {
            Instance = this;
            Targets = new ObservableCollection<Transform>();
        }

        // Start is called before the first frame update
        void Start()
        {


            //add delegates to the jet:
            Instance.LocalJetInstance = Instance.LocalJetInstance ?? GetComponentInChildren<JetMovement>().gameObject;

            if (Instance.LocalJetInstance != null)
            {
                Instance.LocalJetInstance.GetComponent<JetMovement>().OnJetExplode += new JetExplode(OnJetExplodeFunc);
                Instance.LocalJetInstance.GetComponent<JetMovement>().OnAltitudeChange += new AltitudeChangeDelegate(OnJetChangeAltitude);
                Debug.Log("added Explotion function");
            }

            //scrl = AltitudeMeter.GetComponent<ScrollRect>();
        }




    }
}
