using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using LuitotGaetan;
using Unity.VisualScripting;

namespace Shared
{
    public class S_SelectionWheel : MonoBehaviour
    {
        private static S_SelectionWheel s_instance;
        [SerializeField] private uint m_nbCharacters = 5; // The number of characters to display in the wheel.
        [SerializeField] private float m_joystickDeadzone = 0.4f;

        [SerializeField] private Vector2 m_defaultSize = new(256f, 256f);
        [SerializeField] private Vector2 m_higlightSize = new(325f, 325f);
        // [SerializeField] private float m_highlightDisplacement = 15f;
        [SerializeField] private float m_highlightScale = 1.2f;

        // Character Parent :
        private List<RectTransform> m_listChar = new List<RectTransform>();
        // Character Image :
        [SerializeField] private List<RectTransform> m_listCharImgTransform = new List<RectTransform>();
        private List<Material> m_listCharImgMat = new List<Material>();
        private List<Image> m_listCharImg = new List<Image>();
        private List<Vector2> m_listCharImgSize = new List<Vector2>();
        private List<Vector2> m_listCharImgPos = new List<Vector2>();
        // Chracter Mask :
        [SerializeField] private List<RectTransform> m_listCharMask = new List<RectTransform>();
        private List<Image> m_listCharMaskImg = new List<Image>();
        // Border :
        [SerializeField] private List<RectTransform> m_listBorder = new List<RectTransform>();
        private List<Material> m_listBorderMat = new List<Material>();

        [SerializeField] private GameObject m_uiWheel;

        private uint m_currentCharacter = 0; // The currently selected character.
        private uint m_selectedCharacter = 0; // The selected character in the Selection Wheel.

        private bool m_isWheelOpen = false;
        private bool m_noSelection = true;

        // Debug :
        [SerializeField] private Color m_unselectedColor = new(0, 0, 0, 0.2941177f);
        // [ColorUsage(true, true)]
        [SerializeField] private List<Color> m_selectedColor = new List<Color>();
        // private float m_wheelSize = 200;
        private float m_rotationAngle = 0.1f * Mathf.PI;
        public static bool HasInstance = s_instance != null;

        private void Start()
        {
            m_nbCharacters = Convert.ToUInt32(m_listCharImgTransform.Count);
            SetWheelPosistion();
            SetList();
        }

        private void Awake()
        {
            SetSingleton();
        }

        private void SetList()
        {
            int i = 0;
            foreach (RectTransform imgTransform in m_listCharImgTransform)
            {
                m_listChar.Add(imgTransform.parent.parent.GetComponent<RectTransform>());
                m_listCharImg.Add(imgTransform.GetComponent<Image>());
                m_listCharImgSize.Add(imgTransform.sizeDelta);
                m_listCharImgPos.Add(imgTransform.anchoredPosition);
                // Materials :
                m_listCharImgMat.Add(new Material(m_listCharImg[i].material));
                m_listCharImg[i].material = m_listCharImgMat[i];
                // Borders :
                m_listBorderMat.Add(new Material(m_listBorder[i].GetComponent<Image>().material));
                m_listBorder[i].GetComponent<Image>().material = m_listBorderMat[i];
                // Masks :
                m_listCharMaskImg.Add(m_listCharMask[i].GetComponent<Image>());
                i++;
            }
        }

        private void OnEnable()
        {
            
        }

        private void SetSingleton()
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

        }

        /// <summary>
        /// Set the position of each character image on the wheel responsively
        /// </summary>
        private void SetWheelPosistion()
        {
            float angle = GL_Utils.TAU / m_nbCharacters;
            float angleOffset = ((angle / 2f) - Mathf.PI - m_rotationAngle) % GL_Utils.TAU;

            /*int i = 0;
            foreach (Image character in m_listCharImg)
            {
                float charAngle = -i * angle - angleOffset;
                character.transform.localPosition = new Vector3(Mathf.Cos(charAngle), Mathf.Sin(charAngle)) * m_wheelSize;
                i++;
            }*/
        }

        /// <summary>
        /// Select a character from the Selection Wheel for the given joystick direction.
        /// </summary>
        private void SelectCharacter(Vector2 joystickDir)
        {
            // Using the sqrMagnitude because less expensive than magnitude :
            if (joystickDir.sqrMagnitude <= m_joystickDeadzone * m_joystickDeadzone)
            {
                m_noSelection = true;
                return; // If in deadzone : do nothing
            }

            float radAngle = Mathf.Abs((Mathf.Atan2(joystickDir.y, joystickDir.x) - Mathf.PI - m_rotationAngle) % GL_Utils.TAU); // Scope -> [0; 2pi]
            // Debug.Log(radAngle + "/" + GL_Utils.TAU + " * " + m_nbCharacters);
            m_selectedCharacter = Convert.ToUInt32(Mathf.FloorToInt(radAngle / GL_Utils.TAU * m_nbCharacters)); // Scope -> [0; m_nbCharacters - 1]
            // Debug.Log(m_selectedCharacter);
            m_noSelection = false;
        }

        #region PlayerInput Triggers
        /// <summary>
        /// Triggered when the player pushes the button that opens the selection wheel.
        /// </summary>
        /// <param name="context">Information from the event.</param>
        private void POpenWheel(InputAction.CallbackContext context)
        {
            if (context.performed) // When the button is pressed : open the wheel.
            {
                m_currentCharacter = (uint)S_GameManager.Instance.CurrentPlayerIndex;
                SetupWheel();
                m_isWheelOpen = true;

                m_uiWheel.SetActive(true); // Display the selection wheel on the screen.
            }
            else if (context.canceled) // When the button is released : close the wheel.
            {
                m_isWheelOpen = false;

                m_uiWheel.SetActive(false); //  Hide the selection wheel.

                if (!m_noSelection && m_selectedCharacter != m_currentCharacter)
                {
                    OnPlayerChange();
                }
            }
        }

        private void SetupWheel()
        {
            int currentCharacter = (int)m_currentCharacter;
            m_listCharMask[currentCharacter].sizeDelta = m_defaultSize;
            // m_listCharMask[currentCharacter].
            m_listBorder[currentCharacter].sizeDelta = m_defaultSize;
            m_listCharImgTransform[currentCharacter].sizeDelta = m_listCharImgSize[currentCharacter];
            m_listBorderMat[currentCharacter].SetFloat("_Selected", 0f);

            for (int i = 0; i < m_listCharImgMat.Count && i < m_listCharMaskImg.Count; i++)
            {
                m_listCharImgMat[i].SetFloat("_Saturation", i == m_currentCharacter ? 0f : 1f);
                m_listCharMaskImg[i].color = i == m_currentCharacter ? m_unselectedColor : Color.white;
            }
        }

        private void OnPlayerChange()
        {
            //Debug.Log("Changing to character " + m_selectedCharacter);
            //S_GameManager.Instance.SwitchPlayer((int)m_selectedCharacter);
            S_GameManager.Instance.CurrentPlayerIndex = (int)m_selectedCharacter;
            S_SkillManager.SetCharacterSprites((int)m_selectedCharacter);
            m_currentCharacter = m_selectedCharacter;
        }

        public static void OpenWheel(InputAction.CallbackContext context)
        {
            s_instance.POpenWheel(context);
        }

        /// <summary>
        /// Select a character from joystick direction.
        /// Triggered when the player moves the left Joystick.
        /// </summary>
        /// <param name="context">Information from the event.</param>
        private void PSelectCharacter(InputAction.CallbackContext context)
        {
            if (m_isWheelOpen && context.performed) // If the wheel is open :
            {
                Vector2 joystickDir = context.ReadValue<Vector2>(); // Get the direction of the joystick.
                SelectCharacter(joystickDir);
                // @TODO : Update the Selection Wheel UI (to show the current selected character)
                UpdateSelection();
            }
        }

        public static void SelectCharacter(InputAction.CallbackContext context)
        {
            s_instance.PSelectCharacter(context);
        }

        public static void SetCurrentCharacter(int newIndex)
        {
            s_instance.m_currentCharacter = (uint)newIndex;
        }
        #endregion

        private void UpdateSelection()
        {
            for (int i = 0; i < m_listCharMask.Count && i < m_listBorder.Count; i++)
            {
                if (i == m_currentCharacter) continue;
                // Debug.Log(m_noSelection + " : " + m_selectedCharacter);
                bool isDefault = (m_noSelection || i != m_selectedCharacter);
                if (isDefault) m_listChar[i].SetAsFirstSibling();
                m_listCharMask[i].sizeDelta = isDefault ? m_defaultSize : m_higlightSize;
                m_listBorder[i].sizeDelta = isDefault ? m_defaultSize : m_higlightSize;
                m_listCharImgTransform[i].sizeDelta = isDefault ? m_listCharImgSize[i] : m_listCharImgSize[i] * m_highlightScale;
                m_listBorderMat[i].SetFloat("_Selected", isDefault ? 0f : 1f);
                m_listCharMaskImg[i].color = isDefault ? Color.white : m_selectedColor[i];
                // m_listCharImgTransform[i].localPosition = isDefault ? m_listCharImgPos[i] : m_listCharImgPos[i] + Vector2.up * m_highlightScale * 15f;
            }
        }

        // GETTERS :

        public static bool IsOpen => s_instance.m_isWheelOpen;
    }
}
