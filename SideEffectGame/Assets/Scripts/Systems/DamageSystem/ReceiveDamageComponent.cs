//#define DATA_IN_MANAGER

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public interface ReceiveDamageInterface
{
    public abstract void ReceiveDamage(GameObject damageSource, float Value);
}

public class ReceiveDamageComponent: MonoBehaviour, ReceiveDamageInterface
{
    public float spawnHealth = 1f;
    public float spawnMana = 0f;
    public float spawnDuration = 1f;

#if !DATA_IN_MANAGER
    [SerializeField]
    public HealthData healthData = new HealthData();

    //public HealthData healthData
    //{
    //    get { return healthData; }
    //    set { healthData = value; }
    //}

#endif

    public UnityEvent postReceiveDamageEvent;

#if DATA_IN_MANAGER
    [Header("DebugData")]
    [SerializeField, Disable]
    private float _health = 0f;

    void UpdateDebugData()
    {
        _health = Health;
    }

    public float Health
    {
        get
        {
            return DamageManager.Singleton.GetHealthData(gameObject).health;
        }
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
#if DATA_IN_MANAGER
        HealthData healthData;
        healthData.health = spawnHealth;
        DamageManager.Singleton.RegisterHealthData(gameObject, healthData);
#else
        healthData.health = spawnHealth;
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR && DATA_IN_MANAGER
        UpdateDebugData();
#endif
    }

    private void OnDestroy()
    {
#if DATA_IN_MANAGER
        DamageManager.Singleton.UnregisterHealthData(gameObject);
#endif
    }

    public void ReceiveDamage(GameObject damageSource, float Value)
    {
        if (enabled)
        {
            DamageManager.Singleton.ReceiveDamage(gameObject, damageSource, Value);
        }
    }

    public void PostReceiveDamage(GameObject damageSource, float Value)
    {
        if (enabled)
        {
            // 处理受伤后
            postReceiveDamageEvent?.Invoke();
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
