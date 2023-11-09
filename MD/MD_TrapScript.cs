using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumortierMatthieu.AI;
using Shared;
using Unity.VisualScripting;

using Unity.VisualScripting.InputSystem;

namespace DumortierMatthieu
{
    public class MD_TrapScript : MonoBehaviour
    {

        Animator m_Animator;
        BoxCollider m_BoxCollider;
        Renderer m_Renderer;
        MaterialPropertyBlock m_MaterialPropertyBlock;
        float m_randomLightTime;

        [SerializeField]
        LayerMask m_TrappedMask;
        [SerializeField]
        LayerMask m_AggroMask;
        [Space(10)]
        [Header("Gameplay values")]
        [Space(10)]

        [SerializeField]
        float m_RootTime;
        [SerializeField]
        int m_Damage = 35; 
        [SerializeField]
        float m_ActivationDistance = 2f;
        [SerializeField]
        bool m_IsActivated = false;

        [SerializeField]
        float m_AggroRadius = 20f;

        [SerializeField]
        float m_AggroTicNumber = 6f;

        [SerializeField]
        List<Collider> m_EnemyAggroList = new List<Collider>();

        [Space(10)]
        [Header("Light values")]
        [Space(10)]

        [SerializeField]
        Light m_Light;
        [SerializeField]
        float m_LightSpeed = 5f;

        [SerializeField]
        float m_globalOpacity;

        [SerializeField]
        float m_DistanceThreshold = 15f; 

        [SerializeField]
        Transform m_PlayerTransform; 

        [Space(10)]
        [Header("Postprocess values")]
        [Space(10)]

        [SerializeField]
        Material m_PostProcessMaterial;

        PostProcessScript m_PostProcessScript;

        [SerializeField]
        float m_RadarValue = 0f;


        [SerializeField]
        float m_ActivationTime = 3f; 

        [SerializeField]
        float m_Speed = 1f;

        [SerializeField]
        Transform m_SoundAlertTransform;

        [SerializeField]
        AnimationCurve m_FadeCurve;

        [Space(10)]
        [Header("Gizmos values")]
        [Space(10)]

        [SerializeField]
        bool m_IsGizmos = false;


        [SerializeField]
        MD_SoundManager m_ActivationSound;
        [SerializeField]
        MD_SoundManager m_AlertSound;

        int m_SoundClipChoice;

        [SerializeField]
        int m_SoundClipChoiceSize = 3;

        [SerializeField]
        float m_AlertRatio = 1f;

        [SerializeField]
        int m_AlertTicNumber = 3;



        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_BoxCollider = GetComponent<BoxCollider>();
            m_Renderer = GetComponentInChildren<Renderer>();

            m_MaterialPropertyBlock = new MaterialPropertyBlock();
        }
        void Start()
        {

            m_SoundClipChoice  = Random.Range(0, m_SoundClipChoiceSize);

            m_randomLightTime = transform.position.z + transform.position.x * Random.insideUnitCircle.x;

            m_PostProcessScript = Camera.main.GetComponent<PostProcessScript>();
            m_PostProcessMaterial.SetFloat("_Value", 0);
            m_PostProcessMaterial.SetFloat("_GlobalOpacity", 0);

            
            StartCoroutine(StartEndOfFrame());
        }

        IEnumerator StartEndOfFrame()
        {
            yield return new WaitForEndOfFrame();

            m_PlayerTransform = S_GameManager.Instance.CurrentPlayer.transform;
        }


        private void OnEnable()
        {
            S_GameManager.OnPlayerSwitch += PlayerPosUpdate; 
        }

        private void OnDisable()
        {
            S_GameManager.OnPlayerSwitch -= PlayerPosUpdate;
        }

        void PlayerPosUpdate()
        {
            m_PlayerTransform = S_GameManager.Instance.CurrentPlayer.transform; 
        }
        // Update is called once per frame
        void Update()
        {
            if (m_IsActivated)
            {
                m_RadarValue += Time.deltaTime * m_Speed;


                m_PostProcessMaterial.SetFloat("_Value", m_RadarValue);

            }

            if (m_PlayerTransform != null)
            {
                m_globalOpacity = Vector3.Distance(transform.position, m_PlayerTransform.position);
                m_globalOpacity /= m_DistanceThreshold;


                m_globalOpacity = Mathf.Clamp01(m_globalOpacity);

                m_globalOpacity = 1 - m_globalOpacity;
            }

           

            
            float lightIntensity = (Mathf.Sin((Time.realtimeSinceStartup + m_randomLightTime  )* m_LightSpeed) + 1) * 0.5f;

            lightIntensity *= m_globalOpacity; 
            m_Light.intensity = lightIntensity;


            m_MaterialPropertyBlock.SetFloat("_Speed", lightIntensity); 
            m_MaterialPropertyBlock.SetFloat("_GlobalEmissionOpacity", m_globalOpacity); 





            m_Renderer.SetPropertyBlock(m_MaterialPropertyBlock);
        }


        void Activation(Collider entity)
        {
            if (m_IsActivated)
            {
                return; 
            }

            if (m_Animator == null) return;

            m_PostProcessMaterial.SetFloat("_Value", 0);
            m_PostProcessMaterial.SetFloat("_GlobalOpacity", 1);

            m_Animator.enabled = true;

            m_ActivationSound.Activation(transform); 

            m_PostProcessScript.enabled = true;

            m_PostProcessMaterial.SetVector("_Pos", m_SoundAlertTransform.position);
            m_IsActivated = true; 


            if (entity.TryGetComponent<IStunnable>(out IStunnable stunEntity))
            {
                stunEntity.Rooted(m_RootTime);
                StartCoroutine(RootRoutine(m_RootTime));
            }

            if (entity.TryGetComponent<IDamageable>(out IDamageable damageEntity))
            {
                damageEntity.TakeDamage(m_Damage);
                StartCoroutine(RootRoutine(0));
            }


            

            Invoke(nameof(StopSoundAlert), m_ActivationTime);
            StartCoroutine(FadeOpacity(m_ActivationTime));

            //AggroEnemies(); 
           

        }

        IEnumerator RootRoutine(float time)
        {
            yield return new WaitForSeconds(time);

            if (m_BoxCollider != null)
            {
                //Destroy(m_BoxCollider);
                m_BoxCollider.isTrigger = false;
                GetComponent<Rigidbody>().isKinematic = false;
            }


        }

        void AggroEnemies()
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, m_AggroRadius, m_AggroMask);

           foreach (Collider c in enemies)
            {
                if (!m_EnemyAggroList.Contains(c))
                {
                    if (c.TryGetComponent<IAggro>(out IAggro enemy))
                    {
                        enemy.Aggro(transform);
                    }

                    m_EnemyAggroList.Add(c); 
                }
            }

            //foreach (var item in enemies)
            //{
            //    if (item.TryGetComponent<IAggro>(out IAggro enemy))
            //    {
            //        enemy.Aggro(transform); 
            //    }
            //}

            
        }

        void StopSoundAlert()
        {
            m_PostProcessScript.enabled = false;
        }

        IEnumerator RepeatSoudAlertRoutine(float delay, int number)
        {

            int numberFlag = 0; 

            while (numberFlag < number)
            {
                m_AlertSound.AddAndActivate(m_SoundAlertTransform, m_SoundClipChoice);
                yield return new WaitForSeconds(delay);
                numberFlag++;
            }


            
        }

        IEnumerator FadeOpacity(float time)
        {

            StartCoroutine(RepeatSoudAlertRoutine(m_AlertRatio, m_AlertTicNumber)); 


            float i = 0;
            float rate = 1 / time;

            float tic = 1 / m_AggroTicNumber;
            float ticFlag = tic;

            


            while (i < 1)
            {
                if (i > ticFlag)
                {
                    AggroEnemies();
                     
                    ticFlag += tic; 
                }

                m_PostProcessMaterial.SetFloat("_GlobalOpacity", m_FadeCurve.Evaluate(i));
                //print("try evaluate : " + m_FadeCurve.Evaluate(i)); 
                i+= Time.deltaTime  *rate;

                yield return 0; 

            }

            

        }

        private void OnTriggerEnter(Collider other)
        {


            if (Physics.CheckSphere(transform.position, m_ActivationDistance, m_TrappedMask))
            {
                if (other.TryGetComponent<MD_EnemyScript>(out MD_EnemyScript enemy))
                {
                    return; 
                }

                Activation(other); 

            }
        }

        private void OnDrawGizmos()
        {
            if (!m_IsGizmos)
            {
                return; 
            }

            Gizmos.color = Color.white; 

            Gizmos.DrawWireSphere(transform.position, m_AggroRadius);
        }
    }
}
