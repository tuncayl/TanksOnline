using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class TankPredictedShoot : NetworkBehaviour
{

    #region SelfVariables

    [SerializeField] private NetworkObject BulletPrefab;

    [SerializeField] private Transform FirePoint;
    
    
    
    
    private bool _spawnBullet = false;

    private float FireRate = 0;
    
    #endregion


    #region UnityMethods

    private void Update()
    {
        
        if(base.IsOwner is false) return;
        if(FireRate>Time.time) return;
        if (Input.GetKeyDown(KeyCode.Space)) _spawnBullet = true;
    }
   
    #endregion



    #region MainMethods
   
  
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        Subscire();
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        UnSubscire();
    }



    private void OnTick()
    {
        if(base.IsOwner is false) return;
        
        
        TrySpawnBullet();
    }
    
    
    private void TrySpawnBullet()
    {
        if (_spawnBullet)
        {
            FireRate = Time.time + 1;
            _spawnBullet = false;

            NetworkObject nob = Instantiate(BulletPrefab, FirePoint.position , FirePoint.rotation);
            PredictedBullet bt = nob.GetComponent<PredictedBullet>();
            bt.SetStartingForce(FirePoint.forward * 20f);
            base.Spawn(nob, base.Owner);
        }
    }
    #endregion



    #region SubscireMethods

    private void Subscire()
    {
        TimeManager.OnTick += OnTick;
    }

    private void UnSubscire()
    {
        TimeManager.OnTick -= OnTick;

    }
    
    #endregion
}
