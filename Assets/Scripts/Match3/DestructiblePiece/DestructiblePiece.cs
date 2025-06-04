using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DestructiblePiece : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<Piece> OnPieceDestroy;
    [HideInInspector]
    public UnityEvent<Piece> OnPieceStartDestroying;

    [SerializeField] private DestructionType[] destructionTypeWhichDestroyPiece;
    private Animator animator;
    private Piece piece;
    protected bool isDestroying = false;
    private string destroyTriggerName = "Destroy";
    protected bool isLastStage = true;

    public bool IsDestroying => isDestroying;
    public bool IsLastStage => isLastStage;

    protected virtual void Awake()
    {
        piece = GetComponent<Piece>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public virtual void DamagePiece(DestructionType type)
    {
        if (!IsPieceDestroyThisType(type)) return;
        if (isDestroying) return;

        DestroyPiece();
    }

    protected void DestroyPiece()
    {
        if (isDestroying) return;

        animator.enabled = true;
        isDestroying = true;
        OnPieceStartDestroying?.Invoke(piece);
        animator.SetTrigger(destroyTriggerName);
    }

    public bool IsPieceDestroyThisType(DestructionType type)
    {
        return destructionTypeWhichDestroyPiece.Contains(type);
    }

    public void OnDestroyAnimationEnd()
    {
        OnPieceDestroy?.Invoke(piece);
        //Destroy(piece.gameObject); 
    }

    public void DestroyImmediately()
    {
        isDestroying = true;
        OnDestroyAnimationEnd();
    }
}
