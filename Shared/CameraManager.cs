using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumortierMatthieu.AI;
using UnityEngine.InputSystem.HID;
using System.Diagnostics;
using static UnityEngine.Rendering.DebugUI;

namespace Shared
{


     

    public class CameraManager : MonoBehaviour
    {

        public enum ECameraState
        {
            Follow,
            Lock,
            Puzzle
        }


        public static CameraManager Instance { get; private set; }
        [SerializeField]
        ECameraState m_CameraState = ECameraState.Follow;

        ECameraState m_CameraStateFlag = ECameraState.Follow;

        public ECameraState CameraState
        {
            get
            {
                return m_CameraState; 
            }
            set
            {
                m_CameraState = value;  
            }
        }
        Camera m_Camera;

        [SerializeField]
        bool m_IsLock = false; 

        [SerializeField]
        Transform m_PlayerTransform;

        [SerializeField]
        Vector3 m_EuleurRotation = new Vector3(35f, -35f, 0);

        public Vector3 EuleurRotation
        {
            get
            {
                return m_EuleurRotation;
            }
            set
            {
                m_EuleurRotation = value;
            }
        }

        //[SerializeField]
        //List<Transform> m_PuzzleActiveTransformList = new List<Transform>();



        [SerializeField]
        Transform m_LockActiveTransform = null;

        public Transform LockActiveTransform
        {
            get
            {
                return m_LockActiveTransform; 
            }
            set
            {
                m_LockActiveTransform = value;
            }
        }


        [SerializeField]
        CameraCharParams[] m_CharCameraParamList = new CameraCharParams[5];

        

        [SerializeField]
        [Range(0, 15)]
        float m_Dezoom = 5f;
        public float Dezoom
        {
            get { return m_Dezoom; }
            set { m_Dezoom = value; }
        }
        [SerializeField]
        [Range(1, 16)]
        float m_Height = 5f;
        public float Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }
        [SerializeField]
        float m_ForwardDistanceFromPlayer = 2f;

        //[SerializeField]
        //float m_MinDistanceFromPlayer = 3f; 

        [SerializeField]
        LayerMask m_DetectionMask;

        [SerializeField]
        float m_OccludeLerpSpeed = 10f; 

        [SerializeField]
        float m_RemovingDistance = 20;

        [SerializeField]
        List<GameObject> m_EnvironmentHiddenRendererList = new List<GameObject>();



        [SerializeField]
        float m_PosLerpSpeed = 1f;


        [SerializeField]
        float m_RotLerpSpeed = 5f;
        [SerializeField]
        float m_LockTime = 5f; 
        public float LockTimee
        {
            get
            {
                return m_LockTime; 
            }

            set
            {
                m_LockTime = value; 
            }
        }

        public float PosLerpSpeed
        {
            get
            {
                return m_PosLerpSpeed;
            }
            set
            {
                m_PosLerpSpeed = value;
            }
        }

        public float RotLerpSpeed
        {
            get
            {
                return m_RotLerpSpeed;
            }
            set
            {
                m_RotLerpSpeed = value;
            }
        }


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            m_Camera = GetComponent<Camera>();



        }

        private void OnEnable()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            m_Camera = GetComponent<Camera>();

            S_GameManager.OnPlayerSwitch += SwitchCameraParams;
            S_GameManager.OnSwitchState += GameManagerSwitchState; 
        }
        private void OnDisable()
        {
            S_GameManager.OnSwitchState -= GameManagerSwitchState;
        }

        void Start()
        {
            
            SwitchCameraParams();


            SetRotation(m_EuleurRotation);
            //StartCoroutine(ToLockPos(10));

            m_CameraStateFlag = m_CameraState; 
        }

        // Update is called once per frame
        void Update()
        {




            if (m_PlayerTransform == null)
            {
                return;
            }

            //HiddingEnvironment();
            StateVerifUpdate(); 
            UpdateState();
        }

        void StateVerifUpdate()
        {
            if (m_CameraStateFlag != m_CameraState)
            {
                OnExitState(m_CameraStateFlag);
                m_CameraStateFlag = m_CameraState;
                OnEnterState(m_CameraState);
            }

        }

        void SwitchCameraParams()
        {
            if (m_IsLock)
            {
                return; 
            }

            m_Dezoom = m_CharCameraParamList[S_GameManager.Instance.CurrentPlayerIndex].Dezoom ;
            m_Height = m_CharCameraParamList[S_GameManager.Instance.CurrentPlayerIndex].Height ;
            m_ForwardDistanceFromPlayer = m_CharCameraParamList[S_GameManager.Instance.CurrentPlayerIndex].DistanceFromPlayer ;
        }

        private void FixedUpdate()
        {

            //SetPos(m_PlayerTransform);
            FixedUpdateState();


        }

        #region StateMachine

        void UpdateState()
        {
            switch (m_CameraState)
            {
                case ECameraState.Follow:

                    SetPos(m_PlayerTransform);


                    break;
                case ECameraState.Lock:

                    //print("LockState");

                    if (m_LockActiveTransform == null)
                    {
                        return;
                    }

                    SetPuzzlePos(m_LockActiveTransform);
                    break;
                case ECameraState.Puzzle:

                    //if (m_PuzzleActiveTransformList.Count ==0)
                    //{
                    //    return; 
                    //}
                    if (m_LockActiveTransform == null)
                    {
                        return; 
                    }

                    SetPuzzlePos(m_LockActiveTransform);
                    break;
                default:
                    break;
            }
        }

        void FixedUpdateState()
        {
            switch (m_CameraState)
            {
                case ECameraState.Follow:


                    break;
                case ECameraState.Lock:
                    break;
                case ECameraState.Puzzle:
                    break;
                default:
                    break;
            }
        }

        void SwitchState(ECameraState state)
        {

            if (state == m_CameraState)
            {
                return;
            }

            OnExitState(m_CameraState);
            m_CameraState = state;
            OnEnterState(m_CameraState);

        }

        void GameManagerSwitchState(S_GameManager.EGameState state)
        {
            switch (state)
            {
                case S_GameManager.EGameState.Menu:
                    break;
                case S_GameManager.EGameState.Ingame:

                    //SwitchState(ECameraState.Follow); 

                    break;
                case S_GameManager.EGameState.Puzzle:
                    //SwitchState(ECameraState.Puzzle);

                    break;
                case S_GameManager.EGameState.Pause:
                    break;
                case S_GameManager.EGameState.Death:
                    break;
                default:
                    break;
            }
        }

        void OnEnterState(ECameraState state)
        {

            switch (m_CameraState)
            {
                case ECameraState.Follow:


                    break;
                case ECameraState.Lock:
                    //print("enter lockpos"); 
                    StartCoroutine( ToLockPos(m_LockTime)); 
                    break;
                case ECameraState.Puzzle:
                    break;
                default:
                    break;
            }

        }

        void OnExitState(ECameraState state)
        {

            switch (m_CameraState)
            {
                case ECameraState.Follow:


                    break;
                case ECameraState.Lock:
                    break;
                case ECameraState.Puzzle:
                    break;
                default:
                    break;
            }

        }


        #endregion




        void SetRotation(Vector3 euleurRotation)
        {
            transform.rotation = Quaternion.Euler(euleurRotation);
        }


        void SetPos (Transform pos)
        {
            Vector3 newPos = pos.position + (pos.forward * m_ForwardDistanceFromPlayer);

            newPos = Vector3.Lerp(transform.position, newPos + GetPosOffset(m_Dezoom, m_Height), m_PosLerpSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(m_EuleurRotation), m_RotLerpSpeed * Time.deltaTime);


            transform.position = newPos;
            // print("camera test 4");

            if (Physics.Raycast(transform.position, m_PlayerTransform.position - transform.position, out RaycastHit hito, 20f, m_DetectionMask))
            {
                transform.position = Vector3.Lerp(transform.position,hito.point, m_OccludeLerpSpeed * Time.deltaTime);
                return; 
            }
            





        }

        void SetPuzzlePos(Transform pos)
        {
            Vector3 newPos = pos.position;

            Quaternion newRot = pos.rotation; 

            newPos = Vector3.Lerp(transform.position, newPos , m_PosLerpSpeed * Time.deltaTime);


            newRot = Quaternion.Slerp(transform.rotation, newRot, m_RotLerpSpeed * Time.deltaTime); 

            transform.position = newPos;

            transform.rotation = newRot;
           

            if (Physics.Raycast(transform.position, m_PlayerTransform.position - transform.position, out RaycastHit hito, 20f, m_DetectionMask))
            {
                transform.position = Vector3.Lerp(transform.position, hito.point, m_OccludeLerpSpeed * Time.deltaTime);
                return;
            }


        }

         IEnumerator ToLockPos(float time)
        {

            SwitchState(ECameraState.Lock); 

            yield return new WaitForSeconds(time);

            SwitchState(ECameraState.Follow); 
        }

        void SearchingNextPos()
        {
            Vector3 newPos = Vector3.Lerp(transform.position, m_PlayerTransform.position + Vector3.up, Time.deltaTime * m_PosLerpSpeed);

            transform.position = newPos;
        }

        public void SetPlayerTransform(Transform playerTransform)
        {

            m_PlayerTransform = playerTransform;

        }

        Vector3 GetPosOffset(float dezoom, float height)
        {
            Vector3 offset = new Vector3(dezoom, height, -dezoom);

            return offset;
        }

        void HiddingEnvironment()
        {

            AddingMeshInViewInList();
            EnablingNotUccludingMeshes();
            CleaningMeshInViewList();




        }

        


        void AddingMeshInViewInList()
        {

           


            Collider touchedCollider;
            if (Physics.Linecast(transform.position, m_PlayerTransform.position + Vector3.up, out RaycastHit hit, m_DetectionMask))
            {
                touchedCollider = hit.collider;

                MeshRenderer touchedColliderRenderer;

                if (touchedCollider.gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
                {
                    touchedColliderRenderer = renderer;
                    touchedColliderRenderer.enabled = false;


                    if (!m_EnvironmentHiddenRendererList.Contains(touchedColliderRenderer.gameObject))
                    {
                        m_EnvironmentHiddenRendererList.Add(touchedColliderRenderer.gameObject);
                    }

                }
            }
        }

        void EnablingNotUccludingMeshes()
        {
            if (m_EnvironmentHiddenRendererList.Count > 0)
            {


                for (int i = 0; i < m_EnvironmentHiddenRendererList.Count; i++)
                {
                    if (AIMethods.IsTargetFirstTouched(transform.position, m_PlayerTransform.gameObject))
                    {
                        m_EnvironmentHiddenRendererList[i].GetComponent<Renderer>().enabled = true;

                    }
                }


            }
        }

        void CleaningMeshInViewList()
        {
            for (int i = 0; i < m_EnvironmentHiddenRendererList.Count; i++)
            {
                if (Vector3.Distance(m_EnvironmentHiddenRendererList[i].transform.position, transform.position) > m_RemovingDistance)
                {
                    m_EnvironmentHiddenRendererList[i].GetComponent<Renderer>().enabled = true;
                    m_EnvironmentHiddenRendererList.Remove(m_EnvironmentHiddenRendererList[i]);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //print("Camera Collision"); 
        }

        





        private void OnDrawGizmos()
        {
            if (m_PlayerTransform == null)
            {
                return;
            }

            Gizmos.color = Color.gray;

            Gizmos.DrawLine(m_PlayerTransform.position, transform.position);


            //AIMethods.IsOnLineSightGizmos(transform.position - Vector3.up, m_PlayerTransform.gameObject, m_DetectionMask);

            Gizmos.color = Color.blue;

            Gizmos.DrawRay(transform.position, m_PlayerTransform.position - transform.position);

            Gizmos.color = Color.cyan; 

            if (Physics.Raycast(transform.position, m_PlayerTransform.position - transform.position, out RaycastHit hito,20f, m_DetectionMask ))
            {
                Gizmos.DrawRay(transform.position, m_PlayerTransform.position - transform.position);
            }


            if (Physics.Linecast(transform.position, m_PlayerTransform.position, out RaycastHit hit, m_DetectionMask))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(hit.point, 1f); 
            }


        }


    }



}
