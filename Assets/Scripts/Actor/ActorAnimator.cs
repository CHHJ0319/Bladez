using UnityEngine;

namespace Actor
{
    public class ActorAnimator : MonoBehaviour
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

        public float GetAnimationDistance()
        {
            return anim.deltaPosition.magnitude;
        }

        public float GetVerticalDelta()
        {
            return anim.deltaPosition.y;
        }

        public bool IsTransitioning()
        {
            return anim.IsInTransition(0);
        }

        public void PlayMoving(bool isMoving)
        {
            anim.SetBool("IsMoving", isMoving);
        }

        public void PlayJump()
        {
            anim.SetBool("Jump", true);
            audioSource.PlayOneShot(jumpSound);
        }

        public void PlaySliding()
        {
            anim.SetBool("Sliding", true);
            audioSource.PlayOneShot(slidingSound);
        }

        public void PlayShot()
        {
            anim.SetTrigger("Shot");
        }

        public void PlaySlash()
        {
            anim.SetTrigger("Slash");
        }

        public void PlayImpact()
        {
            anim.SetTrigger("Hit");
        }

        public void PlayInteract()
        {
            anim.SetTrigger("Act");
        }

        public void PlayReload()
        {
            anim.SetBool("Reload", true);
            audioSource.Play();
        }

        public float GetJumpHeight()
        {
            return anim.GetFloat("JumpHeight");
        }

        public float GetGravityControlt()
        {
            return anim.GetFloat("GravityControl");
        }

        public void StopJump()
        {
            anim.SetBool("Jump", false);
        }

        public void StopSliding()
        {
            anim.SetBool("Sliding", false);
        }

        public void StopReload()
        {
            anim.SetBool("Reload", false);
        }
    }
}
