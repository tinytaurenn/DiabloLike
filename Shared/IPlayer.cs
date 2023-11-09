using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public interface IPlayer : IEntity
    {
        // Inputs :
        /// <summary>
        /// Update player's movement
        /// </summary>
        public void Move();

        /// <summary>
        /// Triggers dodge
        /// </summary>
        public void Dodge();

        /// <summary>
        /// Triggers heal spell
        /// </summary>
        public void Heal();

        /// <summary>
        /// Triggers summon spell
        /// </summary>
        public void Summon();
    }
}
