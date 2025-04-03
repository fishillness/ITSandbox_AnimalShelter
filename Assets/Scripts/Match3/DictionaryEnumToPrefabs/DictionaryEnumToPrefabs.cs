using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DictionaryEnumToPrefabs<T1, T2> : MonoBehaviour
{
    [Serializable, SerializeField]
    private struct EnumToPrefab
    {
        public T1 Enum;
        public T2 Prefab;
    }

    [SerializeField] private EnumToPrefab[] enumToPrefabs;

    protected Dictionary<T1, T2> enumToPrefabsDict;

    public void InitDictionaty()
    {
        if (enumToPrefabsDict != null) return;
        
        enumToPrefabsDict = new Dictionary<T1, T2>();

        for (int i = 0; i < enumToPrefabs.Length; i++)
        {
            if (!enumToPrefabsDict.ContainsKey(enumToPrefabs[i].Enum))
            {
                enumToPrefabsDict.Add(enumToPrefabs[i].Enum, enumToPrefabs[i].Prefab);
            }
        }
    }

    public T2 GetPrefabByEnum(T1 Enum)
    {
        if (enumToPrefabsDict.ContainsKey(Enum))
            return enumToPrefabsDict[Enum];
        
        return default(T2);
    }
}
