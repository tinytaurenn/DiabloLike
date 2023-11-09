using LuitotGaetan;
using System;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Shared.S_CharacterSprites;
using static UnityEngine.Rendering.DebugUI;

namespace Shared
{
    public class S_SkillManager : MonoBehaviour
    {
        private static S_SkillManager s_instance;
        [Header("Characters Sprites :")]
        [SerializeField] private List<S_CharacterSprites> m_charactersSprites;

        [Header("UI Elements :")]
        [SerializeField] private GameObject m_healSkill;
        [SerializeField] private GameObject m_meleeSkill;
        [SerializeField] private GameObject m_rangeSkill;
        [SerializeField] private GameObject m_dodgeSkill;
        [SerializeField] private GameObject m_summonSkill;
        // UI Gameobject :
        private Dictionary<ESkillSprite, GameObject> m_skills = new Dictionary<ESkillSprite, GameObject>();
        // UI Images components :
        private Dictionary<ESkillSprite, Image> m_skillsImg = new Dictionary<ESkillSprite, Image>();
        // UI Material components :
        private Dictionary<ESkillSprite, Material> m_skillsMat = new Dictionary<ESkillSprite, Material>();
        // Cooldown duration
        private CooldownCollection m_cooldown;

        public class CooldownCollection
        {
            private List<Dictionary<ESkillSprite, Vector2>> m_cooldown = new List<Dictionary<ESkillSprite, Vector2>>();

            public void Init(int m_nbPlayer)
            {
                for (int i = 0; i < m_nbPlayer; i++)
                {
                    m_cooldown.Add(new Dictionary<ESkillSprite, Vector2>());
                }
                // m_cooldown[S_GameManager.Instance.CurrentPlayerIndex].Add(playerCooldown);
            }

            public Dictionary<ESkillSprite, Vector2> GetCooldownsFromPlayer(int playerIndex)
            {
                return m_cooldown[playerIndex % Count];
            }

            public float this[ESkillSprite skill]
            {
                get { return m_cooldown[CurrentCharacter][skill].x; }
                set { m_cooldown[CurrentCharacter][skill] = new Vector2(0f, value); }
            }

            private int CurrentCharacter => S_GameManager.Instance.CurrentPlayerIndex % m_cooldown.Count;

            public int Count => m_cooldown.Count;
        }

        public static CooldownCollection Cooldown => s_instance.m_cooldown;


        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else
            {
                Debug.Log("Multiple instances of S_SelectionWheel.");
                Destroy(this.gameObject);
            }

            GenerateDictionary();
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            SetCurrentCharacterSprites();
        }

        private void Update()
        {
            MaterialUpdate();
        }

        private void OnEnable()
        {
            SetCurrentCharacterSprites(); 
        }

        private void SetCurrentCharacterSprites()
        {

            try
            {
                Debug.Log(" current player index is : " + S_GameManager.Instance.CurrentPlayerIndex);
                SetCharacterSprites(S_GameManager.Instance.CurrentPlayerIndex);
            }
            catch (NullReferenceException)
            {

                 Debug.LogWarning("set current char null reference");
            }
            
        }

        private void MaterialUpdate()
        {
            foreach (ESkillSprite skill in m_skillsMat.Keys)
            {
                Vector2 timer = m_cooldown.GetCooldownsFromPlayer(CurrentCharacter)[skill];
                if (timer.x <= timer.y)
                {
                    timer.x += Time.deltaTime;
                    m_skillsMat[skill].SetFloat("_Cooldown", timer.x / timer.y);
                    m_cooldown.GetCooldownsFromPlayer(CurrentCharacter)[skill] = timer;
                }
            }
        }

        private void ResetMaterials()
        {
            foreach (ESkillSprite skill in m_skillsMat.Keys)
            {
                Vector2 timer = m_cooldown.GetCooldownsFromPlayer(CurrentCharacter)[skill];
                if (timer.x >= timer.y)
                    m_skillsMat[skill].SetFloat("_Cooldown", 1f);
            }
        }

        private void GenerateDictionary()
        {
            m_cooldown = new CooldownCollection();

            m_skills.Add(ESkillSprite.Heal, m_healSkill);
            m_skills.Add(ESkillSprite.MeleeAttack, m_meleeSkill);
            m_skills.Add(ESkillSprite.RangeAttack, m_rangeSkill);
            m_skills.Add(ESkillSprite.Dodge, m_dodgeSkill);
            m_skills.Add(ESkillSprite.Summon, m_summonSkill);


            m_cooldown.Init(m_charactersSprites.Count);

            foreach (ESkillSprite skill in m_skills.Keys)
            {
                m_skillsImg.Add(skill, m_skills[skill].GetComponent<Image>());
                
                // Create material instance :
                m_skillsImg[skill].material = new(m_skillsImg[skill].material);
                m_skillsMat.Add(skill, m_skillsImg[skill].material);
                
                for (int i = 0; i < m_charactersSprites.Count; i++)
                {
                    m_cooldown.GetCooldownsFromPlayer(i).Add(skill, new Vector2(Mathf.Infinity, 1f));
                }
            }
        }
        

        public static void SetCharacterSprites(S_CharacterSprites characterSprites)
        {
            foreach (ESkillSprite skill in s_instance.m_skillsImg.Keys)
            {
                s_instance.m_skillsImg[skill].sprite = characterSprites.GetSkillSprite(skill);
            }
            s_instance.ResetMaterials();
        }

        public static void SetCharacterSprites(int characterIndex)
        {
            SetCharacterSprites(s_instance.m_charactersSprites[characterIndex % s_instance.m_charactersSprites.Count]);
        }


        /* On a pas le droit :
        

        /// <summary>
        /// Set cooldown to a given skill.
        /// </summary>
        /// <param name="skill">The skill to set the cooldown.</param>
        /// <param name="duration">The cooldown duration in seconds.</param>
        public static void SetCooldown(ESkillSprite skill, float duration)
        {
            s_instance.m_cooldown[CurrentCharacter][skill] = new Vector2(0f, duration);
        }

        /// <summary>
        /// Get current cooldown for the given skill of current character.
        /// </summary>
        /// <param name="skill">The skill to get the cooldown.</param>
        public static float GetCooldown(ESkillSprite skill)
        {
            return s_instance.m_cooldown[CurrentCharacter][skill].x;
        }

        /// <summary>
        /// Get current cooldown for given character and skill.
        /// </summary>
        /// <param name="characterIndex">The character to target.</param>
        /// <param name="skill">The skill to get the cooldown.</param>
        public static float GetCooldown(int characterIndex, ESkillSprite skill)
        {
            return s_instance.m_cooldown[characterIndex % s_instance.m_cooldown.Count][skill].x;
        }

        /// <summary>
        /// Get the remaining time of cooldown for the given character and skill.
        /// </summary>
        /// <param name="characterIndex">The character to target.</param>
        /// <param name="skill">The skill to get the cooldown.</param>
        
        public static float GetRemainingCooldown(int characterIndex, ESkillSprite skill)
        {
            Vector2 time = s_instance.m_cooldown[characterIndex % s_instance.m_cooldown.Count][skill];
            return Mathf.Clamp(time.y - time.x, 0f, time.y);
        }

        /// <summary>
        /// Get the remaining time of cooldown for the given skill of current character.
        /// </summary>
        /// <param name="skill">The skill to get the cooldown.</param>
        public static float GetRemainingCooldown(ESkillSprite skill)
        {
            Vector2 time = s_instance.m_cooldown[CurrentCharacter][skill];
            return Mathf.Clamp(time.y - time.x, 0f, time.y);
        }
        */

        private static int CurrentCharacter => S_GameManager.Instance.CurrentPlayerIndex % s_instance.m_cooldown.Count;
    }
}
