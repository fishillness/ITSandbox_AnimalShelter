using System;
using UnityEngine;

public class SpecifierRequiredBooster : MonoBehaviour
{
    [Serializable, SerializeField]
    public struct RequiredBoosterPosition
    {
        public BoosterType type;
        public Vector2Int position;
    }

    [SerializeField] private RequiredBoosterPosition[] requiredBoosters;

    public RequiredBoosterPosition[] RequiredBoosters => requiredBoosters;
}
