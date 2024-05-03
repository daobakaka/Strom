using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;


public class SpawnAuthoring : MonoBehaviour
{
    //����1 2����=========================================================
    [Header("����1 2����----------------------------------------------")]
    public GameObject JiDi_Team1;
    public GameObject JiDi_Team2;
    public GameObject MonsterBirthPoint;

    //ʿ��Ԥ����==========================================================
    [Header("ʿ��Ԥ����-----------------------------------------------")]
    public GameObject Team1_BingYing;
    public GameObject Team2_BingYing;
    public GameObject Team1_BaoLei;//����
    public GameObject Team2_BaoLei;
    public GameObject Team1_ChangGong;//����
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

    [Header("����Ԥ����-----------------------------------------------")]
    public GameObject Monster_1;
    public GameObject Monster_2;
    public GameObject Monster_3;
    public GameObject Monster_4;
    public GameObject Monster_5;
    public GameObject Monster_6;
    public GameObject Monster_7;


    //�ӵ�Ԥ����==========================================================
    [Header("�ӵ�Ԥ����-----------------------------------------------")]
    public GameObject BaoLeiBullet;//�����ӵ�
    public GameObject ChangGongBullet;//�����ӵ�
    public GameObject JianYaBullet;
    public GameObject HuGuangBullet;
    public GameObject HuGuangBullet2;
    public GameObject PaChongBullet;
    public GameObject TieChuiBullet;
    public GameObject BingFengBullet;
    public GameObject ZhanZhengGongChangBullet;
    public GameObject BaoYuBullet_1_Air;
    public GameObject BaoYuBullet_2_Air;


    //��ЧԤ����==========================================================
    [Header("��ЧԤ����-----------------------------------------------")]
    public GameObject BaoLei_Muzzle_1;//ǹ����Ч
    public GameObject BaoLei_Muzzle_2;//ǹ����Ч
    public GameObject BaoLei_Foot;//������Ч
    [Header("==")]
    public GameObject ChangGong_Muzzle_1;//ǹ����Ч
    public GameObject ChangGong_Muzzle_2;//ǹ����Ч
    [Header("==")]
    public GameObject JianYa_Muzzle_1;//ǹ����Ч
    public GameObject JianYa_Muzzle_2;//ǹ����Ч
    [Header("==")]
    public GameObject HuGuang_Muzzle_1;
    public GameObject HuGuang_Muzzle_2;
    [Header("==")]
    public GameObject PaChong_Muzzle_1;
    public GameObject PaChong_Muzzle_2;
    [Header("==")]
    public GameObject TieChui_Muzzle_1;
    public GameObject TieChui_Muzzle_2;
    public GameObject TieChui_Foot;//����������Ч
    [Header("==")]
    public GameObject BingFeng_Muzzle_1;
    [Header("==")]
    public GameObject ZhanZhengGongChang_Muzzle_1;
    [Header("==")]
    public GameObject HaiKe_Muzzle_1;
    [Header("==")]
    [Tooltip("--�˺�ƽ̯��Ч--")]public GameObject DamageFlatteningEffect;

    //��������Ԥ����======================================================
    [Header("��������Ԥ����-------------------------------------------")]
    [Tooltip("--���湥������--")]public GameObject PaChong_AttackBox;
    [Tooltip("--���ṥ������--")]public GameObject LiuSuan_AttackBox;//���ṥ������
    [Tooltip("--�ػ𹥻�����--")]public GameObject DiHuo_AttackBox;
    [Tooltip("--���޹�������--")]public GameObject Monster_AttackBox;


    //Buff��Ч======================================================
    [Header("Buff��Ч-------------------------------------------")]
    [Tooltip("--����buff��Ч--")] public GameObject buffRampage;
    [Tooltip("--HPbuff��Ч--")] public GameObject buffHP;
    [Tooltip("--ATbuff��Ч--")] public GameObject buffAT;//������buff��Ч
    [Tooltip("--DPbuff��Ч--")] public GameObject buffDP;//������buff��Ч
    [Tooltip("--DBbuff��Ч--")] public GameObject buffDB;//��buff��Ч
    [Tooltip("--����buff��Ч--")] public GameObject debuffSpeed;
    [Tooltip("--������buff��Ч--")] public GameObject debuffShootTime;
    [Tooltip("--����buff��Ч--")] public GameObject debuffNotMove;
    [Tooltip("--�߷�buff��Ч--")] public GameObject debuffMutiny;


    [Header("���λ-------------------------------------------")]
    [Tooltip("--��˵���Ӱ--")] public GameObject FengHuangRebirth_1;
    [Tooltip("--��˵���Ӱ--")] public GameObject FengHuangRebirth_2;

    [Header("�ۻ�����ը��---------------------------------------")]
    [Tooltip("--����ը�������-")] public GameObject SceneBombFirePoint;
    [Tooltip("--����ը��_1 ԭ�ӵ�-")] public GameObject SceneBomb_NucleaBomb;
    [Tooltip("--����ը��_2 ����-")] public GameObject SceneBomb_Missile;
    [Tooltip("--����ը��_3 ����-")] public GameObject SceneBomb_Laser;

    [Header("ָ�ӹټ���---------------------------------------")]
    [Tooltip("--ԭ�ӵ�����--")] public GameObject comdSkill_NucleaBomb;
    [Tooltip("--������ը����--")] public GameObject comdSkill_Missile;
    [Tooltip("--���⼼��--")] public GameObject comdSkill_Laser;

    [Header("����---------------------------------------------")]
    public GameObject HaiKe_Shield_1;
    public GameObject HaiKe_Shield_2;
    public GameObject JiDi_Shield_1;
    public GameObject JiDi_Shield_2;

    [Header("���Entity��Ϊ�����DOTS�е�ΨһID---------------")]
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

            //ʿ��Entity
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


            //�ӵ�Entity
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

            //��ЧEntity
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

            //��������Entity
            PaChong_AttackBox = GetEntity(authoring.PaChong_AttackBox, TransformUsageFlags.Dynamic),
            LiuSuan_AttackBox = GetEntity(authoring.LiuSuan_AttackBox, TransformUsageFlags.Dynamic),
            DiHuo_AttackBox = GetEntity(authoring.DiHuo_AttackBox, TransformUsageFlags.Dynamic),
            Monster_AttackBox = GetEntity(authoring.Monster_AttackBox, TransformUsageFlags.Dynamic),

            //buff��Ч
            buffRampage = GetEntity(authoring.buffRampage, TransformUsageFlags.Dynamic),
            buffHP = GetEntity(authoring.buffHP, TransformUsageFlags.Dynamic),
            buffAT = GetEntity(authoring.buffAT, TransformUsageFlags.Dynamic),
            buffDP = GetEntity(authoring.buffDP, TransformUsageFlags.Dynamic),
            buffDB = GetEntity(authoring.buffDB, TransformUsageFlags.Dynamic),
            debuffMutiny = GetEntity(authoring.debuffMutiny, TransformUsageFlags.Dynamic),
            debuffNotMove = GetEntity(authoring.debuffNotMove, TransformUsageFlags.Dynamic),
            debuffShootTime = GetEntity(authoring.debuffShootTime, TransformUsageFlags.Dynamic),
            debuffSpeed = GetEntity(authoring.debuffSpeed, TransformUsageFlags.Dynamic),

            //���λ
            FengHuangRebirth_1 = GetEntity(authoring.FengHuangRebirth_1, TransformUsageFlags.Dynamic),
            FengHuangRebirth_2 = GetEntity(authoring.FengHuangRebirth_2, TransformUsageFlags.Dynamic),

            //����ը�������
            SceneBombFirePoint = GetEntity(authoring.SceneBombFirePoint, TransformUsageFlags.Dynamic),
            SceneBomb_NucleaBomb = GetEntity(authoring.SceneBomb_NucleaBomb, TransformUsageFlags.Dynamic),
            SceneBomb_Missile = GetEntity(authoring.SceneBomb_Missile, TransformUsageFlags.Dynamic),
            SceneBomb_Laser = GetEntity(authoring.SceneBomb_Laser, TransformUsageFlags.Dynamic),

            //ָ�ӹټ�����Ч
            comdSkill_NucleaBomb = GetEntity(authoring.comdSkill_NucleaBomb, TransformUsageFlags.Dynamic),
            comdSkill_Missile = GetEntity(authoring.comdSkill_Missile, TransformUsageFlags.Dynamic),
            comdSkill_Laser = GetEntity(authoring.comdSkill_Laser, TransformUsageFlags.Dynamic),

            //����
            HaiKe_Shield_1 = GetEntity(authoring.HaiKe_Shield_1, TransformUsageFlags.Dynamic),
            HaiKe_Shield_2 = GetEntity(authoring.HaiKe_Shield_2, TransformUsageFlags.Dynamic),
            JiDi_Shield_1 = GetEntity(authoring.JiDi_Shield_1, TransformUsageFlags.Dynamic),
            JiDi_Shield_2 = GetEntity(authoring.JiDi_Shield_2, TransformUsageFlags.Dynamic),

            //�����DOTS�е�ΨһID
            PlayerEntityID = GetEntity(authoring.PlayerEntityID, TransformUsageFlags.Dynamic),

        };
        AddComponent(entity, spawn);

    }
}