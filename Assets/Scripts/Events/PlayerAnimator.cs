using UnityEngine;

namespace Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        public float animSpeed = 1.5f;

        private Animator anim;

        void Start()
        {
            anim = GetComponent<Animator>();

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

        public void PlayWalking(bool isWalkin)
        {
            anim.SetBool("IsWalking", isWalkin);
        }

        public void PlayJump()
        {
            anim.SetBool("Jump", true);
        }

        public void PlaySliding()
        {
            anim.SetBool("Sliding", true);
        }

        public void PlayFire()
        {
            anim.SetTrigger("Shot");
        }

        public void PlayReload()
        {
            anim.SetBool("Reload", true);
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
