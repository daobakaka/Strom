using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public enum CameraAct
{
    In_CameraPoint,//在摄像机点位
    In_EntityPoint,//在Entity单位上
    In_Free,//自由视角
}

public class CameraCtrl : MonoBehaviour
{
    private static CameraCtrl _CameraCtrl;
    public static CameraCtrl cameraCtrl { get { return _CameraCtrl; } }


    public CameraAct CameraAct_;//摄像机状态
    public GameObject CameraPoint_1;
    public GameObject CameraPoint_2;
    public GameObject CameraPoint_3;
    public GameObject CameraPoint_4;
    public GameObject CameraPoint_5;
    public GameObject CameraPoint_6;
    public GameObject CameraPoint_7;
    private GameObject ChosenCameraPoint;//选中的摄像机点位
    public Entity ChosenEntityCameraPoint;//选中的Entity摄像机点位
    public Vector3 EntityCameraPointV3;//选中的Entity摄像机点位的Vector3
    public Quaternion EntityCameraPointQuaternion;//选中的Entity摄像机点位的旋转值
    public Vector3 preMoveCameraPoint;//摄像机移动之前的位置
    public Quaternion preMoveCameraQuaternion;//摄像机移动之前的角度

    public bool Is_Click = false;//是否已经选中了
    public bool Is_QuitEntityPoint = true;//是否已经退出EntityPoint;
    private float Offset = 50f;//摄像机的偏移量

    [Tooltip("摄像机移动速度")] public float CameraPosSpeed;
    [Tooltip("摄像机滚轮速度")] public float CameraRollerSpeed;
    private float rotationX;
    private float rotationY;
    [Tooltip("水平灵敏度")] public float sensitivityHor;
    [Tooltip("垂直灵敏度")] public float sensitivityVert;
    [Tooltip("垂直方向最小距离")] public float minimumVert;
    [Tooltip("垂直方向最大距离")] public float maximumVert;


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
        if (CameraAct_ == CameraAct.In_CameraPoint)//在摄像机点位
        {
            UpDateCameraCtrl();

        }
        else if(CameraAct_ == CameraAct.In_EntityPoint)//在Entity单位上
        {
            CharacterPerspective();
        }
        else if(CameraAct_ == CameraAct.In_Free)//自由视角
        {
            UpDateCameraCtrl();

        }


        PauseGame();


        if (ChosenEntityCameraPoint == Entity.Null)
            Is_Click = false;
        //// 退出游戏
        //if (Input.GetKeyDown(KeyCode.Escape))
        //    Application.Quit();
    }

    //键盘控制摄像机点位
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
    //更新摄像机控制
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
    //更新鼠标对相机的控制
    void UpDateMouseCtrl()
    {
        Cursor.visible = true; // 当不按鼠标右键时显示鼠标光标

        // 当按下鼠标右键时
        if (Input.GetMouseButtonDown(1))
        {
            
        }
        // 当按下鼠标持续右键时
        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false; // 隐藏光标
            Cursor.lockState = CursorLockMode.Locked; // 锁定光标到当前位置

            rotationY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityHor;
            rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            rotationX = Mathf.Clamp(rotationX, minimumVert, maximumVert);
            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

        }
        // 当释放鼠标右键时，停止旋转
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None; // 解锁光标
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            Vector3 dir = Vector3.zero;
            // 如果scroll值大于0，表示鼠标滚轮向前滚动
            if (scroll > 0f)
                dir = transform.forward * CameraRollerSpeed * Time.deltaTime;
            else// 如果scroll值小于0，表示鼠标滚轮向后滚动
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
    //摄像机跟随单位
    void CharacterPerspective()
    {
        if (ChosenCameraPoint != null)
        {
            ChosenEntityCameraPoint = Entity.Null;
            //Debug.Log("   选中单位归零");
            return;
        }
        if (ChosenEntityCameraPoint == Entity.Null)
        {
            EntityCameraPointV3 = Vector3.zero;
            //Debug.Log("   选中单位归零");
            return;
        }
        if(Is_Click == false)
        {
            transform.rotation = EntityCameraPointQuaternion;
            Is_Click = true;
        }

        transform.position = EntityCameraPointV3;
    }

    void PauseGame()//暂停游戏
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
