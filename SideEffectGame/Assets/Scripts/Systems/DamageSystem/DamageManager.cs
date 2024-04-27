//#define DATA_IN_MANAGER

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct HealthData
{
    [Tooltip("Hp")]
    public float health;

    [Tooltip("Mp")]
    public float mana;

    [Tooltip("Dp")]
    public float duration;
}

public class DamageManager : SceneSystemManagerBase
{
    static protected DamageManager _singleton = null;
    static public DamageManager Singleton
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = SceneSystemManagerBase.GetSingleton<DamageManager>();
            }
            return _singleton;
        }
    }

#if DATA_IN_MANAGER
    Dictionary<int, HealthData> _healthData = new Dictionary<int, HealthData>();

    public HealthData GetHealthData(GameObject gameObject)
    {
        HealthData result;
        _healthData.TryGetValue(gameObject.GetInstanceID(), out result);
        return result;
    }
    
    public void RegisterHealthData(GameObject gameObject, HealthData healthData)
    {
        _healthData.Add(gameObject.GetInstanceID(), healthData);
    }
    public void UnregisterHealthData(GameObject gameObject)
    {
        _healthData.Remove(gameObject.GetInstanceID());
    }
#else
    public HealthData GetHealthData(GameObject gameObject)
    {
        ReceiveDamageComponent receiveDamageComponent = gameObject.GetComponent<ReceiveDamageComponent>();
        if (receiveDamageComponent != null)
        {
            return receiveDamageComponent.healthData;
        }
        return new HealthData();
    }
#endif

    public void ReceiveDamage(GameObject targetObject, GameObject damageSource, float Value)
    {
        ReceiveDamageComponent receiveDamageComponent = targetObject?.GetComponent<ReceiveDamageComponent>();
        if (receiveDamageComponent != null)
        {
            // 处理共通的受伤逻辑：扣血，调用PostReceiveDamage
#if DATA_IN_MANAGER
            HealthData targetHealthData;
            if (_healthData.TryGetValue(targetObject.GetInstanceID(), out targetHealthData))
            {
                targetHealthData.health = Mathf.Max(targetHealthData.health - Value, 0f);
                if (targetHealthData.health <= 0f)
                {
                    NotifyNoHealth(targetObject);
                }
                else
                {
                    _healthData[targetObject.GetInstanceID()] = targetHealthData;
                    receiveDamageComponent.PostReceiveDamage(damageSource, Value);
                }
            }
#else
            float targetHealth = receiveDamageComponent.healthData.health;
            targetHealth = Mathf.Max(targetHealth - Value, 0f);
            if (targetHealth <= 0f)
            {
                NotifyNoHealth(targetObject);
            }
            else
            {
                receiveDamageComponent.healthData.health = targetHealth;
                receiveDamageComponent.PostReceiveDamage(damageSource, Value);
            }
#endif
        }
    }

    public void NotifyNoHealth(GameObject gameObject)
    {
#if DATA_IN_MANAGER
        UnregisterHealthData(gameObject);
#endif
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    new void Awake()
    {
        SceneSystemManagerBase.RegisterSceneSystemManager(this);
    }

    new void OnDestroy()
    {
        SceneSystemManagerBase.UnregisterSceneSystemManager(this);
    }
}
