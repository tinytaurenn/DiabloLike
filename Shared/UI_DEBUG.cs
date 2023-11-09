using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

namespace Shared
{
    public class UI_DEBUG : MonoBehaviour
    {
        public static UI_DEBUG s_Instance;

        [SerializeField]
        Image m_LifeImage;
        [SerializeField]
        Image m_ManaImage;

        [SerializeField]
        [Range(1f,5f)]
        private float m_LerpSpeed = 2f;

        private int m_maxLife;
        private int m_maxMana;
        Material m_lifeMat;
        Material m_manaMat;



        Transform m_fpsDisplay;
        float fps = 0f;

        private void Awake()
        {

            if (s_Instance == null)
            {
                s_Instance = this;
            }
            else
            {
                
                Destroy(this.gameObject);
            }

            m_lifeMat = Instantiate(m_LifeImage.material);
            m_manaMat = Instantiate(m_ManaImage.material);

          
            
            StartCoroutine(StartEndOfFrame());



        }

        

        IEnumerator StartEndOfFrame()
        {
            yield return new WaitForEndOfFrame();

            m_maxLife = S_GameManager.Instance.Life;

            print("max life " + m_maxLife);
            m_maxMana = S_GameManager.Instance.Mana;

            m_lifeMat.SetFloat("_Level", (float)S_GameManager.Instance.Life / m_maxLife);
            m_manaMat.SetFloat("_Level", (float)S_GameManager.Instance.Mana / m_maxMana);
        }

        private void OnEnable()
        {

            if (S_GameManager.Instance != null)
            {
                m_lifeMat.SetFloat("_Level", (float)S_GameManager.Instance.Life / m_maxLife);
                m_manaMat.SetFloat("_Level", (float)S_GameManager.Instance.Mana / m_maxMana);
            }

           
            m_LifeImage.material = m_lifeMat;
            m_ManaImage.material = m_manaMat;


            S_GameManager.OnLifeUpdate += UpdateLife; 
            S_GameManager.OnManaUpdate += UpdateMana;
        }

        private void OnDisable()
        {
            //if (S_GameManager.Instance)
            //{
            //    m_lifeMat.SetFloat("_Level", S_GameManager.Instance.MaxLife);
            //    m_manaMat.SetFloat("_Level", S_GameManager.Instance.MaxMana);
            //}
            
            S_GameManager.OnLifeUpdate -= UpdateLife;
            S_GameManager.OnManaUpdate -= UpdateMana;
        }

        void Start()
        {
            m_fpsDisplay = transform.GetChild(0).transform.GetChild(0);

            if (!Debug.isDebugBuild) m_fpsDisplay.gameObject.SetActive(false);
        }
        
        // Update is called once per frame
        void Update()
        {
            if (Debug.isDebugBuild)
            {
                if (!S_GameManager.Instance.GameState.Equals(S_GameManager.EGameState.Pause))
                    fps += (Time.unscaledDeltaTime - fps) * 0.1f;
                m_fpsDisplay.GetComponent<TextMeshProUGUI>().SetText("FPS : " + (int)(1 / fps));
            }
        }



        void UpdateLife(int value)
        {
            StartCoroutine(UpdateLerpLife());
        }
        IEnumerator UpdateLerpLife()
        {
            while (m_lifeMat.GetFloat("_Level") != S_GameManager.Instance.Life)
            {
                m_lifeMat.SetFloat("_Level", Mathf.Lerp(m_lifeMat.GetFloat("_Level"), (float)S_GameManager.Instance.Life / m_maxLife, Time.deltaTime *  m_LerpSpeed));
                yield return new WaitForEndOfFrame();
            }
        }
        void UpdateMana(int value)
        {
            StartCoroutine(UpdateLerpMana());
        }
        IEnumerator UpdateLerpMana()
        {
            while (m_manaMat.GetFloat("_Level") != S_GameManager.Instance.Mana)
            {
                m_manaMat.SetFloat("_Level", Mathf.Lerp(m_manaMat.GetFloat("_Level"), (float)S_GameManager.Instance.Mana / m_maxMana, Time.deltaTime *  m_LerpSpeed));
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
