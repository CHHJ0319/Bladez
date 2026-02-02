using UnityEngine;

namespace Actor.Player
{
    public static class PlayerState
    {
        public static int IdleState = Animator.StringToHash("Base Layer.Idle");
        public static int LocoState = Animator.StringToHash("Base Layer.Locomotion");
        public static int JumpState = Animator.StringToHash("Base Layer.Jump");
        public static int restState = Animator.StringToHash("Base Layer.Rest");

        public static int SlidingState = Animator.StringToHash("Base Layer.Sliding");

        public static int ReloadingState = Animator.StringToHash("UpperBody.Reloading");
    }
}

