using UnityEngine;


public class GameInitConfigManager : MonoBehaviour
{
    public TextAsset SoldierData;
    //string name;
    //float HP;
    //float Speed;
    //float AT;
    //float DB;
    //public SXAspects sxAsp;




    private static GameInitConfigManager _GameInitConfigManager;
    public static GameInitConfigManager gameInitConfigManager { get { return _GameInitConfigManager; } }


    private void Awake()
    {
        _GameInitConfigManager = this;

        //ShiBingName SBName = ShiBingName.Null;

        //JsonData Jdata = JsonMapper.ToObject(SoldierData.text);
        //for (int i = 0; i < Jdata.Count; ++i)
        //{
        //    JsonData jsondata = Jdata[i];

        //    SBName = JsonUtil.ToEnum<ShiBingName>(jsondata, "id");

        //    HP = JsonUtil.ToFloat(jsondata, "HP");
        //    Speed = JsonUtil.ToFloat(jsondata, "Speed");
        //    AT = JsonUtil.ToFloat(jsondata, "AT");
        //    DB = JsonUtil.ToFloat(jsondata, "DB");
        //    if (SBName == ShiBingName.JianYa)
        //    {
        //        GameObject jianya = PrefabManager.prefabManager.Team1_JianYa;
        //        SXAuthoring sxAut = jianya.GetComponent<SXAuthoring>();
        //        sxAut.HP = JsonUtil.ToFloat(jsondata, "HP");
        //        Debug.Log("  Ãû×Ö  " + SBName + "   HP : " + sxAut.HP);
        //    }
        //}
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
