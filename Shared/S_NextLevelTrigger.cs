using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shared
{
    public class S_NextLevelTrigger : MonoBehaviour
    {
        // [SerializeField] int m_nextLevelIndex = 0;
#if UNITY_EDITOR
        [SerializeField] bool m_disableLoading = false;
#endif
        AsyncOperation m_nextLevel;    

        void Start()
        {
#if UNITY_EDITOR
            if (m_disableLoading) return;
#endif
            int nextScene = (SceneManager.GetActiveScene().buildIndex % (SceneManager.sceneCountInBuildSettings - 1)) + 1;
            m_nextLevel = SceneManager.LoadSceneAsync(nextScene);
            Debug.Log("build index is " + SceneManager.GetActiveScene().buildIndex);
            Debug.Log("build number is " + SceneManager.sceneCountInBuildSettings);
            Debug.Log("NExt level is " + nextScene);
            m_nextLevel.allowSceneActivation = false;
        }

        private void OnTriggerEnter(Collider other)
        {
#if UNITY_EDITOR
            if (m_disableLoading) return;
#endif
            if (other.gameObject.layer != 6) return;

            // S_GameManager.Instance.GameState = S_GameManager.EGameState.Menu; 

            m_nextLevel.allowSceneActivation = true;
        }
    }
}
