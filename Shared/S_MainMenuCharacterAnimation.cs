using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public class S_MainMenuCharacterAnimation : MonoBehaviour
    {

        
        private void OnEnable()
        {
            //Debug.Log(transform.GetSiblingIndex().ToString());
            transform.GetComponent<Animator>().SetTrigger(transform.GetSiblingIndex().ToString());
        }
    }
}
