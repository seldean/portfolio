///
/// This file is old/depricated, needs optimization and rework.
/// Wrote by Sertan Balta
///

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RebornGuardian.Characters;
using UnityEngine.Rendering.Universal;

public class Bunny : BaseCharacter, ICharacterHealth, ICharacterHealthBar, ICharacterAttack, ICharacterMove, ICharacterJump, ICharacterDash
{
    #region Variables
    [HideInInspector]
    public CharacterStates CharacterState { get; private set; } = CharacterStates.Idle;

    [Header("Health Settings")]
    [SerializeField, Range(0f, 1000f)]
    private float maxHealth = 100f;
    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = Mathf.Clamp(value, 0f, 1000f); // Clamps the data according to the range attribute
    }

    [SerializeField, Range(-1000f, 0f)]
    private float minHealth = 0f;
    public float MinHealth
    {
        get => minHealth;
        set => minHealth = Mathf.Clamp(value, -1000f, 0f); // Clamps the data according to the range attribute
    }

    [SerializeField, Range(-999f, 999f)]
    private float currentHealth = 100f;
    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, -999f, 999f); // Clamps the data according to the range attribute
    }

    [SerializeField]
    private string hurtAnimationName = "Hurt";
    public string HurtAnimationName
    {
        get => hurtAnimationName;
        set => hurtAnimationName = value;
    }

    [SerializeField]
    private string dieAnimationName = "Die";
    public string DieAnimationName
    {
        get => dieAnimationName;
        set => dieAnimationName = value;
    }

    [SerializeField]
    private Material hurtMaterial = null;
    public Material HurtMaterial
    {
        get => hurtMaterial;
        set => hurtMaterial = value;
    }

    [SerializeField]
    private RectTransform healthBarFillArea = null;
    public RectTransform HealthBarFillArea
    {
        get => healthBarFillArea;
        set => healthBarFillArea = value;
    }

    public bool IsGettingHurt { get; set; }

    [Header("Attack Settings")]
    [SerializeField]
    private List<Attack> attacks = new List<Attack>();
    public List<Attack> Attacks
    {
        get => attacks;
        set => attacks = value;
    }

    public float HealthBarFillAreaMaxSizeX { get; set; }

    [SerializeField]
    private string isAttackingAnimationName = "IsAttacking";
    public string IsAttackingAnimationName
    {
        get => isAttackingAnimationName;
        set => isAttackingAnimationName = value;
    }

    [HideInInspector]
    public Attack CurrentAttack { get; set; } = new Attack();

    [Header("Movement Settings")]
    [SerializeField, Range(0f, 20f)]
    private float moveSpeed = 3f;
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Clamp(value, 0f, 20f); // Clamps the data according to the range attribute
    }

    [SerializeField, Range(0f, 80f)]
    private float runSpeed = 12f;
    public float RunSpeed
    {
        get => runSpeed;
        set => runSpeed = Mathf.Clamp(value, 0f, 80f); // Clamps the data according to the range attribute
    }

    [SerializeField]
    private KeyCode moveLeftKey = KeyCode.A;
    public KeyCode MoveLeftKey
    {
        get => moveLeftKey;
        set => moveLeftKey = value;
    }

    [SerializeField]
    private KeyCode moveRightKey = KeyCode.D;
    public KeyCode MoveRightKey
    {
        get => moveRightKey;
        set => moveRightKey = value;
    }

    [SerializeField]
    private KeyCode runKey = KeyCode.LeftControl;
    public KeyCode RunKey
    {
        get => runKey;
        set => runKey = value;
    }

    [SerializeField]
    private string moveAnimationTriggerName = "Move";
    public string MoveAnimationTriggerName
    {
        get => moveAnimationTriggerName;
        set => moveAnimationTriggerName = value;
    }

    [SerializeField]
    private string isMovingAnimationName = "IsMoving";
    public string IsMovingAnimationName
    {
        get => isMovingAnimationName;
        set => isMovingAnimationName = value;
    }

    [SerializeField]
    private string runAnimationTriggerName = "Run";
    public string RunAnimationTriggerName
    {
        get => runAnimationTriggerName;
        set => runAnimationTriggerName = value;
    }

    [SerializeField]
    private string isRunningAnimationName = "IsRunning";
    public string IsRunningAnimationName
    {
        get => isRunningAnimationName;
        set => isRunningAnimationName = value;
    }

    [HideInInspector]
    public bool IsGrounded { get; set; } = false;

    [SerializeField, Range(0f, 20000f)]
    private float jumpForce = 4000f;
    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = Mathf.Clamp(value, 0f, 20000f);
    }

    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;
    public KeyCode JumpKey
    {
        get => jumpKey;
        set => jumpKey = value;
    }

    [SerializeField]
    private string jumpAnimationTriggerName = "Jump";
    public string JumpAnimationTriggerName
    {
        get => jumpAnimationTriggerName;
        set => jumpAnimationTriggerName = value;
    }

    [SerializeField]
    private string isJumpingAnimationName = "IsJumping";
    public string IsJumpingAnimationName
    {
        get => isJumpingAnimationName;
        set => isJumpingAnimationName = value;
    }

    [Header("Dash Settings")]
    [SerializeField, Range(0f, 200f)]
    private float dashForce = 120f;
    public float DashForce
    {
        get => dashForce;
        set => dashForce = Mathf.Clamp(value, 0f, 200f);
    }

    [SerializeField]
    private KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode DashKey
    {
        get => dashKey;
        set => dashKey = value;
    }

    [SerializeField]
    private Vector2 originalCollisionSize = new Vector2(5f, 14f);
    public Vector2 OriginalCollisionSize
    {
        get => originalCollisionSize;
        set => originalCollisionSize = value;
    }

    [SerializeField]
    private Vector2 dashCollisionSize = new Vector2(0.5f, 0.5f);
    public Vector2 DashCollisionSize
    {
        get => dashCollisionSize;
        set => dashCollisionSize = value;
    }

    [SerializeField]
    private string dashAnimationTriggerName = "Dash";
    public string DashAnimationTriggerName
    {
        get => dashAnimationTriggerName;
        set => dashAnimationTriggerName = value;
    }

    [SerializeField]
    private string isDashingAnimationName = "IsDashing";
    public string IsDashingAnimationName
    {
        get => isDashingAnimationName;
        set => isDashingAnimationName = value;
    }

    [HideInInspector]
    public float DashAnimationLengthInSeconds { get; set; } = 0.5f;

    public ParticleSystem DamageParticle;
    public ParticleSystem DashParticle;
    public Light2D BunnyAmbienceLight;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        CharacterBoxCollider2D.size = OriginalCollisionSize;
        HealthBarFillAreaMaxSizeX = HealthBarFillArea.sizeDelta.x;

        //The below code updates the dash animation length in seconds for Dash() method to be able to hold for a while before resetting the collision size. - Seldean
        RuntimeAnimatorController runtimeAnimatorController = CharacterAnimator.runtimeAnimatorController;
        foreach (AnimationClip animationClip in runtimeAnimatorController.animationClips)
        {
            if (animationClip.name == DashAnimationTriggerName)
            {
                DashAnimationLengthInSeconds = animationClip.length;
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        HandleCharacterState();

        //if (BunnyAmbienceLight.color == Color.red)
        //{
        //    BunnyAmbienceLight.color = Color.white;
        //}

        // DEBUGGING ONLY
        if (Input.GetKey(KeyCode.Backspace))
        {
            TakeDamage(this, new Attack());
        }

        //TakeDamage(this, new Attack());
    }

    void ChangeState(CharacterStates NewCharacterState)
    {
        try
        {
            if (CharacterState == CharacterStates.Dead)
                return;

            CharacterState = NewCharacterState;
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR: {e.Message} - {e.HelpLink}");
            return;
        }
    }

    public void ChangeStateToIdle()
    {
        CharacterState = CharacterStates.Idle;
    }

    private void HandleCharacterState()
    {
        switch (CharacterState)
        {
            case CharacterStates.Idle:
                if (Attack()) { }
                if (Jump()) { }
                if (Move()) { }
                StartCoroutine(Dash());
                break;
            case CharacterStates.Moving:
                if (Attack()) { }
                if (Jump()) { }
                if (Move()) { }
                StartCoroutine(Dash());
                break;
            case CharacterStates.Running:
                if (Attack()) { }
                if (Jump()) { }
                if (Move()) { }

                StartCoroutine(Dash());
                break;
            case CharacterStates.Jumping:
                if (Move()) { }

                StartCoroutine(Dash());
                break;
            case CharacterStates.Attacking:

                break;
            case CharacterStates.Dashing:

                break;
            case CharacterStates.Dead:
                break;
            default:
                Debug.LogError($"Character state error on: {transform.name}");
                break;
        }
    }

    public bool Attack()
    {
        try
        {
            foreach (Attack attack in Attacks)
            {
                if (attack.Keys.TrueForAll(Input.GetKeyDown))
                {
                    ChangeState(CharacterStates.Attacking);
                    CurrentAttack = attack;
                    CharacterAnimator.SetTrigger(attack.AnimationTriggerName);
                    CharacterAnimator.SetBool(IsAttackingAnimationName, true);
                    return true;
                }
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR: {e.Message} - {e.HelpLink}");
            return false;
        }
    }

    public void ResetIsAttacking()
    {
        CharacterAnimator.SetBool(IsAttackingAnimationName, false);
    }

    public bool DamageTo(ICharacterHealth CharacterToDamage)
    {
        try
        {
            return CharacterToDamage.TakeDamage(this, CurrentAttack);
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR: {e.Message} - {e.HelpLink}");
            return false;
        }
    }

    public IEnumerator Dash()
    {
        if (Input.GetKeyDown(DashKey))
        {
            ChangeState(CharacterStates.Dashing);

            if (!CharacterSpriteRenderer.flipX)
            {
                CharacterRigidBody2D.AddForce(new Vector2(DashForce, 0), ForceMode2D.Impulse);
                if (DashParticle != null)
                {
                    DashParticle.transform.rotation = Quaternion.Euler(0, 0, 90);
                    DashParticle.Play();
                }
            }
            else if (CharacterSpriteRenderer.flipX)
            {
                CharacterRigidBody2D.AddForce(new Vector2(-DashForce, 0), ForceMode2D.Impulse);
                if (DashParticle != null)
                {
                    DashParticle.transform.rotation = Quaternion.Euler(0, 0, 270);
                    DashParticle.Play();
                }
            }

            CharacterAnimator.SetTrigger(DashAnimationTriggerName);
            CharacterAnimator.SetBool(IsDashingAnimationName, true);

            // The code below adjusts the collision size while dashing. After it waits for animation to end, it goes back to the original size.
            CharacterBoxCollider2D.size = DashCollisionSize;
            yield return new WaitForSeconds(DashAnimationLengthInSeconds);
            CharacterBoxCollider2D.size = OriginalCollisionSize;
            CharacterAnimator.SetBool(IsDashingAnimationName, false);
            ChangeState(CharacterStates.Idle);
        }
    }

    public bool Jump()
    {
        try
        {
            if (Input.GetKeyDown(JumpKey))
            {
                Debug.Log("Jump triggered!!!");
                ChangeState(CharacterStates.Jumping);
                CharacterRigidBody2D.AddForce(new Vector2(0, JumpForce));
                CharacterAnimator.SetTrigger(JumpAnimationTriggerName);
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR: {e.Message} - {e.HelpLink}");
            return false;
        }
    }

    public bool Move()
    {
        try
        {
            if (Input.GetKey(MoveLeftKey))
            {
                CharacterSpriteRenderer.flipX = true;
                return MoveContinue(true, Input.GetKey(RunKey), IsGrounded ? false : true);
            }
            else if (Input.GetKey(MoveRightKey))
            {
                CharacterSpriteRenderer.flipX = false;
                return MoveContinue(false, Input.GetKey(RunKey), IsGrounded ? false : true);
            }
            else
            {
                CharacterAnimator.SetBool(IsMovingAnimationName, false);
                CharacterAnimator.SetBool(IsRunningAnimationName, false);
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR: {e.Message} - {e.HelpLink}");
            return false;
        }
    }

    private bool MoveContinue(bool axis, bool isRunning, bool isAirborne)
    {
        try
        {
            if (!isAirborne)
            {
                ChangeState(CharacterStates.Moving);
                CharacterAnimator.SetBool(isRunning ? IsRunningAnimationName : IsMovingAnimationName, true);
                CharacterAnimator.SetBool(isRunning ? IsMovingAnimationName : IsRunningAnimationName, false);
            }

            Vector2 movement = new Vector2(axis ? -1 : +1, 0) * (isRunning ? RunSpeed : MoveSpeed);
            CharacterRigidBody2D.linearVelocity = new Vector2(movement.x, CharacterRigidBody2D.linearVelocity.y);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR: {e.Message} - {e.HelpLink}");
            return false;
        }
    }

    public bool TakeDamage(ICharacterAttack CharacterToTakeDamageFrom, Attack AttackToTakeDamageFrom)
    {
        try
        {
            if (CurrentHealth > 0)
            {
                CurrentHealth -= AttackToTakeDamageFrom.Damage;
                BunnyAmbienceLight.color = Color.red;
                UpdateHealthBarFillArea();
                if (DamageParticle != null)
                {
                    DamageParticle.Play();
                }
            }

            if (CurrentHealth <= 0)
            {
                Die();
                UpdateHealthBarFillArea();
            }
            else
            {
                UpdateHealthBarFillArea();
            }

            if (HurtMaterial != null) StartCoroutine(TakeDamageMaterialChange(0.5f));

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR: {e.Message} - {e.HelpLink}");
            return false;
        }
    }

    public void UpdateHealthBarFillArea()
    {
        float scaledHealth = (CurrentHealth / MaxHealth) * HealthBarFillAreaMaxSizeX;
        float NewHealthBarSize = Mathf.Clamp(scaledHealth, 0, HealthBarFillAreaMaxSizeX);

        // Correctly update the sizeDelta
        HealthBarFillArea.sizeDelta = new Vector2(NewHealthBarSize, HealthBarFillArea.sizeDelta.y);
    }

    public IEnumerator TakeDamageMaterialChange(float TimeInSeconds)
    {
        CharacterSpriteRenderer.material = HurtMaterial;
        yield return new WaitForSeconds(TimeInSeconds);
        CharacterSpriteRenderer.material = CharacterDefaultMaterial;
    }

    public bool Die()
    {
        try
        {
            ChangeState(CharacterStates.Dead);
            CharacterAnimator.SetTrigger(DieAnimationName);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR: {e.Message} - {e.HelpLink}");
            return false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            IsGrounded = true;
            CharacterAnimator.SetBool("IsGrounded", IsGrounded);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            IsGrounded = false;
            CharacterAnimator.SetBool("IsGrounded", IsGrounded);
        }
    }
}
