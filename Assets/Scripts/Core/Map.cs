using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AirBattle.Core.UI
{
    public class Map : MonoBehaviour
    {
        public class MapTarget
        {
            public Transform target { get; set; }
            public RectTransform maptarget { get; set; }

            public override bool Equals(object obj)
            {
                return ((MapTarget)obj).target == target;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private Transform jet; //the Transform of the player in game
        private float WorldLength; //terrain length

        public RectTransform MapTargetsPrefab; //prefab to instantiate for the targets in the map.
        public RectTransform MapPlayerPrefab; //prefab to instantiate for the player in the map.

        //private List<Transform> targets; //the targets to register

        private RectTransform map_player; // RectTranform of the player in the Map.
        private List<MapTarget> map_targets; //RectTransform of targets in the Map.

        private void Awake()
        {
            AddRegistersToAllTargets();
            map_targets = new List<MapTarget>();
        }

        // Start is called before the first frame update
        void Start()
        {
            //get instance of the Main player's jet.
            jet = GameManagement.Instance.LocalJetInstance.transform;

            FindTerrainLength();


            //for every static target, we instantiate in the Map:
            //for(int i = 0; i < targets.Length; i++)
            //{
            //    if (targets[i] != null)
            //    {
            //        targets[i].GetComponent<Health>().OnObjectDestroy += DestroyTargetOnMap;
            //        map_targets[i] = Instantiate(MapTargetsPrefab, transform);
            //        Debug.Log("instatniated target in map");
            //    }
            //}

            //set the player in the middle:
            float player_x = 0.5f * GetComponent<Image>().rectTransform.rect.width;
            float player_y = 0.5f * GetComponent<Image>().rectTransform.rect.height;
            map_player = Instantiate(MapPlayerPrefab, transform);
            map_player.localPosition = new Vector3(player_x, player_y, 0);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            //for (int i = 0; i < targets.Length; i++)
            //{
            //    if (targets[i] != null && jet!=null) {
            //        Vector3 targetDiff = targets[i].transform.position - jet.position;
            //        targetDiff.Set(targetDiff.x, 0, targetDiff.z);

            //        Vector3 forward2d = new Vector3(jet.forward.x,0,jet.forward.z);

            //        Quaternion rotDiff = Quaternion.FromToRotation(forward2d, targetDiff);
            //        float yRot = rotDiff.eulerAngles.y;

            //        map_targets[i].localPosition = new Vector3(0.5f * GetComponent<Image>().rectTransform.rect.width, 0.5f * GetComponent<Image>().rectTransform.rect.height, 0);
            //        map_targets[i].localRotation = Quaternion.Euler(0, 0 , -yRot);

            //        float xposAdd = map_targets[i].up.x * 0.5f * targetDiff.magnitude / WorldLength * GetComponent<Image>().rectTransform.rect.width;
            //        float yposAdd = map_targets[i].up.y * 0.5f * targetDiff.magnitude / WorldLength * GetComponent<Image>().rectTransform.rect.height;
            //        map_targets[i].localPosition += new Vector3(xposAdd,yposAdd,0);

            //        float targetXClamp = Mathf.Clamp(map_targets[i].localPosition.x, 0, GetComponent<Image>().rectTransform.rect.width);
            //        float targetYClamp = Mathf.Clamp(map_targets[i].localPosition.y, 0, GetComponent<Image>().rectTransform.rect.height);
            //        map_targets[i].localPosition = new Vector3(targetXClamp, targetYClamp, 0);
            //    }
            //}
            if (jet != null)
            {
                foreach (var target in map_targets)
                {
                    Vector3 targetDiff = target.target.position - jet.position;
                    targetDiff.Set(targetDiff.x, 0, targetDiff.z);

                    Vector3 forward2d = new Vector3(jet.forward.x, 0, jet.forward.z);

                    Quaternion rotDiff = Quaternion.FromToRotation(forward2d, targetDiff);
                    float yRot = rotDiff.eulerAngles.y;

                    target.maptarget.localPosition = new Vector3(0.5f * GetComponent<Image>().rectTransform.rect.width, 0.5f * GetComponent<Image>().rectTransform.rect.height, 0);
                    target.maptarget.localRotation = Quaternion.Euler(0, 0, -yRot);

                    float xposAdd = target.maptarget.up.x * 0.5f * targetDiff.magnitude / WorldLength * GetComponent<Image>().rectTransform.rect.width;
                    float yposAdd = target.maptarget.up.y * 0.5f * targetDiff.magnitude / WorldLength * GetComponent<Image>().rectTransform.rect.height;
                    target.maptarget.localPosition += new Vector3(xposAdd, yposAdd, 0);

                    float targetXClamp = Mathf.Clamp(target.maptarget.localPosition.x, 0, GetComponent<Image>().rectTransform.rect.width);
                    float targetYClamp = Mathf.Clamp(target.maptarget.localPosition.y, 0, GetComponent<Image>().rectTransform.rect.height);
                    target.maptarget.localPosition = new Vector3(targetXClamp, targetYClamp, 0);
                }
            }
        }

        //a function to find the target in the map and destroy it.
        //used as a delegate and envoked when the Target is destroyed.
        void DestroyTargetOnMap(Transform t)
        {
            //int i = 0;
            //for (i = 0; i < targets.Length; i++)
            //{
            //    if (targets[i].Equals(t))
            //    {
            //        break;
            //    }
            //    if (i == targets.Length) return;
            //}
            //Destroy(map_targets[i].gameObject);
            UnRegisterTargets(new List<Transform>() { t });
        }

        private void OnDestroy()
        {
            foreach (var item in map_targets)
            {
                Destroy(item.maptarget.gameObject);
            }
            Destroy(map_player.gameObject);
        }

        private void AddRegistersToAllTargets()
        {
            GameManagement.Instance.Targets.CollectionChanged += Targets_CollectionChanged;
        }

        private void Targets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                RegisterTargets(e.NewItems);
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                UnRegisterTargets(e.OldItems);
            }
        }

        private void FindTerrainLength()
        {
            WorldLength = GetComponentInParent<GameManagement>().TerrainLength;
        }

        public void RegisterTargets(IList targets)
        {
            foreach (var target in targets)
            {
                MapTarget mapt = new MapTarget();
                mapt.target = (Transform)target;
                mapt.target.GetComponent<Health>().OnObjectDestroy += DestroyTargetOnMap;
                mapt.maptarget = Instantiate(MapTargetsPrefab, transform);
                map_targets.Add(mapt);
            }
        }

        public void UnRegisterTargets(IList targets)
        {
            foreach (var target in targets)
            {
                MapTarget maptToRemove = new MapTarget();
                maptToRemove.target = (Transform)target;

                int indx = map_targets.IndexOf(maptToRemove);
                if (indx != -1)
                {
                    Destroy(map_targets[indx].maptarget.gameObject);
                    map_targets.Remove(maptToRemove);
                }

            }
        }
    }
}
