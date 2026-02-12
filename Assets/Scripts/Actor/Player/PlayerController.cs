using Actor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Actor.Player
{
    public class PlayerController : CharaterController
    {
        [Header("UI")]
        public GameObject hpBarPrefab;
        public GameObject staminaBarPrefab;

        private PlayerInputHandler playerInputHandler;
        
        private Transform playerUI;

        protected override void Awake()
        {
            base.Awake();

            playerInputHandler = GetComponent<PlayerInputHandler>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            float h = playerInputHandler.Horizontal;
            float v = playerInputHandler.Vertical;    

            Move(h, v);
            JumpWithPlayerInput();
            SlidingWithPlayerInput();
            AttackWithPlayerInput();
			InteractWithPlayerInput();
        }

        public void CreatePlayerUI()
        {
            if (SceneManager.GetActiveScene().name == "DuelLobbyScene")
            {
                playerUI = DualLobbyScene.UIManager.Instance.PlayerUI;
            }
            else if (SceneManager.GetActiveScene().name == "TestScene")
            {
                playerUI = UIManager.Instance.PlayerUI;
                hpBar = Instantiate(hpBarPrefab, playerUI).GetComponent<GaugeBar>();
                staminaBar = Instantiate(staminaBarPrefab, playerUI).GetComponent<GaugeBar>();

                hpBar.UpdateGaugeBar(hp, maxHP);
            }
                
        }

        void CalculateVelocity(float vertical)
        {
            velocity = new Vector3(0, 0, vertical);
            velocity = transform.TransformDirection(velocity);

            if (vertical > 0.1f)
            {
                velocity *= forwardSpeed;
            }
            else if (vertical < -0.1f)
            {
                velocity *= backwardSpeed;
            }
        }

        void JumpWithPlayerInput()
        {
            if (playerInputHandler.JumpTriggered)
            {
                NetworkCharacterHandler.SubmitJumpRequestServerRpc();
                Jump();
            }
        }

        void SlidingWithPlayerInput()
        {
            if (playerInputHandler.SlidingTriggered)
            {
                NetworkCharacterHandler.SubmitslidingRequestServerRpc();
                Sliding();
            }
        }

        void AttackWithPlayerInput()
        {
            if (playerInputHandler.AttackTriggered)
            {
                NetworkCharacterHandler.SubmitAttackRequestServerRpc();
                Attack();
            }
        }

        protected void InteractWithPlayerInput()
        {
            if (playerInputHandler.InteractTriggered)
            {
                if(SceneManager.GetActiveScene().name == "DuelLobbyScene")
                {
                    Rest();
				}
                else
                {
					PickUp();
				}
			}
        }

        public void Move(float horizontal, float vertical)
        {
            NetworkCharacterHandler.SubmitTransfromRequestServerRpc(transform.localPosition, transform.localRotation);

            networkCharacterAnimator.UpdateMovementAnimation(horizontal, vertical);

            CalculateVelocity(vertical);
            if(SceneManager.GetActiveScene().name != "DuelLobbyScene")
            {
                transform.localPosition += velocity * Time.fixedDeltaTime;
            }
            transform.Rotate(0, horizontal * rotateSpeed, 0);

        }
    }
}


