using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoxAbility : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    private CharacterController characterController;
    private AudioSource audioSource;
    public enum BoxState { Empty, Hold, Place, Dash }
    public BoxState boxState;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform playerTransformHold;
    [SerializeField] private Transform playerTransformPlace;
    public GameObject box;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        boxState = BoxState.Empty;
    }

    private IEnumerator CreateBox()
    {
        Destroy(box);
        box = Instantiate(boxPrefab, playerTransformHold.position, playerTransformHold.rotation, playerTransformHold);
        yield return new WaitForSeconds(0.5f);

        if (boxState == BoxState.Empty)
        {
            boxState = BoxState.Hold;
        }

        else
        {
            boxState = BoxState.Hold;
        }
    }

    private IEnumerator PlacingBox()
    {
        Destroy(box);
        boxState = BoxState.Place;
        box = Instantiate(boxPrefab, playerTransformPlace.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);

        if (boxState == BoxState.Place)
        {
            boxState = BoxState.Empty;
        }
        
        else
        {
            boxState = BoxState.Hold;
        }
    }

    private IEnumerator DashBox()
    {
        Destroy(box);
        boxState = BoxState.Dash;
        box = Instantiate(boxPrefab, playerTransformPlace.position, playerTransformPlace.rotation, playerTransformPlace);
        playerMovement.isInvincible = true;
        playerMovement.speed = 12f;
        audioSource.clip = playerMovement.dashSound;
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        playerMovement.speed = 6f;
        playerMovement.isInvincible = false;

        if (boxState == BoxState.Dash)
        {
            Destroy(box);
            box = Instantiate(boxPrefab, playerTransformHold.position, playerTransformHold.rotation, playerTransformHold);
            boxState = BoxState.Hold;
        }

        else
        {
            boxState = BoxState.Empty;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (boxState == BoxState.Dash)
        {
            if (collider.gameObject.tag == "Enemy")
            {
                StopCoroutine(DashBox());
                Destroy(collider.gameObject);
                audioSource.clip = playerMovement.enemyDeathSound;
                audioSource.Play();
                Destroy(box);
                playerMovement.speed = 6f;
                boxState = BoxState.Empty;
            }
        }
    }

    private void Update()
    {
        if (characterController.isGrounded && boxState == BoxState.Empty)
        {
            if (playerInput.spawnBoxInput)
            {
                StartCoroutine(CreateBox());
            }
        }

        if (characterController.isGrounded && boxState == BoxState.Hold)
        {
            if (playerInput.spawnBoxInput)
            {
                StartCoroutine(PlacingBox());
            }
        }

        if (boxState == BoxState.Hold)
        {
            if (playerInput.dashBoxInput)
            {
                StartCoroutine(DashBox());
            }
        }
    }
}