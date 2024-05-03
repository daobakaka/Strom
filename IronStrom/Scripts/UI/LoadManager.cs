using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadManager : MonoBehaviour
{
    public GameObject loadScreen;
    public Slider slider;
    public TextMeshProUGUI text;


    private void Start()
    {
        LoadNextLevel();
    }

    private void Update()
    {
        
    }

    public void LoadNextLevel()
    {
        StartCoroutine(Loadlevel());
    }

    IEnumerator Loadlevel()
    {
        //loadScreen.SetActive(true);

        //获取现在场景Index+1，也就是获取下一个场景
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        //下一个场景加载好后，不要自动跳转
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            slider.value = operation.progress;
            text.text = operation.progress * 100 + "%";
            if(operation.progress >= 0.9f)
            {
                slider.value = 1;
                text.text = "请按任意键继续";
                if(Input.anyKeyDown)
                    operation.allowSceneActivation = true;
            }
            yield return null;
        }

    }


}
