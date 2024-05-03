using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SynchronizeManager : MonoBehaviour
{
    private static SynchronizeManager _SynchronizeManager;
    public static SynchronizeManager synchronizeManager { get { return _SynchronizeManager; } }

    //Entity∫ÕGameObjµƒ◊÷µ‰
    public Dictionary<Entity, GameObject> SynAniDic = new Dictionary<Entity, GameObject>();

    //GameObj
    public GameObject HuoShenAni_1;
    public GameObject HuoShenAni_2;
    public GameObject RongDianAni_1;
    public GameObject RongDianAni_2;
    public GameObject XiNiuAni_1;
    public GameObject XiNiuAni_2;
    public GameObject Monster_1;//π÷ ﬁ
    public GameObject Monster_2;//π÷ ﬁ
    public GameObject Monster_3;//π÷ ﬁ
    public GameObject Monster_4;//π÷ ﬁ
    public GameObject Monster_5;//π÷ ﬁ
    public GameObject Monster_6;//π÷ ﬁ
    public GameObject Monster_7;//π÷ ﬁ

    // Start is called before the first frame update
    void Start()
    {
        _SynchronizeManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject InstanCorrespondObj(ShiBingName name, int teamID)//Entity∂‘”¶µƒObj
    {
        if (teamID == 1)
        {
            switch (name)
            {
                case ShiBingName.HuoShen: return Instantiate(HuoShenAni_1);
                case ShiBingName.RongDian: return Instantiate(RongDianAni_1);
                case ShiBingName.XiNiu: return Instantiate(XiNiuAni_1);
            }
        }
        else if (teamID == 2)
        {
            switch (name)
            {
                case ShiBingName.HuoShen: return Instantiate(HuoShenAni_2);
                case ShiBingName.RongDian: return Instantiate(RongDianAni_2);
                case ShiBingName.XiNiu: return Instantiate(XiNiuAni_2);
            }
        }
        else if (teamID == 3)
        {
            switch (name)
            {
                case ShiBingName.Monster_1: return Instantiate(Monster_1);
                case ShiBingName.Monster_2: return Instantiate(Monster_2);
                case ShiBingName.Monster_3: return Instantiate(Monster_3);
                case ShiBingName.Monster_4: return Instantiate(Monster_4);
                case ShiBingName.Monster_5: return Instantiate(Monster_5);
                case ShiBingName.Monster_6: return Instantiate(Monster_6);
                case ShiBingName.Monster_7: return Instantiate(Monster_7);

            }
        }



        return null;
    }

    public void DestroyObj(GameObject obj, Entity entity)
    {
        Destroy(obj);
        SynAniDic.Remove(entity);
    }
}
