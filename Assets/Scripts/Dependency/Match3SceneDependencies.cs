using UnityEngine;

public class Match3SceneDependencies : Dependency
{
    [SerializeField] private PieceCounter pieceCounter;

    [SerializeField] private PieceColorDictionary pieceColorDictionary;
    [SerializeField] private BoosterDictionary boosterDictionary;
    [SerializeField] private TilemapsDictionary tilemapsDictionary;
    [SerializeField] private PiecesDictionary piecesDictionary;

    [SerializeField] private SpecifierRequiredPiecesOfType specifierRequiredPiecesOfType;
    [SerializeField] private SpecifierRequiredBooster specifierRequiredBooster;
    [SerializeField] private UIMatch3LevelPanel levelPanel;
    [SerializeField] private TaskInfoCounter taskInfoCounter;

    [SerializeField] private PieceMatrixController matrixController;
    [SerializeField] private PiecesSpawnerController spawnerController;
    [SerializeField] private Match3Level level;

    [SerializeField] private FieldController fieldController;
    [SerializeField] private PieceControl pieceControl;
    [SerializeField] private UIEndLevelPanel endLevelPanel;

    private void Awake()
    {
        FindAllObjectToBind();
    }

    protected override void BindAll(MonoBehaviour monoBehaviourInScene)
    {
        Bind<PieceCounter>(pieceCounter, monoBehaviourInScene);

        Bind<PieceColorDictionary>(pieceColorDictionary, monoBehaviourInScene);
        Bind<BoosterDictionary>(boosterDictionary, monoBehaviourInScene);
        Bind<TilemapsDictionary>(tilemapsDictionary, monoBehaviourInScene);
        Bind<PiecesDictionary>(piecesDictionary, monoBehaviourInScene);

        Bind<SpecifierRequiredPiecesOfType>(specifierRequiredPiecesOfType, monoBehaviourInScene);
        Bind<SpecifierRequiredBooster>(specifierRequiredBooster, monoBehaviourInScene);
        Bind<UIMatch3LevelPanel>(levelPanel, monoBehaviourInScene);
        Bind<TaskInfoCounter>(taskInfoCounter, monoBehaviourInScene);

        Bind<PieceMatrixController>(matrixController, monoBehaviourInScene);
        Bind<PiecesSpawnerController>(spawnerController, monoBehaviourInScene);
        Bind<Match3Level>(level, monoBehaviourInScene);

        Bind<FieldController>(fieldController, monoBehaviourInScene);
        Bind<PieceControl>(pieceControl, monoBehaviourInScene);
        Bind<UIEndLevelPanel>(endLevelPanel, monoBehaviourInScene);
    }
}
