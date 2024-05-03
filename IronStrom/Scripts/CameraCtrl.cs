using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public enum CameraAct
{
    In_CameraPoint,//���������λ
    In_EntityPoint,//��Entity��λ��
    In_Free,//�����ӽ�
}

public class CameraCtrl : MonoBehaviour
{
    private static CameraCtrl _CameraCtrl;
    public static CameraCtrl cameraCtrl { get { return _CameraCtrl; } }


    public CameraAct CameraAct_;//�����״̬
    public GameObject CameraPoint_1;
    public GameObject CameraPoint_2;
    public GameObject CameraPoint_3;
    public GameObject CameraPoint_4;
    public GameObject CameraPoint_5;
    public GameObject CameraPoint_6;
    public GameObject CameraPoint_7;
    private GameObject ChosenCameraPoint;//ѡ�е��������λ
    public Entity ChosenEntityCameraPoint;//ѡ�е�Entity�������λ
    public Vector3 EntityCameraPointV3;//ѡ�е�Entity�������λ��Vector3
    public Quaternion EntityCameraPointQuaternion;//ѡ�е�Entity�������λ����תֵ
    public Vector3 preMoveCameraPoint;//������ƶ�֮ǰ��λ��
    public Quaternion preMoveCameraQuaternion;//������ƶ�֮ǰ�ĽǶ�

    public bool Is_Click = false;//�Ƿ��Ѿ�ѡ����
    public bool Is_QuitEntityPoint = true;//�Ƿ��Ѿ��˳�EntityPoint;
    private float Offset = 50f;//�������ƫ����

    [Tooltip("������ƶ��ٶ�")] public float CameraPosSpeed;
    [Tooltip("����������ٶ�")] public float CameraRollerSpeed;
    private float rotationX;
    private float rotationY;
    [Tooltip("ˮƽ������")] public float sensitivityHor;
    [Tooltip("��ֱ������")] public float sensitivityVert;
    [Tooltip("��ֱ������С����")] public float minimumVert;
    [Tooltip("��ֱ����������")] public float maximumVert;


    private void Awake()
    {
        _CameraCtrl = this;
        ChosenEntityCameraPoint = Entity.Null;
    }

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        UpDateMouseCtrl();
        KeyboardCameraCtrl();
        if (CameraAct_ == CameraAct.In_CameraPoint)//���������λ
        {
            UpDateCameraCtrl();

        }
        else if(CameraAct_ == CameraAct.In_EntityPoint)//��Entity��λ��
        {
            CharacterPerspective();
        }
        else if(CameraAct_ == CameraAct.In_Free)//�����ӽ�
        {
            UpDateCameraCtrl();

        }


        PauseGame();


        if (ChosenEntityCameraPoint == Entity.Null)
            Is_Click = false;
        //// �˳���Ϸ
        //if (Input.GetKeyDown(KeyCode.Escape))
        //    Application.Quit();
    }

    //���̿����������λ
    void KeyboardCameraCtrl()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) && Input.GetKey(KeyCode.F4))
            ChosenCameraPoint = CameraPoint_1;
        else if (Input.GetKeyDown(KeyCode.Keypad2) && Input.GetKey(KeyCode.F4))
            ChosenCameraPoint = CameraPoint_2;
        else if (Input.GetKeyDown(KeyCode.Keypad3) && Input.GetKey(KeyCode.F4))
            ChosenCameraPoint = CameraPoint_3;
        else if (Input.GetKeyDown(KeyCode.Keypad4) && Input.GetKey(KeyCode.F4))
            ChosenCameraPoint = CameraPoint_4;
        else if (Input.GetKeyDown(KeyCode.Keypad5) && Input.GetKey(KeyCode.F4))
            ChosenCameraPoint = CameraPoint_5;
        //else if (Input.GetKeyDown(KeyCode.Keypad6) && Input.GetKey(KeyCode.F4))
        //    ChosenCameraPoint = CameraPoint_6;
        //else if (Input.GetKeyDown(KeyCode.Keypad7) && Input.GetKey(KeyCode.F4))
        //    ChosenCameraPoint = CameraPoint_7;
        else
            ChosenCameraPoint = null;

        if (ChosenCameraPoint != null)
        {
            transform.position = ChosenCameraPoint.transform.position;
            transform.rotation = ChosenCameraPoint.transform.rotation;
            ChosenEntityCameraPoint = Entity.Null;
            CameraAct_ = CameraAct.In_CameraPoint;
            Is_QuitEntityPoint = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (preMoveCameraPoint == Vector3.zero || preMoveCameraQuaternion == Quaternion.identity)
                return;
            transform.position = preMoveCameraPoint;
            transform.rotation = preMoveCameraQuaternion;
            preMoveCameraPoint = Vector3.zero;
            preMoveCameraQuaternion = Quaternion.identity;
            ChosenEntityCameraPoint = Entity.Null;
            CameraAct_ = CameraAct.In_Free;
            Is_QuitEntityPoint = true;
        }
    }
    //�������������
    void UpDateCameraCtrl()
    {
        var Pos = transform.position;
        var Worldforward = transform.forward;
        Worldforward.y = 0;
        if (Input.GetKey(KeyCode.W))
        {
            Pos += Worldforward * CameraPosSpeed * Time.deltaTime;
            CameraAct_ = CameraAct.In_Free;
        }
        else if(Input.GetKey(KeyCode.S))
        {
            Pos -= Worldforward * CameraPosSpeed * Time.deltaTime;
            CameraAct_ = CameraAct.In_Free;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Pos -= transform.right * CameraPosSpeed * Time.deltaTime;
            CameraAct_ = CameraAct.In_Free;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Pos += transform.right * CameraPosSpeed * Time.deltaTime;
            CameraAct_ = CameraAct.In_Free;
        }
        //if (Is_Click == false)
        //    ChosenEntityCameraPoint = Entity.Null;


        //if (Input.GetKey(KeyCode.F))
        //    Pos += Vector3.up * CameraPosSpeed * Time.deltaTime;
        //else if (Input.GetKey(KeyCode.R))
        //    Pos -= Vector3.up * CameraPosSpeed * Time.deltaTime;

            //if (Input.GetKey(KeyCode.E))
            //    transform.Rotate(0, CameraPosSpeed * Time.deltaTime, 0, Space.World);
            //else if (Input.GetKey(KeyCode.Q))
            //    transform.Rotate(0, -CameraPosSpeed * Time.deltaTime, 0, Space.World);

        transform.position = Pos;


    }
    //������������Ŀ���
    void UpDateMouseCtrl()
    {
        Cursor.visible = true; // ����������Ҽ�ʱ��ʾ�����

        // ����������Ҽ�ʱ
        if (Input.GetMouseButtonDown(1))
        {
            
        }
        // �������������Ҽ�ʱ
        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false; // ���ع��
            Cursor.lockState = CursorLockMode.Locked; // ������굽��ǰλ��

            rotationY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityHor;
            rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            rotationX = Mathf.Clamp(rotationX, minimumVert, maximumVert);
            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

        }
        // ���ͷ�����Ҽ�ʱ��ֹͣ��ת
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None; // �������
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            Vector3 dir = Vector3.zero;
            // ���scrollֵ����0����ʾ��������ǰ����
            if (scroll > 0f)
                dir = transform.forward * CameraRollerSpeed * Time.deltaTime;
            else// ���scrollֵС��0����ʾ������������
                dir = -transform.forward * CameraRollerSpeed * Time.deltaTime;
            transform.position += dir;
            if (transform.position.y >= CameraPoint_5.transform.position.y + Offset ||
                transform.position.y <= 10f ||
                transform.position.x >= CameraPoint_1.transform.position.x + Offset ||
                transform.position.x <= CameraPoint_2.transform.position.x - Offset ||
                transform.position.z >= CameraPoint_3.transform.position.z + Offset ||
                transform.position.z <= CameraPoint_4.transform.position.z - Offset)
                transform.position -= dir;
        }
    }
    //��������浥λ
    void CharacterPerspective()
    {
        if (ChosenCameraPoint != null)
        {
            ChosenEntityCameraPoint = Entity.Null;
            //Debug.Log("   ѡ�е�λ����");
            return;
        }
        if (ChosenEntityCameraPoint == Entity.Null)
        {
            EntityCameraPointV3 = Vector3.zero;
            //Debug.Log("   ѡ�е�λ����");
            return;
        }
        if(Is_Click == false)
        {
            transform.rotation = EntityCameraPointQuaternion;
            Is_Click = true;
        }

        transform.position = EntityCameraPointV3;
    }

    void PauseGame()//��ͣ��Ϸ
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 0)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;
        }
    }

}
