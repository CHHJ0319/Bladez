using UnityEngine;

namespace Actor
{
    public class CharacterAnimator : MonoBehaviour
    {
        public float animSpeed = 1.5f;

        [Header("SoundList")]
        public AudioClip smileSound;
        public AudioClip startSound;
        public AudioClip jumpSound;
        public AudioClip slidingSound;
        public AudioClip winSound;

        private Animator anim;
        private AudioSource audioSource;

        void Start()
        {
            anim = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();

        }

        public void UpdateMovementAnimation(float horizontal, float vertical)
        {
            anim.SetFloat("Speed", vertical);
            anim.SetFloat("Direction", horizontal);

            anim.speed = animSpeed;
        }

        public AnimatorStateInfo GetBaseLayerState()
        {
            return anim.GetCurrentAnimatorStateInfo(0);
        }

        public AnimatorStateInfo GetUpperBodyState()
        {
            return anim.GetCurrentAnimatorStateInfo(2);
        }
       

        public bool IsTransitioning()
        {
            return anim.IsInTransition(0);
        }

        public void SetJump(bool isJump)
        {
            anim.SetBool("Jump", isJump);
            if (isJump)
            {
                //audioSource.PlayOneShot(jumpSound);
            }
        }

        public void SetSliding(bool isSliding)
        {
            anim.SetBool("Sliding", isSliding);
            if(isSliding)
            {
                //audioSource.PlayOneShot(slidingSound);

            }
        }

        public void PlayAttack()
        {
            anim.SetTrigger("Slash");
        }

        public void PlayTakeDamage()
        {
            anim.SetTrigger("Hit");
        }

        public float GetJumpHeight()
        {
            return anim.GetFloat("JumpHeight");
        }

        public float GetGravityControl()
        {
            return anim.GetFloat("GravityControl");
        }
    }
}
