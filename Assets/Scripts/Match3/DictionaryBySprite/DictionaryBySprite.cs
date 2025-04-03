using System;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryBySprite<T> :MonoBehaviour
{
    [Serializable, SerializeField]
    private struct TypeSprite
    {
        public T type;
        public Sprite sprite;
    }

    [SerializeField] private TypeSprite[] typeSprites;

    protected Dictionary<T, Sprite> typeSpriteDict;

    public int NumberTypes => typeSprites.Length;


    public void InitDictionaty()
    {
        typeSpriteDict = new Dictionary<T, Sprite>();

        for (int i = 0; i < typeSprites.Length; i++)
        {
            if (!typeSpriteDict.ContainsKey(typeSprites[i].type))
            {
                typeSpriteDict.Add(typeSprites[i].type, typeSprites[i].sprite);
            }
        }
    }

    public Sprite GetSpriteByT(T type)
    {
        if (typeSpriteDict.ContainsKey(type))
            return typeSpriteDict[type];

        return null;
    }
}
