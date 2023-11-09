using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace Shared
{
    public class S_UI_SplashScreen : MonoBehaviour
    {

        [SerializeField]
        private Image m_splashScreenTransparency;
        [SerializeField]
        private TextMeshProUGUI m_splashScreenTextTransparency;
        [SerializeField]
        [Range(1f, 10f)]
        private float m_ScreenOpacitySpeedModifier = 2f;
        [SerializeField]
        private float m_waitForSecBeforeDisapeared = 3f;
        [SerializeField]
        private int m_mainMenuID = 1;
        AsyncOperation m_menuScene;


        
        void Start()
        {

            StartCoroutine(MoveToPrimaryDisplay());
            StartCoroutine(ScreenEffect());
            m_menuScene = SceneManager.LoadSceneAsync(m_mainMenuID);
            m_menuScene.allowSceneActivation = false;

            
        }


        // code borrowed on https://forum.unity.com/threads/how-to-get-game-build-to-run-on-main-display.1273394/ to fix double screen issue 
        IEnumerator MoveToPrimaryDisplay()
        {
            List<DisplayInfo> displays = new List<DisplayInfo>();
            Screen.GetDisplayLayout(displays);
            if (displays?.Count > 0)
            {
                var moveOperation = Screen.MoveMainWindowTo(displays[0], new Vector2Int(displays[0].width / 2, displays[0].height / 2));
                yield return moveOperation;
            }
        }


        IEnumerator ScreenEffect()
        {
            float alpha = 0f;
            while (alpha < 1)
            {
                m_splashScreenTransparency.color = new Color(m_splashScreenTransparency.color.r, m_splashScreenTransparency.color.g, m_splashScreenTransparency.color.b, alpha);
                alpha += Time.deltaTime / (m_ScreenOpacitySpeedModifier/2);
                yield return new WaitForEndOfFrame();
            }
            alpha = 1f;
            yield return new WaitForSeconds(m_waitForSecBeforeDisapeared/2);
            while (alpha > 0)
            {
                m_splashScreenTransparency.color = new Color(m_splashScreenTransparency.color.r, m_splashScreenTransparency.color.g, m_splashScreenTransparency.color.b, alpha);
                alpha -= Time.deltaTime / m_ScreenOpacitySpeedModifier;
                yield return new WaitForEndOfFrame();
            }
            alpha = 0f;
            m_splashScreenTransparency.color = new Color(m_splashScreenTransparency.color.r, m_splashScreenTransparency.color.g, m_splashScreenTransparency.color.b, alpha);
            while (alpha < 1)
            {
                m_splashScreenTextTransparency.color = new Color(m_splashScreenTextTransparency.color.r, m_splashScreenTextTransparency.color.g, m_splashScreenTextTransparency.color.b, alpha);
                alpha += Time.deltaTime / m_ScreenOpacitySpeedModifier;
                yield return new WaitForEndOfFrame();
            }
            alpha = 1f;
            yield return new WaitForSeconds(m_waitForSecBeforeDisapeared);
            while (alpha > 0)
            {
                m_splashScreenTextTransparency.color = new Color(m_splashScreenTextTransparency.color.r, m_splashScreenTextTransparency.color.g, m_splashScreenTextTransparency.color.b, alpha);
                alpha -= Time.deltaTime / m_ScreenOpacitySpeedModifier;
                yield return new WaitForEndOfFrame();
            }
            m_menuScene.allowSceneActivation = true;          
            
        }
    }
}
