using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_DEBUG_Combat_Dummy : MonoBehaviour,IDamageable,IHealable
    {
        public void Heal(int healAmount)
        {
            print("Healing " + healAmount);
        }

        public void TakeDamage(int dmg)
        {
            print("Taking " + dmg );
        }

        public void TakeDamage(int dmg, IDamageable.EAttackSource src)
        {
            print ("Taking " + dmg +" from " + src.ToString ());
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
