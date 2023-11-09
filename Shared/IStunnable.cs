using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public interface IStunnable
    {
        public void Rooted(float time);

        public void Rooted();

        public void UnRooted();

        public void Stunned(); 

        public void Stunned(float time);

        public void UnStunned(); 
        
    }
}
