using UnityEngine;

namespace Actor.Player
{
    public static class PlayerState
    {
        public static int IdleState = Animator.StringToHash("Base Layer.Idle");
        public static int LocoState = Animator.StringToHash("Base Layer.Locomotion");
        public static int JumpState = Animator.StringToHash("Base Layer.Jump");
        public static int SlidingState = Animator.StringToHash("Base Layer.Sliding");
        public static int TakeDamageState = Animator.StringToHash("Base Layer.TakeDamage");
        public static int RestState = Animator.StringToHash("Base Layer.Rest");

        public static int AttackState1 = Animator.StringToHash("Upperbody Layer.OneHandSwordCombo(1)");
        public static int AttackState2 = Animator.StringToHash("Upperbody Layer.OneHandSwordCombo(2)");
    }
}

