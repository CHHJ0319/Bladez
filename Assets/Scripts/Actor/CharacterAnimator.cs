using System.Collections;
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

        private int _comboCount = 0;
        private bool _canCombo = false;

        void Awake()
        {
            anim = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        public void UpdateMovementAnimation(float horizontal, float vertical)
        {
            anim.SetFloat("Speed", vertical);
            anim.SetFloat("Direction", horizontal);

            //Debug.Log("h : " + horizontal + "v :" + vertical);

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
            if (_comboCount == 0)
            {
                StartCoroutine(AttackProcess());
            }
            else if (_canCombo && _comboCount < 2)
            {
                _canCombo = false;
                _comboCount++;
            }
        }

        public void PlayTakeDamage()
        {
            anim.SetTrigger("TakeDamage");
        }

        public float GetJumpHeight()
        {
            return anim.GetFloat("JumpHeight");
        }

        public float GetGravityControl()
        {
            return anim.GetFloat("GravityControl");
        }

        IEnumerator AttackProcess()
        {
            _comboCount = 1;

            while (_comboCount <= 2)
            {
                int currentStep = _comboCount;

                anim.SetInteger("ComboCount", currentStep);
                anim.SetTrigger("Attack");

                _canCombo = true;
                yield return new WaitForSeconds(0.4f);

                if (_comboCount == currentStep) break;
            }

            _comboCount = 0;
            _canCombo = false;
        }
    }
}
