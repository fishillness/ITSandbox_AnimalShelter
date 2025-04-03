using System;
using UnityEngine;

public class SpecifierRequiredPiecesOfType : MonoBehaviour
{
    [Serializable, SerializeField]
    public struct RequiredPiecePosition
    {
        public PieceType type;
        public Vector2Int position;
    }

    [SerializeField] private RequiredPiecePosition[] requiredPieces;

    public RequiredPiecePosition[] RequiredPieces => requiredPieces;
}