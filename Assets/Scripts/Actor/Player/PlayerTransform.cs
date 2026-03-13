using Unity.Netcode.Components;
using UnityEngine;

namespace Actor.Player 
{
    public class PlayerTransform : NetworkTransform
    {
        public Vector2 MoveInput { get; set; }
        
    }
}