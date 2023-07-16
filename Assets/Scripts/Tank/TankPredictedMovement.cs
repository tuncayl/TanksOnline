using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using Signals;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class TankPredictedMovement : NetworkBehaviour
{
#if !PREDICTION_V2

    public struct MoveData : IReplicateData
    {
        public float Horizontal;
        public float Vertical;

        public MoveData(float horizontal, float vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            _tick = 0;
        }

        private uint _tick;

        public void Dispose()
        {
        }

        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }

    public struct ReconcileData : IReconcileData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;

        public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
        {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
            _tick = 0;
        }

        private uint _tick;

        public void Dispose()
        {
        }

        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }

    #region SelfVariables

    [SerializeField] private float m_Speed;
    [SerializeField] private float m_TurnSpeed;

    private Rigidbody m_Rigidbody;

    private bool IsInitialized = true;
    
    #endregion


    #region UnityMethods

  

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    

    #endregion


    #region MainMethods

    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            Reconciliation(default, false);
            BuildMoveData(out MoveData md);
            MoveReplicate(md, false);
        }

        if (base.IsServer)
        {
            MoveReplicate(default, true);
        }
    }


    private void BuildMoveData(out MoveData md)
    {
        md = default;
        if (IsInitialized is false) return;
   
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0f && vertical == 0f)
            return;

        md = new MoveData(horizontal, vertical);
    }

    [Replicate]
    private void MoveReplicate(MoveData md, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
    {
        Move(md);
        Turn(md);
    }

    [Reconcile]
    private void Reconciliation(ReconcileData rd, bool asServer, Channel channel = Channel.Unreliable)
    {
        transform.position = rd.Position;
        transform.rotation = rd.Rotation;
        m_Rigidbody.velocity = rd.Velocity;
        m_Rigidbody.angularVelocity = rd.AngularVelocity;
    }


    private void Move(MoveData md)
    {
        Vector3 movement = transform.forward * md.Vertical * m_Speed;
        m_Rigidbody.AddForce(movement);
    }

    private void Turn(MoveData md)
    {
        float turn = md.Horizontal * m_TurnSpeed * (float)base.TimeManager.TickDelta;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    private void TimeManager_OnPostTick()
    {
        if (base.IsServer)
        {
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation, m_Rigidbody.velocity,
                m_Rigidbody.angularVelocity);
            Reconciliation(rd, true);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(IsOwner is false) return;
        PlayerSignals.Instance.OnPlayerDead += OnPlayerDead;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if(IsOwner is false) return;
        PlayerSignals.Instance.OnPlayerDead -= OnPlayerDead;

    }

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

    private void OnPlayerDead()
    {
        IsInitialized = false;
    }
    #endregion


    #region SubscireMethods

    private void Subscire()
    {
        TimeManager.OnTick += TimeManager_OnTick;
        TimeManager.OnPostTick += TimeManager_OnPostTick;
    }


    private void UnSubscire()
    {
        TimeManager.OnTick -= TimeManager_OnTick;
        TimeManager.OnPostTick -= TimeManager_OnPostTick;
    }

    #endregion

#endif
}