using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntoNoticeFrame : MonoBehaviour//�������֪ͨ
{
    public Image PlayerHeadshot;//���ͷ��
    public TextMeshProUGUI playerName;//�������
    public TextMeshProUGUI IntoTeam;//��Ҽ���Ķ���

    //��ͬ����ͷ���
    public GameObject IntoNoticeFrame1_3;
    public GameObject IntoNoticeFrame4_10;
    public GameObject IntoNoticeFrame11_30;
    public GameObject IntoNoticeFrame31_50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //֪ͨ��Ч�ر��Լ�
    public void CloseActive()
    {
        IntoNoticeManager.Instance.Is_IntoNoticePlaying = false;
        gameObject.SetActive(false);
        IntoNoticeFrame1_3.SetActive(false);
        IntoNoticeFrame4_10.SetActive(false);
        IntoNoticeFrame11_30.SetActive(false);
        IntoNoticeFrame31_50.SetActive(false);
    }
}
