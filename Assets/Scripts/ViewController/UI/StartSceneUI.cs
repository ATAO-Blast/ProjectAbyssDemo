using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AbyssDemo
{
    public class StartSceneUI : MonoBehaviour
    {
        public void ToScene()
        {
            SceneManager.LoadSceneAsync(1);
        }
        public void Quit()
        {
            Application.Quit();
        }
    }
}