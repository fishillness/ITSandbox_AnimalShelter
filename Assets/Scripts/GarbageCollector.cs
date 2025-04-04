using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCollector : MonoBehaviour
{
    [SerializeField] private float garbageCollectionInterval = 2f;

    private Queue<GameObject> garbageQueue;
    private Coroutine coroutine = null;

    private void Awake()
    {
        garbageQueue = new Queue<GameObject>();
    }

    private IEnumerator Delete()
    {
        Destroy(garbageQueue.Dequeue());

        yield return new WaitForSeconds(garbageCollectionInterval);

        if (garbageQueue.Count > 0)
            coroutine = StartCoroutine(Delete());
        
        coroutine = null;
    }

    public void AddGarbage(GameObject gameObject)
    {
        bool isEmpty = garbageQueue.Count == 0;

        garbageQueue.Enqueue(gameObject);
        gameObject.SetActive(false);

        if (isEmpty && coroutine == null)
            coroutine = StartCoroutine(Delete());
    }

    public void AddGarbage(GameObject[] gameObjects)
    {
        bool isEmpty = garbageQueue.Count == 0;
        foreach (var gameObject in gameObjects)
        {
            garbageQueue.Enqueue(gameObject);
            gameObject.SetActive(false);
        }
        if (isEmpty && coroutine == null)
            coroutine = StartCoroutine(Delete());
    }
}
