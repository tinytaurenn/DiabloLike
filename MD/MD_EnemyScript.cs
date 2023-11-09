using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using DumortierMatthieu.AI;
using Shared;




namespace DumortierMatthieu
{

    

    

    public class MD_EnemyScript : MonoBehaviour, IEntity, IDamageable,IEntityStyle,IHealable,IAggro
    {
       
        
        enum EDetectionState
        {
            Aggro,
            Suspicious,
            Waiting,
            Death // death 


        }

        enum EGizmosState
        {
            Detection,
            Pattern,
            CloseAttack,
            RangeAttack

        }
        [SerializeField]
        IEntityStyle.Style m_Style = IEntityStyle.Style.Good; 

        NavMeshAgent m_NavMeshAgent;

        Rigidbody m_RigidBody;

        Animator m_Animator;

        #region Global fields

        [SerializeField]
        bool m_IsGizmosActivated = true;
        [SerializeField]
        int m_GizmosDrawFrequency = 5;
        [Space(10)]
        [SerializeField]
        EGizmosState m_GizmosState = EGizmosState.Detection; 
        [Space(10)]
        [Space(10)]
        [Header(" [ Enemy Info fields ] ///////////////////////////////////////")]
        [Space(10)]      
        [SerializeField]
        EDetectionState m_DetectionState = EDetectionState.Waiting;
        [SerializeField]
        int m_EnemyLife = 50;
        int m_EnemyMaxLife; 
        [SerializeField]
        float m_EnemySpeed = 5f;
        [SerializeField]
        bool m_IsAttacking = false;

        [SerializeField]
        bool m_CanRotate = true;

        bool m_IsStun = false; 
        #endregion

        #region Patrol
        [Space(10)]
        [Header(" Patrol fields")]
        [Space(10)]
        [SerializeField]
        List<Transform> m_PatrolPointList = new List<Transform>();
        [SerializeField]
        bool m_IsRandomPatrol = false;
        [SerializeField]
        bool m_IsPatroling = true;
        [SerializeField]
        float m_PatrolSpeed = 2f;
        [SerializeField]
        float m_AggroSpeed = 3f; 
        [SerializeField]
        float m_RandomPatrolDistance = 15f;
        [SerializeField]
        float m_PatrolSwitchDistance = 1.5f;
        [SerializeField]
        int m_PatrolIndex = 0;
        Vector3 m_StartPos;  

        [SerializeField]
        float m_PatrolTimeMax = 5f; 
        [SerializeField]
        float m_PatrolTimeMin = 2f;
        [SerializeField]
        bool m_CanMove = true;
        [SerializeField]
        bool m_IsStoppingOnPatrolPoints = true; 

        #endregion
        #region Detection Fields
        [Space(10)]
        [Header(" [ Detection fields ] ///////////////////////////////////////")]
        [Space(10)]
        [SerializeField]
        float m_FovAngle = 25f; 
        [SerializeField]
        float m_DetectionDistance = 10f;
        [SerializeField]
        float m_CloseDetectionDistance = 3f; 
        [SerializeField]
        float m_RangeAttackDistance = 8f;
        [SerializeField]
        float m_CloseAttackDistance = 2f;
        [SerializeField]
        float m_ForgetDetectionTimer = 5f;
        [SerializeField]
        bool m_IsPlayerInMind = false;
        //[SerializeField]
        //float m_DetectionDistanceOffset = 3f; 
        Vector3 m_LastTargetPos; 
        Coroutine m_PlayerInMindRoutine; 

        [Space(10)]
        [SerializeField]
        Transform m_TargetTransform; 



        [SerializeField]
        LayerMask m_TargetMask;
        [SerializeField]
        LayerMask m_DetectionMask;
        #endregion
        #region Range Attack fields

        [Space(10)]
        [Header(" [ RangeAttack fields ] ///////////////////////////////////////")]
        [Space(10)]
        [SerializeField]
        GameObject m_ArrowGameObject;
        [SerializeField]
        float m_RangeAttackAngle = 20f; 
        //[SerializeField]
        //int m_ArrowDamage = 25;
        [SerializeField]
        bool m_IsRangeAttacking = false;
        [SerializeField]
        float m_RangeAttackRotationSlerp = 0.5f;
        [SerializeField]
        Transform m_ArrowAnchor;
        [SerializeField]
        Transform m_ArrowPrepVFXAnchor;

        [SerializeField]
        GameObject m_ArrowPrepFXGameObject; 

        [SerializeField] 
        GameObject  m_ArrowPrepInstantiatedGameObject;

        //[SerializeField]
        //float m_ArrowDestroyTime = 4f; 
        //[SerializeField]
        
        #endregion
        #region Close Attack fields

        [Space(10)]
        [Header(" [ CloseAttack fields ] ///////////////////////////////////////")]
        [Space(10)]
        [SerializeField]
        float m_CloseAttackAnle = 30f;
        [SerializeField]
        bool m_IsCloseAttacking = false;
        [SerializeField]
        int m_KickDamage = 15;

        [SerializeField]
        GameObject m_MeleeAttackPS;

        [SerializeField]
        GameObject m_MeleeAttackHitPS; 

        //[SerializeField]
        //GameObject m_MeleeAttackLastObject; 
        
        [SerializeField]
        Transform m_MeleeAttackAnchor;

        [SerializeField]
        bool m_CanCloseAttack = true;

        [SerializeField]
        float m_CloseAttackCoolDown = 2f;

        [SerializeField]
        float m_StunTime = 1f;

        [SerializeField]
        ParticleSystemRenderer m_KickStartRenderer;
        [SerializeField]
        AnimationCurve m_KickStartCurve;
        [SerializeField]
        float m_KickStartFadeTime = 0.5f; 


        #endregion

        #region Death
        [SerializeField]
        float m_DeathDelay = 0f;
        [SerializeField]
        float m_DeathDelayBeforeDesactivate = 2f;


        [Space(10)]
        [Header(" [ Sounds fields ] ///////////////////////////////////////")]
        [Space(10)]

        [SerializeField]
        MD_SoundManager m_DeathSound;

        [SerializeField]
        MD_SoundManager m_CloseAttackSound;

        [SerializeField]
        MD_SoundManager m_CloseAttackHitSound;

        [SerializeField]
        MD_SoundManager m_RangeAttackLoadingSound;

        [SerializeField]
        MD_SoundManager m_RangeAttackShootSound;

        [SerializeField]
        MD_SoundManager m_EnemyHurtSound;

        [SerializeField]
        MD_SoundManager m_StepSound; 

        #endregion



        #region Unity Callbacks

        private void Awake()
        {


            m_NavMeshAgent = GetComponent<NavMeshAgent>();

            m_RigidBody = GetComponent<Rigidbody>();


            m_Animator = GetComponent<Animator>(); 
        }

        void Start()
        {
            m_EnemyMaxLife = m_EnemyLife;

            m_StartPos = transform.position; 
            
            
            //m_IsPlayerInMind = true;  
            //
        }

        private void OnEnable()
        {
            S_GameManager.OnSwitchState += OnGameStateChange;
            S_GameManager.OnPlayerSwitch += ResetAI; 
        }

        private void OnDisable()
        {
            S_GameManager.OnSwitchState -= OnGameStateChange;
            S_GameManager.OnPlayerSwitch -= ResetAI;
        }

        // Update is called once per frame
        void Update()
        {

            if (m_NavMeshAgent == null )
            {
               
                return; 
            }


            

            MoveAnimator(); 

            UpdateState();

            
        }

        #endregion


        void MoveAnimator ()
        {
            if (m_NavMeshAgent == null) return; 

            m_Animator.SetFloat("Speed", m_NavMeshAgent.velocity.magnitude / m_NavMeshAgent.speed);

            if (!m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("RangeAttack") && m_DetectionState == EDetectionState.Suspicious)
            {
                m_NavMeshAgent.isStopped = false; 
            }

            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("RangeAttack") && m_CanRotate && m_TargetTransform !=null)
            {
                AIMethods.RotationLookAt(transform, m_TargetTransform, m_RangeAttackRotationSlerp * Time.deltaTime);
            }

        }


        #region StateMachine

        void UpdateState()
        {
            //AttackDebugger();
            //
            
           

            switch (m_DetectionState)
            {
                case EDetectionState.Aggro:///////////////////////////////////////////////////////////////////


                    if (m_TargetTransform == null)
                    {
                        return;
                    }


                    CheckTarget();

                    Attacking();
                    break;
                case EDetectionState.Suspicious:///////////////////////////////////////////////////////////////////
                    //Patroling();

                    if (m_IsRangeAttacking && !m_NavMeshAgent.isStopped) m_NavMeshAgent.isStopped = true;

                    if (m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("RangeAttack")  )
                    {
                        m_NavMeshAgent.isStopped = true;
                    }

                    CheckTarget();
                    break;
                case EDetectionState.Waiting:///////////////////////////////////////////////////////////////////

                    Patroling();
                    CheckTarget();

                    break;
                case EDetectionState.Death:///////////////////////////////////////////////////////////////////
                    break;
                default:
                    break;
            }
        }

        void FixedUpdateState()
        {
            

            switch (m_DetectionState)
            {
                case EDetectionState.Aggro:///////////////////////////////////////////////////////////////////
                    break;
                case EDetectionState.Suspicious:///////////////////////////////////////////////////////////////////


                    break;
                case EDetectionState.Waiting:///////////////////////////////////////////////////////////////////
                    break;
                case EDetectionState.Death:///////////////////////////////////////////////////////////////////
                    break;
                default:
                    break;
            }
        }




        void OnEnterDetectionState(EDetectionState state)
        {
            if (m_NavMeshAgent ==  null)
            {
                return; 
            }

            switch (state)
            {
                case EDetectionState.Aggro:///////////////////////////////////////////////////////////////////

                    m_NavMeshAgent.speed = m_AggroSpeed;

                    

                    m_IsPlayerInMind = true;
                    break;
                case EDetectionState.Suspicious:///////////////////////////////////////////////////////////////////

                    m_IsPatroling = true;
                    m_NavMeshAgent.isStopped = false;

                    m_NavMeshAgent.speed = m_AggroSpeed; 

                    m_Animator.SetBool("Aggro", true); //not the same walk animation 

                    //m_NavMeshAgent.destination = m_TargetTransform.position; //not same destination
                    //print("set destination 2");

                    
                   
                    m_NavMeshAgent.SetDestination(m_LastTargetPos);

                    

                    m_PlayerInMindRoutine =  StartCoroutine(PlayerInMind(m_ForgetDetectionTimer));
                    break;
                case EDetectionState.Waiting:///////////////////////////////////////////////////////////////////


                    m_IsPatroling = true;
                    m_NavMeshAgent.isStopped = false;


                    m_NavMeshAgent.speed = m_PatrolSpeed;

                    m_Animator.SetBool("Aggro", false);

                    m_TargetTransform = null;

                   
                    break;

                case EDetectionState.Death:

                    

                    const int DEATH_ANIM_NUMBER = 2; 

                    m_Animator.SetInteger("DeathNumber", UnityEngine.Random.Range(0,DEATH_ANIM_NUMBER));
                    m_Animator.SetTrigger("DeathTrigger");

                    m_DeathSound.Activation(transform, true); 

                    //print("destroy vfx"); 
                    //Destroy(m_MeleeAttackLastObject); 
                    

                    //print("started routine");
                    StartCoroutine( DeathRoutine(m_DeathDelay, m_DeathDelayBeforeDesactivate)); 

                   
                    break;
                default:
                    break;
            }
        }

        void OnExitDetectionState(EDetectionState state)
        {
            if (m_NavMeshAgent ==null)
            {
                return; 
            }

            switch (state)
            {
                case EDetectionState.Aggro:///////////////////////////////////////////////////////////////////


                    m_Animator.ResetTrigger("RangeAttack");
                    m_Animator.ResetTrigger("CloseAttack");

                    if (m_TargetTransform != null)
                    {
                        m_LastTargetPos = m_TargetTransform.position;
                    }
                    break;
                case EDetectionState.Suspicious:///////////////////////////////////////////////////////////////////

                    m_NavMeshAgent.isStopped = true;
                    m_IsPatroling = false;
                    m_NavMeshAgent.ResetPath();



                    StopCoroutine(m_PlayerInMindRoutine); 
                    break;
                case EDetectionState.Waiting:///////////////////////////////////////////////////////////////////


                    m_NavMeshAgent.isStopped = true;
                    m_IsPatroling = false;
                    m_NavMeshAgent.ResetPath();

                    break;
                case EDetectionState.Death:///////////////////////////////////////////////////////////////////
                    break;
                default:
                    break;
            }
        }

        void SwitchDetectionState(EDetectionState state)
        {
            if (m_DetectionState == state)
            {
                return; 
            }

            OnExitDetectionState(m_DetectionState);
            m_DetectionState = state;
            OnEnterDetectionState(m_DetectionState); 
        }

        #endregion

        #region Detection Update

        /// <summary>
        /// update target tranform
        /// </summary>
        void CheckTarget()
        {
            TargetValid(m_TargetTransform);

            if (!Physics.CheckSphere(transform.position, m_DetectionDistance, m_TargetMask))
            {
                return;
            }

            if (S_GameManager.Instance.CurrentPlayer !=null)
            {
                m_TargetTransform = S_GameManager.Instance.CurrentPlayer.transform;
                TargetValid(m_TargetTransform);
                return; 
            }

            if (S_GameManager.Instance.GameState == S_GameManager.EGameState.Death) return;


            

            Collider[] targetList = Physics.OverlapSphere(transform.position, m_DetectionDistance, m_TargetMask);

            Collider target =  AIMethods.GetNearestColliderInFOV(transform, targetList, m_FovAngle);

            Collider targetNoFov = AIMethods.GetNearestCollider(transform.position, targetList);

            if (Vector3.Distance(transform.position, targetNoFov.transform.position) <= m_CloseDetectionDistance)
            {
                target = targetNoFov; 
            }



            if (target == null)
            {
                return; 
            }

            if (m_TargetTransform !=null )
            {
                if (target.transform == m_TargetTransform)
                {
                    return;
                }
            }

            



            m_TargetTransform = target.transform; 


           


        }


        /// <summary>
        /// manage switchstate 
        /// </summary>
        /// <param name="target"></param>
        void TargetValid(Transform target)
        {

            if (m_IsAttacking)
            {
                return; 
            }



            if (target == null  )
            {

                

                //print("target = null "); 
                SwitchDetectionState(EDetectionState.Waiting);
                return ; 
            }


            if (Vector3.Distance(transform.position, target.position) <= m_CloseDetectionDistance)
            {
                if (AIMethods.IsOnLineSight(transform.position, target.gameObject, m_DetectionMask))
                {
                    SwitchDetectionState(EDetectionState.Aggro);
                    return;
                }
            }
            

            if (Vector3.Distance(transform.position, target.position) <= m_DetectionDistance)
            {
                if (AIMethods.IsFOV(m_FovAngle, transform, target.position))
                {

                    if (AIMethods.IsOnLineSight(transform.position, target.gameObject, m_DetectionMask))
                    {
                        SwitchDetectionState(EDetectionState.Aggro);
                        return;
                    }
                    
                }
                
            }

            if (m_DetectionState == EDetectionState.Aggro && !m_IsRangeAttacking)
            {
                SwitchDetectionState(EDetectionState.Suspicious);
            }
            

            return ; 
        }

        #endregion

        #region Coroutines
        IEnumerator PlayerInMind(float time)
        {
            

            yield return new WaitForSeconds(time);

            SwitchDetectionState(EDetectionState.Waiting);

            m_IsPlayerInMind = false; 
        }

        IEnumerator EnemyStopPatrol(float time)
        {

            m_CanMove = false;

            m_NavMeshAgent.isStopped = true; 
            

            yield return new WaitForSeconds(time);


            if (m_NavMeshAgent !=null )
            {
                m_NavMeshAgent.isStopped = false;
            }

            
            PatrolNextPoint();
            m_CanMove = true; 

        }

        #endregion

        #region Patrol

        /// <summary>
        /// manage patroling 
        /// </summary>
        void Patroling()
        {
            //print("patroling"); 

            if (m_NavMeshAgent == null)
            {
                return; 
            }

            if (m_IsPlayerInMind)
            {
                //m_NavMeshAgent.destination = m_TargetTransform.position;
                return; 
            }

            if (!m_CanMove)
            {
                return; 
            }

            if (!m_NavMeshAgent.pathPending && m_NavMeshAgent.remainingDistance < m_PatrolSwitchDistance)
            {

                if (m_IsStoppingOnPatrolPoints)
                {
                    StartCoroutine(EnemyStopPatrol(UnityEngine.Random.Range(m_PatrolTimeMin, m_PatrolTimeMax)));
                    return; 
                }

                PatrolNextPoint();




            }

            
  


        }

        void PatrolNextPoint()
        {

            if (m_NavMeshAgent == null)
            {
                return; 
            }

            if (m_IsRandomPatrol)
            {

                Vector2 nextPos = UnityEngine.Random.insideUnitCircle * m_RandomPatrolDistance;
                Vector3 nextPos3 = new Vector3(nextPos.x, 0, nextPos.y);

                nextPos3 += m_StartPos; 

                m_NavMeshAgent.SetDestination(nextPos3);
                return; 
            }

            //print("no random patrol");
            if (m_PatrolPointList.Count == 0)
            {
                return; 
            }

            m_NavMeshAgent.SetDestination(m_PatrolPointList[m_PatrolIndex].position);
            m_PatrolIndex = (m_PatrolIndex + 1) % m_PatrolPointList.Count; 

        }

        #endregion


        #region Attacks
        void Attacking()
        {
            if (!m_TargetTransform)
            {
                return; 
            }
            if (!m_CanCloseAttack)
            {
                return;
            }

            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("RangeAttack") && !m_CanRotate)
            {
                m_CanRotate = true; 
            }


            


            float distanceFromTarget = Vector3.Distance(transform.position, m_TargetTransform.position);
            

            if (distanceFromTarget <= m_CloseAttackDistance)
            {
                
                if (m_NavMeshAgent.pathPending || m_NavMeshAgent.hasPath)
                {
                    //m_NavMeshAgent.ResetPath();
                    m_NavMeshAgent.isStopped = true;
                }

                if (m_CanRotate)
                {
                    AIMethods.RotationLookAt(transform, m_TargetTransform, m_RangeAttackRotationSlerp * Time.deltaTime);
                }
               
                MeleeAttack();



                return; 

            }
            if (distanceFromTarget <= m_DetectionDistance && m_IsRangeAttacking)
            {

                //print("rangeattack test ");
                if (m_NavMeshAgent.pathPending || m_NavMeshAgent.hasPath)
                {
                    print("rangeattack test  2 ");
                    //m_NavMeshAgent.ResetPath();
                    m_NavMeshAgent.isStopped = true;
                }
                RangeAttack();
                return; 
            }

            if (distanceFromTarget <= m_RangeAttackDistance)
            {
                //print("rangeattack test 3  ");

                if (m_NavMeshAgent.pathPending || m_NavMeshAgent.hasPath)
                {
                    //m_NavMeshAgent.ResetPath();
                    m_NavMeshAgent.isStopped = true;
                }

                RangeAttack();

                return; 
            }

            if (m_IsRangeAttacking)
            {
                //print("rangeattack test 4 ");
                return; 
            }

            if (distanceFromTarget <= m_DetectionDistance && !m_IsRangeAttacking)
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("RangeAttack"))
                {
                    //print("rangeattack test 7 ");
                    return; 
                }

                //print("rangeattack test 6 ");
                m_NavMeshAgent.SetDestination(m_TargetTransform.position);
                return; 
            }

            //print("rangeattack test5 ");


            //print("set destination 1");
            //m_NavMeshAgent.SetDestination(m_TargetTransform.position);


        }

        void AttackDebugger()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && m_IsAttacking)
            {
                m_IsAttacking = false; 
            }
        }

        

        void IsRangeAttacking()
        {
            m_IsRangeAttacking = true;
            m_IsAttacking = true; 
        }
        void IsNotRangeAttacking()
        {
            m_IsRangeAttacking = false;
            m_IsAttacking = false; 
        }
        void IsCloseAttacking()
        {
            m_IsCloseAttacking = true;
            m_IsAttacking = true;
        }
        void IsNotCloseAttacking()
        {
            m_IsCloseAttacking = false;
            m_IsAttacking = false;
        }

        public void RangeAttack( )
        {
            Transform target = m_TargetTransform;


            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                //print("currentlyInIdle");

                if (m_IsRangeAttacking)
                {
                    m_IsAttacking = false;

                    m_IsRangeAttacking = false; 
                }

            }

            if (!m_IsAttacking && !m_IsRangeAttacking)
            {


               
                m_Animator.ResetTrigger("CloseAttack");
                m_IsCloseAttacking = false; 
                m_Animator.SetTrigger("RangeAttack");


            }

            if (!AIMethods.IsOnLineSight(transform.position, target.gameObject, m_DetectionMask) && m_IsRangeAttacking)
            {
                m_Animator.ResetTrigger("RangeAttack");
                m_Animator.SetTrigger("StopTrigger");

                return;
            }


            if (m_CanRotate)
            {
                AIMethods.RotationLookAt(transform, target, m_RangeAttackRotationSlerp * Time.deltaTime);
            }

            
        }

        void RangeAttackArrow()
        {
            if (m_TargetTransform == null)
            {
                return; 
            }

            m_RangeAttackShootSound.Activation(m_ArrowPrepVFXAnchor);

            GameObject arrow = Instantiate(m_ArrowGameObject, m_ArrowAnchor.position, m_ArrowAnchor.rotation);

            arrow.GetComponent<MD_EnemyArrowScript>().GetBezierTransforms(m_ArrowAnchor, m_TargetTransform );

            

            
        }

        void ArrowPreparation()
        {
            m_CanRotate = true; 
             m_ArrowPrepInstantiatedGameObject = Instantiate(m_ArrowPrepFXGameObject, m_ArrowPrepVFXAnchor);

            m_RangeAttackLoadingSound.Activation(m_ArrowPrepVFXAnchor); 
        }

        void ArrowTimer()
        {



        }

       

        public void MeleeAttack()
        {
            Transform target = m_TargetTransform;

            

            if (!m_CanCloseAttack)
            {

                //print("melee attack 2 ");

                if (m_CanRotate)
                {
                    AIMethods.RotationLookAt(transform, target, m_RangeAttackRotationSlerp * Time.deltaTime);
                }
                

                return; 
    
            }

            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                //print("currentlyInIdle");

                if (m_IsCloseAttacking)
                {
                    //print("currentlyInIdle 2 ");
                    m_IsCloseAttacking = false;
                }
                
            }

            

            if (!m_IsCloseAttacking )
            {
                //print("melee attack 3");
                if (m_ArrowPrepInstantiatedGameObject !=null)
                {
                    Destroy(m_ArrowPrepInstantiatedGameObject);
                }

                
                m_Animator.ResetTrigger("RangeAttack");
                
                
                m_IsRangeAttacking = false;
                

                m_Animator.SetTrigger("CloseAttack");
                

                 

                //
            }

            if (Vector3.Distance(transform.position, target.position) > m_CloseAttackDistance)
            {
                

                //print("melee attack 4 ");
                m_Animator.ResetTrigger("CloseAttack");
                m_Animator.SetTrigger("StopTrigger");

                //print("too far");

                return; 
            }

            if (!AIMethods.IsOnLineSight(transform.position, target.gameObject, m_DetectionMask) && m_IsCloseAttacking)
            {
                //print("melee attack 5 ");
                m_Animator.ResetTrigger("CloseAttack");
                m_Animator.SetTrigger("StopTrigger");

                return;
            }


            if (m_CanRotate)
            {
                //print("melee attack 6 ");

                AIMethods.RotationLookAt(transform, target, m_RangeAttackRotationSlerp * Time.deltaTime);

            }

            
        }

        void CloseAttackSound()
        {
            m_CloseAttackSound.Activation(transform, true);
        }

        IEnumerator CloseAttackRoutine(float time)
        {
            m_CanCloseAttack = false;
            
            

            yield return new WaitForSeconds(time);

            m_CanCloseAttack = true;
        }

        IEnumerator StunRoutine(float time)
        {
            m_IsStun = true; 

            yield return new WaitForSeconds(time);
            m_IsStun = false;
        }

        void CloseAttackCooldown()
        {
            StartCoroutine(CloseAttackRoutine(m_CloseAttackCoolDown));
        }

        void MeleeAttackPS()
        {
            if (m_DetectionState == EDetectionState.Death)
            {
                return; 
            }

            if (!m_CanCloseAttack)
            {
                return; 
            }

            m_KickStartRenderer.material.SetFloat("_GlobalOpacity", 1);

            //Instantiate(m_MeleeAttackPS, m_MeleeAttackAnchor.position, m_MeleeAttackAnchor.rotation);

            foreach (ParticleSystem item in m_MeleeAttackPS.GetComponentsInChildren<ParticleSystem>())
            {
                item.Play(); 
            }   
           
        }

        void MeleeAttackDamage()
        {
            if (!m_TargetTransform)
            {
                return; 
            }

            if (m_IsStun)
            {
                return; 
            }

            if (Vector3.Distance(m_TargetTransform.position, transform.position)<= m_CloseAttackDistance)
            {
                if (AIMethods.IsFOV(m_CloseAttackAnle, transform, m_TargetTransform.position))
                {
                    if (m_TargetTransform.gameObject.TryGetComponent<IDamageable>(out IDamageable player))
                    {
                        m_CloseAttackHitSound.Activation(transform);

                        

                        GameObject hitPS;

                        hitPS = Instantiate(m_MeleeAttackHitPS, m_MeleeAttackAnchor.position, m_MeleeAttackHitPS.transform.rotation);

                        Destroy(hitPS, 2); 


                        player.TakeDamage(m_KickDamage, IDamageable.EAttackSource.Melee); 
                    }
                }
            }
        }

        #endregion

        #region Enemy data

       void StepSound()
        {
            m_StepSound.AddAndActivate(transform); 
        }

        void CanRotate()
        {
            m_CanRotate = true; 
        }

        void CannotRotate()
        {
            m_CanRotate = false; 
        }

        public void ResetAttacks()
        {
            m_IsAttacking = false;
            m_IsRangeAttacking = false;
            m_CanRotate = true; 
        }

        IEnumerator KickCancelFade(float time)
        {

            print("cancel opacity");

            float i = 0;
            float rate = 1 / time;

            MaterialPropertyBlock mpb = new MaterialPropertyBlock(); 


            while(i < 1)
            {
                 
                mpb.SetFloat("_GlobalOpacity", m_KickStartCurve.Evaluate(i));

                m_KickStartRenderer.material.SetFloat("_GlobalOpacity", m_KickStartCurve.Evaluate(i)); 


                i += rate * Time.deltaTime;

                yield return 0; 
            }

            foreach (ParticleSystem item in m_MeleeAttackPS.GetComponentsInChildren<ParticleSystem>())
            {
                item.Stop();
                
            }




        }

        public void TakeDamage(int dmg)
        {

            StartCoroutine(KickCancelFade(m_KickStartFadeTime));



            m_Animator.ResetTrigger("CloseAttack"); 

            print("taking " + dmg + "damage");
            const int DAMAGE_ANIMATION_NUMBER = 2;


            StartCoroutine(CloseAttackRoutine(m_StunTime));
            StartCoroutine(StunRoutine(m_StunTime));

            ResetAttacks(); 

            m_Animator.SetTrigger("StopTrigger");
            m_Animator.SetInteger("DamagedNumber",UnityEngine.Random.Range(0, DAMAGE_ANIMATION_NUMBER));    
            m_Animator.SetTrigger("DamageTrigger");

            

            //m_Animator.



            m_EnemyLife = Mathf.Clamp(m_EnemyLife - dmg, 0, m_EnemyMaxLife);

            m_EnemyHurtSound.AddAndActivate(transform); 


            CheckLife();

            

            
        }

        public void TakeDamage(int dmg, IDamageable.EAttackSource attackStyle)
        {

            TakeDamage(dmg);

            if (S_GameManager.Instance.CurrentPlayer != null)
            {
                Aggro(S_GameManager.Instance.CurrentPlayer.transform);
            }
        }

        public void KillEntity()
        {
            SwitchDetectionState(EDetectionState.Death);
        }

        void CheckLife()
        {
            if (m_EnemyLife <= 0)
            {
                KillEntity();  //death
            }
        }


        IEnumerator DeathRoutine(float delay, float timeBeforeDesactivate)
        {

           

            CapsuleCollider capsuleCollider  =GetComponent<CapsuleCollider>();

            yield return new WaitForSeconds(delay);

            //

            Destroy(m_NavMeshAgent);
            gameObject.layer = 0;
            gameObject.tag = "Untagged";
            capsuleCollider.isTrigger = true; 
            

            yield return new WaitForSeconds(timeBeforeDesactivate);

            //print("destroying ");



            Destroy(capsuleCollider);


            m_Animator.enabled = false;
            

            
            
            Destroy(m_RigidBody);

            Destroy(this);

            StopAllCoroutines();
            
             

        }

        void OnGameStateChange(S_GameManager.EGameState state)
        {
            switch (state)
            {
                case S_GameManager.EGameState.Menu:
                    break;
                case S_GameManager.EGameState.Ingame:

                    ResetAI(); 

                    break;
                case S_GameManager.EGameState.Puzzle:
                    break;
                case S_GameManager.EGameState.Pause:
                    break;
                case S_GameManager.EGameState.Death:


                    ResetAI(); 

                    

                    break;
                default:
                    break;
            }
        }


        void ResetAI()
        {
            m_TargetTransform = null;
            m_Animator.SetTrigger("StopTrigger");
            SwitchDetectionState(EDetectionState.Waiting);
            m_IsPlayerInMind = false;
            ResetAttacks();
        }

        #endregion

        public IEntityStyle.Style GetStyle()
        {
            return m_Style; 
        }

        public void Heal(int healAmount)
        {
            m_EnemyLife += healAmount;

            Mathf.Clamp(m_EnemyLife += healAmount, 0, m_EnemyMaxLife); 
        }

        public void Aggro(Transform posTransform)
        {
            if (m_DetectionState == EDetectionState.Death)
            {
                return;
            }

            if (m_DetectionState == EDetectionState.Aggro)
            {
                return; 
            }

            if (m_NavMeshAgent == null)
            {
                print("no navmeshagent"); 
                return; 

            }

            print("enemy aggro : " + transform.name + " on : " + transform.name); 
            SwitchDetectionState(EDetectionState.Suspicious);
            m_NavMeshAgent.SetDestination(posTransform.position);
            m_LastTargetPos = posTransform.position; 
            m_TargetTransform = posTransform; 
        }

        #region Gizmos

        private void OnDrawGizmos()
        {
            if (!m_IsGizmosActivated)
            {
                return; 

            }
            

            switch (m_GizmosState)
            {
                case EGizmosState.Detection:

                    Gizmos.color = Color.gray;
                    Gizmos.DrawWireSphere(transform.position, m_DetectionDistance);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(transform.position, m_RangeAttackDistance);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(transform.position, m_CloseAttackDistance);

                    Gizmos.color = Color.white;
                    AIMethods.ShowFOV(m_FovAngle, m_DetectionDistance, transform, m_GizmosDrawFrequency);


                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireCube(m_LastTargetPos, Vector3.one / 2);

                    if (m_TargetTransform != null)
                    {

                        if (!AIMethods.IsFOV(m_FovAngle, transform, m_TargetTransform.position))
                        {
                            return; 
                        }

                        if (Vector3.Distance(transform.position, m_TargetTransform.position) > m_DetectionDistance)
                        {
                            return; 
                        }


                        AIMethods.IsOnLineSightGizmos(transform.position, m_TargetTransform.gameObject, m_DetectionMask);
                    }

                    

                    

                    break;
                case EGizmosState.Pattern:

                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(transform.position, m_RandomPatrolDistance); 


                    if (m_PatrolPointList.Count >0)
                    {
                        foreach (var item in m_PatrolPointList)
                        {
                            Gizmos.DrawWireCube(item.position, Vector3.one / 2);
                        }
                    }

                    

                    break;
                case EGizmosState.CloseAttack:

                    AIMethods.ShowFOV(m_CloseAttackAnle, m_CloseAttackDistance, transform, m_GizmosDrawFrequency);

                    break;
                case EGizmosState.RangeAttack:

                    AIMethods.ShowFOV(m_RangeAttackAngle, m_RangeAttackDistance, transform, m_GizmosDrawFrequency); 
                    break;
                default:
                    break;
            }
        }

        

        #endregion

    }
}
