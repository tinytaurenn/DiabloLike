using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public interface IEntity : IDamageable
    {
        /// <summary>
        /// Triggers range attack
        /// </summary>
        public void RangeAttack();

        /// <summary>
        /// Triggers melee attack
        /// </summary>
        public void MeleeAttack();
    }
}
