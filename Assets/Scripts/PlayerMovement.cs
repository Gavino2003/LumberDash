using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Salto")]
    public float jumpForce = 7f;
    public float fallForce = 20f;

    [Header("Movimento")]
    public float laneDistance = 2.5f;
    public float laneSwitchSpeed = 10f;

    [Header("Roll")]
    public float rollDuration = 1f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private bool isGrounded = false;
    private int currentLane = 1;
    private float targetX = 0f;
    private float rollTimer = 0f;
    private bool isRolling = false;
    private bool pendingRoll = false;
    private bool isDashing = false;
    private Animator animator;
    private Vector3 originalColliderCenter;
    private float originalColliderHeight;
    private Chaser chaser;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        originalColliderCenter = capsuleCollider.center;
        originalColliderHeight = capsuleCollider.height;
         chaser = FindFirstObjectByType<Chaser>();
    }

    void Update()
    {
        if (GameManager.Instance.IsWaiting) return;
        // Salto
        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) && isGrounded)
        {
            if (isRolling)
            {
                StopRoll();
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            }
            pendingRoll = false;
            AudioManager.Instance.PlayJump();
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            animator.SetTrigger("Jump");

            chaser?.OnPlayerJump();
        }

        // Movimento lateral
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame && currentLane > 0)
        {
            currentLane--;
            targetX = (currentLane - 1) * laneDistance;
             chaser?.OnPlayerChangeLane(targetX);
            if (!isRolling)
            {
                animator.SetTrigger("DashLeft");
                animator.SetBool("IsDashing", true);
                StartCoroutine(StopDash());
            }
        }

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame && currentLane < 2)
        {
            currentLane++;
            targetX = (currentLane - 1) * laneDistance;
             chaser?.OnPlayerChangeLane(targetX);
            if (!isRolling)
            {
                animator.SetTrigger("DashRight");
                animator.SetBool("IsDashing", true);
                StartCoroutine(StopDash());
            }
        }

        // Roll
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            if (!isGrounded)
            {
                AudioManager.Instance.PlayRoll();
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.down * fallForce, ForceMode.Impulse);
                pendingRoll = true;
                animator.SetTrigger("Roll");
                animator.SetBool("IsRolling", true);
            }
            else if (!isRolling)
            {
                StartRoll();
            }
        }

        // Contar duração do roll
        if (isRolling)
        {
            rollTimer += Time.deltaTime;
            if (rollTimer >= rollDuration)
                StopRoll();
        }
    }

    IEnumerator StopDash()
    {
        float distanceToTarget = Mathf.Abs(rb.position.x - targetX);
        float timeToReach = distanceToTarget / laneSwitchSpeed;
        yield return new WaitForSeconds(timeToReach);
        animator.SetBool("IsDashing", false);
    }

    void StartRoll()
{
    AudioManager.Instance.PlayRoll();
    isRolling = true;
    rollTimer = 0f;
    capsuleCollider.height = originalColliderHeight * 0.5f;
    capsuleCollider.center = new Vector3(
        originalColliderCenter.x,
        originalColliderCenter.y - (originalColliderHeight * 0.25f),
        originalColliderCenter.z
    );
    animator.SetTrigger("Roll");
    animator.SetBool("IsRolling", true);
}

    void StopRoll()
    {
        isRolling = false;
        rollTimer = 0f;
        capsuleCollider.height = originalColliderHeight;
        capsuleCollider.center = originalColliderCenter;
        animator.SetBool("IsRolling", false);
    }

    public void PushToLane(int direction)
{
    
    currentLane = Mathf.Clamp(currentLane + direction, 0, 2);
    targetX = (currentLane - 1) * laneDistance;
    animator.SetTrigger("Stumble");
    chaser?.StartChasing();
    chaser?.OnPlayerChangeLane(targetX);

    // Se estiver no ar cai ao bater
    if (!isGrounded)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.down * fallForce * 0.5f, ForceMode.Impulse);
    }
}

    public void StopPlayer()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        enabled = false;
    }

    public void ResetPlayer()
    {
        enabled = true;
        pendingRoll = false;
        currentLane = 1;
        targetX = 0f;
        isGrounded = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localScale = new Vector3(1f, 1f, 1f);
        transform.position = new Vector3(0f, transform.position.y, -6f);
        StopAllCoroutines();
        StopRoll();
        animator.SetBool("IsDashing", false);
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("Roll");
        animator.ResetTrigger("DashLeft");
        animator.ResetTrigger("DashRight");
        animator.ResetTrigger("Stumble");
        animator.ResetTrigger("Side_Death");
        
    }

    void FixedUpdate()
    {
        Vector3 pos = rb.position;
        pos.x = Mathf.MoveTowards(rb.position.x, targetX, laneSwitchSpeed * Time.fixedDeltaTime);
        rb.MovePosition(pos);
    }

    void OnCollisionStay(Collision collision)
{
    if (collision.gameObject.CompareTag("Ground"))
    {
        isGrounded = true;

        if (pendingRoll)
        {
            pendingRoll = false;
            isRolling = true;
            rollTimer = 0f;
            capsuleCollider.height = originalColliderHeight * 0.5f;
            capsuleCollider.center = new Vector3(
                originalColliderCenter.x,
                originalColliderCenter.y - (originalColliderHeight * 0.25f),
                originalColliderCenter.z
            );
        }
    }
}

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger("Side_Death");
    }
    public void PlayFrontHitAnimation()
{
    animator.SetTrigger("Front_Hit");
}
public void StartRunning()
{
    animator.SetTrigger("StartRun");
}

public void StartWaiting()
    {
        animator.ResetTrigger("StartRun");
        animator.Play("idle_fixed", 0, 0f);
    }
}