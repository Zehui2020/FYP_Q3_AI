using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static Utility Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetTopmostParent(Transform child)
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

    public List<T> SetListSize<T>(List<T> list, int desiredSize)
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

    public string ToRoman(int number)
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

    public bool CheckLayer(GameObject collidedGO, LayerMask layer)
    {
        return (layer & 1 << collidedGO.layer) == 1 << collidedGO.layer;
    }
}