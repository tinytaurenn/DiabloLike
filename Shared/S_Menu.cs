using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public class S_Menu : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_firstSelectableButton;

        public GameObject FirstSelectableButton
        {
            get
            {
                return m_firstSelectableButton;
            }
        }
    }
}
