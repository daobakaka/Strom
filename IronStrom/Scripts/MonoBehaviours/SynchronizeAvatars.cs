using TMPro;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class SynchronizeAvatars : MonoBehaviour
{
    public Image _avatar;//头像
    public TextMeshProUGUI _name;//名字
    public GameObject _HPSlider;//士兵的血条Obj
    public GameObject _MutinySlider;//策反条Obj
    public Image _HPImage;
    public Image _MutinyImage;
    public Entity SynEntity;//跟随的Entity

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 这将使血条的前面朝向摄像机，如果需要调整朝向，可以修改transform.forward参数
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }


}
