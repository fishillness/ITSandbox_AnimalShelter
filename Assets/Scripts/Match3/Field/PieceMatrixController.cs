using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMatrixController : MonoBehaviour,
    IDependency<PieceColorDictionary>, IDependency<BoosterDictionary>, IDependency<PieceCounter>,
    IDependency<FieldController>, IDependency<SpecifierRequiredPiecesOfType>, IDependency<SpecifierRequiredBooster>,
    IDependency<SoundsPlayer>
{
    [SerializeField] private SpecifierRequiredColorPieces specifierRequiredColorPieces;

    private PieceColorDictionary colorDictionary;
    private BoosterDictionary boosterDictionary;
    private PieceCounter pieceCounter;
    private FieldController field;
    private SpecifierRequiredPiecesOfType specifierRequiredPieces;
    private SpecifierRequiredBooster specifierRequiredBooster;
    private SoundsPlayer soundsPlayer;

    #region Constructs
    public void Construct(PieceColorDictionary colorDictionary) => this.colorDictionary = colorDictionary;
    public void Construct(BoosterDictionary boosterDictionary) => this.boosterDictionary = boosterDictionary;
    public void Construct(PieceCounter pieceCounter) => this.pieceCounter = pieceCounter;
    public void Construct(FieldController fieldController) => field = fieldController;
    public void Construct(SpecifierRequiredPiecesOfType specifierRequiredPieces) => this.specifierRequiredPieces = specifierRequiredPieces;
    public void Construct(SpecifierRequiredBooster specifierRequiredBooster) => this.specifierRequiredBooster = specifierRequiredBooster;
    public void Construct(SoundsPlayer soundsPlayer) => this.soundsPlayer = soundsPlayer;
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

        FillFieldEmptyPieces();
        FillFieldColorPieces();
        SetRequiredPieces();
    }

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

    public void DeleteEmptyPiece(int x, int y)
    {
        if (pieces[x, y] == null) return;
        if (pieces[x, y].Type != PieceType.Empty)
        {
            return;
        }

        Destroy(pieces[x, y].gameObject);
        pieces[x, y] = null;
    }

    public void DamageNotEmptyPiece(int x, int y, DestructionType destructionType)
    {
        if (pieces[x, y] == null) return;
        if (pieces[x, y].Type == PieceType.Empty)
        {
            Debug.Log($"Попытка удалить пустую фишку: {x}, {y}");
            return;
        }
        if (!pieces[x, y].IsDestructible) return;

        if (pieces[x, y].Destructible.IsPieceDestroyThisType(destructionType))
        {
            if (destructionType == DestructionType.ByMatch && pieces[x, y].Destructible.IsDestroying == false)
                ActivedDamageForNearPieces(x, y, DestructionType.NearToMatch);

            if (destructionType == DestructionType.Rainbow && pieces[x, y].Destructible.IsDestroying == false)
                ActivedDamageForNearPieces(x, y, DestructionType.NearRainbow);

            if (pieces[x, y].Destructible.IsLastStage)
                pieces[x, y].Destructible.OnPieceDestroy.AddListener(RemoveDestroyingPieceFromMatrix);

            pieces[x,y].Destructible.DamagePiece(destructionType);
        }
    }

    private void RemoveDestroyingPieceFromMatrix(Piece piece)
    {
        piece.Destructible.OnPieceDestroy.RemoveListener(RemoveDestroyingPieceFromMatrix);

        if (pieces[piece.X, piece.Y] == piece)
        {
            pieces[piece.X, piece.Y] = null;
            SpawnNewPiece(piece.X, piece.Y, PieceType.Empty);
        }

        field.StartDropPieces(field.DroppingTime);
    }

    public bool CheckTypeOfPieceInGrid(int x, int y, PieceType type)
    {
        if (pieces[x, y] == null)
            return false;

        return pieces[x, y].Type == type;
    }

    public void SwapEmptyPieceWithNonEmpty(int xEmpty, int yEmpty, int xNonEmpty, int yNonEmpty, bool immediately)
    {
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

    private void CheckNeedDamagePieceByReachEnd(Piece piece)
    {
        piece.Movable.OnMoveEnd.RemoveListener(CheckNeedDamagePieceByReachEnd);

        if (!piece.IsDestructible) return;
        //if (pieces[piece.X, piece.Y] != null) return;
        //if (piece.Y + 1 < yDim) return;

        if (piece.Y + 1 == yDim || pieces[piece.X, piece.Y + 1] == null)
            DamageNotEmptyPiece(piece.X, piece.Y, DestructionType.ByReachEnd);
    }

    public void SwapPiecesOnlyInMatrix(Piece piece1, Piece piece2)
    {
        pieces[piece1.X, piece1.Y] = piece2;
        pieces[piece2.X, piece2.Y] = piece1;
    }

    public void DeleteSomePieces(List<Piece> listPieces, DestructionType destructionType, bool immediately)
    {
        if (immediately)
        {
            foreach (Piece piece in listPieces)
            {
                piece.Destructible.DestroyImmediately();
                pieces[piece.X, piece.Y] = null;
                SpawnNewPiece(piece.X, piece.Y, PieceType.Empty);
            }
        }
        else
        {
            foreach (Piece piece in listPieces)
            {
                DamageNotEmptyPiece(piece.X, piece.Y, destructionType);
            }
        }
    }

    public void DeleteRow(int x, int y, float time)
    {
        BoosterActivated(pieces[x, y].name);

        StartCoroutine(DeleteRowCoroutine(x, y, time));
    }

    private IEnumerator DeleteRowCoroutine(int x, int y, float time)
    {
        string boosterName = pieces[x,y].name;
        int xLeft = x - 1;
        int xRight = x + 1;

        DamageNotEmptyPiece(x, y, DestructionType.Rocket);

        while (xLeft >= 0 || xRight < xDim)
        {
            if (xLeft >= 0 && pieces[xLeft, y] != null && pieces[xLeft, y].IsDestructible)
            {
                DamageNotEmptyPiece(xLeft, y, DestructionType.Rocket);
                xLeft--;
            }
            else if (xLeft >= 0 && pieces[xLeft, y] == null)
            {
                xLeft = -1;
            }

            if (xRight < xDim && pieces[xRight, y] != null && pieces[xRight, y].IsDestructible)
            {
                DamageNotEmptyPiece(xRight, y, DestructionType.Rocket);
                xRight++;
            }
            else if (xRight < xDim && pieces[xRight, y] == null)
            {
                xRight = xDim;
            }

            yield return new WaitForSeconds(time);
        }

        BoosterActionEnded(boosterName);
    }

    public void DeleteColumn(int x, int y, float time)
    {
        BoosterActivated(pieces[x, y].name);

        StartCoroutine(DeleteColumnCoroutine(x, y, time));
    }

    private IEnumerator DeleteColumnCoroutine(int x, int y, float time)
    {
        string boosterName = pieces[x, y].name;
        int yAbove = y - 1;
        int yBelow = y + 1;

        DamageNotEmptyPiece(x, y, DestructionType.Rocket);

        while (yAbove >= 0 || yBelow < yDim)
        {
            if (yAbove >= 0 && pieces[x, yAbove] != null && pieces[x, yAbove].IsDestructible)
            {
                DamageNotEmptyPiece(x, yAbove, DestructionType.Rocket);
                yAbove--;
            }
            else if (yAbove >= 0 && pieces[x, yAbove] == null)
            {
                yAbove = -1;
            }

            if (yBelow < yDim && pieces[x, yBelow] != null && pieces[x, yBelow].IsDestructible)
            {
                DamageNotEmptyPiece(x, yBelow, DestructionType.Rocket);
                yBelow++;
            }
            else if (yBelow < yDim && pieces[x, yBelow] == null)
            {
                yBelow = yDim;
            }

            yield return new WaitForSeconds(time);
        }

        BoosterActionEnded(boosterName);
    }

    public void DeleteNearPiece(int x, int y)
    {
        BoosterActivated(pieces[x, y].name);

        string boosterName = pieces[x, y].name;

        DamageNotEmptyPiece(x, y, DestructionType.Bomb);
        ActivedDamageForNearPieces(x, y, DestructionType.Bomb);

        BoosterActionEnded(boosterName);
    }

    public void DeleteManyNearPieces(int x, int y, float time)
    {
        BoosterActivated(pieces[x, y].name);

        StartCoroutine(DeleteManyNearPiecesCoroutine(x, y, time));
    }

    private IEnumerator DeleteManyNearPiecesCoroutine(int x, int y, float time)
    {
        string boosterName = pieces[x, y].name;

        int xMin = x - 2;
        int xMax = x + 2;

        int yMin = y - 2;
        int yMax = y + 2;

        DamageNotEmptyPiece(x, y, DestructionType.Bomb);
        yield return new WaitForSeconds(time);

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
                    DamageNotEmptyPiece(i, j, DestructionType.Bomb);
                    yield return new WaitForSeconds(time);
                }
            }
        }

        BoosterActionEnded(boosterName);
    }

    public void DeleteAllPiecesByColor(int x, int y, Piece swapPiece, float time)
    {
        BoosterActivated(pieces[x, y].name);

        StartCoroutine(DeleteAllPiecesByColorCoroutine(x, y, swapPiece, time));
    }

    private IEnumerator DeleteAllPiecesByColorCoroutine(int x, int y, Piece swapPiece, float time)
    {
        string boosterName = pieces[x, y].name;

        ColorType color;
        if (!swapPiece.IsColorable)
            color = colorDictionary.GetRandomColorFromDictionaty();
        else
            color = swapPiece.Colorable.Color;

        DamageNotEmptyPiece(x, y, DestructionType.ByActivationByself);

        foreach(Piece piece in pieces)
        {
            if (piece == null) continue;

            if (piece.IsColorable && piece.IsDestructible && piece.Colorable.Color == color)
            {
                DamageNotEmptyPiece(piece.X, piece.Y, DestructionType.Rainbow);
                yield return new WaitForSeconds(time);
            }
        }

        BoosterActionEnded(boosterName);
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

    private void ActivedDamageForNearPieces(int x, int y, DestructionType type)
    {
        if (y - 1 >= 0 && pieces[x, y - 1] != null && pieces[x, y - 1].IsDestructible)
            DamageNotEmptyPiece(x, y - 1, type);

        if (x + 1 < xDim && pieces[x + 1, y] != null && pieces[x + 1, y].IsDestructible)
            DamageNotEmptyPiece(x + 1, y, type);

        if (y + 1 < yDim && pieces[x, y + 1] != null && pieces[x, y + 1].IsDestructible)
            DamageNotEmptyPiece(x, y + 1, type);

        if (x - 1 >= 0 && pieces[x - 1, y] != null && pieces[x - 1, y].IsDestructible)
            DamageNotEmptyPiece(x - 1, y, type);
    }

    public void ActivateClip(AudioClip audioClip)
    {
        soundsPlayer.LaunchSound(audioClip);
    }
}
