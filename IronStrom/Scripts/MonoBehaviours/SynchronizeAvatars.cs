using TMPro;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class SynchronizeAvatars : MonoBehaviour
{
    public Image _avatar;//ͷ��
    public TextMeshProUGUI _name;//����
    public GameObject _HPSlider;//ʿ����Ѫ��Obj
    public GameObject _MutinySlider;//�߷���Obj
    public Image _HPImage;
    public Image _MutinyImage;
    public Entity SynEntity;//�����Entity

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // �⽫ʹѪ����ǰ�泯��������������Ҫ�������򣬿����޸�transform.forward����
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }


}
