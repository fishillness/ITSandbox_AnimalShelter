using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameDependenciesContainer : Dependency
{
    private static GlobalGameDependenciesContainer instance;

    [SerializeField] private Match3LevelManager levelManager;
    [SerializeField] private ValueManager valueManager;
    [SerializeField] private InputController inputController;
    [SerializeField] private MissionController missionController;
    [SerializeField] private MusicPlayer musicPlayer;
    [SerializeField] private SoundsPlayer soundsPlayer;
    [SerializeField] private GarbageCollector garbageCollector;

    public static GlobalGameDependenciesContainer Instance => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        FindAllObjectToBind();
    }

    protected override void BindAll(MonoBehaviour monoBehaviourInScene)
    {
        Bind<Match3LevelManager>(levelManager, monoBehaviourInScene);
        Bind<ValueManager>(valueManager, monoBehaviourInScene);
        Bind<InputController>(inputController, monoBehaviourInScene);
        Bind<MissionController>(missionController, monoBehaviourInScene);
        Bind<MusicPlayer>(musicPlayer, monoBehaviourInScene);
        Bind<SoundsPlayer>(soundsPlayer, monoBehaviourInScene);
        Bind<GarbageCollector>(garbageCollector, monoBehaviourInScene);
    }

    public void Rebind(MonoBehaviour monoBehaviour)
    {
        BindAll(monoBehaviour);
    }
}
