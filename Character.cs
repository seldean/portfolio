///
/// This code is old and depricated. May need optimization and rework on it.
/// Wrote by Sertan Balta
///

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RebornGuardian.Characters
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator)), RequireComponent(typeof(SpriteRenderer))]
    public abstract class BaseCharacter : MonoBehaviour
    {
        public Rigidbody2D CharacterRigidBody2D { get; set; }
        public BoxCollider2D CharacterBoxCollider2D { get; set; }
        public Animator CharacterAnimator { get; set; }
        public SpriteRenderer CharacterSpriteRenderer { get; set; }

        public Material CharacterDefaultMaterial { get; set; }

        protected virtual void Awake()
        {
            CharacterRigidBody2D = GetComponent<Rigidbody2D>();
            CharacterBoxCollider2D = GetComponent<BoxCollider2D>();
            CharacterAnimator = GetComponent<Animator>();
            CharacterSpriteRenderer = GetComponent<SpriteRenderer>();
            CharacterDefaultMaterial = CharacterSpriteRenderer.material;
        }

        protected virtual void Update()
        {

        }
    }

    public interface ICharacterHealth
    {
        public float MaxHealth { get; set; }
        public float MinHealth { get; set; }
        public float CurrentHealth { get; set; }
        public string HurtAnimationName { get; set; }
        public string DieAnimationName { get; set; }

        public Material HurtMaterial { get; set; }
        public bool IsGettingHurt { get; set; }

        public bool TakeDamage(ICharacterAttack CharacterToTakeDamageFrom, Attack AttackToTakeDamageFrom);
        public IEnumerator TakeDamageMaterialChange(float TimeInSeconds);
        public bool Die();
    }

    public interface ICharacterHealthBar
    {
        public RectTransform HealthBarFillArea { get; set; }
        public float HealthBarFillAreaMaxSizeX { get; set; }

        public void UpdateHealthBarFillArea();
    }

    [System.Serializable]
    public class Attack
    {
        public List<KeyCode> Keys = new List<KeyCode>() { KeyCode.F };
        public float Cooldown = 1f;
        public string AnimationTriggerName = "Attack";
        public float Damage = 1f;
    }

    public interface ICharacterAttack
    {
        public List<Attack> Attacks { get; set; }
        public Attack CurrentAttack { get; set; }
        public string IsAttackingAnimationName { get; set; }

        public bool Attack();
        public bool DamageTo(ICharacterHealth CharacterToDamage);
    }

    public interface ICharacterMove
    {
        public float MoveSpeed { get; set; }
        public float RunSpeed { get; set; }

        public KeyCode MoveLeftKey { get; set; }
        public KeyCode MoveRightKey { get; set; }

        public KeyCode RunKey { get; set; }

        public string MoveAnimationTriggerName { get; set; }
        public string IsMovingAnimationName { get; set; }
        public string RunAnimationTriggerName { get; set; }
        public string IsRunningAnimationName { get; set; }

        public bool IsGrounded { get; set; }

        public bool Move();
    }

    public interface ICharacterJump
    {
        public float JumpForce { get; set; }

        public KeyCode JumpKey { get; set; }

        public string JumpAnimationTriggerName { get; set; }
        public string IsJumpingAnimationName { get; set; }

        public bool Jump();
    }

    public interface ICharacterDash
    {
        public float DashForce { get; set; }
        public KeyCode DashKey { get; set; }

        public Vector2 OriginalCollisionSize { get; set; }
        public Vector2 DashCollisionSize { get; set; }

        public string DashAnimationTriggerName { get; set; }
        public string IsDashingAnimationName { get; set; }

        public IEnumerator Dash();
    }

    public enum CharacterStates
    {
        Idle,
        Moving,
        Running,
        Jumping,
        Attacking,
        Dashing,
        Dead
    }
}
