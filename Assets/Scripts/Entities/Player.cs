using AbstractGameEntities;
using MovableEntities;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class Player : GameEntity<PlayerStats, PlayerTemplate>
    {
        public static Player instance { get; private set; }

        public GameObject pauseMenu;
        public GameObject overlay;

        private PlayerMovement playerMovement;
        private PlayerGameplay playerGameplay;
        private PlayerCamera playerCamera;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }

            attributes = new PlayerTemplate();
            playerCamera = new PlayerCamera(transform.parent.GetComponentInChildren<Camera>(),
                                            GetComponent<PlayerInput>());
            playerMovement = new PlayerMovement(GetComponentInParent<CharacterController>(),
                                                GetComponent<PlayerInput>(),
                                                transform.GetChild(0).GetComponent<GroundColliderCheck>(),
                                                attributes,
                                                playerCamera);
            playerGameplay = new PlayerGameplay(GetComponent<PlayerInput>(),
                                                playerCamera,
                                                pauseMenu,
                                                overlay);
        }

        private void Update()
        {
            playerMovement.CheckMovementAmount();
            playerCamera.MoveCamera();
            playerGameplay.UpdateInteractable();
        }

        private void FixedUpdate()
        {
            playerMovement.MovePlayer();
        }

        public override T GetStat<T>(PlayerStats playerStats)
        {
            throw new NotImplementedException();
        }

        public void UpdateSensitivity(float scale)
        {
            playerCamera.UpdateCameraSensitivity(scale);
        }
    }

    public class PlayerMovement
    {
        private CharacterController characterController;
        private PlayerInput playerInput;
        private GroundColliderCheck groundCheck;
        private PlayerTemplate playerAttributes;
        private PlayerCamera camera;

        private InputAction walkAction;
        private InputAction runAction;

        public Vector2 moveInpAmt {set; private get;}
        private Vector3 desiredMoveVect;

        public bool affectedByGravity { get; private set; }
        public int currJumpCount { get; private set; }

        private const float moveAccelAmt = 0.3f;
        private const float jumpHeightScale = 0.08f;
        private const float gravityMultiplier = 4f;

        public PlayerMovement(CharacterController c, PlayerInput i, GroundColliderCheck gc, PlayerTemplate a, PlayerCamera cam)
        {
            characterController = c;
            playerInput = i;
            groundCheck = gc;
            playerAttributes = a;
            camera = cam;

            affectedByGravity = true;
            currJumpCount = 0;

            walkAction = playerInput.currentActionMap.FindAction("Walk");
            playerInput.currentActionMap.FindAction("Jump").performed += OnJump;
            runAction = playerInput.currentActionMap.FindAction("Run");

            groundCheck.onGroundHit += ResetJump;
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (currJumpCount < playerAttributes.jumpCountTotal)
            {
                desiredMoveVect.y = playerAttributes.jumpHeight * jumpHeightScale;
                currJumpCount++;
            }
            //Debug.Log("Boing");
        }

        private void ResetJump()
        {
            desiredMoveVect.y = 0;
            currJumpCount = 0;
        }

        public void CheckMovementAmount()
        {
            moveInpAmt = walkAction.ReadValue<Vector2>();
        }

        public void MovePlayer()
        {
            float movX = CalculateHorizontalMovement(desiredMoveVect.x, moveInpAmt.x, runAction.IsPressed());
            float movY = CalculateVerticalMovement(desiredMoveVect.y);
            float movZ = CalculateHorizontalMovement(desiredMoveVect.z, moveInpAmt.y, runAction.IsPressed());
            desiredMoveVect = new Vector3(movX, movY, movZ);

            //Debug.DrawRay(camera.playerModel.transform.position, new Vector3(movX, 0, movZ) * 100, Color.blue);
            Vector3 adjustedMovement = movX * camera.playerModel.transform.right + movZ * camera.playerModel.transform.forward;
            adjustedMovement.y = movY;
            //Debug.DrawRay(camera.playerModel.transform.position, adjustedMovement * 100, Color.green);

            characterController.Move(adjustedMovement);
        }

        private float CalculateHorizontalMovement(float currMoveAmt, float desiredMoveAmt, bool sprinting)
        {
            return Mathf.Lerp(currMoveAmt, desiredMoveAmt * (playerAttributes.baseMaxMovementSpeed) * (sprinting ? playerAttributes.sprintSpeed : 1) * Time.deltaTime, moveAccelAmt);
        }

        private float CalculateVerticalMovement(float currYVel)
        {
            float movY = currYVel;

            if (!groundCheck.isGrounded && affectedByGravity)
            {
                movY = movY + (Physics.gravity.y * gravityMultiplier * Time.deltaTime * Time.deltaTime);
            }
            
            return movY;
        }
    }

    public class PlayerGameplay
    {
        private PlayerInput playerInput;
        private PlayerCamera camera;
        private GameObject pauseMenu;
        private GameObject overlay;

        private Interactable currInteractable;

        private int layerMask;

        public bool currPaused { private set; get; }

        public PlayerGameplay(PlayerInput i, PlayerCamera cam, GameObject pauseMenu, GameObject overlay)
        {
            playerInput = i;
            camera = cam;
            currPaused = false;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            playerInput.currentActionMap.FindAction("Pause").performed += OnPause;
            playerInput.currentActionMap.FindAction("Interact").performed += OnInteract;

            this.pauseMenu = pauseMenu;
            this.overlay = overlay;

            layerMask = 1 << 2;
            layerMask = ~layerMask;
        }

        public void UpdateInteractable()
        {
            RaycastHit hit;

            Debug.DrawRay(camera.playerCam.transform.position, camera.playerCam.transform.forward * 3, Color.red);
            if (Physics.Raycast(camera.playerCam.transform.position, camera.playerCam.transform.forward, out hit, 3f, layerMask))
            {
                Debug.DrawRay(camera.playerCam.transform.position, camera.playerCam.transform.forward * 3, Color.blue);
                Debug.Log(hit.collider.tag);
                if (hit.collider.tag == "Interactable")
                {
                    overlay.SetActive(true);
                    currInteractable = hit.collider.gameObject.GetComponent<Interactable>();
                }
                else
                {
                    overlay.SetActive(false);
                    currInteractable = null;
                }
            }
            else
            {
                overlay.SetActive(false);
                currInteractable = null;
            }
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            if (currInteractable != null)
            {
                currInteractable.OnInteract();
            }
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            currPaused = !currPaused;

            if (currPaused == true)
            {
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else if (currPaused == false)
            {
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public class PlayerCamera
    {
        public Camera playerCam { get; private set; }
        public Transform playerModel { get; private set; }

        public float cameraRate;

        private InputAction cameraMoveAction;

        private Vector2 cameraRotationAmount;

        private const float mouseSensitivityScaler = 20f;

        public PlayerCamera(Camera playerCam, PlayerInput playerInput)
        {
            this.playerCam = playerCam;

            cameraMoveAction = playerInput.currentActionMap.FindAction("LookAround");
            cameraMoveAction.performed += OnCameraRotate;

            cameraRate = 1f;

            playerModel = playerCam.transform.parent.transform;
        }

        public void MoveCamera()
        {
            //Debug.DrawRay(playerModel.position, playerModel.forward, Color.yellow);
            //Debug.DrawRay(playerModel.position, playerModel.right, Color.yellow);

            //Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward, Color.yellow);
            //Debug.DrawRay(playerCam.transform.position, playerCam.transform.right, Color.yellow);

            playerCam.transform.rotation = Quaternion.Euler(new Vector3(cameraRotationAmount.x, cameraRotationAmount.y, 0));
            playerModel.transform.rotation = Quaternion.Euler(new Vector3(0, cameraRotationAmount.y, 0));
        }

        public void UpdateCameraSensitivity(float scale)
        {
            cameraRate = scale;
        }

        public void OnCameraRotate(InputAction.CallbackContext context)
        {
            Vector2 lookValue = cameraMoveAction.ReadValue<Vector2>();

            cameraRotationAmount.x += -lookValue.y * cameraRate * mouseSensitivityScaler * Time.deltaTime;
            cameraRotationAmount.y += lookValue.x * cameraRate * mouseSensitivityScaler * Time.deltaTime;

            cameraRotationAmount.x = Mathf.Clamp(cameraRotationAmount.x, -60, 60);
        }
    }

    [Serializable]
    public class PlayerTemplate : MovingEntityTemplate
    {
        public float jumpHeight;
        public float sprintSpeed;
        public int jumpCountTotal;

        public PlayerTemplate() : base()
        {
            jumpHeight = 3f;
            sprintSpeed = 2f;
            jumpCountTotal = 2;
        }
    }

    public enum PlayerStats
    {
        currHealth,
        baseMaxHealth,
        takesDamage,
        currMovementSpeed,
        baseMaxMovementSpeed,
        sprintSpeed,
        affectedByGravity,
        baseJumpHeight,
        currJumpCount,
        baseJumpCountTotal,
    }
}
