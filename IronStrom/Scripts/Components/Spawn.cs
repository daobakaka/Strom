using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Spawn : IComponentData
{
    public Entity JiDi_Team1;//����1 2����
    public Entity JiDi_Team2;
    public Entity MonsterBirthPoint;//���������


    public Entity Team1_BingYing;//��Ӫ
    public Entity Team2_BingYing;//��Ӫ
    public Entity Team1_BaoLei;//ʿ��
    public Entity Team2_BaoLei;//ʿ��
    public Entity Team1_ChangGong;
    public Entity Team2_ChangGong;
    public Entity Team1_JianYa;
    public Entity Team2_JianYa;
    public Entity Team1_HuGuang;
    public Entity Team2_HuGuang;
    public Entity Team1_PaChong;
    public Entity Team2_PaChong;
    public Entity Team1_TieChui;
    public Entity Team2_TieChui;
    public Entity Team1_YeMa;
    public Entity Team2_YeMa;
    public Entity Team1_BaoYu;
    public Entity Team2_BaoYu;
    public Entity Team1_BingFeng;
    public Entity Team2_BingFeng;
    public Entity Team1_BaZhu;
    public Entity Team1_BaZhuLevelUp;
    public Entity Team2_BaZhu;
    public Entity Team2_BaZhuLevelUp;
    public Entity Team1_GangQiu;
    public Entity Team2_GangQiu;
    public Entity Team1_FengHuang;
    public Entity Team2_FengHuang;
    public Entity Team1_ZhanZhengGongChang;
    public Entity Team1_ZhanZhengGongChangLevelUp;
    public Entity Team2_ZhanZhengGongChang;
    public Entity Team2_ZhanZhengGongChangLevelUp;
    public Entity Team1_HuoShen;
    public Entity Team2_HuoShen;
    public Entity Team1_HaiKe;
    public Entity Team2_HaiKe;
    public Entity Team1_RongDian;
    public Entity Team2_RongDian;
    public Entity Team1_XiNiu;
    public Entity Team2_XiNiu;

    public Entity Monster_1;
    public Entity Monster_2;
    public Entity Monster_3;
    public Entity Monster_4;
    public Entity Monster_5;
    public Entity Monster_6;
    public Entity Monster_7;


    public Entity BaoLeiBullet;//�ӵ�
    public Entity ChangGongBullet;//�ӵ�
    public Entity JianYaBullet;//�ӵ�
    public Entity HuGuangBullet;
    public Entity HuGuangBullet2;
    public Entity PaChongBullet;
    public Entity TieChuiBullet;
    public Entity BingFengBullet;
    public Entity ZhanZhengGongChangBullet;
    public Entity BaoYuBullet_1_Air;
    public Entity BaoYuBullet_2_Air;

    public Entity BaoLei_Muzzle_1;//ǹ����Ч
    public Entity BaoLei_Muzzle_2;//ǹ����Ч
    public Entity BaoLei_Foot;//������Ч
    public Entity ChangGong_Muzzle_1;//ǹ����Ч
    public Entity ChangGong_Muzzle_2;//ǹ����Ч
    public Entity JianYa_Muzzle_1;
    public Entity JianYa_Muzzle_2;
    public Entity HuGuang_Muzzle_1;
    public Entity HuGuang_Muzzle_2;
    public Entity PaChong_Muzzle_1;
    public Entity PaChong_Muzzle_2;
    public Entity TieChui_Muzzle_1;
    public Entity TieChui_Muzzle_2;
    public Entity TieChui_Foot;//����������Ч
    public Entity BingFeng_Muzzle_1;
    public Entity ZhanZhengGongChang_Muzzle_1;
    public Entity HaiKe_Muzzle_1;
    public Entity DamageFlatteningEffect;

    public Entity PaChong_AttackBox;
    public Entity LiuSuan_AttackBox;
    public Entity DiHuo_AttackBox;//�ػ�
    public Entity Monster_AttackBox;


    public Entity buffRampage;//����buff��Ч
    public Entity buffHP;//����ֵbuff��Ч
    public Entity buffAT;//������buff��Ч
    public Entity buffDP;//������buff��Ч
    public Entity buffDB;//��buff��Ч
    public Entity debuffSpeed;
    public Entity debuffShootTime;
    public Entity debuffNotMove;
    public Entity debuffMutiny;

    public Entity FengHuangRebirth_1;
    public Entity FengHuangRebirth_2;

    public Entity SceneBombFirePoint;
    public Entity SceneBomb_NucleaBomb;
    public Entity SceneBomb_Missile;
    public Entity SceneBomb_Laser;

    public Entity comdSkill_NucleaBomb;
    public Entity comdSkill_Missile;
    public Entity comdSkill_Laser;

    public Entity HaiKe_Shield_1;//����
    public Entity HaiKe_Shield_2;//����
    public Entity JiDi_Shield_1;
    public Entity JiDi_Shield_2;

    public Entity PlayerEntityID;//���Entity,��Ϊÿһ����ҵ�ΨһID
    public void f1(EntityManager entiMager)
    {
    }
    public void f2(EntityCommandBuffer.ParallelWriter ecb, int ChunkIdex)
    {
        Debug.Log("������spawn��f2����");
    }

    public Entity ShiBingNameEntity(in ShiBingName sbName, in layer team, in Spawn spawn, bool Is_LevelUp)
    {
        Entity ShiBingEnti = Entity.Null;
        if (sbName == ShiBingName.PaChong)
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_PaChong; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_PaChong; break;
            }
        }
        else if (sbName == ShiBingName.JianYa)
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_JianYa; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_JianYa; break;
            }
        }
        else if (sbName == ShiBingName.ChangGong)
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_ChangGong; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_ChangGong; break;
            }
        }
        else if (sbName == ShiBingName.HuGuang)
        {
            if (Is_LevelUp)//���Ϊ������λ
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_BaoLei; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_BaoLei; break;
                }
            }
            else
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_HuGuang; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_HuGuang; break;
                }
            }
        }
        else if (sbName == ShiBingName.BingFeng)
        {
            if (Is_LevelUp)//���Ϊ������λ
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_FengHuang; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_FengHuang; break;
                }
            }
            else
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_BingFeng; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_BingFeng; break;
                }
            }
        }
        else if (sbName == ShiBingName.YeMa)
        {
            if (Is_LevelUp)//���Ϊ������λ
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_TieChui; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_TieChui; break;
                }
            }
            else
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_YeMa; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_YeMa; break;
                }
            }
        }
        else if (sbName == ShiBingName.TieChui)
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_TieChui; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_TieChui; break;
            }
        }
        else if (sbName == ShiBingName.GangQiu)
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_GangQiu; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_GangQiu; break;
            }
        }
        else if (sbName == ShiBingName.BaoYu)
        {
            if (Is_LevelUp)//���Ϊ������λ
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_HuoShen; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_HuoShen; break;
                }
            }
            else
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_BaoYu; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_BaoYu; break;
                }
            }

        }
        else if (sbName == ShiBingName.FengHuang)
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_FengHuang; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_FengHuang; break;
            }
        }
        else if (sbName == ShiBingName.XiNiu)
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_XiNiu; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_XiNiu; break;
            }
        }
        else if (sbName == ShiBingName.HaiKe)
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_HaiKe; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_HaiKe; break;
            }
        }
        else if (sbName == ShiBingName.HuoShen)
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_HuoShen; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_HuoShen; break;
            }
        }
        else if (sbName == ShiBingName.BaoLei)
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_BaoLei; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_BaoLei; break;
            }
        }
        else if (sbName == ShiBingName.RongDian)
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_RongDian; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_RongDian; break;
            }
        }
        else if (sbName == ShiBingName.BaZhu)
        {
            if (Is_LevelUp)//���Ϊ������λ
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_BaZhuLevelUp; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_BaZhuLevelUp; break;
                }
            }
            else
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_BaZhu; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_BaZhu; break;
                }
            }
        }
        else if (sbName == ShiBingName.ZhanZhengGongChang)
        {
            if (Is_LevelUp)//���Ϊ������λ
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_ZhanZhengGongChangLevelUp; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_ZhanZhengGongChangLevelUp; break;
                }
            }
            else
            {
                switch (team)
                {
                    case layer.Team1: ShiBingEnti = spawn.Team1_ZhanZhengGongChang; break;
                    case layer.Team2: ShiBingEnti = spawn.Team2_ZhanZhengGongChang; break;
                }
            }
        }

        return ShiBingEnti;
    }
    public string GetShiBingName(in ShiBingName sbName)
    {
        string shibingname = "";

        switch (sbName)
        {
            case ShiBingName.PaChong: shibingname = "����";break;
            case ShiBingName.JianYa: shibingname = "����սʿ"; break;
            case ShiBingName.ChangGong: shibingname = "��������"; break;
            case ShiBingName.HuGuang: shibingname = "�������"; break;
            case ShiBingName.YeMa: shibingname = "Ұ��ս��"; break;
            case ShiBingName.TieChui: shibingname = "����ս��"; break;
            case ShiBingName.GangQiu: shibingname = "����ս��"; break;
            case ShiBingName.BaoYu: shibingname = "����ս��"; break;
            case ShiBingName.FengHuang: shibingname = "���ս��"; break;
            case ShiBingName.XiNiu: shibingname = "��Ыս��"; break;
            case ShiBingName.HaiKe: shibingname = "�ڿ�"; break;
            case ShiBingName.HuoShen: shibingname = "�������"; break;
            case ShiBingName.BaoLei: shibingname = "���ݻ���"; break;
            case ShiBingName.RongDian: shibingname = "��������"; break;
            case ShiBingName.BaZhu: shibingname = "����ս��"; break;
            case ShiBingName.BingFeng: shibingname = "����ս��"; break;
            case ShiBingName.ZhanZhengGongChang: shibingname = "�Ǽʹ���"; break;
        }
        return shibingname;
    }

    public Entity ShiBingNameEntity(in string sbName, in layer team, in Spawn spawn)
    {
        Entity ShiBingEnti = Entity.Null;
        if (sbName == "16����")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_PaChong; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_PaChong; break;
            }
        }
        else if (sbName == "02����սʿ")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_JianYa; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_JianYa; break;
            }
        }
        else if (sbName == "07��������")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_ChangGong; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_ChangGong; break;
            }
        }
        else if (sbName == "11�������")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_HuGuang; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_HuGuang; break;
            }
        }
        else if (sbName == "08����ս��")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_BingFeng; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_BingFeng; break;
            }
        }
        else if (sbName == "01��ǹս��")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_YeMa; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_YeMa; break;
            }
        }
        else if (sbName == "14����̹��")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_TieChui; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_TieChui; break;
            }
        }
        else if (sbName == "03����ս��")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_GangQiu; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_GangQiu; break;
            }
        }
        else if (sbName == "04�����")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_BaoYu; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_BaoYu; break;
            }
        }
        else if (sbName == "09���ս��")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_FengHuang; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_FengHuang; break;
            }
        }
        else if (sbName == "12��ǹ����")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_XiNiu; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_XiNiu; break;
            }
        }
        else if (sbName == "10�ڿ�")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_HaiKe; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_HaiKe; break;
            }
        }
        else if (sbName == "17����")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_HuoShen; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_HuoShen; break;
            }
        }
        else if (sbName == "13�������ͻ���")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_BaoLei; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_BaoLei; break;
            }
        }
        else if (sbName == "15��������")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_RongDian; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_RongDian; break;
            }
        }
        else if (sbName == "05����ս��")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_BaZhu; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_BaZhu; break;
            }
        }
        else if (sbName == "06ս������")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_ZhanZhengGongChang; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_ZhanZhengGongChang; break;
            }
        }

        return ShiBingEnti;
    }

    //�鿴�Ƿ�ԵضԿ�����
    public float Is_Advantage(in float AT,in SX sx, in SX enemysx)
    {
        if (sx.Advantage == AirGroundAdvantage.Null) return AT;
        float AdvAT = AT;
        if (sx.Advantage == AirGroundAdvantage.To_Ground && !enemysx.Is_AirForce || 
            sx.Advantage == AirGroundAdvantage.To_Air && enemysx.Is_AirForce)
            AdvAT *= 1.3f;

        return AdvAT;
    }
    //�ӵ�����Box�ж��Ƿ�ԿնԵ�����
    public float Is_Advantage(in float AT, Entity entity,in SX enemysx)
    {
        // �õ��ⷢ�ӵ������box���������
        var entiMager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (!entiMager.HasComponent<OwnerData>(entity)) return AT;
        var owner = entiMager.GetComponentData<OwnerData>(entity).Owner;//����ⷢ�ӵ���box�����
        if (!entiMager.HasComponent<SX>(owner)) return AT;
        var sx = entiMager.GetComponentData<SX>(owner);

        return Is_Advantage(in AT, in sx, in enemysx);
    }


}
