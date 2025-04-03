using System;

public class PieceColorDictionary : DictionaryBySprite<ColorType>
{
    public ColorType GetRandomColorFromDictionaty()
    {
        var colorTypes = Enum.GetValues(typeof(ColorType));

        int random = UnityEngine.Random.Range(0, colorTypes.Length);
        ColorType color = (ColorType)colorTypes.GetValue(random);

        while (!typeSpriteDict.ContainsKey(color))
        {
            random = UnityEngine.Random.Range(0, colorTypes.Length);
            color = (ColorType)colorTypes.GetValue(random);
        }

        return color;
    }
}
