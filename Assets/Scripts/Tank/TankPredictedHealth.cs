using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Interface;
using Signals;
using UnityEngine;
using UnityEngine.UI;

public class TankPredictedHealth : NetworkBehaviour,IDamageable
{
    [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(OnChangeHealth))] 
    private float NetworkHealth=100f;


    
    #region SelfVariables
    public float m_StartingHealth = 100f;            
    public Slider m_Slider;                             
    public Image m_FillImage;                           

    public GameObject m_ExplosionPrefab;
    private AudioSource m_ExplosionAudio;               
    private ParticleSystem m_ExplosionParticles;      
    private float m_CurrentHealth;                     
    private bool m_Dead;     
    #endregion


    #region UnityMethods

    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }

    #endregion


    #region MainMethods

    public override void OnStartClient()
    {
        base.OnStartClient();
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
        SetHealthUI();
    }

    private void OnChangeHealth(float prev,float next,bool asserver)
    {
        if(base.IsServer) return;

        m_CurrentHealth = next;
        SetHealthUI();
        
        if(IsOwner is false) return;
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            PlayerSignals.Instance.OnPlayerDead.Invoke();
            SendServerDead();
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void SendServerDead() => ObserverDead();

    [ObserversRpc]
    private void ObserverDead() => OnDeath();
    
    public void TakeDamage(float amount)
    {
        if (base.IsServer)
        {
            NetworkHealth -= amount;
            return;
        }
        
        m_CurrentHealth -= amount;
        
        SetHealthUI();
        
      
    }


    private void SetHealthUI()
    {
        m_Slider.value = m_CurrentHealth;

        m_FillImage.color = Color.Lerp(Color.red, Color.green, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
     
        m_Dead = true;

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

       
    }

    #endregion


    #region SubscireMethods

    private void Subscire()
    {
    }

    private void UnSubscire()
    {
    }

    #endregion
}