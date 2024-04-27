using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillAttackType
{
    Single,
    AOE,
    Self,
}
public enum SelectorType
{
    None,
    /// <summary>
    /// 通过实体碰撞接触选择
    /// </summary>
    Contact,
    /// <summary>
    /// 通过射线选择（RayCast方式）
    /// </summary>
    RayCast,
    /// <summary>
    /// 通过扇形范围选择
    /// </summary>
    Sector,
}
public enum DisappearType
{
    TimeOver,
    CheckOver,
}
public enum ImpactType
{
    /// <summary>
    /// 造成体力流失（流失类是否可以统一）
    /// </summary>
    DamageHp,
    /// <summary>
    /// 造成魔力流失
    /// </summary>
    DamageMp,
    /// <summary>
    /// 造成耐力流失
    /// </summary>
    DamageDp,

    /// <summary>
    /// 消耗体力（消耗类是否可以统一）
    /// </summary>
    CostHp,
    /// <summary>
    /// 消耗魔力
    /// </summary>
    CostMp,
    /// <summary>
    /// 消耗耐力
    /// </summary>
    CostDp,

    /// <summary>
    /// 弹飞
    /// </summary>
    BlowFly,

    /// <summary>
    /// 能力值改变
    /// </summary>
    Buff,
}

/// <summary>
/// 与场景角色设置无关的技能配置
/// </summary>
[Serializable]
public struct SkillBasicConfig
{
    /// <summary>
    /// 技能ID
    /// </summary>
    [SerializeField]
    public int skillId; // = 0;

    /// <summary>
    /// 技能名称
    /// </summary>
    [SerializeField]
    public string name; // = "Melee"; 

    /// <summary>
    /// 技能描述
    /// </summary>
    [SerializeField]
    public string description; // = "平A"; 

    /// <summary>
    /// 技能冷却时间，0则不需要冷却
    /// </summary>
    [SerializeField]
    public float skillCd; // = 0f;

    /// <summary>
    /// 魔力消耗，0则不需要消耗
    /// </summary>
    [SerializeField]
    public float costMp; // = 0f;

    /// <summary>
    /// 体力消耗，0则不需要消耗
    /// </summary>
    [SerializeField]
    public float costHp; // = 0f;

    /// <summary>
    /// 耐力消耗，0则不需要消耗
    /// </summary>
    [SerializeField]
    public float costDp; // = 0f;

    /// <summary>
    /// 技能等级
    /// </summary>
    [SerializeField]
    public int level;

    /// <summary>
    /// 技能影响类型（一般为{ "Damage" }）
    /// </summary>
    [SerializeField]
    public ImpactType[] impactType; // = { ImpactType.CostMp, ImpactType.Damage };

    /// <summary>
    /// 体力伤害数值
    /// </summary>
    [SerializeField]
    public float damageHp; // = 1;

    /// <summary>
    /// 魔力伤害数值
    /// </summary>
    [SerializeField]
    public float damageMp; // = 0;

    /// <summary>
    /// 耐力伤害数值
    /// </summary>
    [SerializeField]
    public float damageDp; // = 1;

    /// <summary>
    /// 前摇时间
    /// </summary>
    [SerializeField]
    public float warmUpTime; // = 0;

    /// <summary>
    /// 持续时间（前摇结束开始计算）
    /// </summary>
    [SerializeField]
    public float durationTime; // = 0;

    /// <summary>
    /// 伤害间隔
    /// </summary>
    [SerializeField]
    public float attackInterval; // = 0;

    /// <summary>
    /// 连击的技能ID（一般为-1）
    /// </summary>
    [SerializeField]
    public int nextComboId;

    /// <summary>
    /// 技能距离，0则不需要定义距离
    /// </summary>
    [Header("Range")]
    [SerializeField]
    public float attackDistance; // = 0;
    /// <summary>
    /// 技能攻击角度，0则不需要定义角度
    /// </summary>
    [SerializeField]
    public float attackAngle; // = 0;
    /// <summary>
    /// 能作用的目标Tag，空则不适用
    /// </summary>
    [SerializeField]
    public string[] attackTargetTags; // = { "Enemy" };
    /// <summary>
    /// 能作用的目标Layer（一般是物理接口需要），空则全部
    /// </summary>
    [SerializeField]
    public string[] attackTargetLayers; // = { "Hittable" };
    /// <summary>
    /// AOE或者单体
    /// </summary>
    [SerializeField]
    public SkillAttackType attackType;
    /// <summary>
    /// 释放范围类型（圆形，扇形，矩形）
    /// </summary>
    [SerializeField]
    public SelectorType selectorType;

    /// <summary>
    /// 动画进入时间
    /// </summary>
    [SerializeField]
    public float animationStartTime; // = 0;

    /// <summary>
    /// 动画持续时间
    /// </summary>
    [SerializeField]
    public float animationDurationTime; // = 0;

    /// <summary>
    /// 技能预制体名称（空则不生成预制体）
    /// </summary>
    [Header("Prefab")]
    [SerializeField]
    public string skillPrefabName;
    /// <summary>
    /// 动画名称（空则不播放动画）
    /// </summary>
    [SerializeField]
    public string[] animationNames;
    /// <summary>
    /// 释放特效名称（空则不生成释放特效）
    /// </summary>
    [SerializeField]
    public string releaseVFXName;
    /// <summary>
    /// 释放音效名称（空则不生成释放音效）
    /// </summary>
    [SerializeField]
    public string releaseAFXName;
    /// <summary>
    /// 释放音效音量
    /// </summary>
    [SerializeField]
    public float releaseAFXVolume;
    /// <summary>
    /// 受击特效名称（空则不生成受击特效）
    /// </summary>
    [SerializeField]
    public string hitVFXName;
    /// <summary>
    /// 受击特效持续时间（<0则随受击特效预制体）
    /// </summary>
    [SerializeField]
    public float hitVFXDuration;
    /// <summary>
    /// 受击音效名称（空则不生成受击音效）
    /// </summary>
    [SerializeField]
    public string hitAFXName;
    /// <summary>
    /// 受击音效音量
    /// </summary>
    [SerializeField]
    public float hitAFXVolume;
    /// <summary>
    /// 技能预制体消失方式
    /// </summary>
    [SerializeField]
    public DisappearType disappearType;

    /// <summary>
    /// 技能指示器名字
    /// </summary>
    [Header("UI")]
    public string skillIndicator;
    /// <summary>
    /// 技能显示图标名字
    /// </summary>
    public string skillIconName;
}

/// <summary>
/// 需要运行时读取的技能配置
/// </summary>
[Serializable]
public struct SkillPostLoadSettings
{
    /// <summary>
    /// 预制体对象
    /// </summary>
    [Header("Prefab")]
    public GameObject skillSourcePrefab;
    /// <summary>
    /// 释放特效预制体
    /// </summary>
    public GameObject releaseVFXPrefab;
    /// <summary>
    /// 释放音效片段
    /// </summary>
    public AudioClip releaseAFXClip;
    /// <summary>
    /// 受击特效预制体
    /// </summary>
    public GameObject hitVFXPrefab;
    /// <summary>
    /// 受击音效片段
    /// </summary>
    public AudioClip hitAFXClip;

    [Header("UI")]
    public Image skillIcon;//技能事件图标
}
