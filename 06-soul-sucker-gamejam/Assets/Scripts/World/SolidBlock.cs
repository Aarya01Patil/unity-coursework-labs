using SoulSucker.Data;
using UnityEngine;

namespace SoulSucker.World
{
    public sealed class SolidBlock : MonoBehaviour
    {
        [SerializeField] private IntRect bounds;

        public IntRect Bounds => bounds;

        public void Initialize(IntRect rect)
        {
            bounds = rect;
        }
    }
}
