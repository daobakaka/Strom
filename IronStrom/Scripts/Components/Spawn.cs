using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Spawn : IComponentData
{
    public Entity JiDi_Team1;//队伍1 2基地
    public Entity JiDi_Team2;
    public Entity MonsterBirthPoint;//怪物出生点


    public Entity Team1_BingYing;//兵营
    public Entity Team2_BingYing;//兵营
    public Entity Team1_BaoLei;//士兵
    public Entity Team2_BaoLei;//士兵
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


    public Entity BaoLeiBullet;//子弹
    public Entity ChangGongBullet;//子弹
    public Entity JianYaBullet;//子弹
    public Entity HuGuangBullet;
    public Entity HuGuangBullet2;
    public Entity PaChongBullet;
    public Entity TieChuiBullet;
    public Entity BingFengBullet;
    public Entity ZhanZhengGongChangBullet;
    public Entity BaoYuBullet_1_Air;
    public Entity BaoYuBullet_2_Air;

    public Entity BaoLei_Muzzle_1;//枪口特效
    public Entity BaoLei_Muzzle_2;//枪口特效
    public Entity BaoLei_Foot;//脚下特效
    public Entity ChangGong_Muzzle_1;//枪口特效
    public Entity ChangGong_Muzzle_2;//枪口特效
    public Entity JianYa_Muzzle_1;
    public Entity JianYa_Muzzle_2;
    public Entity HuGuang_Muzzle_1;
    public Entity HuGuang_Muzzle_2;
    public Entity PaChong_Muzzle_1;
    public Entity PaChong_Muzzle_2;
    public Entity TieChui_Muzzle_1;
    public Entity TieChui_Muzzle_2;
    public Entity TieChui_Foot;//铁锤脚下特效
    public Entity BingFeng_Muzzle_1;
    public Entity ZhanZhengGongChang_Muzzle_1;
    public Entity HaiKe_Muzzle_1;
    public Entity DamageFlatteningEffect;

    public Entity PaChong_AttackBox;
    public Entity LiuSuan_AttackBox;
    public Entity DiHuo_AttackBox;//地火
    public Entity Monster_AttackBox;


    public Entity buffRampage;//暴走buff特效
    public Entity buffHP;//生命值buff特效
    public Entity buffAT;//攻击力buff特效
    public Entity buffDP;//防御力buff特效
    public Entity buffDB;//格挡buff特效
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

    public Entity HaiKe_Shield_1;//护盾
    public Entity HaiKe_Shield_2;//护盾
    public Entity JiDi_Shield_1;
    public Entity JiDi_Shield_2;

    public Entity PlayerEntityID;//玩家Entity,作为每一个玩家的唯一ID
    public void f1(EntityManager entiMager)
    {
    }
    public void f2(EntityCommandBuffer.ParallelWriter ecb, int ChunkIdex)
    {
        Debug.Log("调用了spawn的f2函数");
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
            if (Is_LevelUp)//如果为升级单位
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
            if (Is_LevelUp)//如果为升级单位
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
            if (Is_LevelUp)//如果为升级单位
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
            if (Is_LevelUp)//如果为升级单位
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
            if (Is_LevelUp)//如果为升级单位
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
            if (Is_LevelUp)//如果为升级单位
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
            case ShiBingName.PaChong: shibingname = "爬虫";break;
            case ShiBingName.JianYa: shibingname = "机甲战士"; break;
            case ShiBingName.ChangGong: shibingname = "长弓机甲"; break;
            case ShiBingName.HuGuang: shibingname = "弧光机甲"; break;
            case ShiBingName.YeMa: shibingname = "野马战车"; break;
            case ShiBingName.TieChui: shibingname = "铁锤战车"; break;
            case ShiBingName.GangQiu: shibingname = "激光战车"; break;
            case ShiBingName.BaoYu: shibingname = "暴雨战车"; break;
            case ShiBingName.FengHuang: shibingname = "凤凰战机"; break;
            case ShiBingName.XiNiu: shibingname = "狂蝎战车"; break;
            case ShiBingName.HaiKe: shibingname = "黑客"; break;
            case ShiBingName.HuoShen: shibingname = "火神机甲"; break;
            case ShiBingName.BaoLei: shibingname = "堡垒机甲"; break;
            case ShiBingName.RongDian: shibingname = "激光电磁炮"; break;
            case ShiBingName.BaZhu: shibingname = "霸主战舰"; break;
            case ShiBingName.BingFeng: shibingname = "蜂王战机"; break;
            case ShiBingName.ZhanZhengGongChang: shibingname = "星际工厂"; break;
        }
        return shibingname;
    }

    public Entity ShiBingNameEntity(in string sbName, in layer team, in Spawn spawn)
    {
        Entity ShiBingEnti = Entity.Null;
        if (sbName == "16爬虫")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_PaChong; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_PaChong; break;
            }
        }
        else if (sbName == "02机甲战士")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_JianYa; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_JianYa; break;
            }
        }
        else if (sbName == "07长弓机甲")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_ChangGong; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_ChangGong; break;
            }
        }
        else if (sbName == "11弧光机甲")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_HuGuang; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_HuGuang; break;
            }
        }
        else if (sbName == "08蜂王战机")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_BingFeng; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_BingFeng; break;
            }
        }
        else if (sbName == "01机枪战车")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_YeMa; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_YeMa; break;
            }
        }
        else if (sbName == "14重型坦克")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_TieChui; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_TieChui; break;
            }
        }
        else if (sbName == "03激光战车")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_GangQiu; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_GangQiu; break;
            }
        }
        else if (sbName == "04火箭炮")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_BaoYu; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_BaoYu; break;
            }
        }
        else if (sbName == "09凤凰战机")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_FengHuang; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_FengHuang; break;
            }
        }
        else if (sbName == "12机枪机甲")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_XiNiu; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_XiNiu; break;
            }
        }
        else if (sbName == "10黑客")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_HaiKe; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_HaiKe; break;
            }
        }
        else if (sbName == "17火神")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_HuoShen; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_HuoShen; break;
            }
        }
        else if (sbName == "13火炮重型机甲")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_BaoLei; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_BaoLei; break;
            }
        }
        else if (sbName == "15激光电磁炮")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_RongDian; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_RongDian; break;
            }
        }
        else if (sbName == "05霸主战舰")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_BaZhu; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_BaZhu; break;
            }
        }
        else if (sbName == "06战争工厂")
        {
            switch (team)
            {
                case layer.Team1: ShiBingEnti = spawn.Team1_ZhanZhengGongChang; break;
                case layer.Team2: ShiBingEnti = spawn.Team2_ZhanZhengGongChang; break;
            }
        }

        return ShiBingEnti;
    }

    //查看是否对地对空优势
    public float Is_Advantage(in float AT,in SX sx, in SX enemysx)
    {
        if (sx.Advantage == AirGroundAdvantage.Null) return AT;
        float AdvAT = AT;
        if (sx.Advantage == AirGroundAdvantage.To_Ground && !enemysx.Is_AirForce || 
            sx.Advantage == AirGroundAdvantage.To_Air && enemysx.Is_AirForce)
            AdvAT *= 1.3f;

        return AdvAT;
    }
    //子弹或者Box判断是否对空对地优势
    public float Is_Advantage(in float AT, Entity entity,in SX enemysx)
    {
        // 拿到这发子弹或这个box所属的玩家
        var entiMager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (!entiMager.HasComponent<OwnerData>(entity)) return AT;
        var owner = entiMager.GetComponentData<OwnerData>(entity).Owner;//打出这发子弹或box的玩家
        if (!entiMager.HasComponent<SX>(owner)) return AT;
        var sx = entiMager.GetComponentData<SX>(owner);

        return Is_Advantage(in AT, in sx, in enemysx);
    }


}
