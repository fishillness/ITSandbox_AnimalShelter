using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PieceMatrixController : MonoBehaviour,
    IDependency<PieceColorDictionary>, IDependency<BoosterDictionary>, IDependency<PieceCounter>,
    IDependency<FieldController>, IDependency<SpecifierRequiredPiecesOfType>, IDependency<SpecifierRequiredBooster>,
    IDependency<SoundsPlayer>, IDependency<GarbageCollector>
{
    [SerializeField] private SpecifierRequiredColorPieces specifierRequiredColorPieces;

    private PieceColorDictionary colorDictionary;
    private BoosterDictionary boosterDictionary;
    private PieceCounter pieceCounter;
    private FieldController field;
    private SpecifierRequiredPiecesOfType specifierRequiredPieces;
    private SpecifierRequiredBooster specifierRequiredBooster;
    private SoundsPlayer soundsPlayer;
    private GarbageCollector garbageCollector;

    #region Constructs
    public void Construct(PieceColorDictionary colorDictionary) => this.colorDictionary = colorDictionary;
    public void Construct(BoosterDictionary boosterDictionary) => this.boosterDictionary = boosterDictionary;
    public void Construct(PieceCounter pieceCounter) => this.pieceCounter = pieceCounter;
    public void Construct(FieldController fieldController) => field = fieldController;
    public void Construct(SpecifierRequiredPiecesOfType specifierRequiredPieces) => this.specifierRequiredPieces = specifierRequiredPieces;
    public void Construct(SpecifierRequiredBooster specifierRequiredBooster) => this.specifierRequiredBooster = specifierRequiredBooster;
    public void Construct(SoundsPlayer soundsPlayer) => this.soundsPlayer = soundsPlayer;
    public void Construct(GarbageCollector garbageCollector) => this.garbageCollector = garbageCollector;
    #endregion

    private Piece[,] pieces;
    private int xDim;
    private int yDim;

    private List<string> activeBoosterNames;
    private bool isThereActiveBooster;

    public Piece[,] Pieces => pieces;
    public bool IsThereActiveBooster => isThereActiveBooster;

    private void Awake()
    {
        activeBoosterNames = new List<string>();
    }

    private void Start()
    {
        if (soundsPlayer == null)
            GlobalGameDependenciesContainer.Instance.Rebind(this);
    }

    public void InitMatrix(int xDim, int yDim)
    {
        this.xDim = xDim;
        this.yDim = yDim;

        colorDictionary.InitDictionaty();
        boosterDictionary.InitDictionaty();

        pieces = new Piece[xDim, yDim];

        //FillFieldEmptyPieces();
        FillFieldColorPieces();
        SetRequiredPieces();
    }
    /*
    private void FillFieldEmptyPieces()
    {
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (field.IsTileNotEmpty(x,y, TilemapsType.Field))
                {
                    SpawnNewPiece(x, y, PieceType.Empty);
                }
                else
                    pieces[x, y] = null;
            }
        }
    }
    */
    private void SetRequiredPieces()
    {
        foreach (var piece in specifierRequiredPieces.RequiredPieces)
        {
            if (pieces[piece.position.x, piece.position.y] != null)
            {
                if (pieces[piece.position.x, piece.position.y].Type == PieceType.Empty)
                {
                    DeleteEmptyPiece(piece.position.x, piece.position.y);
                }
                else if (pieces[piece.position.x, piece.position.y].IsDestructible)
                {
                    pieces[piece.position.x, piece.position.y].Destructible.DestroyImmediately();
                }
                else
                {
                    continue;
                }

                pieces[piece.position.x, piece.position.y] = null;
            }

            SpawnNewPiece(piece.position.x, piece.position.y, piece.type);
        }

        foreach (var piece in specifierRequiredBooster.RequiredBoosters)
        {
            if (pieces[piece.position.x, piece.position.y] != null)
            {
                if (pieces[piece.position.x, piece.position.y].Type == PieceType.Empty)
                {
                    DeleteEmptyPiece(piece.position.x, piece.position.y);
                }
                else if (pieces[piece.position.x, piece.position.y].IsDestructible)
                {
                    pieces[piece.position.x, piece.position.y].Destructible.DestroyImmediately();
                }
                else
                {
                    continue;
                }

                pieces[piece.position.x, piece.position.y] = null;
            }

            SpawnNewBooster(piece.position.x, piece.position.y, piece.type);
        }

        if (specifierRequiredColorPieces != null)
            foreach (var piece in specifierRequiredColorPieces.RequiredColorPieces)
            {
                if (pieces[piece.position.x, piece.position.y] != null)
                {
                    if (pieces[piece.position.x, piece.position.y].Type == PieceType.Empty)
                    {
                        DeleteEmptyPiece(piece.position.x, piece.position.y);
                    }
                    else if (pieces[piece.position.x, piece.position.y].IsDestructible)
                    {
                        pieces[piece.position.x, piece.position.y].Destructible.DestroyImmediately();
                    }
                    else
                    {
                        continue;
                    }

                    pieces[piece.position.x, piece.position.y] = null;
                }

                SpawnNewPiece(piece.position.x, piece.position.y, PieceType.Normal, piece.colorType);

            }
    }

    private void FillFieldColorPieces()
    {
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (field.IsTileNotEmpty(x, y, TilemapsType.Field))
                {
                    SpawnNewPiece(x, y, PieceType.Normal);
                }
                else
                    pieces[x, y] = null;
            }
        }

        Debug.Log("End FillFieldColorPieces");
        /*
        foreach (Piece piece in pieces)
        {

            if (piece != null)
            {
                if (piece.Type == PieceType.Empty)
                {
                    DeleteEmptyPiece(piece.X, piece.Y);
                    SpawnNewPiece(piece.X, piece.Y, PieceType.Normal);
                    field.StartDropPieces(0);
                }
            }
        }
        */
    }

    public Piece SpawnNewPiece(int x, int y, PieceType type)
    {
        if (pieces[x,y] != null)
        {
            if (pieces[x, y].Type == PieceType.Empty)
                DeleteEmptyPiece(x, y);
            else 
                return null;
        }

        Piece newPiece = Instantiate(field.PiecesDict.GetPrefabByEnum(type),
            field.GetPiecePositionOnWorld(x, y, TilemapsType.Field), Quaternion.identity);
        newPiece.transform.parent = transform;
        newPiece.Init(x, y, type);

        if (newPiece.Colorable != null)
        {
            if (!newPiece.Colorable.IsColorDictionarySet)
                newPiece.Colorable.SetColorDictionary(colorDictionary);

            newPiece.Colorable.SetColor(colorDictionary.GetRandomColorFromDictionaty());
        }

        pieces[x, y] = newPiece;
        pieceCounter.AddPiece(newPiece);
        return newPiece;
    }

    public Piece SpawnNewPiece(int x, int y, PieceType type, ColorType colorType)
    {
        if (pieces[x, y] != null)
        {
            if (pieces[x, y].Type == PieceType.Empty)
                DeleteEmptyPiece(x, y);
            else
                return null;
        }

        Piece newPiece = Instantiate(field.PiecesDict.GetPrefabByEnum(type), 
            field.GetPiecePositionOnWorld(x, y, TilemapsType.Field), Quaternion.identity);
        newPiece.transform.parent = transform;
        newPiece.Init(x, y, type);

        if (newPiece.Colorable != null)
        {
            if (!newPiece.Colorable.IsColorDictionarySet)
                newPiece.Colorable.SetColorDictionary(colorDictionary);

            newPiece.Colorable.SetColor(colorType);
        }

        pieces[x, y] = newPiece;
        pieceCounter.AddPiece(newPiece);
        return newPiece;
    }

    public void SpawnNewBooster(int x, int y, BoosterType type)
    {
        if (pieces[x, y] != null)
        {
            if (!pieces[x, y].IsDestructible) return;

            if (pieces[x, y].Type == PieceType.Empty)
                DeleteEmptyPiece(x, y);
            else
            {
                pieces[x,y].Destructible.DestroyImmediately();
                pieces[x, y] = null;
            }
        }

        Piece boosterPiece = SpawnNewPiece(x, y, PieceType.Booster);

        boosterPiece.Booster.SetProperties(type, this);
        boosterPiece.Booster.SetBoosterSprite(boosterDictionary.GetSpriteByT(type));
    }
    
    public bool CheckTypeOfPieceInGrid(int x, int y, PieceType type)
    {
        if (pieces[x, y] == null)
            return false;

        return pieces[x, y].Type == type;
    }

    public void SwapEmptyPieceWithNonEmpty(int xEmpty, int yEmpty, int xNonEmpty, int yNonEmpty, bool immediately)
    {
        if (pieces[xEmpty, yEmpty].Type != PieceType.Empty)
        {
            Debug.LogError("Trying to use the method on two not empty pieces");
        }

        DeleteEmptyPiece(xEmpty, yEmpty);

        if (immediately)
        {
            pieces[xNonEmpty, yNonEmpty].Movable.Move(xEmpty, yEmpty, field.GetPiecePositionOnWorld(xEmpty, yEmpty, TilemapsType.Field), 0);
        }
        else
        {
            pieces[xNonEmpty, yNonEmpty].Movable.Move(xEmpty, yEmpty, field.GetPiecePositionOnWorld(xEmpty, yEmpty, TilemapsType.Field), field.MovingTime);//field.DroppingTime);
        }
        
        
        pieces[xEmpty, yEmpty] = pieces[xNonEmpty, yNonEmpty];

        if (pieces[xEmpty, yEmpty].IsDestructible && pieces[xEmpty, yEmpty].Destructible.IsPieceDestroyThisType(DestructionType.ByReachEnd))
            pieces[xEmpty, yEmpty].Movable.OnMoveEnd.AddListener(CheckNeedDamagePieceByReachEnd);

        pieces[xNonEmpty, yNonEmpty] = null;
        SpawnNewPiece(xNonEmpty, yNonEmpty, PieceType.Empty);
    }

    public void AddListenerToCheckWhenMoveEnd(Piece piece)
    {
        piece.Movable.OnMoveEnd.AddListener(CheckNeedDamagePieceByReachEnd);
    }
    
    public void SwapPiecesOnlyInMatrix(Piece piece1, Piece piece2)
    {
        pieces[piece1.X, piece1.Y] = piece2;
        pieces[piece2.X, piece2.Y] = piece1;
    }

    private void BoosterActivated(string boosterName)
    {
        activeBoosterNames.Add(boosterName);
        isThereActiveBooster = true;
    }

    private void BoosterActionEnded(string boosterName)
    {
        if (activeBoosterNames.Contains(boosterName))
            activeBoosterNames.Remove(boosterName);

        if (activeBoosterNames.Count == 0)
        {
            isThereActiveBooster = false;
            field.StartDropPieces(field.DroppingTime);
        }
    }
    
    public void ActivateClip(AudioClip audioClip)
    {
        soundsPlayer.LaunchSound(audioClip);
    }

    #region Damage and deleting pieces
    public void DeleteEmptyPiece(int x, int y)
    {
        if (pieces[x, y] == null) return;
        if (pieces[x, y].Type != PieceType.Empty) return;

        //Destroy(pieces[x, y].gameObject);
        garbageCollector.AddGarbage(pieces[x, y].gameObject);
        pieces[x, y] = null;
    }

    public void DamagePieces(Queue<Vector2Int> piecesIndexes, DestructionType destructionType)
    {
        List<Piece> damagePieces = new List<Piece>();
        Queue<Vector2Int> nearToMatchPiecess = new Queue<Vector2Int>();
        Queue<Vector2Int> nearToRainbowPieces = new Queue<Vector2Int>();

        while (piecesIndexes.Count > 0)
        {
            Vector2Int piecePosition = piecesIndexes.Dequeue();
            int x = piecePosition.x;
            int y = piecePosition.y;

            if (pieces[x, y] == null) continue;
            if (pieces[x, y].Type == PieceType.Empty) continue;
            if (!pieces[x, y].IsDestructible) continue;
            if (!pieces[x, y].Destructible.IsPieceDestroyThisType(destructionType)) continue;

            damagePieces.Add(pieces[x, y]);

            if (pieces[x, y].Destructible.IsLastStage)
                pieces[x, y].Destructible.OnPieceDestroy.AddListener(RemoveDestroyingPieceFromMatrix);

            if (destructionType == DestructionType.ByMatch && pieces[x, y].Destructible.IsDestroying == false)
            {
                if (y - 1 >= 0 && pieces[x, y - 1] != null && pieces[x, y - 1].IsDestructible
                    && pieces[x, y - 1].Destructible.IsPieceDestroyThisType(DestructionType.NearToMatch))
                    nearToMatchPiecess.Enqueue(new Vector2Int(x, y - 1));//pieces[x, y - 1]);

                if (x + 1 < xDim && pieces[x + 1, y] != null && pieces[x + 1, y].IsDestructible
                    && pieces[x + 1, y].Destructible.IsPieceDestroyThisType(DestructionType.NearToMatch))
                    nearToMatchPiecess.Enqueue(new Vector2Int(x + 1, y));// pieces[x + 1, y]);

                if (y + 1 < yDim && pieces[x, y + 1] != null && pieces[x, y + 1].IsDestructible
                    && pieces[x, y + 1].Destructible.IsPieceDestroyThisType(DestructionType.NearToMatch))
                    nearToMatchPiecess.Enqueue(new Vector2Int(x, y + 1));// pieces[x, y + 1]);

                if (x - 1 >= 0 && pieces[x - 1, y] != null && pieces[x - 1, y].IsDestructible
                    && pieces[x - 1, y].Destructible.IsPieceDestroyThisType(DestructionType.NearToMatch))
                    nearToMatchPiecess.Enqueue(new Vector2Int(x - 1, y));// pieces[x - 1, y]);
            }

            if (destructionType == DestructionType.Rainbow && pieces[x, y].Destructible.IsDestroying == false)
            {
                if (y - 1 >= 0 && pieces[x, y - 1] != null && pieces[x, y - 1].IsDestructible
                    && pieces[x, y - 1].Destructible.IsPieceDestroyThisType(DestructionType.NearRainbow))
                    nearToRainbowPieces.Enqueue(new Vector2Int(x, y - 1));// pieces[x, y - 1]);

                if (x + 1 < xDim && pieces[x + 1, y] != null && pieces[x + 1, y].IsDestructible
                    && pieces[x + 1, y].Destructible.IsPieceDestroyThisType(DestructionType.NearRainbow))
                    nearToRainbowPieces.Enqueue(new Vector2Int(x + 1, y));// pieces[x + 1, y]);

                if (y + 1 < yDim && pieces[x, y + 1] != null && pieces[x, y + 1].IsDestructible
                    && pieces[x, y + 1].Destructible.IsPieceDestroyThisType(DestructionType.NearRainbow))
                    nearToRainbowPieces.Enqueue(new Vector2Int(x, y + 1));// pieces[x, y + 1]);

                if (x - 1 >= 0 && pieces[x - 1, y] != null && pieces[x - 1, y].IsDestructible
                    && pieces[x - 1, y].Destructible.IsPieceDestroyThisType(DestructionType.NearRainbow))
                    nearToRainbowPieces.Enqueue(new Vector2Int(x - 1, y));// pieces[x - 1, y]);
            }
        }

        for (int i = 0; i < damagePieces.Count; i++)
        {
            damagePieces[i].Destructible.DamagePiece(destructionType);
        }

        if (nearToMatchPiecess.Count > 0)
        {
            DamagePieces(nearToMatchPiecess, DestructionType.NearToMatch);
        }
        if (nearToRainbowPieces.Count > 0)
        {
            DamagePieces(nearToRainbowPieces, DestructionType.NearRainbow);
        }
    }

    private void RemoveDestroyingPieceFromMatrix(Piece piece)
    {
        piece.Destructible.OnPieceDestroy.RemoveListener(RemoveDestroyingPieceFromMatrix);
        garbageCollector.AddGarbage(piece.gameObject);

        if (pieces[piece.X, piece.Y] == piece)
        {
            pieces[piece.X, piece.Y] = null;
            SpawnNewPiece(piece.X, piece.Y, PieceType.Empty);
        }

        field.StartDropPieces(field.DroppingTime);
    }

    public void DeleteSomePieces(List<Piece> listPieces, DestructionType destructionType, bool immediately)
    {
        if (immediately)
        {
            //garbageCollector.AddGarbage(listPieces.ConvertAll(pieceIndex => pieces[pieceIndex.x, pieceIndex.y].gameObject).ToArray());
            garbageCollector.AddGarbage(listPieces.ConvertAll(piece => piece.gameObject).ToArray());
            foreach (Piece piece in listPieces)
            {
                //piece.Destructible.DestroyImmediately();
                pieces[piece.X, piece.Y] = null;
                SpawnNewPiece(piece.X, piece.Y, PieceType.Empty);
            }
        }
        else
        {
            Queue<Vector2Int> piecesIndexes = new Queue<Vector2Int>();
            foreach (Piece piece in listPieces)
            {
                piecesIndexes.Enqueue(new Vector2Int(piece.X, piece.Y));
                //DamageNotEmptyPiece(piece.X, piece.Y, destructionType);
            }
            DamagePieces(piecesIndexes, destructionType);
        }
    }
    private void CheckNeedDamagePieceByReachEnd(Piece piece)
    {
        piece.Movable.OnMoveEnd.RemoveListener(CheckNeedDamagePieceByReachEnd);

        if (!piece.IsDestructible) return;
        if (!piece.Destructible.IsPieceDestroyThisType(DestructionType.ByReachEnd)) return;
        //if (pieces[piece.X, piece.Y] != null) return;
        //if (piece.Y + 1 < yDim) return;

        if (piece.Y + 1 == yDim || pieces[piece.X, piece.Y + 1] == null)
            DamagePieces(new Queue<Vector2Int>(new Vector2Int[] { new Vector2Int(piece.X, piece.Y) }), DestructionType.ByReachEnd);
        //DamageNotEmptyPiece(piece.X, piece.Y, DestructionType.ByReachEnd);
    }


    public void UseHorizontalRocket(int x, int y, float time) //DeleteRow
    {
        BoosterActivated(pieces[x, y].name);

        StartCoroutine(UseHorizontalRocketCoroutine(x, y, time));
    }

    private IEnumerator UseHorizontalRocketCoroutine(int x, int y, float time) //DeleteRowCoroutine
    {
        string boosterName = pieces[x, y].name;
        int xLeft = x - 1;
        int xRight = x + 1;

        DamagePieces(new Queue<Vector2Int>(new Vector2Int[] { new Vector2Int(x, y) }), DestructionType.Rocket);
        //DamageNotEmptyPiece(x, y, DestructionType.Rocket);

        while (xLeft >= 0 || xRight < xDim)
        {
            Queue<Vector2Int> piecesIndexes = new Queue<Vector2Int>();

            if (xLeft >= 0 && pieces[xLeft, y] != null && pieces[xLeft, y].IsDestructible)
            {
                //DamageNotEmptyPiece(xLeft, y, DestructionType.Rocket);
                piecesIndexes.Enqueue(new Vector2Int(xLeft, y));
                xLeft--;
            }
            else if (xLeft >= 0 && pieces[xLeft, y] == null)
            {
                xLeft = -1;
            }

            if (xRight < xDim && pieces[xRight, y] != null && pieces[xRight, y].IsDestructible)
            {
                //DamageNotEmptyPiece(xRight, y, DestructionType.Rocket);
                piecesIndexes.Enqueue(new Vector2Int(xRight, y));
                xRight++;
            }
            else if (xRight < xDim && pieces[xRight, y] == null)
            {
                xRight = xDim;
            }

            DamagePieces(piecesIndexes, DestructionType.Rocket);
            yield return new WaitForSeconds(time);
        }

        yield return new WaitForSeconds(time);
        BoosterActionEnded(boosterName);
    }

    public void UseVerticalBomb(int x, int y, float time) // DeleteColumn
    {
        BoosterActivated(pieces[x, y].name);

        StartCoroutine(UseVerticalBombCoroutine(x, y, time));
    }

    private IEnumerator UseVerticalBombCoroutine(int x, int y, float time) //DeleteColumnCoroutine
    {
        string boosterName = pieces[x, y].name;
        int yAbove = y - 1;
        int yBelow = y + 1;

        DamagePieces(new Queue<Vector2Int>(new Vector2Int[] { new Vector2Int(x, y) }), DestructionType.Rocket);
        //DamageNotEmptyPiece(x, y, DestructionType.Rocket);

        while (yAbove >= 0 || yBelow < yDim)
        {
            Queue<Vector2Int> piecesIndexes = new Queue<Vector2Int>();

            if (yAbove >= 0 && pieces[x, yAbove] != null && pieces[x, yAbove].IsDestructible)
            {
                //DamageNotEmptyPiece(x, yAbove, DestructionType.Rocket);
                piecesIndexes.Enqueue(new Vector2Int(x, yAbove));
                yAbove--;
            }
            else if (yAbove >= 0 && pieces[x, yAbove] == null)
            {
                yAbove = -1;
            }

            if (yBelow < yDim && pieces[x, yBelow] != null && pieces[x, yBelow].IsDestructible)
            {
                //DamageNotEmptyPiece(x, yBelow, DestructionType.Rocket);
                piecesIndexes.Enqueue(new Vector2Int(x, yBelow));
                yBelow++;
            }
            else if (yBelow < yDim && pieces[x, yBelow] == null)
            {
                yBelow = yDim;
            }
            
            DamagePieces(piecesIndexes, DestructionType.Rocket);
            yield return new WaitForSeconds(time);
        }

        yield return new WaitForSeconds(time);
        BoosterActionEnded(boosterName);
    }
    public void UseMiniBomb(int x, int y)  //DeleteNearPiece
    {
        BoosterActivated(pieces[x, y].name);

        string boosterName = pieces[x, y].name;

        Queue<Vector2Int> piecesIndexes = new Queue<Vector2Int>();
        piecesIndexes.Enqueue(new Vector2Int(x, y)); 
        //DamageNotEmptyPiece(x, y, DestructionType.Bomb);
        //ActivedDamageForNearPieces(x, y, DestructionType.Bomb);

        if (y - 1 >= 0 && pieces[x, y - 1] != null && pieces[x, y - 1].IsDestructible)
            piecesIndexes.Enqueue(new Vector2Int(x, y - 1));// pieces[x, y - 1]);

        if (x + 1 < xDim && pieces[x + 1, y] != null && pieces[x + 1, y].IsDestructible)
            piecesIndexes.Enqueue(new Vector2Int(x + 1, y));// pieces[x + 1, y]);

        if (y + 1 < yDim && pieces[x, y + 1] != null && pieces[x, y + 1].IsDestructible)
            piecesIndexes.Enqueue(new Vector2Int(x, y + 1));// pieces[x, y + 1]);

        if (x - 1 >= 0 && pieces[x - 1, y] != null && pieces[x - 1, y].IsDestructible)
            piecesIndexes.Enqueue(new Vector2Int(x - 1, y));// pieces[x - 1, y]);

        DamagePieces(piecesIndexes, DestructionType.Bomb);
        BoosterActionEnded(boosterName);
    }

    public void UseBomb(int x, int y, float time) //DeleteManyNearPieces
    {
        BoosterActivated(pieces[x, y].name);

        StartCoroutine(UseBombCoroutine(x, y, time));
    }

    private IEnumerator UseBombCoroutine(int x, int y, float time) //DeleteManyNearPiecesCoroutine
    {
        string boosterName = pieces[x, y].name;

        int xMin = x - 2;
        int xMax = x + 2;

        int yMin = y - 2;
        int yMax = y + 2;

        DamagePieces(new Queue<Vector2Int>(new Vector2Int[] { new Vector2Int(x, y) }), DestructionType.Bomb);
        //DamageNotEmptyPiece(x, y, DestructionType.Bomb);
        //yield return new WaitForSeconds(time);

        Queue<Vector2Int> piecesIndexes = new Queue<Vector2Int>();

        for (int i = xMin; i <= xMax; i++)
        {
            for (int j = yMin; j <= yMax; j++)
            {
                if (i == xMin && j == yMin) continue;
                if (i == xMax && j == yMax) continue;
                if (i == xMin && j == yMax) continue;
                if (i == xMax && j == yMin) continue;
                if (i < 0 || i >= xDim) continue;
                if (j < 0 || j >= yDim) continue;
                if (i == x && j == y) continue;

                if (pieces[i, j] != null && pieces[i, j].IsDestructible)
                {
                    //DamageNotEmptyPiece(i, j, DestructionType.Bomb);
                    //yield return new WaitForSeconds(time);
                    piecesIndexes.Enqueue(new Vector2Int(i, j));
                }
            }
        }

        DamagePieces(piecesIndexes, DestructionType.Bomb);

        yield return new WaitForSeconds(time);
        BoosterActionEnded(boosterName);
    }
    public void UseRainbow(int x, int y, Piece swapPiece, float time) //DeleteAllPiecesByColor
    {
        string boosterName = pieces[x, y].name;
        BoosterActivated(pieces[x, y].name);

        ColorType color;
        if (!swapPiece.IsColorable)
            color = colorDictionary.GetRandomColorFromDictionaty();
        else
            color = swapPiece.Colorable.Color;

        DamagePieces(new Queue<Vector2Int>(new Vector2Int[] { new Vector2Int(x, y) }), DestructionType.ByActivationByself);
        Queue<Vector2Int> piecesIndexes = new Queue<Vector2Int>();

        foreach (Piece piece in pieces)
        {
            if (piece == null) continue;

            if (piece.IsColorable && piece.IsDestructible && piece.Colorable.Color == color)
            {
                piecesIndexes.Enqueue(new Vector2Int(piece.X, piece.Y));
            }
        }

        DamagePieces(piecesIndexes, DestructionType.Rainbow);
        BoosterActionEnded(boosterName);
    }
    #endregion
}
