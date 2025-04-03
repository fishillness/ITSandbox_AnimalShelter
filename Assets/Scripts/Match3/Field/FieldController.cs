using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FieldController : WorkingWithGrid,
    IDependency<PiecesSpawnerController>, IDependency<PieceCounter>,
    IDependency<Match3Level>, IDependency<PieceMatrixController>
{
    [HideInInspector] 
    public UnityEvent OnMove;
    [HideInInspector]
    public UnityEvent OnDropEnd;

    [SerializeField] private PiecePrefab[] piecePrefabs;
    [SerializeField] private float droppingTime;
    [SerializeField] private float movingTime;

    private PiecesSpawnerController spawnerController;
    private PieceCounter pieceCounter; //
    private Match3Level level;
    private PieceMatrixController matrixController;

    #region Constructs
    public void Construct(PiecesSpawnerController spawnerController) => this.spawnerController = spawnerController;
    public void Construct(PieceCounter pieceCounter) => this.pieceCounter = pieceCounter;
    public void Construct(Match3Level level) => this.level = level;
    public void Construct(PieceMatrixController matrixController) => this.matrixController = matrixController;
    #endregion

    [Serializable, SerializeField]
    public struct PiecePrefab
    {
        public PieceType type;
        public Piece prefab;
    }

    public struct MatchingPieces
    {
        public List<Piece> matchPieces;
        public BoosterType boosterType;
    }

    private Dictionary<PieceType, Piece> piecePrefabDict;
    private bool isLevelEnd = true;
    private bool areMovesAllowed = true;
    private bool continueDropping;
    private bool isStartFillingField = true;
    
    public bool IsDroppingContinue => continueDropping;
    public Dictionary<PieceType, Piece>  PiecePrefabDict => piecePrefabDict;
    public float DroppingTime => droppingTime;  
    public float MovingTime => movingTime;
    public bool IsLevelEnd => isLevelEnd;
    public bool AreMovesAllowed => areMovesAllowed;
    public bool IsStartFillingField => isStartFillingField;

    private void Awake()
    {
        piecePrefabDict = new Dictionary<PieceType, Piece>();
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
        pieceCounter.InitDictionery(piecePrefabs);

        InitTilemapsGrid();
        SetBounds();

        matrixController.InitMatrix(xDim, yDim);
    }

    private void Start()
    {
        spawnerController.InitSpawners();
        spawnerController.CheckNeedOfSpawnPiece();
        StartDropPieces(droppingTime);
        level.OnStopMoves.AddListener(OnLevelEnd);
    }

    private void OnDestroy()
    {
        level.OnStopMoves.RemoveListener(OnLevelEnd);
    }

    private void OnLevelEnd()
    {
        isLevelEnd = false;
        areMovesAllowed = false;
    }

    public void StartDropPieces(float time)
    {
        if (IsDroppingContinue) return;
        if (matrixController.IsThereActiveBooster) return;
            
        StartCoroutine(DroppingPieces(time));
    }

    private IEnumerator DroppingPieces(float time)
    {
        continueDropping = true;
        areMovesAllowed = false;
        bool immediately = time == 0;

        while (continueDropping)
        {
            yield return new WaitForSeconds(time);

            while (DropPieces(immediately))
            {
                yield return new WaitForSeconds(time);

                if (spawnerController.CheckNeedOfSpawnPiece())
                    continueDropping = true;
            }

            continueDropping = ClearAllMatches(time);

            if (spawnerController.CheckNeedOfSpawnPiece())
                continueDropping = true;
        }

        areMovesAllowed = true;
        OnDropEnd?.Invoke();

        if (immediately)
            isStartFillingField = false;
    }

    private bool DropPieces(bool immediately)
    {
        bool isPieceDrop = false;

        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (matrixController.Pieces[x, y] == null)
                    continue;

                if (matrixController.Pieces[x, y].IsMovable)
                {
                    Piece pieceBelow = matrixController.Pieces[x, y + 1];

                    if (pieceBelow != null)
                    {
                        if (pieceBelow.Type == PieceType.Empty)
                        {
                            matrixController.SwapEmptyPieceWithNonEmpty(x, y + 1, x, y, immediately);
                            //spawnerController.CheckNeedOfSpawnPieceAfterTime(droppingTime);
                            isPieceDrop = true;
                        }
                    }
                    
                    if (isPieceDrop == false)
                    {
                        Piece pieceBelowRight = null;
                        Piece pieceBelowLeft = null;
                        Piece piece = null;

                        if (x + 1 < xDim && matrixController.Pieces[x + 1, y + 1] != null
                            && matrixController.Pieces[x + 1, y + 1].Type == PieceType.Empty)
                        {
                            pieceBelowRight = matrixController.Pieces[x + 1, y + 1];

                            if (matrixController.Pieces[x + 1, y] != null &&
                                matrixController.Pieces[x + 1, y].IsMovable)
                                pieceBelowRight = null;
                        }

                        if (x - 1 >= 0 && matrixController.Pieces[x - 1, y + 1] != null
                            && matrixController.Pieces[x - 1, y + 1].Type == PieceType.Empty)
                        {
                            pieceBelowLeft = matrixController.Pieces[x - 1, y + 1];

                            if (matrixController.Pieces[x - 1, y] != null &&
                                matrixController.Pieces[x - 1, y].IsMovable)
                                pieceBelowLeft = null;
                        }

                        if (pieceBelowRight != null && pieceBelowLeft == null)
                            piece = pieceBelowRight;
                        else if (pieceBelowLeft != null && pieceBelowRight == null)
                            piece = pieceBelowLeft;
                        else if (pieceBelowLeft != null && pieceBelowRight != null)
                        {
                            int random = UnityEngine.Random.Range(0, 2);

                            if (random == 0)
                                piece = pieceBelowRight;
                            else 
                                piece = pieceBelowLeft;
                        }

                        if (piece != null)
                        {
                            matrixController.SwapEmptyPieceWithNonEmpty(piece.X, piece.Y, x, y, immediately);
                            //spawnerController.CheckNeedOfSpawnPieceAfterTime(droppingTime);
                            isPieceDrop = true;
                        }
                    }
                }
            }
        }

        return isPieceDrop;
    }

    private MatchingPieces FindMatch(Piece piece)
    {
        MatchingPieces matching = new MatchingPieces();
        matching.boosterType = BoosterType.None;
        if (piece == null)
        {
            matching.matchPieces = null;
            return matching;
        }
        if (piece.IsColorable == false)
        {
            matching.matchPieces = null;
            return matching;
        }

        ColorType color = piece.Colorable.Color;
        List<Piece> horizontelPieces = new List<Piece>();
        List<Piece> verticalPieces = new List<Piece>();
        List<Piece> matchingPieces = new List<Piece>();

        for (int x = piece.X + 1; x < xDim; x++)
        {
            Piece horPiece = matrixController.Pieces[x, piece.Y];
            if (horPiece == null)
                break;
            if (!horPiece.IsColorable)
                break;
            if (horPiece == piece)
                continue;

            if (horPiece.Colorable.Color == color)
                horizontelPieces.Add(horPiece);
            else
                break;
        }

        for (int x = piece.X - 1; x >= 0; x--)
        {
            Piece horPiece = matrixController.Pieces[x, piece.Y];
            if (horPiece == null)
                break;
            if (!horPiece.IsColorable)
                break;
            if (horPiece == piece)
                continue;

            if (horPiece.Colorable.Color == color)
                horizontelPieces.Add(horPiece);
            else
                break;
        }

        for (int y = piece.Y + 1; y < yDim; y++)
        {
            Piece verPiece = matrixController.Pieces[piece.X, y];
            if (verPiece == null)
                break;
            if (!verPiece.IsColorable)
                break;
            if (verPiece == piece)
                continue;

            if (verPiece.Colorable.Color == color)
                verticalPieces.Add(verPiece);
            else
                break;
        }

        for (int y = piece.Y - 1; y >= 0; y--)
        {
            Piece verPiece = matrixController.Pieces[piece.X, y];
            if (verPiece == null)
                break;

            if (!verPiece.IsColorable)
                break;
            if (verPiece == piece)
                continue;

            if (verPiece.Colorable.Color == color)
                verticalPieces.Add(verPiece);
            else
                break;
        }

        matchingPieces.Add(piece);

        matching.boosterType = IdentifyBooster(piece, ref horizontelPieces, verticalPieces);

        if (matching.boosterType != BoosterType.MiniBomb)
        {
            if (horizontelPieces.Count < 2)
                horizontelPieces.Clear();

            if (verticalPieces.Count < 2)
                verticalPieces.Clear();
        }


        foreach (Piece horPiece in horizontelPieces)
        {
            matchingPieces.Add(horPiece);
        }

        foreach (Piece verPiece in verticalPieces)
        {
            matchingPieces.Add(verPiece);
        }


        if (matchingPieces.Count < 3)
            matchingPieces.Clear();

        matching.matchPieces = matchingPieces;
        return matching;
    }

    private bool ClearAllMatches(float time)
    {
        bool isMatchFound = false;
        bool deleteImmediately = time == 0;

        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (matrixController.Pieces[x, y] == null)
                    continue;

                if (!matrixController.Pieces[x, y].IsMovable)
                    continue;

                MatchingPieces matchPieces = new MatchingPieces();
                matchPieces = FindMatch(matrixController.Pieces[x, y]);

                if (matchPieces.matchPieces != null
                    && matchPieces.matchPieces.Count > 0)
                {
                    matrixController.DeleteSomePieces(matchPieces.matchPieces, DestructionType.ByMatch, deleteImmediately);

                    isMatchFound = true;
                }

                if (matchPieces.boosterType != BoosterType.None)
                {
                    matrixController.SpawnNewBooster(matchPieces.matchPieces[0].X, matchPieces.matchPieces[0].Y, matchPieces.boosterType);
                }
            }
        }

        return isMatchFound;
    }

    public bool IsAdjacent(Piece piece1, Piece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1)
            || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    public bool SwapPieces(Piece piece1, Piece piece2)
    {
        if (!isLevelEnd) return false;
        if (!areMovesAllowed) return false;

        if (piece1 == null || piece2 == null)
        {
            Debug.Log($"Кто-то нуловьй. piece1: {piece1}, piece2: {piece2}");
            return false;
        }
        if (piece1.IsMovable == false || piece2.IsMovable == false)
        {
            Debug.Log($"Кто-то не мувобал. piece1: {piece1.IsMovable}, piece2: {piece2.IsMovable}");
            return false;
        }
        if (IsAdjacent(piece1, piece2) == false)
        {
            Debug.Log("Не р9дом");
            return false;
        }

        matrixController.SwapPiecesOnlyInMatrix(piece1, piece2);

        Vector2Int piece1XY = new Vector2Int(piece1.X, piece1.Y);
        Vector2Int piece2XY = new Vector2Int(piece2.X, piece2.Y);

        piece1.Movable.Move(piece2XY.x, piece2XY.y, GetPiecePositionOnWorld(piece2XY.x, piece2XY.y, TilemapsType.Field), movingTime);//droppingTime);
        piece2.Movable.Move(piece1XY.x, piece1XY.y, GetPiecePositionOnWorld(piece1XY.x, piece1XY.y, TilemapsType.Field), movingTime);//droppingTime);

        MatchingPieces matchPiece1 = new MatchingPieces();
        matchPiece1 = FindMatch(matrixController.Pieces[piece1.X, piece1.Y]);

        MatchingPieces matchPiece2 = new MatchingPieces();
        matchPiece2 = FindMatch(matrixController.Pieces[piece2.X, piece2.Y]);

        if ((matchPiece1.matchPieces != null && matchPiece1.matchPieces.Count >= 3) 
            || (matchPiece2.matchPieces != null && matchPiece2.matchPieces.Count >= 3)
            || piece1.IsBooster || piece2.IsBooster)
        {
            if (matchPiece1.matchPieces != null)
            {
                matrixController.DeleteSomePieces(matchPiece1.matchPieces, DestructionType.ByMatch, false);
            }
            if (matchPiece2.matchPieces != null)
            {
                matrixController.DeleteSomePieces(matchPiece2 .matchPieces, DestructionType.ByMatch, false);
            }

            if (matchPiece1.boosterType != BoosterType.None)
            {
                matrixController.SpawnNewBooster(matchPiece1.matchPieces[0].X, matchPiece1.matchPieces[0].Y, matchPiece1.boosterType);
            }
            else if (matchPiece2.boosterType != BoosterType.None)
            {
                matrixController.SpawnNewBooster(matchPiece2.matchPieces[0].X, matchPiece2.matchPieces[0].Y, matchPiece2.boosterType);
            }

            if (piece1.IsBooster)
                piece1.Booster.Activate(piece2);
            if (piece2.IsBooster) 
                piece2.Booster.Activate(piece1);

            if (piece1.IsDestructible && piece1.Destructible.IsPieceDestroyThisType(DestructionType.ByReachEnd))
                matrixController.AddListenerToCheckWhenMoveEnd(piece1);

            if (piece1.IsDestructible && piece1.Destructible.IsPieceDestroyThisType(DestructionType.ByReachEnd))
                matrixController.AddListenerToCheckWhenMoveEnd(piece2);

            StartDropPieces(droppingTime);
            OnMove?.Invoke();

            return true;
        }
        else
        {
            matrixController.SwapPiecesOnlyInMatrix(piece1, piece2);

            piece1.Movable.Move(piece1XY.x, piece1XY.y, GetPiecePositionOnWorld(piece1XY.x, piece1XY.y, TilemapsType.Field), movingTime);//droppingTime);
            piece2.Movable.Move(piece2XY.x, piece2XY.y, GetPiecePositionOnWorld(piece2XY.x, piece2XY.y, TilemapsType.Field), movingTime);//droppingTime);

            return false;
        }
    }
    
    private BoosterType IdentifyBooster(Piece piece, ref List<Piece> horizontelPieces, List<Piece> verticalPieces)
    {
        BoosterType boosterType = BoosterType.None;

        /*
        // Check the box
        for (int i = 0; i < horizontelPieces.Count; i++)
        {
            for (int j = 0; j < verticalPieces.Count; j++)
            {
                Piece diagPiece;
                if (piece.X + 1 < xDim && piece.Y + 1 < yDim)
                {
                    diagPiece = pieces[piece.X + 1, piece.Y + 1];
                    if (horizontelPieces[i].X == piece.X + 1 && verticalPieces[j].Y == piece.Y + 1
                        && diagPiece != null)
                    {
                        if (diagPiece.IsColorable)
                        {
                            if (diagPiece.Colorable.Color == piece.Colorable.Color)
                            {
                                boosterType = BoosterType.MiniBomb;
                                horizontelPieces.Add(diagPiece);
                            }
                        }
                    }
                }

                if (piece.X - 1 >= 0 && piece.Y + 1 < yDim)
                {
                    diagPiece = pieces[piece.X - 1, piece.Y + 1];
                    if (horizontelPieces[i].X == piece.X - 1 && verticalPieces[j].Y == piece.Y + 1
                        && diagPiece != null)
                    {
                        if (diagPiece.IsColorable)
                        {
                            if (diagPiece.Colorable.Color == piece.Colorable.Color)
                            {
                                boosterType = BoosterType.MiniBomb;
                                horizontelPieces.Add(diagPiece);
                            }
                        }
                    }
                }

                if (piece.X + 1 < xDim && piece.Y - 1 >= 0)
                {
                    diagPiece = pieces[piece.X + 1, piece.Y - 1];
                    if (horizontelPieces[i].X == piece.X + 1 && verticalPieces[j].Y == piece.Y - 1
                        && diagPiece != null)
                    {
                        if (diagPiece.IsColorable)
                        {
                            if (diagPiece.Colorable.Color == piece.Colorable.Color)
                            {
                                boosterType = BoosterType.MiniBomb;
                                horizontelPieces.Add(diagPiece);
                            }
                        }
                    }
                }

                if (piece.X - 1 >= 0 && piece.Y - 1 >= 0)
                {
                    diagPiece = pieces[piece.X - 1, piece.Y - 1];
                    if (horizontelPieces[i].X == piece.X - 1 && verticalPieces[j].Y == piece.Y - 1
                        && diagPiece != null)
                    {
                        if (diagPiece.IsColorable)
                        {
                            if (diagPiece.Colorable.Color == piece.Colorable.Color)
                            {
                                boosterType = BoosterType.MiniBomb;
                                horizontelPieces.Add(diagPiece);
                            }
                        }
                    }
                }
            }
        }
        */
        
        // Check the line of four
        if (verticalPieces.Count == 3 && horizontelPieces.Count < 1)
        {
            boosterType = BoosterType.VerticalRocket;
        }

        if (horizontelPieces.Count == 3 && verticalPieces.Count < 1)
        {
            boosterType = BoosterType.HorizontalRocket;
        }

        // Check the T or L shape
        if (verticalPieces.Count >= 2 && horizontelPieces.Count >= 2)
        {
            boosterType = BoosterType.MaxiBomb;
        }

        // Check the line of five
        if (verticalPieces.Count == 4 || horizontelPieces.Count == 4)
        {
            boosterType = BoosterType.Rainbow;
        }

        return boosterType;
    }
}
