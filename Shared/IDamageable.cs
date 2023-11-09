using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public interface IDamageable
    {

        enum EAttackSource
        {
            Melee,
            Range,
            Heal,
            Summon,
            Dodge,
        }

        /// <summary>
        /// Loses an amount of health points
        /// </summary>
        /// <param name="dmg">The amount of damage taken.</param>
        public void TakeDamage(int dmg);

        /// <summary>
        /// Loses an amount of health points
        /// </summary>
        /// <param name="dmg">The amount of damage taken.</param>
        /// <param name="src">The source of the damage.</param>
        public void TakeDamage(int dmg, EAttackSource src);
    }
}