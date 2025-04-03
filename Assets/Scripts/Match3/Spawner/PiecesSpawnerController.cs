using System.Collections.Generic;
using UnityEngine;

public class PiecesSpawnerController : WorkingWithGrid,
    IDependency<PieceMatrixController>
{
    [SerializeField] private PiecesSpawner spawnerPrefab;

    private PieceMatrixController matrixController;

    #region Constructs
    public void Construct(PieceMatrixController matrixController) => this.matrixController = matrixController;
    #endregion

    private List<PiecesSpawner> piecesSpawners;

    public void InitSpawners()
    {
        piecesSpawners = new List<PiecesSpawner>();

        InitTilemapsGrid();
        SetBounds();
        CreateSpawners();
    }

    private void CreateSpawners()
    {
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (IsTileNotEmpty(x, y, TilemapsType.Spawners))
                {
                    PiecesSpawner spawner = Instantiate(spawnerPrefab, GetPiecePositionOnWorld(x ,y, TilemapsType.Spawners), Quaternion.identity);
                    spawner.transform.parent = transform;
                    spawner.name = $"PiecesSpawners [{x}, {y}]";
                    spawner.Init(x, y);
                    piecesSpawners.Add(spawner);
                }
            }
        }
    }

    public bool CheckNeedOfSpawnPiece()
    {
        bool somethingSpawned = false;

        foreach (var spawner in piecesSpawners)
        {
            if (matrixController.CheckTypeOfPieceInGrid(spawner.X, spawner.Y, PieceType.Empty))
            {
                matrixController.DeleteEmptyPiece(spawner.X, spawner.Y);
                matrixController.SpawnNewPiece(spawner.X, spawner.Y, PieceType.Normal);

                somethingSpawned = true;
            }
        }

        return somethingSpawned;
    }
}
