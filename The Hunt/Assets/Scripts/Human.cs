using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Human : MonoBehaviour
{
    public static List<Human> AllHumans = new List<Human>();

    public enum HumanState
    {
        Idle,
        Running,
        Dead
    }

    [Header("Health")]
    public float MaxHealth = 100f;
    public float Health;
    public float InjuredThreshold = 30f;

    [Header("Movement")]
    public float RunSpeed = 4f;
    public NavMeshAgent Agent;

    [Header("Exits")]
    public Transform[] Exits;

    [Header("Animations")]
    public Animator Animator;
    public List<string> IdleAnimations;
    public List<string> RunAnimations;
    public List<string> InjuredRunAnimations;

    public CapsuleCollider CapsuleCollider;
    
   
    private HumanState CurrentState = HumanState.Idle;
    private bool IsInjured;

    public RagdollController Ragdoll;

    void Awake()
    {
        AllHumans.Add(this);
    }

    void OnDestroy()
    {
        AllHumans.Remove(this);
    }

    void Start()
    {
        Health = MaxHealth;
        PickRandomIdle();
    }

    void Update()
    {
        if (CurrentState == HumanState.Dead)
            return;

        if (CurrentState == HumanState.Running)
        {
            if (Health <= InjuredThreshold && !IsInjured)
            {
                IsInjured = true;
                PickRandomInjuredRun();
            }
        }
    }

    // =========================
    // DAMAGE
    // =========================
    public void TakeDamage(float damage)
    {
        if (CurrentState == HumanState.Dead)
            return;

        Health -= damage;

        TriggerGlobalPanic();

        if (Health <= 0)
        {
            Die();
        }
    }

    // =========================
    // PANIC
    // =========================
    public void TriggerGlobalPanic()
    {
        foreach (Human human in AllHumans)
        {
            if (human.CurrentState != HumanState.Dead)
                human.RunToExit();
        }
    }

    void RunToExit()
    {
        if (CurrentState == HumanState.Running)
            return;

        CurrentState = HumanState.Running;

        Agent.speed = RunSpeed;

        Transform exit = Exits[Random.Range(0, Exits.Length)];
        Agent.SetDestination(exit.position);

        if (Health <= InjuredThreshold)
        {
            IsInjured = true;
            PickRandomInjuredRun();
        }
        else
        {
            PickRandomRun();
        }
    }

    // =========================
    // ANIMATIONS
    // =========================
    void PickRandomIdle()
    {
        if (IdleAnimations.Count == 0)
            return;

        Animator.Play(IdleAnimations[Random.Range(0, IdleAnimations.Count)]);
    }

    void PickRandomRun()
    {
        if (RunAnimations.Count == 0)
            return;

        Animator.Play(RunAnimations[Random.Range(0, RunAnimations.Count)]);
    }

    void PickRandomInjuredRun()
    {
        if (InjuredRunAnimations.Count == 0)
            return;

        Animator.Play(InjuredRunAnimations[Random.Range(0, InjuredRunAnimations.Count)]);
    }

    // =========================
    // DEATH
    // =========================
    void Die()
    {
        CurrentState = HumanState.Dead;

        Agent.enabled = false;

        Ragdoll.SetRagdoll(true);

        this.enabled = false;
        CapsuleCollider.enabled = false;
    }

}