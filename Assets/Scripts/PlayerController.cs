using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 12f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float iFramesDuration = 1.5f;
    [SerializeField] private float dodgeCooldown = 0.75f;
    [SerializeField] private Image cooldownImage;

    [Header("Tool Pivot")]
    [SerializeField] private Transform toolHolder; // Drag ToolHolder child GameObject here

    private Rigidbody2D rb;
    private SpriteFlasher spriteFlasher;

    private Vector2 moveInput;
    private Vector2 dodgeDirection;
    private Vector2 lastFacingDirection = Vector2.up; // Default facing up

    private bool isDodging;
    private bool cooldown = false;
    private float dodgeTimer;
    private float iFramesTimer;
    private float dodgeCooldownTimer;

    public bool IsDodging => isDodging;
    public bool IsInvincible => iFramesTimer > 0f;
    public Vector2 FacingDirection => lastFacingDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteFlasher = GetComponent<SpriteFlasher>();
    }

    private void Update()
    {
        ReadInput();
        UpdateFacingAndToolHolder();

        if (dodgeCooldownTimer > 0f)
            dodgeCooldownTimer -= Time.deltaTime;

        if (iFramesTimer > 0f)
            iFramesTimer -= Time.deltaTime;

        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;

            if (dodgeTimer <= 0f && !cooldown)
            {

                isDodging = false;

                StartCoroutine(DodgeCooldown());

            }
        }
    }

    private void FixedUpdate()
    {
        if (isDodging)
        {
            rb.linearVelocity = dodgeDirection * dodgeSpeed;
        }
        else
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    private void ReadInput()
    {
        if (Keyboard.current == null)
            return;

        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            moveInput.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            moveInput.y -= 1f;

        if (Keyboard.current.aKey.isPressed)
            moveInput.x -= 1f;

        if (Keyboard.current.dKey.isPressed)
            moveInput.x += 1f;

        moveInput = moveInput.normalized;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryDodge();
        }
    }

    // Called every frame in update. Grabs the mouse position in game and makes the player sprite face it.
    private void UpdateFacingAndToolHolder()
    {

        Vector3 mousePos = Input.mousePosition;

        mousePos.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = (mousePos - transform.position);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

    }


    private void TryDodge()
    {
        if (isDodging || dodgeCooldownTimer > 0f)
            return;

        dodgeDirection = moveInput;

        // Default to last facing direction if pressing space while standing still
        if (dodgeDirection == Vector2.zero)
        {
            dodgeDirection = lastFacingDirection;
        }

        isDodging = true;
        dodgeTimer = dodgeDuration;
        iFramesTimer = iFramesDuration;
        dodgeCooldownTimer = dodgeCooldown;

        if (spriteFlasher != null)
        {
            spriteFlasher.FlashWhite(iFramesDuration);
        }
    }

    // Sets the color of the on screen cooldown to black
    // Over the time of dodgeCooldownTimer it will fade back to white
    IEnumerator DodgeCooldown()
    {

        cooldown = true;
        float timer = 0f;
        cooldownImage.CrossFadeColor(Color.black, 0, true, true);
        

        while (timer < dodgeCooldownTimer)
        {

            timer += Time.deltaTime;
            yield return null;

        }

        cooldownImage.CrossFadeColor(Color.white, dodgeCooldownTimer, true, true);
        cooldown = false;

    }


}
