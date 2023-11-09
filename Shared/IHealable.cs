using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public interface IHealable
    {
        /// <summary>
        /// Heal target for a certain amount
        /// </summary>
        /// <param name="healAmount"></param>
        public void Heal(int healAmount); 
    }
}
