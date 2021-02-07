using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using AirBattle.Core;

namespace AirBattle.JetControl
{
    public delegate void JetExplode();
    public delegate void AltitudeChangeDelegate(float CurrAltitude);
    public class JetMovement : MonoBehaviourPun
    {

        public float zTurnSpeed;
        public float xTurnSpeed;
        public float yTurnSpeed;
        private float SpeedForce = 0;
        public float AdjustedSpeedForce
        {
            get
            {
                return Mathf.Abs(SpeedForce) < 1500 ? 0 : SpeedForce;
            }
        }
        public float MaxForce = 80000;
        public float MinForce = 0;
        public float MinNegForce = -10000;
        public float MinSpeed = 0;
        public float MaxSpeed = 60;
        public float MinSpeedForTakeOff = 20;
        public float AngleForForceApply = 5;
        public float waitUntilDestroy;

        //the max angle which he is allowed to be when he lands:
        public float MaxLandingAngle;

        private float zTurn;
        private float xTurn;
        private float yTurn;

        //private CharacterController jet = null;
        private Rigidbody rb = null;
        private float horizontalMove = 0f;
        private float verticalMove = 0f;
        private float AltitudeChange = 0f;
        private bool isLanded { get { return checkIfOnLand(); } }
        public bool isDead = false;

        public Transform ExplotionPrefab;
        public Transform SmokePrefab;

        public JetExplode OnJetExplode;
        public AltitudeChangeDelegate OnAltitudeChange;

        private Transform expl;
        private Transform smoke;

        //the target rotation. if its not null then need to rotate to it.
        //private Quaternion targetRot;
        private Vector3 forceToAplly;
        private Vector3 TorqueToApply;

        //calculate speed with gravity:
        public Vector3 SpeedWithGravity { get { return rb.velocity; } }
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            zTurn = 0f;
            xTurn = 0f;
            SpeedForce = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //the next should be applied in SingleMode or in Multiplyaer if its the players jet.
            if (PhotonNetwork.OfflineMode || photonView.IsMine)
            {
                horizontalMove = isDead ? 0 : Input.GetAxis("Horizontal");  //z-axis
                verticalMove = isDead ? 0 : Input.GetAxis("Vertical"); //speed thrust
                AltitudeChange = isDead ? 0 : Input.GetAxis("Altitude"); //x-axis

                SpeedForce += MakeRound(verticalMove) * 500;
                SpeedForce = isLanded ? Mathf.Clamp(SpeedForce, MinNegForce, MaxForce) : Mathf.Clamp(SpeedForce, MinForce, MaxForce);

            }

            zTurn = xTurn = yTurn = 0;

            //Debug.Log("curr speed:" + currSpeed);

            //if he's landed then he shoudlnt turn his wings:
            if (!isLanded)
            {
                zTurn = horizontalMove * zTurnSpeed;
            }
            else
            {
                
                yTurn = MakeRound(AdjustedSpeedForce) * horizontalMove * yTurnSpeed;
            }

            //if he's landed then he can only go up in the x-axis:
            if ((isLanded && AltitudeChange < 0 && SpeedWithGravity.magnitude > MinSpeedForTakeOff) || !isLanded)
            {
                xTurn = AltitudeChange * xTurnSpeed;
            }



        }

        private void FixedUpdate()
        {
            //AdjustDrag();
            MakeMove();
            MakeTurn();
        }

        private int MakeRound(float n)
        {

            if (Mathf.Abs(n) < 0.05)
            {
                return 0;
            }
            else if (n > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        private void MakeTurn()
        {
            TorqueToApply = transform.right * xTurn - transform.forward * zTurn + transform.up * yTurn;
            rb.AddTorque(TorqueToApply, ForceMode.Force);
        }

        private void MakeMove()
        {
            forceToAplly = transform.forward * ForceForWantedSpeed - Physics.gravity * rb.mass * (isLanded ? 1 : 0);
            Debug.Log("Force:" + AdjustedSpeedForce + ". Force for wanted:" + ForceForWantedSpeed + ". speed:" + (Vector3.Dot(SpeedWithGravity, transform.forward)));
            rb.AddForce(forceToAplly, ForceMode.Force);
            //OnAltitudeChange(transform.position.y);
        }


        ////adjust the vertical move. if its on a landing plane, then it can go back. otherwise, just go straight.
        //private float adjustVerticalMove(float n)
        //{
        //    if (!isLanded)
        //    {
        //        if (n >= -0.2f && n <= 0.2f)
        //        {
        //            return 0;
        //        }
        //        return 1;
        //    }
        //    return n;
        //}

        //private void AdjustDrag()
        //{
        //    //if no gas is used, then drag should be almost 0.
        //    if (!isLanded && Mathf.Abs(MakeRound(verticalMove)) < 0.05)
        //    {
        //        rb.drag = MinDrag;
        //    }
        //    else
        //    {
        //        rb.drag = MaxDrag;
        //    }
        //}

        private float ForceForWantedSpeed
        {
            get
            {
                if (AdjustedSpeedForce > 0 && Vector3.Dot(SpeedWithGravity, transform.forward) < MaxSpeed)
                {
                    return AdjustedSpeedForce;
                }
                if (AdjustedSpeedForce < 0 && Vector3.Dot(SpeedWithGravity, transform.forward) > MinSpeed)
                {
                    return AdjustedSpeedForce;
                }
                if (AdjustedSpeedForce > 0 && Vector3.Angle(SpeedWithGravity, transform.forward) > AngleForForceApply)
                {
                    return AdjustedSpeedForce;
                }
                return 0;
            }
        }

        void OnCollisionEnter(Collision col)
        {
            Debug.DrawLine(col.GetContact(0).point, col.GetContact(0).point - transform.up, Color.blue, 3);
            Debug.Log("jet collisioned: " + col.GetContacts(new ContactPoint[10]));
            if (!isDead)
            {
                //check if trying to land:
                if ((!CheckIfAllowedToLand(col.gameObject, col.relativeVelocity)) || (!checkAngleOnLanding(col)))
                {
                    //Die();
                    //downgrade his life to 0 so the Die will happen in Health Class.
                    Health jetHealth = GetComponent<Health>();
                    jetHealth.TakeDamage(jetHealth.CurrentLife);
                }

            }
        }

        //check that landing was with the correct angle.
        private bool checkAngleOnLanding(Collision col)
        {
            List<ContactPoint> contactsOfHit = new List<ContactPoint>();
            int count = col.GetContacts(contactsOfHit);
            for (int i = 0; i < count; i++)
            {
                Debug.Log("angle of impact: " + (Vector3.Angle(contactsOfHit[i].normal, transform.up)));
                if (Vector3.Angle(contactsOfHit[i].normal, transform.up) > MaxLandingAngle)
                {
                    return false;
                }
            }
            Debug.Log("number of hits on impact:" + count);
            return true;
        }

        private bool CheckIfAllowedToLand(GameObject LandingTile, Vector3 hitSpeed)
        {
            Debug.Log("checking collision with:" + LandingTile.name);
            LandTile ltile = LandingTile.GetComponent<LandTile>();
            if (ltile == null) return false;
            Debug.Log("the speed og hit:" + hitSpeed);
            return ltile.AllowedToLand(hitSpeed);
        }

        private bool checkIfOnLand()
        {
            Ray checkGround = new Ray(transform.position, -transform.up);
            RaycastHit groundobj;
            BoxCollider collide = GetComponent<BoxCollider>();
            if (collide == null) return false;

            float CheckDistance = collide.bounds.size.y + 0.002f;
            Debug.DrawLine(checkGround.origin, checkGround.origin + checkGround.direction * CheckDistance, Color.green, 3);

            if (Physics.Raycast(checkGround, out groundobj, CheckDistance))
            {
                Debug.Log("is on Ground:" + groundobj.collider.gameObject.name);
                return true;
            }
            return false;
        }

        private void OnDestroy()
        {
            Debug.Log("destrot jet");
            if (expl != null) Destroy(expl);
            if (smoke != null) Destroy(smoke);
        }

        //public void Die()
        //{
        //    if (!isDead)
        //    {
        //        Debug.Log("died");

        //        //explosion:
        //        expl = Instantiate(ExplotionPrefab, transform.position, Quaternion.identity);
        //        Destroy(expl.gameObject, waitUntilDestroy - 1);
        //        //Smoke that follows the jet:
        //        smoke = Instantiate(SmokePrefab, transform.position, Quaternion.identity, transform);
        //        Destroy(smoke.gameObject, waitUntilDestroy - 1);

        //        isDead = true;
        //        //next function is delegate:
        //        OnJetExplode();
        //        //destroy the jet:
        //        Destroy(gameObject, waitUntilDestroy);
        //    }
        //}
    }
}
