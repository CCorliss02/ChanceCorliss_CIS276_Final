using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = 32f;
    [Range(-1000f, -1f)] private float terminalVelocity = -1000f;
    private float gravityComponent;
    private Vector3 velocity;
    private bool midairJump;
    public bool isInvincible;
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int currentHealth = 3;
    public Text scoreText;
    public Text healthText;
    public Text tipsText;
    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioClip enemyDeathSound;
    public AudioClip collectCrystalSound;
    public AudioClip winLevelSound;
    public AudioClip playerHurtSound;

    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;
    private PlayerBoxAbility playerBoxAbility;
    private ChangeToScene changeToScene;
    private GameSceneManager gameSceneManager;
    private AudioSource audioSource;

    [SerializeField] private Transform cameraTransform;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerBoxAbility = GetComponent<PlayerBoxAbility>();
        changeToScene = GetComponent<ChangeToScene>();
        gameSceneManager = GetComponent<GameSceneManager>();
        audioSource = GetComponent<AudioSource>();
        midairJump = false;
        isInvincible = false;
    }

    private void MovePlayer()
    {
        float moveInputMagnitude = playerInput.moveInput.magnitude * speed;
        Vector3 direction = playerInput.moveInput.normalized;
        direction = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * direction;
        Vector3 finalVelocity = moveInputMagnitude * direction + (gravityComponent * Vector3.up);
        velocity = finalVelocity;
        characterController.Move(finalVelocity * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            animator.SetFloat("movement", 1);
        }

        else
        {
            animator.SetFloat("movement", 0);
        }
    }

    private void CalculateAnimations()
    {
        if (playerInput.jumpInput && characterController.isGrounded)
        {
            animator.SetTrigger("jump");
            audioSource.clip = jumpSound;
            audioSource.Play();
        }

        else
        {
            if (playerInput.jumpInput && !characterController.isGrounded && midairJump)
            {
                animator.SetTrigger("midairjump");
                audioSource.clip = jumpSound;
                audioSource.Play();
            }
        }

        animator.SetBool("airborne", !characterController.isGrounded);
    }

    private void CalculateVerticalSpeed()
    {
        if (characterController.isGrounded)
        {
            midairJump = true;

            if (playerInput.jumpInput)
            {
                gravityComponent = Mathf.Sqrt(-2f * -gravity * jumpHeight);
            }
        }

        else
        {
            if (playerInput.jumpInput && midairJump)
            {
                gravityComponent = Mathf.Sqrt(-2f * -gravity * jumpHeight);
                midairJump = false;
            }
        }

        if (gravityComponent > terminalVelocity)
        {
            gravityComponent -= gravity * Time.deltaTime;
        }
    }

    private void HideTips()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            tipsText.text = " ";
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Collectable")
        {
            Destroy(collider.gameObject);
            currentScore += 10;
            audioSource.clip = collectCrystalSound;
            audioSource.Play();
        }

        if (collider.gameObject.tag == "EndCollectable")
        {
            Destroy(collider.gameObject);
            Destroy(playerBoxAbility.box);
            playerInput.isInputEnabled = false;
            isInvincible = true;
            audioSource.clip = winLevelSound;
            audioSource.Play();
            StartCoroutine(EndLevel());
        }

        if (collider.gameObject.tag == "Enemy")
        {
            if (!isInvincible && currentHealth > 0)
            {
                currentHealth -= 1;
                audioSource.clip = playerHurtSound;
                audioSource.Play();
                StartCoroutine(InvincibleTimer());
            }
        }

        if (collider.gameObject.tag == "KillZone")
        {
            if (gameSceneManager.currentScene.name == "LevelOne")
            {
                changeToScene.LoadSceneLevelOne();
            }

            if (gameSceneManager.currentScene.name == "LevelTwo")
            {
                changeToScene.LoadSceneLevelTwo();
            }
        }
    }

    private IEnumerator InvincibleTimer()
    {
        if (currentHealth == 0)
        {
            StopCoroutine(InvincibleTimer());
            StartCoroutine(HealthHandler());
        }

        isInvincible = true;
        yield return new WaitForSeconds(2f);
        isInvincible = false;
    }

    private IEnumerator HealthHandler()
    {
        Destroy(playerBoxAbility.box);
        playerInput.isInputEnabled = false;
        animator.SetTrigger("defeat");
        yield return new WaitForSeconds(3f);

        if (gameSceneManager.currentScene.name == "LevelOne")
        {
            changeToScene.LoadSceneLevelOne();
        }

        if (gameSceneManager.currentScene.name == "LevelTwo")
        {
            changeToScene.LoadSceneLevelTwo();
        }
    }

    private IEnumerator EndLevel()
    {
        animator.SetTrigger("celebrate");
        yield return new WaitForSeconds(3f);

        if (gameSceneManager.currentScene.name == "LevelOne")
        {
            changeToScene.LoadSceneLevelTwo();
        }

        if (gameSceneManager.currentScene.name == "LevelTwo")
        {
            changeToScene.LoadSceneMainMenu();
        }
    }

    private void UIHandler()
    {
        scoreText.text = "Score: " + currentScore;
        healthText.text = "Health: " + currentHealth;
    }

    private void Update()
    {
        CalculateAnimations();
        CalculateVerticalSpeed();
        MovePlayer();
        UIHandler();
        HideTips();
    }
}
