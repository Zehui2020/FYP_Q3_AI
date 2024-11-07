using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static Transform GetTopmostParent(Transform child)
    {
        if (child == null)
            return null;

        Transform parent = child;

        while (parent.parent != null)
        {
            parent = parent.parent;
        }

        return parent;
    }

    public static List<T> SetListSize<T>(List<T> list, int desiredSize)
    {
        List<T> removedList = new List<T>();

        if (list == null) return removedList;

        if (list.Count > desiredSize)
        {
            int removeCount = list.Count - desiredSize;
            removedList.AddRange(list.GetRange(desiredSize, removeCount));
            list.RemoveRange(desiredSize, removeCount);
        }

        return removedList;
    }

    public static string ToRoman(int number)
    {
        if (number < 1 || number > 3999)
            return string.Empty;

        if (number >= 1000) return "M" + ToRoman(number - 1000);
        if (number >= 900) return "CM" + ToRoman(number - 900);
        if (number >= 500) return "D" + ToRoman(number - 500);
        if (number >= 400) return "CD" + ToRoman(number - 400);
        if (number >= 100) return "C" + ToRoman(number - 100);
        if (number >= 90) return "XC" + ToRoman(number - 90);
        if (number >= 50) return "L" + ToRoman(number - 50);
        if (number >= 40) return "XL" + ToRoman(number - 40);
        if (number >= 10) return "X" + ToRoman(number - 10);
        if (number >= 9) return "IX" + ToRoman(number - 9);
        if (number >= 5) return "V" + ToRoman(number - 5);
        if (number >= 4) return "IV" + ToRoman(number - 4);
        return "I" + ToRoman(number - 1);
    }

    public static bool CheckLayer(GameObject collidedGO, LayerMask layer)
    {
        return (layer & 1 << collidedGO.layer) == 1 << collidedGO.layer;
    }

    public static Vector2 GetDirectionFromAngle(float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        float x = Mathf.Cos(angleInRadians);
        float y = Mathf.Sin(angleInRadians);

        return new Vector2(x, y).normalized;
    }

    public static float GetAngleFromDirection(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    public static bool StringExistsInString(string stringToFind, string fullString)
    {
        if (string.IsNullOrEmpty(stringToFind) || string.IsNullOrEmpty(fullString))
            return false;

        return fullString.Contains(stringToFind);
    }

    public static int ParseJsonValue(string json, string key)
    {
        // Find the key in the JSON string
        int keyIndex = json.IndexOf($"\"{key}\"");
        if (keyIndex == -1)
        {
            return -1;
        }

        // Find the colon after the key
        int colonIndex = json.IndexOf(':', keyIndex);
        if (colonIndex == -1)
        {
            return -1;
        }

        // Extract the value part (assumes the value is an integer)
        int commaIndex = json.IndexOf(',', colonIndex);
        int endIndex = commaIndex != -1 ? commaIndex : json.Length;

        string valueString = json.Substring(colonIndex + 1, endIndex - colonIndex - 1).Trim();

        // Parse the value to an integer
        if (int.TryParse(valueString, out int result))
        {
            return result;
        }
        else
        {
            Debug.LogError($"Failed to parse value for key \"{key}\".");
            return -1;
        }
    }
}