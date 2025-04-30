using UnityEngine;
using UnityEngine.InputSystem;

namespace Invector.vCharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region Variables       

        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;
        public KeyCode attackInput = KeyCode.Mouse0;
        public KeyCode abilityInput = KeyCode.F;

        [Header("Camera Input")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        [HideInInspector] public vThirdPersonController cc;
        [HideInInspector] public vThirdPersonCamera tpCamera;
        [HideInInspector] public Camera cameraMain;

        Attack sword;

        #endregion
        #region Variables Controller


        private InputSystem_Actions inputActions;

        Vector2 moveInput;
        Vector2 lookInput;
        bool jumpPressed;
        bool sprintHeld;
        bool strafePressed;
        bool attackPressed;
        bool abilityPressed;

        #endregion

        protected virtual void Start()
        {
            if (GameObject.FindWithTag("Sword") != null)
            {
                sword = GameObject.FindWithTag("Sword").GetComponent<Attack>();
                sword.col.enabled = false;
            }
            else
                Debug.LogWarning("Tag the sword with 'Sword'!");

            InitilizeController();
            InitializeTpCamera();
        }

        void Awake()
        {
            inputActions = new InputSystem_Actions();
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            cc.ControlRotationType();       // handle the controller rotation type
        }
        void OnEnable()
        {
            inputActions.Player.Enable();
            inputActions.Player.Jump.performed += ctx => jumpPressed = true;
            inputActions.Player.Attack.performed += ctx => attackPressed = true;
            inputActions.Player.Sprint.performed += ctx => cc.Sprint(true);
            inputActions.Player.Sprint.canceled += ctx => cc.Sprint(false);
            inputActions.Player.Crouch.performed += ctx => abilityPressed = true;
        }
        void OnDisable()
        {
            inputActions.Player.Disable();
        }
        protected virtual void Update()
        {
            cc.UpdateAnimator();            // updates the Animator Parameters

            // Handle both controller and keyboard/mouse inputs
            moveInput = inputActions.Player.Move.ReadValue<Vector2>();
            lookInput = inputActions.Player.Look.ReadValue<Vector2>();

            // Keyboard/Mouse inputs
            if (Input.GetKeyDown(jumpInput) && JumpConditions())
            {
                cc.Jump();
            }

            if (Input.GetKeyDown(attackInput) && !Input.GetMouseButton(1))
            {
                AttackInput();
            }

            if (Input.GetKeyDown(strafeInput))
            {
                StrafeInput();
            }

            // Handle controller inputs
            if (jumpPressed && JumpConditions())
            {
                cc.Jump();
                jumpPressed = false;
            }

            if (attackPressed)
            {
                AttackInput();
                attackPressed = false;
            }

            if (strafePressed)
            {
                StrafeInput();
                strafePressed = false;
            }
            if (abilityPressed)
            {
                abilityPressed = false;
            }

            // Common input handling
            MoveInput();
            CameraInput();
        }

        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<vThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle()
        {
            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
            JumpInput();
            AttackInput();
        }

        public virtual void AttackInput()
        {
            if (!Input.GetMouseButton(1))
            {
                sword.col.enabled = true;
                cc.isAttacking = true;
                Invoke(nameof(AttackEnd), 1.2f);

            }
        }

        

        public virtual void AttackEnd()
        {
            sword.col.enabled = false;
            cc.isAttacking = false;
            Debug.Log("Animation ended. Set isAttacking to " + cc.isAttacking);
        }

        public virtual void MoveInput()
        {
            cc.input.x = Input.GetAxis(horizontalInput);
            cc.input.z = Input.GetAxis(verticallInput);
            cc.input.x = moveInput.x;
            cc.input.z = moveInput.y;
        }

        protected virtual void CameraInput()
        {
            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;
            if (tpCamera != null)
                tpCamera.RotateCamera(lookInput.x, lookInput.y);


            var Y = Input.GetAxis(rotateCameraYInput);
            var X = Input.GetAxis(rotateCameraXInput);

            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
            if (Input.GetKeyDown(strafeInput))
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            // Sprint is now handled through input actions in OnEnable
            if (Input.GetKeyDown(sprintInput))
                cc.Sprint(true);
            else if (Input.GetKeyUp(sprintInput))
                cc.Sprint(false);
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            // Jump is now handled in Update for both input methods
            if (JumpConditions())
                cc.Jump();
        }

        #endregion       
    }
}