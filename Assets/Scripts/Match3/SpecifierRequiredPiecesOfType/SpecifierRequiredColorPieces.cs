using System;
using UnityEngine;

public class SpecifierRequiredColorPieces : MonoBehaviour
{
    [Serializable, SerializeField]
    public struct RequiredColorPiecesPosition
    {
        public ColorType colorType;
        public Vector2Int position;
    }

    [SerializeField] private RequiredColorPiecesPosition[] requiredColorPieces;

    public RequiredColorPiecesPosition[] RequiredColorPieces => requiredColorPieces;
}
