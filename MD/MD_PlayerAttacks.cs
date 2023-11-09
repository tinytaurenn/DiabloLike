using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DumortierMatthieu.AI;
using System;
using UnityEngine.AI;

using Shared;



namespace DumortierMatthieu
{



    //bug lock dash cast

    public class MD_PlayerAttacks : MonoBehaviour, IEntityStyle, IHealable, IDamageable
    {

        enum EGizmosDrawState
        {
            CloseAttack,
            RangeAttack,
            SummonSpell,
            HealSpell,
            Dash

        }

        #region Global Fields
        [SerializeField]
        EGizmosDrawState m_GizmoState = EGizmosDrawState.CloseAttack;

        [SerializeField]
        IEntityStyle.Style m_EntityStyle = IEntityStyle.Style.Evil;

        [SerializeField]
        LayerMask m_EnemyMask;

        Animator m_Animator;

        PlayerInput m_PlayerInput;

        NavMeshAgent m_NavMeshAgent;


        [SerializeField]
        bool m_IsPendingAction = false;
        //AIMethods m_AIMethods; 

        [SerializeField]
        bool m_CanInput = true;

        [SerializeField]
        bool m_CanPlayerAttack = true;

        float m_BaseSpeed;

        Renderer m_Renderer;
        Material m_BaseMaterial;

        //bool m_IsWeaponOnHand = true;
        [Space(10)]
        [SerializeField]
        GameObject m_OffHandWeapon;
        [SerializeField]
        GameObject m_OnHandWeapon;
        #endregion


        [SerializeField]
        float m_ManaRegenSpeed = 3f; 
        
        float m_ManaRegen = 0f;

        bool m_CanManaAudio = true;

        [SerializeField]
        float m_ManaAudioCooldown = 3f; 

        
        


        #region Events

        public static event Action<bool> OnStandingAttack;
        public static event Action<bool> OnRotationLock;
        public static event Action OnPlayerUnlock;
        public static event Action OnPlayerLock;

        #endregion



        #region closeAttack fields

        [Space(15)]
        [Header("[Close Attack ] //////////////////////////////////////////////////")]
        [Space(15)]
        [SerializeField]
        bool m_CanPlayerCloseAttack = true;
        [SerializeField]
        int m_CloseAttackDamage = 20;
        [SerializeField]
        float m_CloseAttackCooldown = 0.5f;
        [SerializeField]
        int m_CloseAttackVariationsNumber = 2;
        [SerializeField]
        Transform m_CloseAttackAnchor; 

        

        
        [Space(5)]
        [Header("Hit Detection")]
        [Space(5)]
        [SerializeField]
        [Range(0.1f, 5)]
        float m_CloseAttackDistance = 5f;
        [SerializeField]
        [Range(5, 90)]
        float m_CloseAttackAngle = 25f;
        [Space(5)]
        [Header("Gizmos")]
        [Space(5)]
        [SerializeField]
        [Range(1, 30)]
        int m_GizmoFOVDistanceFrequency = 15;
        [Space(5)]
        [Header("VFX")]
        [Space(5)]
        [SerializeField]
        ParticleSystem m_SideSlashPS;
        [SerializeField]
        ParticleSystem m_SideSlashReversePS;
        [SerializeField]
        ParticleSystem m_SlashHitPS;

        [SerializeField]
        GameObject m_BloodSplatVFX;


        int m_MaxComboSize = 2;
        int m_ComboNumber = 0;


        #endregion

        #region rangeAttack Fields

        [Space(15)]
        [Header("[ RangeAttack ] ///////////////////////////////////////////////////////////////////////////////////////////")]
        [Space(15)]
        [SerializeField]
        float m_RangeAttackCooldown = 2f;
        [SerializeField]
        int m_RangeAttackDamage = 10;

        [SerializeField]
        int m_RangeAttackManaCost = 15;

        

        

        [Space(5)]
        [Header("Projectil Prefab")]
        [Space(5)]
        [SerializeField]
        GameObject m_RangeAttackProjectil;

        [SerializeField]
        float m_RangeInternalCooldownTime = 0.5f;

        [Space(5)]
        [Header("Range Attack VFX")]
        [Space(5)]
        [SerializeField]
        GameObject m_RangeAttackParticleSystem;
        [SerializeField]
        Transform m_RangeCastPSTransform;

        float m_RangeAttackCastTimer = 0f;
        [SerializeField]
        float m_RangeCastGrowSpeed = 0.5f;
        [SerializeField]
        float m_RangeCastMaxGrowValue = 0.595f;
        [SerializeField]
        float m_RangeAttackMinPower = 0.15f;

        [Space(5)]
        [Header("Range Attack Detection")]
        [Space(5)]
        [Space(10)]
        [SerializeField]
        LayerMask m_RangeAttackDetectionMask;
        [SerializeField]
        GameObject m_NearestEnemy = null;


        [SerializeField]
        bool m_IsRangeAttackButtonPressed = false;
        [SerializeField]
        bool m_CanPlayerRangeAttack = true;
        [SerializeField]
        [Range(0f, 20f)]
        float m_RangeAttackDetectionDistance = 3f;

        [SerializeField]
        [Range(5, 90)]
        float m_RangeAttackDetectionAngle = 15f;

        [SerializeField]
        bool m_IsRangeCanceled = false; 



        #endregion
        #region DashAttack Fields
        [Space(15)]
        [Header("[Dash Attack ] ///////////////////////////////////////////////////////////////////")]
        [Space(15)]
        [SerializeField]
        bool m_CanPlayerDashAttack = true;
        [SerializeField]
        bool m_IsDashButtonPressed = false;
        [SerializeField]
        float m_DashCooldown = 1f;

        [SerializeField]
        int m_DashManaCost = 15;

        

        

        [Space(5)]
        [Header("Dash Hit Detection")]
        [Space(5)]
        [SerializeField]
        [Range(0f, 20f)]
        float m_DashAttackHitDistane = 1f;
        [SerializeField]
        [Range(0f, 5f)]
        float m_DashSpeedForce = 1f;
        [Space(5)]
        [Header("Dash Duration")]
        [Space(5)]
        [SerializeField]
        float m_DashDurationTimer = 0f;
        [SerializeField]
        float m_DashMaxDuration = 5f;
        [SerializeField]
        float m_DashMinDuration = 1f;

        [SerializeField]
        ParticleSystem m_DashPS;

        [SerializeField]
        AnimationCurve m_CloneFadeCurve; 


        #endregion


        #region HealSpell Fields




        [Space(15)]
        [Header("[ Heal Spell ] /////////////////////////////////////////////////////////////////////////////////")]
        [Space(15)]
        [SerializeField]
        GameObject m_PotionVFX;
        [SerializeField]
        float m_playerHealCooldown = 5f;
        [SerializeField]
        int m_HealSpellManaCost = 30;
        [SerializeField]
        bool m_CanPlayerHeal = true;
        [SerializeField]
        float m_PotionDistance = 2f;
        [SerializeField]
        float m_PotionDestroyTime = 5f;
        //[SerializeField]
        //float m_PotionEffectDuration = 6f;
        //[SerializeField]
        //int m_PotionEffectValue = 40; 
        //[SerializeField]
        //float m_PotionTicTime = 0.5f;
        //[SerializeField]
        //float m_PotionDelay = 0.2f; 
        [SerializeField]
        GameObject m_PotionOffhand;
        [SerializeField]
        GameObject m_PotionInhand;

        

        



        #endregion


        #region SummonSpell Fields

        [Space(15)]
        [Header("[ Summon Spell ] //////////////////////////////////////////////////////////////////////////////")]
        [Space(15)]
        [SerializeField]
        int m_SummonAttackManaCost = 55;
        [SerializeField]
        bool m_CanSummonAttack = true;
        [SerializeField]
        bool m_IsSummonPowerActivated = false;

        [SerializeField]
        bool m_IsSummonGodMode  = false;

        Coroutine m_PP_Coroutine;

        const float PP_ROUTINE_FADE_TIME = 5f; 




        [Space(5)]
        [Header("Summon Activation Duration")]
        [Space(5)]
        [SerializeField]
        float m_SummonPosTimer = 0f;
        [SerializeField]
        float m_SummonPosGlobalTimer = 0f;
        [SerializeField]
        float m_SummonPosTic = 3f;
        [SerializeField]
        float m_SummonCooldown = 3f;
        [SerializeField]
        float m_SummonSpellPowerDuration = 3f;
        [SerializeField]
        float m_SummonStartTime = 1f;

        Coroutine m_SummonMaxSpellDurationRoutine;
        [SerializeField]
        float m_SummonTeleportTic = 0.5f;

        [SerializeField]
        float m_SummonInternalCooldownTime = 0.5f;

        [Space(5)]
        [Header("Teleport Conditions")]
        [Space(5)]
        [SerializeField]
        float m_SummonTeleportDistanceFromTarget = 1f;

        [SerializeField]
        float m_SummonMinDistanceFromLastTeleport = 1.5f;

        [SerializeField]
        List<Vector3> m_SummonReturnPosList = new List<Vector3>();

        [SerializeField]
        List<GameObject> m_SummonEnemiesTargetList = new List<GameObject>(); // not yet used

        [SerializeField]
        int m_SummonAttackVariationNumber = 3;


        Vector3 m_SummonStartPos;

        [SerializeField]
        float m_SummonPosDetectionRadius = 2f;

        [SerializeField]
        SkinnedMeshRenderer[] m_SkinnedMeshRenderers;

        [SerializeField]
        Material m_SummonCloneMaterial;

        Vector3 m_LastSummonPos;
        [SerializeField]
        float m_MinSummonDistance = 0.2f;

        // VFX ///////////////////
        [Space(5)]
        [Header("VFX")]
        [Space(5)]
        [SerializeField]
        GameObject m_SummonDarkTrail;

        [SerializeField]
        Material m_SummonTrailMaterial;

        [SerializeField]
        TrailRenderer m_TrailRenderer;

        string m_SummonTrailPower = "_Power";
        string m_SummonTrailIntensity = "_ColorIntensity";

        [SerializeField]
        float m_SummonTrailPowerLerpValue = 0.5f; 
        [SerializeField]
        AnimationCurve m_SummonTrailAnimationCurve;
        [SerializeField]
        AnimationCurve m_SummonTrailSpeedAnimationCurve;
        [SerializeField]
        GameObject m_SummonMark;
        [SerializeField]
        GameObject m_SummonStartPS;
        [SerializeField]
        GameObject m_SummonEmbergenSmokePlane;

        [SerializeField]
        Vector3 m_SummonEmbergenSpawnOffset;

        float m_SummonPressedCloneTimer = 0f;
        [SerializeField]
        float m_SummonPressedTimeToClone = 1f;
        [SerializeField]
        float m_SummonDarkTrailMin = 5f;
        [SerializeField]
        float m_SummonDarkTrailMax = 15f;

        [SerializeField]
        float m_SummonCloneOpacityFadeTime = 1f;
        [SerializeField]
        float m_SummonTimeSincePowerActivated = 0f;

        [SerializeField]
        ParticleSystem m_DarkHitPs; 
        #endregion


        [Space(15)]
        [Header("[SOUNDS ] //////////////////////////////////////////////////")]
        [Space(15)]
        [SerializeField]

        


        MD_SoundManager m_CloseAttackSound;
        [SerializeField]
        MD_SoundManager m_CloseAttackHitSound;

        [SerializeField]
        MD_SoundManager m_DashSound;

        [SerializeField]
        float m_DashSoundFrequency = 0.2f;

        [SerializeField]
        MD_SoundManager m_DashPendingSound;

        [SerializeField]
        float m_DashPendingSoundFadeTime = 1.5f;

        [SerializeField]
        MD_SoundManager m_OutOfManaSound;


        [SerializeField]
        MD_SoundManager m_HealPotionThrow;

        [SerializeField]
        MD_SoundManager m_PlayerHurtSound; 

        

        [SerializeField]
        MD_SoundManager m_RangeAttackLoadingSound;

        Coroutine m_RangeAttackCastRoutine; 

        [SerializeField]
        MD_SoundManager m_RangeAttackShootSound;

        [SerializeField]
        MD_SoundManager m_SummonStartSound;

        [SerializeField]
        MD_SoundManager m_SummonTeleportSound; 




        #region Unity Callbacks


        private void Start()
        {


            
            //IEntityStyle.Style = IEntityStyle.Style.Evil; 

            //StartCoroutine(SummonSuperDash(3)); 


            m_BaseSpeed = m_NavMeshAgent.speed;

            WeaponInHand(true);

            m_LastSummonPos = transform.position;
        }

        private void Awake()
        {

            m_Renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            m_BaseMaterial = m_Renderer.material;

            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_SkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();


            m_Animator = GetComponent<Animator>();

        }

        private void OnEnable()
        {
            S_GameManager.OnPlayerControllerInput += CanInput;
        }

        private void OnDisable()
        {
            S_GameManager.OnPlayerControllerInput -= CanInput;
        }
        private void Update()
        {
            //RangeAttackPending(); 

            if (S_GameManager.Instance.GameState == S_GameManager.EGameState.Ingame)
            {
                ManaRegen(); 
            }

            



            Timers();

            SummonTrailUpdate(); 

            CheckDashDuration();

            RangeAttackPending();
        }

        #endregion




        #region Global methods


        void CanInput(bool canInput)
        {
            m_CanInput = canInput;

            if (m_CanInput == false)
            {
                RangeCastCancel();
            }
        }
        void WeaponInHand(bool inHand)
        {
            m_OffHandWeapon.gameObject.SetActive(!inHand);
            m_OnHandWeapon.gameObject.SetActive(inHand);

        }

        void PutWeaponInHand()
        {
            WeaponInHand(true);
        }

        void CanAttack()
        {
            m_CanPlayerAttack = true;
        }

        bool CanPlayerInteractIngame()
        {
            if (S_GameManager.Instance.GameState != S_GameManager.EGameState.Ingame)
            {
                return false;  ;
            }

            if (!S_GameManager.Instance.IsLevelLoad)
            {
                return false ;
            }

            if (!m_CanInput)
            {
                return false ;
            }

            return true;
        }

        void TerminatePendingAction()
        {
            m_IsPendingAction = false;
        }

        void PlayerUnlock()
        {
            TerminatePendingAction();
            PutWeaponInHand();
            CanAttack();
            OnPlayerUnlock?.Invoke();
        }

        IEnumerator PlayerAllAttackRoutine(float time)
        {
            m_CanPlayerAttack = false;

            yield return new WaitForSeconds(time);

            m_CanPlayerAttack = true;
        }

        void Timers()
        {

            DashTimer();
            SummonTimer();
            HealTimer(); 

        }

        #endregion


        #region Close Attacks Methods
        public void PlayerCloseAttack(InputAction.CallbackContext context)
        {

            if (!CanPlayerInteractIngame())
            {
                return;
            }

            if (!m_CanPlayerAttack)
            {
                return;
            }

            if (!m_CanPlayerCloseAttack)
            {
                return;
            }

            


            m_Animator.SetInteger("AttackEnum", 0);
            m_Animator.SetInteger("CloseAttackVariationNumber", UnityEngine.Random.Range(0, m_CloseAttackVariationsNumber));
            m_Animator.SetInteger("ComboNumber", m_ComboNumber);



            m_Animator.SetTrigger("AttackTrigger");

            m_ComboNumber = (m_ComboNumber + 1) % m_MaxComboSize;

            StartCoroutine(PlayerCloseAttackRoutine(m_CloseAttackCooldown));
        }

        void SideSlash()
        {
            //m_SideSlashPS.Play();

            ParticleSystem slash = Instantiate(m_SideSlashPS, m_SideSlashPS.transform.position, m_SideSlashPS.transform.rotation);
            slash.Play();


            m_CloseAttackSound.Activation(transform); 
        }

        void SideSlashReverse()
        {
            //m_SideSlashReversePS.Play();

            ParticleSystem slash = Instantiate(m_SideSlashReversePS, m_SideSlashReversePS.transform.position, m_SideSlashReversePS.transform.rotation);
            slash.Play();

            m_CloseAttackSound.Activation(transform);
        }
        

        void CloseAttackDamage()
        {
            Collider[] neartItems = Physics.OverlapSphere(transform.position, m_CloseAttackDistance);

            foreach (var item in neartItems)
            {
                if (AIMethods.IsFOV(m_CloseAttackAngle, transform, item.transform.position))
                {
                    if (item.TryGetComponent<IDamageable>(out IDamageable damageableComponent))
                    {
                        //damageableComponent.TakeDamage(m_CloseAttackDamage);
                        damageableComponent.TakeDamage(m_CloseAttackDamage, IDamageable.EAttackSource.Melee);
                        
                        

                        print(item.GetComponentInChildren<Renderer>().name);

                        Vector3 center = HitPositionByAnchor(item); 

                        


                        Instantiate(m_BloodSplatVFX, center, m_BloodSplatVFX.transform.rotation);

                        Instantiate(m_SlashHitPS, center, m_SlashHitPS.transform.rotation);

                    }

                    if (item.CompareTag( "Enemy"))
                    {
                        m_CloseAttackHitSound.Activation(item.transform); 
                    }
                }

            }


        }

        Vector3 CenterOfTarget(Collider Item)
        {

            Vector3 center = Vector3.zero;

            foreach (var item in Item.GetComponentsInChildren<Renderer>())
            {
                if (TryGetComponent<IDamageable>(out IDamageable enemy))
                {
                    center =  item.GetComponent<Renderer>().bounds.center; 

                    

                    var center2=  item.GetComponent<Renderer>().bounds.ClosestPoint(m_CloseAttackAnchor.position);


                    center = MD_Maths.PointBetweenToPoint(center2, center, 0.5f);

                   
                }
            }





            return center; 

        }

        Vector3 HitPositionByAnchor(Collider item)
        {
            Vector3 center = Vector3.zero;


            var anchors = m_CloseAttackAnchor.GetComponentsInChildren<Transform>();



            float dist = 999f;

            

            foreach (var pos in anchors)
            {
                float posDistance = Vector3.Distance(pos.position, item.transform.position);

                if ( posDistance < dist)
                {
                    dist = posDistance;
                    center = pos.position; 
                }

            }



            return center;

        }




        IEnumerator PlayerCloseAttackRoutine(float time)
        {
            m_CanPlayerCloseAttack = false;

            S_SkillManager.Cooldown[S_CharacterSprites.ESkillSprite.MeleeAttack] = time;

            yield return new WaitForSeconds(time);

            m_CanPlayerCloseAttack = true;
        }

        #endregion

        #region Range Attaks Methods

        void RangeAttackPending()
        {

            RangeAttackCastVFX();


            if (!m_IsRangeAttackButtonPressed) return;

            if (!m_IsPendingAction)
            {


                return;
            }





        }

        void RangeAttackCastVFX()
        {

            if (m_IsRangeAttackButtonPressed)
            {
                m_RangeAttackCastTimer += Time.deltaTime * m_RangeCastGrowSpeed;
            }

            m_RangeAttackParticleSystem.GetComponent<MD_RangeSpellScript>().m_VortexPower = Mathf.Clamp(m_RangeAttackCastTimer, 0, m_RangeCastMaxGrowValue);
        }


        void RangeCastVFXStop()
        {
            m_RangeAttackCastTimer = 0f;
        }


        void RangeAttackPendingGizmos()//to rename
        {
            if (!m_IsRangeAttackButtonPressed) return;






            if (!Physics.CheckSphere(transform.position, m_RangeAttackDetectionDistance, m_EnemyMask)) return;


            Collider[] enemies = Physics.OverlapSphere(transform.position, m_RangeAttackDetectionDistance, m_EnemyMask);

            foreach (var item in enemies)
            {



                if (AIMethods.IsFOV(m_RangeAttackDetectionAngle, transform, item.transform.position))
                {

                    Gizmos.color = Color.yellow;

                    if (item == AIMethods.GetNearestColliderInFOV(transform, enemies, m_RangeAttackDetectionAngle))
                    {


                        AIMethods.IsOnLineSightGizmos(transform.position, item.gameObject, m_RangeAttackDetectionMask);
                        Gizmos.color = Color.red;
                    }



                }
                else
                {
                    Gizmos.color = Color.white;
                }



                Gizmos.DrawWireCube(item.transform.position + Vector3.up, Vector3.one);
            }


            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + Vector3.up, Vector3.one);


        }

        public void PlayerRangeAttack(InputAction.CallbackContext context)
        {

            const float AUDIO_UNFADE_TIME = 1f;
            const float AUDIO_FADE_TIME = 2f;
            //const float CANCEL_AUDIO_FADE_TIME = 0.3f; 

            if (!CanPlayerInteractIngame())
            {
                return;
            }

            if (S_GameManager.Instance.Mana < m_RangeAttackManaCost)
            {

                if (m_CanManaAudio && m_CanPlayerRangeAttack)
                {
                    m_OutOfManaSound.AddAndActivate(transform);
                    StartCoroutine(ManaAudioCooldown(m_ManaAudioCooldown));
                }
                
                
                return;
            }

            if (m_IsRangeCanceled)
            {
                return; 
            }


            //if (!m_CanPlayerRangeAttack)
            //{
            //    return;
            //}



            if (context.performed)
            {


                if (!m_CanPlayerRangeAttack)
                {
                    return;
                }

                if (!m_CanPlayerAttack)
                {
                    return;
                }

                m_RangeAttackLoadingSound.AddAndActivate(transform);
                m_RangeAttackCastRoutine = StartCoroutine(m_RangeAttackLoadingSound.UnFadeAudioSource(AUDIO_UNFADE_TIME)); 

                StartCoroutine(RangeAttackInternalRoutine(m_RangeInternalCooldownTime));


                //m_RangeAttackParticleSystem.SetActive(true);


                m_IsPendingAction = true;
                m_CanPlayerAttack = false;
                //print("Range button pressed");


                WeaponInHand(false);

                m_IsRangeAttackButtonPressed = true;
                m_Animator.SetBool("RangeAttackCasting", true);
                m_Animator.SetInteger("AttackEnum", 1);

                OnStandingAttack?.Invoke(true);



                //effect 



            }

            if (context.canceled)
            {
                if (!m_IsRangeAttackButtonPressed)
                {

                    return; 

                }
                

                //m_RangeAttackCastTimer = 0f;


                //m_RangeAttackParticleSystem.SetActive(false); 

                if (!m_CanPlayerRangeAttack)
                {
                    RangeCastCancel();
                    //StartCoroutine(m_RangeAttackLoadingSound.FadeAudioSource(CANCEL_AUDIO_FADE_TIME));
                    StopCoroutine(m_RangeAttackCastRoutine);
                    m_RangeAttackLoadingSound.DestroyAudio(); 


                    m_CanPlayerRangeAttack = true;
                    return;
                }
                m_RangeAttackCastRoutine =  StartCoroutine(m_RangeAttackLoadingSound.FadeAudioSource(AUDIO_FADE_TIME));

                ChangeMana(-m_RangeAttackManaCost);

                m_CanPlayerAttack = true;


                Collider[] list = Physics.OverlapSphere(transform.position, m_RangeAttackDetectionDistance, m_EnemyMask);




                if (list.Length > 0)
                {
                    if (AIMethods.GetNearestColliderInFOV(transform, list, m_RangeAttackDetectionAngle) != null) //if we got a nearest
                    {
                        

                        m_NearestEnemy = AIMethods.GetNearestColliderInFOV(transform, list, m_RangeAttackDetectionAngle).gameObject;


                        if (AIMethods.IsOnLineSight(transform.position, m_NearestEnemy, m_RangeAttackDetectionMask))
                        {


                            if (Mathf.Clamp(m_RangeAttackCastTimer, 0, m_RangeCastMaxGrowValue) > m_RangeAttackMinPower)
                            {


                                RangeCastFollow();


                                return;
                            }


                        }



                    }



                }


                //RangeCastCancel();
                m_NearestEnemy = null;
                RangeCastFollow();
                //Effect




            }






        }

        void RangeLaunchAttack()
        {


            float vortexPower = Mathf.Clamp(m_RangeAttackCastTimer, 0, m_RangeCastMaxGrowValue);

            GameObject projectil = Instantiate(m_RangeAttackProjectil, m_RangeCastPSTransform.position, transform.rotation);

            projectil.GetComponent<MD_RangeSpellScript>().m_VortexPower = vortexPower;

            //print("vortex power is " + vortexPower);

            float damageValue = vortexPower / m_RangeCastMaxGrowValue;
            damageValue *= m_RangeAttackDamage;

            projectil.GetComponent<MD_RangeAttackProjectilScript>().Damage = (int)damageValue;

            if (m_NearestEnemy == null)
            {
                projectil.GetComponent<MD_RangeAttackProjectilScript>().TargetTransform = null;
            }
            else
            {
                projectil.GetComponent<MD_RangeAttackProjectilScript>().TargetTransform = m_NearestEnemy.transform;
            }



            RangeCastVFXStop();

        }


        void RangeCastFollow()
        {
            m_IsRangeAttackButtonPressed = false;
            m_Animator.SetBool("RangeAttackCasting", false);



            m_Animator.SetInteger("AttackEnum", 1);

            m_Animator.SetTrigger("AttackTrigger");

            m_RangeAttackShootSound.Activation(transform); 


            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("Range"))
            {
                if (S_GameManager.Instance.IsGodMode)
                {
                    StartCoroutine(PlayerRangeAttackRoutine(1));
                    return; 

                }

                StartCoroutine(PlayerRangeAttackRoutine(m_RangeAttackCooldown));
            }
        }

        void RangeCastCancel()
        {
            m_IsRangeAttackButtonPressed = false;
            m_Animator.SetBool("RangeAttackCasting", false);

            m_RangeAttackCastTimer = 0f;
            PlayerUnlock();
            //if (m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("Range"))
            //{
            //    StartCoroutine(PlayerRangeAttackRoutine(m_RangeAttackCooldown));
            //}
           
            StartCoroutine(RangeCancelRoutine(m_RangeInternalCooldownTime));

            S_SkillManager.Cooldown[S_CharacterSprites.ESkillSprite.RangeAttack] = m_RangeInternalCooldownTime;
        }



        IEnumerator PlayerRangeAttackRoutine(float time)
        {
            m_CanPlayerRangeAttack = false;

            S_SkillManager.Cooldown[S_CharacterSprites.ESkillSprite.RangeAttack] = time;

            yield return new WaitForSeconds(time);

            m_CanPlayerRangeAttack = true;


        }

        IEnumerator RangeAttackInternalRoutine(float time)
        {
            m_CanPlayerRangeAttack = false;



            yield return new WaitForSeconds(time);

            m_CanPlayerRangeAttack = true;
        }

        IEnumerator RangeCancelRoutine(float time)
        {

            m_IsRangeCanceled = true;

            yield return new WaitForSeconds(time);

            m_IsRangeCanceled = false; 
        }

        #endregion

        #region Dash Attack Methods

        public void PlayerDashAttack(InputAction.CallbackContext context)
        {

            if (!CanPlayerInteractIngame())
            {
                return;
            }

            if (m_IsPendingAction)
            {
                return;
            }

            if (!m_CanPlayerDashAttack)
            {
                return;
            }
           

            if (S_GameManager.Instance.Mana < m_DashManaCost)
            {
                if (m_CanManaAudio)
                {
                    m_OutOfManaSound.AddAndActivate(transform);
                    StartCoroutine(ManaAudioCooldown(m_ManaAudioCooldown));
                }

                return;
            }

            

            if (context.performed)
            {

                m_DashPS.Play();

                StartCoroutine(DashKickStepsSoudRoutine(m_DashSoundFrequency));

                 m_DashPendingSound.AddAndActivate(transform);

               

                m_SkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();





                m_Renderer.material = m_SummonCloneMaterial;
                m_CanPlayerAttack = false;

                m_NavMeshAgent.speed += m_DashSpeedForce;

                OnRotationLock?.Invoke(true);

                m_IsDashButtonPressed = true;
                m_Animator.SetBool("IsDashing", true);

                m_Animator.SetTrigger("AttackTrigger");
                m_Animator.SetInteger("AttackEnum", 4);
            }

            if (context.canceled)
            {

                m_CanPlayerDashAttack = false;

                if (m_DashDurationTimer < m_DashMinDuration)
                {
                    StartCoroutine(PlayerDashAddingTime(m_DashDurationTimer));
                }
                else
                {

                    EndDash();

                }



            }




        }

        IEnumerator PlayerDashAddingTime(float timer)
        {

            float addedTime = m_DashMinDuration - timer;
            yield return new WaitForSeconds(addedTime);

            EndDash();


        }

        void CheckDashDuration()
        {
            if (m_IsDashButtonPressed)
            {
                //print("test_2");
                m_DashDurationTimer += Time.deltaTime;


            }
            else
            {
                m_DashDurationTimer = 0f;
            }


            if (m_DashDurationTimer >= m_DashMaxDuration)
            {

                EndDash();


            }


        }

        public void EndDash()
        {

            StartCoroutine(m_DashPendingSound.FadeAudioSource(m_DashPendingSoundFadeTime));
            ChangeMana(-m_DashManaCost);

            OnRotationLock?.Invoke(false);

            m_DashPS.Stop();

            m_IsDashButtonPressed = false;
            m_Animator.SetBool("IsDashing", false);

            m_Animator.SetInteger("AttackEnum", 4);
            m_NavMeshAgent.speed = m_BaseSpeed;


            m_CanPlayerAttack = true;
            m_CanPlayerDashAttack = true;

            m_Renderer.material = m_BaseMaterial;

            if (S_GameManager.Instance.IsGodMode)
            {
                StartCoroutine(PlayerDashAttackRoutine(1));

            }

            StartCoroutine(PlayerDashAttackRoutine(m_DashCooldown));
        }

        void DashTimer()
        {
            if (m_IsDashButtonPressed)
            {
                m_SummonPressedCloneTimer += Time.deltaTime;

                if (m_SummonPressedCloneTimer >= m_SummonPressedTimeToClone)
                {

                    if (Vector3.Distance(m_LastSummonPos, transform.position) >= m_MinSummonDistance)
                    {

                        CloneCreation();



                        m_LastSummonPos = transform.position;
                        m_SummonPressedCloneTimer = 0f;
                    }
                    //



                }
            }
        }

        

        IEnumerator DashKickStepsSoudRoutine(float time)
        {
            if (m_DashDurationTimer > 0 )
            {
                m_DashSound.Activation(transform,true);
                
            }


            yield return new WaitForSeconds(time);
            if (m_DashDurationTimer >0 )
            {
                StartCoroutine(DashKickStepsSoudRoutine(time)); 
            }
        }


        IEnumerator PlayerDashAttackRoutine(float time)
        {
            m_CanPlayerDashAttack = false;

            S_SkillManager.Cooldown[S_CharacterSprites.ESkillSprite.Dodge] = time;

            yield return new WaitForSeconds(time);

            m_CanPlayerDashAttack = true;
        }

        #endregion

        #region Heal Spell Methods

        public void PlayerHeal(InputAction.CallbackContext context)
        {
            const float GODMODE_COOLDOWN_TIME = 2f;

            if (!CanPlayerInteractIngame())
            {
                return;
            }

            if (m_IsPendingAction)
            {
                return;
            }

            if (!m_CanPlayerAttack)
            {
                return;
            }

            if (!m_CanPlayerHeal)
            {
                return;
            }

            if (S_GameManager.Instance.Mana < m_HealSpellManaCost)
            {
                if (m_CanManaAudio)
                {
                    m_OutOfManaSound.AddAndActivate(transform);
                    StartCoroutine(ManaAudioCooldown(m_ManaAudioCooldown));
                }
                return;
            }

            if (context.performed)
            {

                WeaponInHand(false);

                m_Animator.SetInteger("AttackEnum", 3);
                m_Animator.SetTrigger("AttackTrigger");

                m_HealPotionThrow.Activation(transform); 

                ChangeMana(-m_HealSpellManaCost);


                if (S_GameManager.Instance.IsGodMode)
                {
                    StartCoroutine(PlayerHealRoutine(GODMODE_COOLDOWN_TIME));
                    return; 
                }

                StartCoroutine(PlayerHealRoutine(m_playerHealCooldown));


            }


        }

        void HealTimer()
        {
            if (!m_Renderer.material.HasFloat("_HealValue"))
            {
                return; 
            }

            if (m_Renderer.material.GetFloat("_HealValue") > 0 )
            {
                m_Renderer.material.SetFloat("_HealValue",Mathf.Clamp01( m_Renderer.material.GetFloat("_HealValue") - Time.deltaTime));

            }
        }

        void ThrowingPotion()
        {
            PotionOff();
            GameObject vfx = Instantiate(m_PotionVFX, m_OnHandWeapon.transform.position, m_PotionVFX.transform.rotation);
            //StartCoroutine(PotionEffectRoutine(transform.position,m_PotionEffectDuration, m_PotionTicTime, m_PotionDelay));  
            vfx.GetComponent<MD_PotionEffectScript>().PotionLifeTime = m_PotionDestroyTime; 

            Destroy(vfx, m_PotionDestroyTime);
        }
        void PutPotionInHand(bool InHand)
        {
            m_PotionInhand.SetActive(InHand);

            
            
            m_PotionOffhand.SetActive(!InHand);
        }

        void PotionInHand()
        {
            PutPotionInHand(true);
        }
        void PotionOffHand()
        {
            PutPotionInHand(false);
        }

        void PotionOff()
        {
            m_PotionInhand.SetActive(false);
            m_PotionOffhand.SetActive(false);
        }

        IEnumerator PlayerHealRoutine(float time)
        {
            m_CanPlayerHeal = false;

            S_SkillManager.Cooldown[S_CharacterSprites.ESkillSprite.Heal] = time;

            yield return new WaitForSeconds(time);

            PutPotionInHand(false);
            

            m_CanPlayerHeal = true;
        }

        //all others heal effetcs are on the potion prefab 


        #endregion

        #region Summon Spell Methods
        public void PlayerSummonSpell(InputAction.CallbackContext context)
        {
            
            

            if (!CanPlayerInteractIngame())
            {
                return;
            }

            if (m_IsPendingAction)
            {
                return;
            }

            if (!m_CanPlayerAttack)
            {
                return;
            }


            


            if (context.performed)
            {
                if (!m_CanSummonAttack)
                {
                    return;
                }



                if (m_IsSummonPowerActivated)
                {
                    

                    if (S_GameManager.Instance.IsGodMode)
                    {
                        StartCoroutine(SummonSpellRoutine(1));
                    }
                    else
                    {
                        StartCoroutine(SummonSpellRoutine(m_SummonCooldown));
                    }

                    if (m_PP_Coroutine != null)
                    {
                        StopCoroutine(m_PP_Coroutine);
                    }
                    m_PP_Coroutine = StartCoroutine(Camera.main.GetComponent<MD_SummonPostProcessModifier>().SwitchActivate(PP_ROUTINE_FADE_TIME, false));


                    //m_SummonDarkTrail.SetActive(false); //ici

                    //SummontrailSet(m_SummonDarkTrail.GetComponent<ParticleSystem>(), 0, 0);
                    m_SkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

                    OnPlayerLock?.Invoke();

                    m_SummonReturnPosList.Reverse();


                    StartCoroutine(SummonReleaseEffect(m_SummonTeleportTic));


                    StopCoroutine(m_SummonMaxSpellDurationRoutine);




                    //m_SummonReturnPosList.Clear(); 
                    m_SummonPosTimer = 0f;
                    m_SummonPosGlobalTimer = 0f;

                    m_IsSummonPowerActivated = false;

                }
                else
                {

                    m_TrailRenderer.ResetBounds();

                    if (S_GameManager.Instance.Mana < m_SummonAttackManaCost)
                    {
                        
                        if (m_CanManaAudio)
                        {
                            m_OutOfManaSound.AddAndActivate(transform);
                            StartCoroutine(ManaAudioCooldown(m_ManaAudioCooldown));
                        }
                        return;
                    }

                    ChangeMana(-m_SummonAttackManaCost);

                    m_SummonStartSound.AddAndActivate(transform);

                    if (m_PP_Coroutine != null)
                    {
                        StopCoroutine(m_PP_Coroutine);
                    }

                    m_PP_Coroutine = StartCoroutine(Camera.main.GetComponent<MD_SummonPostProcessModifier>().SwitchActivate(PP_ROUTINE_FADE_TIME, true));

                    GameObject startPS = Instantiate(m_SummonStartPS, transform.position, m_SummonStartPS.transform.rotation);
                    startPS.transform.position += Vector3.up * 0.1f;
                    SummonMarkSet(m_SummonStartPS.GetComponent<ParticleSystem>(), m_SummonSpellPowerDuration);

                    //m_SummonDarkTrail.SetActive(true);//ici
                    //SummontrailSet(m_SummonDarkTrail.GetComponent<ParticleSystem>(), m_SummonDarkTrailMin, m_SummonDarkTrailMax);

                    m_SummonMaxSpellDurationRoutine = StartCoroutine(SummonMaxDurationResetRoutine(m_SummonSpellPowerDuration));

                    m_SummonStartPos = transform.position;

                    StartCoroutine(SummonSpellInternalRoutine(m_SummonInternalCooldownTime));

                    m_IsSummonPowerActivated = true;



                }

            }



        }

        IEnumerator SummonMaxDurationResetRoutine(float time)
        {


            yield return new WaitForSeconds(time);

            if (m_PP_Coroutine != null)
            {
                StopCoroutine(m_PP_Coroutine);
            }
            m_PP_Coroutine = StartCoroutine(Camera.main.GetComponent<MD_SummonPostProcessModifier>().SwitchActivate(PP_ROUTINE_FADE_TIME, false));

            ChangeMana(m_SummonAttackManaCost / 2); 

            m_IsSummonPowerActivated = false;
            //ici
            m_SummonTimeSincePowerActivated = 0f;

            //SummontrailSet(m_SummonDarkTrail.GetComponent<ParticleSystem>(), 0, 0);

            m_SummonPosGlobalTimer = 0f;

            m_SummonEnemiesTargetList.Clear();
            m_SummonReturnPosList.Clear();
        }

        void SummonTrailUpdate()
        {
            const float MAX = 6f;
            const float MIN = 0.5f;

            float evalValue = m_SummonTrailAnimationCurve.Evaluate(m_Animator.GetFloat("Speed")) * MAX;

            

            evalValue = Mathf.Clamp(evalValue, MIN, MAX);

            if (!m_IsSummonPowerActivated)
            {
                evalValue = MAX; 
            }

            m_SummonTrailMaterial.SetFloat(m_SummonTrailPower, Mathf.Lerp(m_SummonTrailMaterial.GetFloat(m_SummonTrailPower), evalValue, m_SummonTrailPowerLerpValue * Time.deltaTime));
            //print("animation curve :" + evalValue);
        }



        void SummonTimer()
        {


            if (m_IsSummonPowerActivated)
            {

                m_SummonTimeSincePowerActivated += Time.deltaTime;




                FillSummonEnemyList();

                m_SummonPosTimer += Time.deltaTime;
                m_SummonPosGlobalTimer += Time.deltaTime;



                var psTime = m_SummonDarkTrail.GetComponent<ParticleSystem>().main;

                psTime.startLifetime = m_SummonSpellPowerDuration - Math.Clamp(m_SummonPosGlobalTimer, 0, m_SummonSpellPowerDuration);


                if (m_SummonPosTimer >= m_SummonPosTic)
                {
                    
                    m_SummonReturnPosList.Add(transform.position);
                    m_SummonPosTimer = 0f;
                }

                //////
                ///






            }
        }

        IEnumerator SummonCloneMaterialOpacityRoutine(float time, Material mat)
        {

            float i = 0;

            float rate = 1 / time;

            while (i < 1)
            {

                

                //print("change opacity");
                mat.SetFloat("_Opacity", m_CloneFadeCurve.Evaluate(i));
                i += Time.deltaTime * rate;
                yield return 0;
            }



        }

        void CloneCreation()
        {
            for (int i = 0; i < m_SkinnedMeshRenderers.Length; i++)
            {
                GameObject obj = new GameObject();
                MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
                //obj.layer = LayerMask.NameToLayer("Shadowed"); 

                Mesh mesh = new Mesh();

                m_SkinnedMeshRenderers[i].BakeMesh(mesh);

                meshFilter.mesh = mesh;

                obj.transform.position = transform.position;

                obj.transform.rotation = transform.rotation;

                meshRenderer.material = m_SummonCloneMaterial;

                Destroy(obj, m_SummonCloneOpacityFadeTime);
                StartCoroutine(SummonCloneMaterialOpacityRoutine(m_SummonCloneOpacityFadeTime, meshRenderer.material));
            }
        }

        GameObject CloneCreationNoDestroy()
        {

            for (int i = 0; i < m_SkinnedMeshRenderers.Length; i++)
            {
                GameObject obj = new GameObject();

                MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = obj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();

                m_SkinnedMeshRenderers[i].BakeMesh(mesh);

                obj.layer = LayerMask.NameToLayer("Shadowed");
                meshFilter.mesh = mesh;


                //obj.transform.position = transform.position;

                //obj.transform.rotation = transform.rotation;

                obj.transform.SetPositionAndRotation(transform.position, transform.rotation);

                meshRenderer.material = m_SummonCloneMaterial;
                return obj;

            }
            return null;

        }


        void SummonTargetListShowGizmos()
        {
            Gizmos.color = Color.red;

            foreach (var item in m_SummonReturnPosList)
            {

                Collider[] enemiesList = Physics.OverlapSphere(item, m_SummonPosDetectionRadius, m_EnemyMask);

                if (AIMethods.GetNearestCollider(item, enemiesList) != null)
                {
                    Collider nearestEnemy = AIMethods.GetNearestCollider(item, enemiesList);
                    Gizmos.DrawWireCube(nearestEnemy.transform.position + Vector3.up, Vector3.one);
                }



            }
        }

        void FillSummonEnemyList()
        {
            const float DESTROY_OFFSET = 1.5f;

            foreach (Vector3 item in m_SummonReturnPosList)
            {

                Collider[] enemiesList = Physics.OverlapSphere(item, m_SummonPosDetectionRadius, m_EnemyMask);

                if (AIMethods.GetNearestCollider(item, enemiesList) != null)
                {
                    Collider nearestEnemy = AIMethods.GetNearestCollider(item, enemiesList);

                    if (!AIMethods.IsGameObjectInList(m_SummonEnemiesTargetList, nearestEnemy.gameObject))
                    {
                        m_SummonEnemiesTargetList.Add(nearestEnemy.gameObject);

                        GameObject summonMarkPS = Instantiate(m_SummonMark, nearestEnemy.transform.position, m_SummonMark.transform.rotation);
                        summonMarkPS.transform.parent = nearestEnemy.transform;
                        SummonMarkSet(summonMarkPS.GetComponent<ParticleSystem>(), m_SummonSpellPowerDuration - m_SummonTimeSincePowerActivated);

                        Destroy(summonMarkPS, (m_SummonSpellPowerDuration - m_SummonTimeSincePowerActivated) + DESTROY_OFFSET);
                    }


                }



            }
        }

        void SummonMarkSet(ParticleSystem PS, float time)
        {
            PS.gameObject.transform.position += Vector3.up * 0.01f;
            var psTime = PS.main;


            //float durationTime = m_SummonSpellPowerDuration - m_SummonTimeSincePowerActivated;

            psTime.startLifetime = time;
        }

        void SummontrailSet(ParticleSystem PS, float minValue, float maxValue)
        {

            var emission = PS.emission;

            var curve = new ParticleSystem.MinMaxCurve();


            curve.constantMin = 1f;



            AnimationCurve curveMin = new AnimationCurve();
            AnimationCurve curveMax = new AnimationCurve();

            curveMin.AddKey(0, minValue);
            curveMin.AddKey(1, minValue);
            curveMax.AddKey(0, maxValue);
            curveMax.AddKey(1, maxValue);

            emission.rateOverDistance = new ParticleSystem.MinMaxCurve(1, curveMin, curveMax);

            //var psTime = PS.main;

            //psTime.startLifetime = m_SummonSpellPowerDuration - m_SummonPosTimer; 

            //rate.rateOverTime = new ParticleSystem.MinMaxCurve(m_VortexPower, minValue, maxValue);

            //rate.rateOverTime = m_VortexPower;





        }


        IEnumerator SummonReleaseEffect(float time)
        {

            const float SUMMON_CAMERA_POS_LERP_SPEED = 4f;
            const float END_CAMERA_DELAY = 0.5f; 


            m_SummonStartSound.FadeAudioSource(1f); 

            Renderer myRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

            Material baseMaterial = myRenderer.material;





            m_CanPlayerAttack = false;

            m_Animator.SetTrigger("SummonStartTrigger");
            m_Animator.SetInteger("AttackEnum", 2);
            m_Animator.SetBool("IsSummonSpell", true);


            yield return new WaitForSeconds(m_SummonStartTime);

            myRenderer.material = m_SummonCloneMaterial;




            m_SummonEnemiesTargetList.Reverse();

            List<GameObject> cloneList = new List<GameObject>();


            m_IsSummonGodMode = true;

            float baseCamPosSpeed = CameraManager.Instance.PosLerpSpeed; 
            

            CameraManager.Instance.PosLerpSpeed = SUMMON_CAMERA_POS_LERP_SPEED; 

            foreach (GameObject item in m_SummonEnemiesTargetList)
            {


                //vanish Teleport effect
                //CloneCreation();
                GameObject clone = CloneCreationNoDestroy();


                cloneList.Add(clone);


                Vector3 pos = item.transform.TransformPoint(-Vector3.forward * m_SummonTeleportDistanceFromTarget);

                
                transform.position = pos;
                m_SummonTeleportSound.Activation(transform); 

                transform.LookAt(item.transform);

                //Teleport spawn effect





                m_Animator.SetInteger("AttackEnum", 2);
                m_Animator.SetBool("IsSummonSpell", true);
                m_Animator.SetTrigger("AttackTrigger");
                m_Animator.SetInteger("SummonAttackVariationNumber", UnityEngine.Random.Range(0, m_SummonAttackVariationNumber));


                yield return new WaitForSeconds(time);

                if (item.TryGetComponent<IDamageable>(out IDamageable enemy))
                {
                    m_CloseAttackHitSound.Activation(transform);
                    enemy.TakeDamage(m_CloseAttackDamage);
                    
                    Vector3 center = CenterOfTarget(item.GetComponent<Collider>()) ;




                    Instantiate(m_BloodSplatVFX, center, m_BloodSplatVFX.transform.rotation);

                    Instantiate(m_DarkHitPs, center, m_SlashHitPS.transform.rotation);

                    Instantiate(m_BloodSplatVFX, center, m_BloodSplatVFX.transform.rotation);


                }


            }
            m_Animator.SetInteger("AttackEnum", 2);
            m_Animator.SetBool("IsSummonSpell", false);
            m_Animator.SetTrigger("AttackTrigger");

            //CloneCreation();

            GameObject shadowClone = CloneCreationNoDestroy();
            cloneList.Add(shadowClone);

            GameObject embergenSummonSpawn = Instantiate(
                m_SummonEmbergenSmokePlane,
                m_SummonStartPos + m_SummonEmbergenSpawnOffset,
                m_SummonEmbergenSmokePlane.transform.rotation
                );

            Destroy(embergenSummonSpawn, m_SummonCooldown);

            transform.position = m_SummonStartPos;
            m_SummonTeleportSound.Activation(transform);

            m_IsSummonGodMode = false;
            

            foreach (var item in cloneList)
            {
                Destroy(item, m_SummonCloneOpacityFadeTime);
                StartCoroutine(SummonCloneMaterialOpacityRoutine(m_SummonCloneOpacityFadeTime, item.GetComponent<MeshRenderer>().material));
            }


            m_SummonEnemiesTargetList.Clear();

            myRenderer.material = baseMaterial;
            //Last Effect 

            m_SummonTimeSincePowerActivated = 0f;

            m_SummonReturnPosList.Clear();

            OnPlayerUnlock?.Invoke();

            m_CanPlayerAttack = true;

            yield return new WaitForSeconds(END_CAMERA_DELAY); 

            CameraManager.Instance.PosLerpSpeed = baseCamPosSpeed; 

        }




        void SummonTeleportSlashEffect(Collider nearestEnemy)
        {

        }


        IEnumerator SummonSpellRoutine(float time)
        {
            m_CanSummonAttack = false;

            S_SkillManager.Cooldown[S_CharacterSprites.ESkillSprite.Summon] = time;

            yield return new WaitForSeconds(time);

            m_CanSummonAttack = true;
        }

        IEnumerator SummonSpellInternalRoutine(float time)
        {
            m_CanSummonAttack = false;



            yield return new WaitForSeconds(time);

            print("summon activation ");
            m_CanSummonAttack = true;
        }






        #endregion


        public void PauseButton(InputAction.CallbackContext context)
        {
            //S_GameManager.Instance.SwitchState(S_GameManager.EGameState.Pause); 



            if (!S_GameManager.Instance.IsLevelLoad)
            {
                return; 
            }

            if (S_GameManager.Instance.GameState == S_GameManager.EGameState.Pause)
            {
                S_GameManager.Instance.GameState = S_GameManager.EGameState.Ingame;
            }
            else
            {
                S_GameManager.Instance.GameState = S_GameManager.EGameState.Pause;
            }


        }

        #region style

        public IEntityStyle.Style GetStyle()
        {
            return m_EntityStyle;
        }

        #endregion

        #region Interfaces 

        public void TakeDamage(int damage)
        {
            if (m_IsSummonGodMode)
            {
                return; 
            }



            //print("damaged of " + damage);

            var center = GetComponentInChildren<Renderer>().bounds.center;
            Instantiate(m_BloodSplatVFX, center, m_BloodSplatVFX.transform.rotation);

            m_PlayerHurtSound.AddAndActivate(transform);
            

            //S_GameManager.Instance.Life -= damage;
            ChangeLife(-damage);
        }

        public void TakeDamage(int dmg, IDamageable.EAttackSource attackStyle)
        {

            if (attackStyle == IDamageable.EAttackSource.Melee)
            {
                CameraManager.Instance.GetComponent<MD_CameraShake>().Shake();
            }

            TakeDamage(dmg);
        }


        public void Heal(int healAmount)
        {
            //print("Healing of " + healAmount);

            //S_GameManager.Instance.Life += healAmount;
            ChangeLife(healAmount);
        }

        void ChangeMana(int manaAmount)
        {
            int value = S_GameManager.Instance.Mana + manaAmount; ;
            S_GameManager.Instance.Mana = Math.Clamp(value, 0, S_GameManager.Instance.MaxMana);
        }

        void ChangeLife(int value)
        {
            int life = S_GameManager.Instance.Life + value; ;
            S_GameManager.Instance.Life = Math.Clamp(life, 0, S_GameManager.Instance.MaxLife);
        }

        void ManaRegen( )
        {
            
            if (S_GameManager.Instance.Mana < S_GameManager.Instance.MaxMana)
            {

                
                m_ManaRegen += Time.deltaTime * m_ManaRegenSpeed;

                if (m_ManaRegen >1 )
                {
                    

                    S_GameManager.Instance.Mana += (int)m_ManaRegen;

                    m_ManaRegen -= 1f;

                }

                
            }
            else
            {
                m_ManaRegen = 0;
            }




        }


        #endregion

        #region Wheel

        public void OpenWheel(InputAction.CallbackContext context)
        {
            if (!S_GameManager.Instance.IsLevelLoad)
            {
                return; 
            }

            if (!m_CanInput)
            {
                return; 
            }

            if (S_GameManager.Instance.GameState == S_GameManager.EGameState.Pause  || S_GameManager.Instance.GameState == S_GameManager.EGameState.Death)
            {
                return; 
            }

            S_SelectionWheel.OpenWheel(context);
        }

        public void SelectCharacter(InputAction.CallbackContext context)
        {

            if (!S_GameManager.Instance.IsLevelLoad)
            {
                return;
            }
            S_SelectionWheel.SelectCharacter(context);
        }

        IEnumerator ManaAudioCooldown(float time)
        {
            m_CanManaAudio = false; 
            yield return new WaitForSeconds(time);
            m_CanManaAudio = true; 
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            switch (m_GizmoState)
            {
                case EGizmosDrawState.CloseAttack:
                    AIMethods.ShowFOV(m_CloseAttackAngle, m_CloseAttackDistance, transform, m_GizmoFOVDistanceFrequency);
                    break;
                case EGizmosDrawState.RangeAttack:



                    AIMethods.ShowFOV(m_RangeAttackDetectionAngle, m_RangeAttackDetectionDistance, transform, m_GizmoFOVDistanceFrequency);


                    Gizmos.DrawWireSphere(transform.position, m_RangeAttackDetectionDistance);
                    RangeAttackPendingGizmos();


                    break;
                case EGizmosDrawState.SummonSpell:

                    if (m_SummonStartPos != null && m_IsSummonPowerActivated)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawWireSphere(m_SummonStartPos + Vector3.up, 1);
                    }
                    Gizmos.color = Color.white;

                    if (m_SummonReturnPosList.Count > 0)
                    {
                        foreach (var item in m_SummonReturnPosList)
                        {
                            Gizmos.DrawWireSphere(item + Vector3.up, 1);
                        }

                        SummonTargetListShowGizmos();
                    }


                    break;
                case EGizmosDrawState.HealSpell:
                    break;
                case EGizmosDrawState.Dash:

                    Gizmos.DrawWireSphere(transform.position, m_DashAttackHitDistane);
                    break;
                default:
                    break;
            }








        }



        #endregion


                      





































































    }


}
