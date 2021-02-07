using UnityEngine;
using AirBattle.Objects.Bullets;
using Photon.Pun;

namespace AirBattle.Core
{
    public delegate void OnDestroy(Transform rt);
    public class Health : MonoBehaviourPun
    {
        public float InitLife;
        //public bool IsStatic;
        public float CurrentLife { get; set; }

        //public Transform ExplosionEffectPrefab;

        //the helath bar to above the target. will be used to update the red line of life.
        private HealthbarManager healthbar;

        public OnDestroy OnObjectDestroy;

        // Start is called before the first frame update
        void Start()
        {
            CurrentLife = InitLife;

            healthbar = GetComponentInChildren<HealthbarManager>();
            if (healthbar)
            {
                healthbar.CurLife = InitLife;
            }

            OnObjectDestroy += GameManagement.Instance.MakeTargetExplodeAffect;
        }

        private void OnCollisionEnter(Collision collision)
        {
            BulletStats bullet = collision.collider.GetComponent<BulletStats>();
            if (bullet != null)
            {
                //take damage:
                TakeDamage(bullet.Damage);
            }
        }
        public void TakeDamage(float damage)
        {
            CurrentLife -= damage;

            //update the health bar animation:
            if (healthbar)
            {
                healthbar.UpdateLife(CurrentLife);
            }

            //if life is 0 then die.
            if (CurrentLife <= 0)
            {
                //exploding animation
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("target die");
            //invoke all the deleagtes regestered:
            if (OnObjectDestroy != null)
            {
                OnObjectDestroy(transform);
            }

            //destroy object:
            if (PhotonNetwork.OfflineMode)
            {
                Destroy(this.gameObject);
            }
            else
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }
}
