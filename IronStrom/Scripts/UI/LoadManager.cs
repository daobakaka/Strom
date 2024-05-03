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

        //��ȡ���ڳ���Index+1��Ҳ���ǻ�ȡ��һ������
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        //��һ���������غú󣬲�Ҫ�Զ���ת
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            slider.value = operation.progress;
            text.text = operation.progress * 100 + "%";
            if(operation.progress >= 0.9f)
            {
                slider.value = 1;
                text.text = "�밴���������";
                if(Input.anyKeyDown)
                    operation.allowSceneActivation = true;
            }
            yield return null;
        }

    }


}
