using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;


public class SpawnAuthoring : MonoBehaviour
{
    //队伍1 2基地=========================================================
    [Header("队伍1 2基地----------------------------------------------")]
    public GameObject JiDi_Team1;
    public GameObject JiDi_Team2;
    public GameObject MonsterBirthPoint;

    //士兵预制体==========================================================
    [Header("士兵预制体-----------------------------------------------")]
    public GameObject Team1_BingYing;
    public GameObject Team2_BingYing;
    public GameObject Team1_BaoLei;//堡垒
    public GameObject Team2_BaoLei;
    public GameObject Team1_ChangGong;//长弓
    public GameObject Team2_ChangGong;
    public GameObject Team1_JianYa;
    public GameObject Team2_JianYa;
    public GameObject Team1_HuGuang;
    public GameObject Team2_HuGuang;
    public GameObject Team1_PaChong;
    public GameObject Team2_PaChong;
    public GameObject Team1_TieChui;
    public GameObject Team2_TieChui;
    public GameObject Team1_YeMa;
    public GameObject Team2_YeMa;
    public GameObject Team1_BaoYu;
    public GameObject Team2_BaoYu;
    public GameObject Team1_BingFeng;
    public GameObject Team2_BingFeng;
    public GameObject Team1_BaZhu;
    public GameObject Team1_BaZhuLevelUp;
    public GameObject Team2_BaZhu;
    public GameObject Team2_BaZhuLevelUp;
    public GameObject Team1_GangQiu;
    public GameObject Team2_GangQiu;
    public GameObject Team1_FengHuang;
    public GameObject Team2_FengHuang;
    public GameObject Team1_ZhanZhengGongChang;
    public GameObject Team1_ZhanZhengGongChangLevelUp;
    public GameObject Team2_ZhanZhengGongChang;
    public GameObject Team2_ZhanZhengGongChangLevelUp;
    public GameObject Team1_HuoShen;
    public GameObject Team2_HuoShen;
    public GameObject Team1_HaiKe;
    public GameObject Team2_HaiKe;
    public GameObject Team1_RongDian;
    public GameObject Team2_RongDian;
    public GameObject Team1_XiNiu;
    public GameObject Team2_XiNiu;

    [Header("怪物预制体-----------------------------------------------")]
    public GameObject Monster_1;
    public GameObject Monster_2;
    public GameObject Monster_3;
    public GameObject Monster_4;
    public GameObject Monster_5;
    public GameObject Monster_6;
    public GameObject Monster_7;


    //子弹预制体==========================================================
    [Header("子弹预制体-----------------------------------------------")]
    public GameObject BaoLeiBullet;//堡垒子弹
    public GameObject ChangGongBullet;//长弓子弹
    public GameObject JianYaBullet;
    public GameObject HuGuangBullet;
    public GameObject HuGuangBullet2;
    public GameObject PaChongBullet;
    public GameObject TieChuiBullet;
    public GameObject BingFengBullet;
    public GameObject ZhanZhengGongChangBullet;
    public GameObject BaoYuBullet_1_Air;
    public GameObject BaoYuBullet_2_Air;


    //特效预制体==========================================================
    [Header("特效预制体-----------------------------------------------")]
    public GameObject BaoLei_Muzzle_1;//枪口特效
    public GameObject BaoLei_Muzzle_2;//枪口特效
    public GameObject BaoLei_Foot;//脚下特效
    [Header("==")]
    public GameObject ChangGong_Muzzle_1;//枪口特效
    public GameObject ChangGong_Muzzle_2;//枪口特效
    [Header("==")]
    public GameObject JianYa_Muzzle_1;//枪口特效
    public GameObject JianYa_Muzzle_2;//枪口特效
    [Header("==")]
    public GameObject HuGuang_Muzzle_1;
    public GameObject HuGuang_Muzzle_2;
    [Header("==")]
    public GameObject PaChong_Muzzle_1;
    public GameObject PaChong_Muzzle_2;
    [Header("==")]
    public GameObject TieChui_Muzzle_1;
    public GameObject TieChui_Muzzle_2;
    public GameObject TieChui_Foot;//铁锤脚下特效
    [Header("==")]
    public GameObject BingFeng_Muzzle_1;
    [Header("==")]
    public GameObject ZhanZhengGongChang_Muzzle_1;
    [Header("==")]
    public GameObject HaiKe_Muzzle_1;
    [Header("==")]
    [Tooltip("--伤害平摊特效--")]public GameObject DamageFlatteningEffect;

    //攻击矩形预制体======================================================
    [Header("攻击矩形预制体-------------------------------------------")]
    [Tooltip("--爬虫攻击矩形--")]public GameObject PaChong_AttackBox;
    [Tooltip("--硫酸攻击矩形--")]public GameObject LiuSuan_AttackBox;//硫酸攻击矩形
    [Tooltip("--地火攻击矩形--")]public GameObject DiHuo_AttackBox;
    [Tooltip("--怪兽攻击矩形--")]public GameObject Monster_AttackBox;


    //Buff特效======================================================
    [Header("Buff特效-------------------------------------------")]
    [Tooltip("--暴走buff特效--")] public GameObject buffRampage;
    [Tooltip("--HPbuff特效--")] public GameObject buffHP;
    [Tooltip("--ATbuff特效--")] public GameObject buffAT;//攻击力buff特效
    [Tooltip("--DPbuff特效--")] public GameObject buffDP;//防御力buff特效
    [Tooltip("--DBbuff特效--")] public GameObject buffDB;//格挡buff特效
    [Tooltip("--减速buff特效--")] public GameObject debuffSpeed;
    [Tooltip("--减攻速buff特效--")] public GameObject debuffShootTime;
    [Tooltip("--禁锢buff特效--")] public GameObject debuffNotMove;
    [Tooltip("--策反buff特效--")] public GameObject debuffMutiny;


    [Header("复活单位-------------------------------------------")]
    [Tooltip("--凤凰的虚影--")] public GameObject FengHuangRebirth_1;
    [Tooltip("--凤凰的虚影--")] public GameObject FengHuangRebirth_2;

    [Header("累积音浪炸弹---------------------------------------")]
    [Tooltip("--场景炸弹发射点-")] public GameObject SceneBombFirePoint;
    [Tooltip("--场景炸弹_1 原子弹-")] public GameObject SceneBomb_NucleaBomb;
    [Tooltip("--场景炸弹_2 导弹-")] public GameObject SceneBomb_Missile;
    [Tooltip("--场景炸弹_3 激光-")] public GameObject SceneBomb_Laser;

    [Header("指挥官技能---------------------------------------")]
    [Tooltip("--原子弹技能--")] public GameObject comdSkill_NucleaBomb;
    [Tooltip("--导弹轰炸技能--")] public GameObject comdSkill_Missile;
    [Tooltip("--激光技能--")] public GameObject comdSkill_Laser;

    [Header("护盾---------------------------------------------")]
    public GameObject HaiKe_Shield_1;
    public GameObject HaiKe_Shield_2;
    public GameObject JiDi_Shield_1;
    public GameObject JiDi_Shield_2;

    [Header("玩家Entity作为玩家在DOTS中的唯一ID---------------")]
    public GameObject PlayerEntityID;



}

public class SpawnBaker : Baker<SpawnAuthoring>
{
    public override void Bake(SpawnAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var spawn = new Spawn
        {
            JiDi_Team1 = GetEntity(authoring.JiDi_Team1, TransformUsageFlags.Dynamic),
            JiDi_Team2 = GetEntity(authoring.JiDi_Team2, TransformUsageFlags.Dynamic),
            MonsterBirthPoint = GetEntity(authoring.MonsterBirthPoint, TransformUsageFlags.Dynamic),

            //士兵Entity
            Team1_BingYing = GetEntity(authoring.Team1_BingYing, TransformUsageFlags.Dynamic),
            Team2_BingYing = GetEntity(authoring.Team2_BingYing, TransformUsageFlags.Dynamic),
            Team1_BaoLei = GetEntity(authoring.Team1_BaoLei, TransformUsageFlags.Dynamic),
            Team2_BaoLei = GetEntity(authoring.Team2_BaoLei, TransformUsageFlags.Dynamic),
            Team1_ChangGong = GetEntity(authoring.Team1_ChangGong, TransformUsageFlags.Dynamic),
            Team2_ChangGong = GetEntity(authoring.Team2_ChangGong, TransformUsageFlags.Dynamic),
            Team1_JianYa = GetEntity(authoring.Team1_JianYa, TransformUsageFlags.Dynamic),
            Team2_JianYa = GetEntity(authoring.Team2_JianYa, TransformUsageFlags.Dynamic),
            Team1_HuGuang = GetEntity(authoring.Team1_HuGuang, TransformUsageFlags.Dynamic),
            Team2_HuGuang = GetEntity(authoring.Team2_HuGuang, TransformUsageFlags.Dynamic),
            Team1_PaChong = GetEntity(authoring.Team1_PaChong, TransformUsageFlags.Dynamic),
            Team2_PaChong = GetEntity(authoring.Team2_PaChong, TransformUsageFlags.Dynamic),
            Team1_TieChui = GetEntity(authoring.Team1_TieChui, TransformUsageFlags.Dynamic),
            Team2_TieChui = GetEntity(authoring.Team2_TieChui, TransformUsageFlags.Dynamic),
            Team1_YeMa = GetEntity(authoring.Team1_YeMa, TransformUsageFlags.Dynamic),
            Team2_YeMa = GetEntity(authoring.Team2_YeMa, TransformUsageFlags.Dynamic),
            Team1_BaoYu = GetEntity(authoring.Team1_BaoYu, TransformUsageFlags.Dynamic),
            Team2_BaoYu = GetEntity(authoring.Team2_BaoYu, TransformUsageFlags.Dynamic),
            Team1_BingFeng = GetEntity(authoring.Team1_BingFeng, TransformUsageFlags.Dynamic),
            Team2_BingFeng = GetEntity(authoring.Team2_BingFeng, TransformUsageFlags.Dynamic),
            Team1_BaZhu = GetEntity(authoring.Team1_BaZhu, TransformUsageFlags.Dynamic),
            Team1_BaZhuLevelUp = GetEntity(authoring.Team1_BaZhuLevelUp, TransformUsageFlags.Dynamic),
            Team2_BaZhu = GetEntity(authoring.Team2_BaZhu, TransformUsageFlags.Dynamic),
            Team2_BaZhuLevelUp = GetEntity(authoring.Team2_BaZhuLevelUp, TransformUsageFlags.Dynamic),
            Team1_GangQiu = GetEntity(authoring.Team1_GangQiu, TransformUsageFlags.Dynamic),
            Team2_GangQiu = GetEntity(authoring.Team2_GangQiu, TransformUsageFlags.Dynamic),
            Team1_FengHuang = GetEntity(authoring.Team1_FengHuang, TransformUsageFlags.Dynamic),
            Team2_FengHuang = GetEntity(authoring.Team2_FengHuang, TransformUsageFlags.Dynamic),
            Team1_ZhanZhengGongChang = GetEntity(authoring.Team1_ZhanZhengGongChang, TransformUsageFlags.Dynamic),
            Team1_ZhanZhengGongChangLevelUp = GetEntity(authoring.Team1_ZhanZhengGongChangLevelUp, TransformUsageFlags.Dynamic),
            Team2_ZhanZhengGongChang = GetEntity(authoring.Team2_ZhanZhengGongChang, TransformUsageFlags.Dynamic),
            Team2_ZhanZhengGongChangLevelUp = GetEntity(authoring.Team2_ZhanZhengGongChangLevelUp, TransformUsageFlags.Dynamic),
            Team1_HuoShen = GetEntity(authoring.Team1_HuoShen, TransformUsageFlags.Dynamic),
            Team2_HuoShen = GetEntity(authoring.Team2_HuoShen, TransformUsageFlags.Dynamic),
            Team1_HaiKe = GetEntity(authoring.Team1_HaiKe, TransformUsageFlags.Dynamic),
            Team2_HaiKe = GetEntity(authoring.Team2_HaiKe, TransformUsageFlags.Dynamic),
            Team1_RongDian = GetEntity(authoring.Team1_RongDian, TransformUsageFlags.Dynamic),
            Team2_RongDian = GetEntity(authoring.Team2_RongDian, TransformUsageFlags.Dynamic),
            Team1_XiNiu = GetEntity(authoring.Team1_XiNiu, TransformUsageFlags.Dynamic),
            Team2_XiNiu = GetEntity(authoring.Team2_XiNiu, TransformUsageFlags.Dynamic),

            Monster_1 = GetEntity(authoring.Monster_1, TransformUsageFlags.Dynamic),
            Monster_2 = GetEntity(authoring.Monster_2, TransformUsageFlags.Dynamic),
            Monster_3 = GetEntity(authoring.Monster_3, TransformUsageFlags.Dynamic),
            Monster_4 = GetEntity(authoring.Monster_4, TransformUsageFlags.Dynamic),
            Monster_5 = GetEntity(authoring.Monster_5, TransformUsageFlags.Dynamic),
            Monster_6 = GetEntity(authoring.Monster_6, TransformUsageFlags.Dynamic),
            Monster_7 = GetEntity(authoring.Monster_7, TransformUsageFlags.Dynamic),


            //子弹Entity
            BaoLeiBullet = GetEntity(authoring.BaoLeiBullet, TransformUsageFlags.Dynamic),
            ChangGongBullet = GetEntity(authoring.ChangGongBullet, TransformUsageFlags.Dynamic),
            JianYaBullet = GetEntity(authoring.JianYaBullet, TransformUsageFlags.Dynamic),
            HuGuangBullet = GetEntity(authoring.HuGuangBullet, TransformUsageFlags.Dynamic),
            HuGuangBullet2 = GetEntity(authoring.HuGuangBullet2, TransformUsageFlags.Dynamic),
            PaChongBullet = GetEntity(authoring.PaChongBullet, TransformUsageFlags.Dynamic),
            TieChuiBullet = GetEntity(authoring.TieChuiBullet, TransformUsageFlags.Dynamic),
            BingFengBullet = GetEntity(authoring.BingFengBullet, TransformUsageFlags.Dynamic),
            ZhanZhengGongChangBullet = GetEntity(authoring.ZhanZhengGongChangBullet, TransformUsageFlags.Dynamic),
            BaoYuBullet_1_Air = GetEntity(authoring.BaoYuBullet_1_Air, TransformUsageFlags.Dynamic),
            BaoYuBullet_2_Air = GetEntity(authoring.BaoYuBullet_2_Air, TransformUsageFlags.Dynamic),

            //特效Entity
            BaoLei_Muzzle_1 = GetEntity(authoring.BaoLei_Muzzle_1, TransformUsageFlags.Dynamic),
            BaoLei_Muzzle_2 = GetEntity(authoring.BaoLei_Muzzle_2, TransformUsageFlags.Dynamic),
            BaoLei_Foot = GetEntity(authoring.BaoLei_Foot, TransformUsageFlags.Dynamic),
            ChangGong_Muzzle_1 = GetEntity(authoring.ChangGong_Muzzle_1, TransformUsageFlags.Dynamic),
            ChangGong_Muzzle_2 = GetEntity(authoring.ChangGong_Muzzle_2, TransformUsageFlags.Dynamic),
            JianYa_Muzzle_1 = GetEntity(authoring.JianYa_Muzzle_1, TransformUsageFlags.Dynamic),
            JianYa_Muzzle_2 = GetEntity(authoring.JianYa_Muzzle_2, TransformUsageFlags.Dynamic),
            HuGuang_Muzzle_1 = GetEntity(authoring.HuGuang_Muzzle_1, TransformUsageFlags.Dynamic),
            HuGuang_Muzzle_2 = GetEntity(authoring.HuGuang_Muzzle_2, TransformUsageFlags.Dynamic),
            PaChong_Muzzle_1 = GetEntity(authoring.PaChong_Muzzle_1, TransformUsageFlags.Dynamic),
            PaChong_Muzzle_2 = GetEntity(authoring.PaChong_Muzzle_2, TransformUsageFlags.Dynamic),
            TieChui_Muzzle_1 = GetEntity(authoring.TieChui_Muzzle_1, TransformUsageFlags.Dynamic),
            TieChui_Muzzle_2 = GetEntity(authoring.TieChui_Muzzle_2, TransformUsageFlags.Dynamic),
            TieChui_Foot = GetEntity(authoring.TieChui_Foot, TransformUsageFlags.Dynamic),
            BingFeng_Muzzle_1 = GetEntity(authoring.BingFeng_Muzzle_1, TransformUsageFlags.Dynamic),
            ZhanZhengGongChang_Muzzle_1 = GetEntity(authoring.ZhanZhengGongChang_Muzzle_1, TransformUsageFlags.Dynamic),
            HaiKe_Muzzle_1 = GetEntity(authoring.HaiKe_Muzzle_1, TransformUsageFlags.Dynamic),
            DamageFlatteningEffect = GetEntity(authoring.DamageFlatteningEffect, TransformUsageFlags.Dynamic),

            //攻击矩形Entity
            PaChong_AttackBox = GetEntity(authoring.PaChong_AttackBox, TransformUsageFlags.Dynamic),
            LiuSuan_AttackBox = GetEntity(authoring.LiuSuan_AttackBox, TransformUsageFlags.Dynamic),
            DiHuo_AttackBox = GetEntity(authoring.DiHuo_AttackBox, TransformUsageFlags.Dynamic),
            Monster_AttackBox = GetEntity(authoring.Monster_AttackBox, TransformUsageFlags.Dynamic),

            //buff特效
            buffRampage = GetEntity(authoring.buffRampage, TransformUsageFlags.Dynamic),
            buffHP = GetEntity(authoring.buffHP, TransformUsageFlags.Dynamic),
            buffAT = GetEntity(authoring.buffAT, TransformUsageFlags.Dynamic),
            buffDP = GetEntity(authoring.buffDP, TransformUsageFlags.Dynamic),
            buffDB = GetEntity(authoring.buffDB, TransformUsageFlags.Dynamic),
            debuffMutiny = GetEntity(authoring.debuffMutiny, TransformUsageFlags.Dynamic),
            debuffNotMove = GetEntity(authoring.debuffNotMove, TransformUsageFlags.Dynamic),
            debuffShootTime = GetEntity(authoring.debuffShootTime, TransformUsageFlags.Dynamic),
            debuffSpeed = GetEntity(authoring.debuffSpeed, TransformUsageFlags.Dynamic),

            //复活单位
            FengHuangRebirth_1 = GetEntity(authoring.FengHuangRebirth_1, TransformUsageFlags.Dynamic),
            FengHuangRebirth_2 = GetEntity(authoring.FengHuangRebirth_2, TransformUsageFlags.Dynamic),

            //场景炸弹发射点
            SceneBombFirePoint = GetEntity(authoring.SceneBombFirePoint, TransformUsageFlags.Dynamic),
            SceneBomb_NucleaBomb = GetEntity(authoring.SceneBomb_NucleaBomb, TransformUsageFlags.Dynamic),
            SceneBomb_Missile = GetEntity(authoring.SceneBomb_Missile, TransformUsageFlags.Dynamic),
            SceneBomb_Laser = GetEntity(authoring.SceneBomb_Laser, TransformUsageFlags.Dynamic),

            //指挥官技能特效
            comdSkill_NucleaBomb = GetEntity(authoring.comdSkill_NucleaBomb, TransformUsageFlags.Dynamic),
            comdSkill_Missile = GetEntity(authoring.comdSkill_Missile, TransformUsageFlags.Dynamic),
            comdSkill_Laser = GetEntity(authoring.comdSkill_Laser, TransformUsageFlags.Dynamic),

            //护盾
            HaiKe_Shield_1 = GetEntity(authoring.HaiKe_Shield_1, TransformUsageFlags.Dynamic),
            HaiKe_Shield_2 = GetEntity(authoring.HaiKe_Shield_2, TransformUsageFlags.Dynamic),
            JiDi_Shield_1 = GetEntity(authoring.JiDi_Shield_1, TransformUsageFlags.Dynamic),
            JiDi_Shield_2 = GetEntity(authoring.JiDi_Shield_2, TransformUsageFlags.Dynamic),

            //玩家在DOTS中的唯一ID
            PlayerEntityID = GetEntity(authoring.PlayerEntityID, TransformUsageFlags.Dynamic),

        };
        AddComponent(entity, spawn);

    }
}