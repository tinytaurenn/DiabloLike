using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    [CreateAssetMenu(fileName = "NP_Player_Sprites", menuName = "ScriptableObjects/CharacterSprites", order = 60)]
    public class S_CharacterSprites : ScriptableObject
    {
        // private static S_CharacterSprites s_instance;

        [Header("SelectionWheel Sprite :")]
        [SerializeField] private Sprite m_selectionWheel;

        [Header("Skills Sprites :")]
        [SerializeField] private Sprite m_meleeAttack;
        [SerializeField] private Sprite m_rangeAttack;
        [SerializeField] private Sprite m_summon;
        [SerializeField] private Sprite m_heal;
        [SerializeField] private Sprite m_dodge;

        public enum ESkillSprite
        {
            MeleeAttack,
            RangeAttack,
            Summon,
            Dodge,
            Heal,
        }

        Dictionary<ESkillSprite, Sprite> m_skills = new Dictionary<ESkillSprite, Sprite>();

        private void SetDictionary()
        {
            if (m_skills.Count <= 0)
            {
                m_skills.Add(ESkillSprite.Heal, m_heal);
                m_skills.Add(ESkillSprite.MeleeAttack, m_meleeAttack);
                m_skills.Add(ESkillSprite.RangeAttack, m_rangeAttack);
                m_skills.Add(ESkillSprite.Summon, m_summon);
                m_skills.Add(ESkillSprite.Dodge, m_dodge);
            }
        }

        public Sprite GetSkillSprite(ESkillSprite sprite)
        {
            SetDictionary();
            return m_skills[sprite];
        }

        // GETTERS :
        public Sprite SelectionWheel => m_selectionWheel;
        public Sprite MeleeAttack => m_meleeAttack;
        public Sprite RangeAttack => m_rangeAttack;
        public Sprite Summon => m_summon;
        public Sprite Heal => m_heal;
        public Sprite Dodge => m_dodge;
    }
}
