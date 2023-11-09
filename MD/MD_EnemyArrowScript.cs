using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared;
using DumortierMatthieu.Bezier;
using System.Linq;

namespace DumortierMatthieu
{
    public class MD_EnemyArrowScript  : MD_ProjectilScript,IDamageable
    {

        [SerializeField]
        bool m_IsDebug = false; 
        [SerializeField]
        float m_GizmosSphereSize = 0.04f;


        [SerializeField]
        ParticleSystem m_PendingPS; 
       
        [SerializeField]
        float m_RoationSpeed = 5f; 
        
        [SerializeField]
        int m_Damage = 15;

        [SerializeField]
        bool m_IsStuckInSomething = false; 

        [Space(10)]
        [Header("Curve Points")]
        [Space(10)]
        [SerializeField] Vector3 m_PointA;
        [SerializeField] Vector3 m_PointB; //targetpos
        

        [SerializeField]
        int m_CurvePointNumberPerMeter = 2;

        [SerializeField]
        Vector3[] m_PointList;

        int m_PosIndex = 0;

        [SerializeField]
        float m_DistanceThreshold = 0.1f;

        [SerializeField]
        Vector3 m_TangentOffset;

        [SerializeField]
        float m_TargetHeight = 1f;
                    
        [SerializeField]
        float m_TargetOffSetDistance = 2f;

        

        [SerializeField]
        float m_ObjectPenetrationTime = 0.1f;

        [SerializeField]
        ParticleSystem m_FlyingPs;
        [SerializeField]
        ParticleSystem m_HitPendingPS;

        [SerializeField]
        GameObject m_HitPs;

        [SerializeField]
        Transform m_HitAnchor; 

        [Space(15)]
        [Header("SOUNDS")]
        [Space(15)]

        [SerializeField]
        MD_SoundManager m_HitSound;

        [SerializeField]
        MD_SoundManager m_FlyBySound;

        [SerializeField]
        MD_SoundManager m_HitEnvSound; 

        


        private void Awake()
        {

            


            

        }
        protected override void  Start()
        {
            
            PointlistCreation();
        }

        // Update is called once per frame
        void Update()
        {

            

            if (m_IsStuckInSomething)
            {
                return; 
            }


            FollowCurve(); 

        }


        void FollowCurve()
        {

            

            //print("arrow else" + Vector3.Distance(transform.position, m_PointList[m_PointList.Length - 1]));
            if (Vector3.Distance(transform.position, m_PointList[m_PointList.Length-1]) < 0.5)
            {

                const float CONTINUE_PATH_FORWARD = 50f;
                const float CONTINUE_PATH_DOWN = 10f;
                //print("arrow else");

                

                m_PointA = transform.position;
                m_PointB =  transform.position +( transform.forward * CONTINUE_PATH_FORWARD) + (-Vector3.up * CONTINUE_PATH_DOWN);  
                PointlistCreation(); 



            }

            if (m_PosIndex + 1 < m_PointList.Length)
            {
                
                Quaternion lookAt = Quaternion.LookRotation(m_PointList[m_PosIndex + 1] - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, m_RoationSpeed * Time.deltaTime);

                transform.position = Vector3.MoveTowards(transform.position, m_PointList[m_PosIndex + 1], m_Speed * Time.deltaTime);

                float distanceFromNext = Vector3.Distance(transform.position, m_PointList[m_PosIndex + 1]);

                if (distanceFromNext < m_DistanceThreshold)
                {
                    if (m_PosIndex < m_PointList.Length)
                    {
                        m_PosIndex++;
                    }

                }
            }
            else
            {

               
                //transform.position = Vector3.MoveTowards(transform.position,
                //    m_PointB + Vector3.up * m_TargetHeight + transform.forward * m_TargetOffSetDistance,
                //    m_Speed * Time.deltaTime);

                
                //Destroy(GetComponent<SphereCollider>());
            }

            

            


            //debug

            if (m_PosIndex == m_PointList.Length - 1)
            {


                if (m_IsDebug)
                {
                    m_PosIndex = 0;
                    transform.position = m_PointList[m_PosIndex];
                    transform.LookAt(m_PointList[m_PosIndex + 1]);
                }
                else
                {


                    //effect, destroy

                    transform.position = Vector3.MoveTowards(transform.position, m_PointB + Vector3.up * m_TargetHeight + transform.forward * m_TargetOffSetDistance, m_Speed * Time.deltaTime);

                    Destroy(this.gameObject , m_DestroyTime);
                }
                




            }
        }

        public void TakeDamage(int dmg)
        {
            if (m_IsStuckInSomething) return; 
            print("this arrow is taking damage");
        }

        public void TakeDamage(int dmg, IDamageable.EAttackSource attackStyle)
        {

            TakeDamage(dmg); 
        }

        public void GetBezierTransforms(Transform pointA, Transform pointB )
        {
            m_PointA = pointA.position;
            m_PointB = pointB.position + Vector3.up * m_TargetHeight + transform.forward * m_TargetOffSetDistance ;
            
        }

        void PointlistCreation()
        {
            m_PointList = BezierMethods.QuadraticBezierPointListCreation(m_CurvePointNumberPerMeter, m_PointA, m_PointB,
                TangentAPos(0.5f, m_PointA, m_PointB));
        }

        void BezierPatternTransformEdit(Vector3 target)
        {
            
            
            

        }

        Vector3 TangentAPos(float distanceValue, Vector3 pointA, Vector3 pointB)
        {
            return Vector3.Lerp(pointA, pointB, distanceValue) + m_TangentOffset;
        }

      

        IEnumerator StuckArrowRoutine(float time, Collider parent)
        {

            Instantiate(m_HitPs, m_HitAnchor.position, m_HitAnchor.rotation); 


            yield return new WaitForSeconds(time);


            if (parent.CompareTag("Player") || parent.CompareTag("Enemy") || parent.CompareTag("Neutral"))
            {

                m_PendingPS.gameObject.SetActive(false); 

                if (parent.gameObject.TryGetComponent<IDamageable>(out IDamageable component))
                {
                    m_HitSound.Activation(transform); 

                    component.TakeDamage(m_Damage); 
                }


                transform.LookAt(parent.ClosestPoint(transform.position));
                transform.parent = parent.transform;


            }
            else
            {
                m_HitEnvSound.Activation(transform);
            }
            Destroy(GetComponent<SphereCollider>());





        }

        private void OnTriggerEnter(Collider other)
        {

            if (other.isTrigger)
            {
                return; 
            }

            
            var emission = m_FlyingPs.emission; 

            emission.rateOverTime = 0f;

            var newEmission = m_HitPendingPS.emission;

            newEmission.rateOverTime = 20f;

            if (other.CompareTag("Projectil"))
            {
                return; 
            }

            print("an arrow touched something ");
            m_IsStuckInSomething = true;


            StartCoroutine(StuckArrowRoutine(m_ObjectPenetrationTime, other));
            Destroy(this.gameObject, m_DestroyTime);
        }


        private void OnDrawGizmos()
        {


            Vector3[] pos = BezierMethods.QuadraticBezierPointListCreation(m_CurvePointNumberPerMeter, m_PointA, transform.forward * m_TargetOffSetDistance +  m_PointB,
                TangentAPos(0.5f, m_PointA, m_PointB));

            foreach (var item in pos)
            {
                Gizmos.color = Color.red; 
                Gizmos.DrawWireSphere(item, m_GizmosSphereSize);
            }

            if (!Application.isPlaying)
            {
                m_PointA = transform.position; 
            }




            Vector3 offSetPos = Vector3.Lerp(m_PointA, m_PointB, 0.5f) + m_TangentOffset;

           

            Gizmos.color = Color.cyan;

            Gizmos.DrawSphere(m_PointA,m_GizmosSphereSize);
            Gizmos.DrawSphere(m_PointB,m_GizmosSphereSize);
            Gizmos.DrawSphere(offSetPos,m_GizmosSphereSize);
            Gizmos.color = Color.yellow; 
            Gizmos.DrawSphere(m_PointB + transform.forward * m_TargetOffSetDistance ,m_GizmosSphereSize);

            

        }

        protected override void DealDamage(GameObject entity)
        {
            m_PendingPS.gameObject.SetActive(false);

            if (entity.TryGetComponent<IDamageable>(out IDamageable component))
            {
                component.TakeDamage(m_Damage);
            }


            transform.LookAt(entity.GetComponent<Collider>().ClosestPoint(transform.position));
            transform.parent = entity.transform;
        }
    }
}
