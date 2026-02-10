using UnityEngine;

namespace Actor.Item
{
    public class ItemController : MonoBehaviour
    {
        public enum Type { Ammo, Coin, Grenade, Heart, Weapon }
        public Type type;
        public int value;
    }
}