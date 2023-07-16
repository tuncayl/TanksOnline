using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Signals;
using UnityEngine;


[RequireComponent(typeof(TankPredictedHealth))]
[RequireComponent(typeof(TankPredictedMovement))]
[RequireComponent(typeof(TankPredictedShoot))]
public class Tank : NetworkBehaviour
{
    #region SelfVariables
    private TankPredictedHealth _TankPredictedHealth { get;  set; }
    private TankPredictedMovement _TankPredictedMovement { get;  set; }
    private TankPredictedShoot _TankPredictedShoot { get;  set; }
    #endregion


    #region UnityMethods

    private void Awake()
    {
        _TankPredictedHealth = GetComponent<TankPredictedHealth>();
        _TankPredictedMovement = GetComponent<TankPredictedMovement>();
        _TankPredictedShoot = GetComponent<TankPredictedShoot>();
    }

  

    #endregion


    #region MainMethods

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner is false) return;
        Subscire();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (IsOwner is false) return;
        UnSubscire();
    }


    private void OnPlayerDead()
    {
        _TankPredictedShoot.enabled = false;
        _TankPredictedHealth.enabled = false;
    }
    #endregion


    #region SubscireMethods

    private void Subscire()
    {
        PlayerSignals.Instance.OnPlayerDead += OnPlayerDead;
    }

    private void UnSubscire()
    {
        PlayerSignals.Instance.OnPlayerDead -= OnPlayerDead;

    }

    #endregion
}