using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Shared;
using System.Diagnostics;

namespace DumortierMatthieu
{

    enum EMovementState
    {
        Idle,
        Moving,
        Dead
    }


    public class MD_PlayerMovementScript : MonoBehaviour, IStunnable
    {

        [SerializeField]
        EMovementState m_MoveState = EMovementState.Idle;

        NavMeshAgent m_NavMeshAgent;
        Rigidbody m_Rigidbody;

        Animator m_Animator;
        PlayerInput m_PlayerInput;


        //[SerializeField]
        //float m_PlayerSpeed = 5f;

        [SerializeField]
        bool m_IsUsingNavMeshToMove = false; 
        [SerializeField]
        [Tooltip(" Set the minimum abs addition of vector 2 directional values for the player to move.  Limitate iceWalking ")]
        float m_PlayerMinimumInputValue = 0.5f;

        [SerializeField]
        float m_RotationSpeed = 0.5f;
        [SerializeField]
        Camera m_Camera;
        [SerializeField]
        bool m_CanRotate = true;
        [SerializeField]
        bool m_CanMove = true;
        [SerializeField]
        bool m_IsPause = false; 




        [SerializeField]
        float m_BreakIdleTimer = 5f;




        [Space(15)]
        [Header("[SOUNDS ] //////////////////////////////////////////////////")]
        [Space(15)]
        


        AudioSource m_AudioSource;
        [SerializeField]
        MD_SoundManager m_DeathSound;

        [SerializeField]
        MD_SoundManager m_RandomVoiceLineSound;

        [SerializeField]
        MD_SoundManager m_PlayerStepSound; 




        private void Awake()
        {

            m_AudioSource = GetComponent<AudioSource>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Camera = Camera.main; 
            m_Animator = GetComponent<Animator>();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_PlayerInput = new PlayerInput();

            if (Shared.CameraManager.Instance != null)
            {
                Shared.CameraManager.Instance.SetPlayerTransform(transform);
            }

            
        }


        private void OnEnable()
        {
            if (Shared.CameraManager.Instance != null)
            {
                Shared.CameraManager.Instance.SetPlayerTransform(transform);
            }

            if (S_GameManager.Instance != null)
            {
                S_GameManager.Instance.CurrentPlayer = this.gameObject;
            }
           
            

            m_PlayerInput.Character.Enable();

            MD_PlayerAttacks.OnStandingAttack += LockMove;
            MD_PlayerAttacks.OnRotationLock += LockRotation;
            MD_PlayerAttacks.OnPlayerUnlock += UnlockTransform;
            MD_PlayerAttacks.OnPlayerLock += LockTransform;
            S_GameManager.OnPause += PauseMode;

            S_GameManager.OnSwitchState += OnGameSwitchState;

            S_GameManager.OnPlayerControllerInput += CanInput; 

        }

        private void OnDisable()
        {

            //m_PlayerInput.Disable();

            m_PlayerInput.Character.Disable(); 

            MD_PlayerAttacks.OnStandingAttack -= LockMove;
            MD_PlayerAttacks.OnRotationLock -= LockRotation;
            MD_PlayerAttacks.OnPlayerUnlock -= UnlockTransform;
            MD_PlayerAttacks.OnPlayerLock -= LockTransform;
            S_GameManager.OnPause -= PauseMode;

            S_GameManager.OnSwitchState -= OnGameSwitchState;

            S_GameManager.OnPlayerControllerInput -= CanInput;



        }
        void Start()
        {
            S_GameManager.Instance.CurrentPlayer = this.gameObject;
            S_GameManager.Instance.CurrentPlayerIndex = 4; //debug 


          
        }

        // Update is called once per frame
        void Update()
        {

            UpdateState();

        }

        void UpdateState()
        {

            if (m_IsPause)
            {
                return; 
            }

            

            switch (m_MoveState)
            {
                case EMovementState.Idle:
                    TransformMovement();

                    BreakIdle();
                    break;
                case EMovementState.Moving:
                    TransformMovement();
                    break;
                case EMovementState.Dead:
                    break;
                default:
                    break;
            }
        }

        private void FixedUpdate()
        {

            FixedUpdateState();


            //TransformMovement();


        }

        void FixedUpdateState()
        {
            switch (m_MoveState)
            {
                case EMovementState.Idle:
                    break;
                case EMovementState.Moving:
                    break;
                case EMovementState.Dead:
                    break;
                default:
                    break;
            }
        }

        void SwitchState(EMovementState state)
        {
            if (state == m_MoveState)
            {
                return;
            }

            OnExitState(m_MoveState);
            m_MoveState = state;
            OnEnterState(m_MoveState);
        }

        void OnEnterState(EMovementState state)
        {
            switch (state)
            {
                case EMovementState.Idle:

                    
                    m_Animator.ResetTrigger("BreakIdle");
                    AddBreakIdleTimer();
                    break;
                case EMovementState.Moving:
                    m_Animator.ResetTrigger("BreakIdle");

                    break;
                case EMovementState.Dead:

                    //print("Player death movement state");

                    GetComponent<MD_PlayerAttacks>().EndDash(); 

                    LockTransform();
                    m_PlayerInput.Character.Disable();
                    m_Animator.SetTrigger("DeathTrigger");

                    m_DeathSound.Activation(transform, true); 



                    Destroy(m_NavMeshAgent);
                    Destroy(GetComponent<CapsuleCollider>());
                     
                    

                    break;
                default:
                    break;
            }
        }

        void OnExitState(EMovementState state)
        {
            switch (state)
            {
                case EMovementState.Idle:
                    break;
                case EMovementState.Moving:
                    break;
                case EMovementState.Dead:
                    break;
                default:
                    break;
            }
        }


        


        public void TransformMovement()
        {

            //move 

            Vector2 direction = m_PlayerInput.Character.Move.ReadValue<Vector2>();




            direction = NoIceWalking(direction);






            CheckMoving(direction);  // state


            //debug 
            //print(direction);

            Vector3 playerDirection = new Vector3(direction.x, 0, direction.y);

            playerDirection = m_Camera.transform.rotation * playerDirection;

            playerDirection = new Vector3(playerDirection.x, 0, playerDirection.z);

            //transform.position += playerDirection * Time.deltaTime * m_PlayerSpeed; // if not on navmesh // see further for error if needed

            //navmesh

            if (m_CanMove)
            {

                if (m_IsUsingNavMeshToMove && m_NavMeshAgent.isActiveAndEnabled && m_NavMeshAgent.isOnNavMesh && m_NavMeshAgent != null)
                {


                    try
                    {
                        m_NavMeshAgent.Move(playerDirection * Time.deltaTime * m_NavMeshAgent.speed);
                    }
                    catch (MissingReferenceException)
                    {

                        return;
                    }
                    
                    

                }
                else
                {
                    
                    transform.position += (playerDirection * Time.deltaTime * m_NavMeshAgent.speed); 
                }

            }





            //m_rotationF

            if (m_CanRotate)
            {
                RotationMovement(playerDirection);
            }






        }

        void RotationMovement(Vector3 playerDirection)
        {
            float targetAngle = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg;

            if (targetAngle != 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, targetAngle, 0f), m_RotationSpeed * Time.fixedDeltaTime);
            }

        }

        void CheckMoving(Vector2 direction)
        {
            if (!m_CanMove  )
            {
                m_Animator.SetFloat("Speed", 0);
                return;
            }

            if (direction == Vector2.zero)
            {




                SwitchState(EMovementState.Idle);

                m_Animator.SetFloat("Speed", 0);


            }
            else
            {
                //print("velocity by readvalue = " + (Mathf.Clamp01(Mathf.Abs(direction.x) + Mathf.Abs(direction.y))));

                SwitchState(EMovementState.Moving);


                m_Animator.SetFloat("Speed", Mathf.Clamp01(Mathf.Abs(direction.x) + Mathf.Abs(direction.y)));
            }
        }

        Vector2 NoIceWalking(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) + Mathf.Abs(direction.y) <= m_PlayerMinimumInputValue)
            {
                return Vector2.zero;
            }

            return direction;
        }

        void PauseMode(bool isPause)
        {
            LockRotation(isPause);
            m_IsPause = isPause;
        }

        void LockRotation()
        {
            LockRotation(true);
        }
        void LockMove()
        {
            LockMove(true);
        }

        void LockTransform()
        {
            LockMove();
            LockRotation();
        }


        void UnlockTransform()
        {
            m_CanRotate = true;
            m_CanMove = true;
        }

        void LockRotation(bool isLock)
        {

            
            
            m_CanRotate = !isLock;
        }

        void LockMove(bool isLock)
        {
            m_CanMove = !isLock;
        }

        void CanInput(bool canInput)
        {
            if (canInput)
            {
                m_PlayerInput.Enable();
            }
            else
            {
                m_PlayerInput.Disable(); 
            }
        }

        void BreakIdle()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(1).IsTag("CloseAttack"))
            {
                m_Animator.SetTrigger("StopBreakIdle");
                return;
            }


            if (Time.time > m_BreakIdleTimer)
            {

                //print("set trigger");
                m_Animator.SetTrigger("BreakIdle");

                m_RandomVoiceLineSound.AddAndActivate(transform); 
                AddBreakIdleTimer();
            }


        }

        void AddBreakIdleTimer()
        {

            const float MIN = 15f;
            const float MAX = 35f;

            m_BreakIdleTimer = Time.time + Random.Range(MIN, MAX);

        }

        void OnGameSwitchState(S_GameManager.EGameState state)
        {
            switch (state)
            {
                case S_GameManager.EGameState.Menu:
                    SwitchState(EMovementState.Dead);
                    break;
                case S_GameManager.EGameState.Ingame:
                    //print("puzzle test 2");
                    //m_PlayerInput.Enable();
                    SwitchState(EMovementState.Idle); 

                    break;
                case S_GameManager.EGameState.Puzzle:
                    m_PlayerInput.Disable(); 
                    break;
                case S_GameManager.EGameState.Pause:

                    
                    break;
                case S_GameManager.EGameState.Death:

                    SwitchState(EMovementState.Dead); 
                    break;
                default:
                    break;
            }
        }

        void StepSound()
        {
            m_PlayerStepSound.AddAndActivate(transform); 
        }


        

        #region Stun interface methods

        public void Rooted(float time)
        {
            StartCoroutine(RootRoutine(time));
        }

        public void Rooted()
        {
            

            m_CanMove = false;
        }

        public void UnRooted()
        {
            m_CanMove = true; 
        }

        public void Stunned()
        {
            LockTransform(); 
        }

        public void Stunned(float time)
        {
            StartCoroutine(StunRoutine(time));
        }

        public void UnStunned()
        {
            UnlockTransform(); 
        }

        IEnumerator StunRoutine(float time)
        {
            LockTransform();

            yield return new WaitForSeconds(time);

            UnlockTransform(); 
        }


        IEnumerator RootRoutine(float time)
        {
            m_CanMove = false;

            yield return new WaitForSeconds(time);

            m_CanMove = true; 
        }
        #endregion
    }
}
