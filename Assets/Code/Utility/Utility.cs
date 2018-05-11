using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public partial class Utility
{
    public static float GetRandom(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static int GetRandom(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static bool isNumeric(string input) 
	{
        float output;
        return float.TryParse(input, out output);
	}

    public static bool isVariable(string input)
	{
        return char.IsLetter(input, 0);
    }

    public static string tolower(string input)
	{
        return input.ToLower();
	}

    public static string toupper(string input)
    {
        return input.ToUpper();
    }

    public static bool isTrue(string input)
    {
        return string.Compare(input, "true", true) == 0;
    }

    public static bool isFalse(string input)
    {
        return string.Compare(input, "false", true) == 0;
    }

    public static bool isBool(string input)
    {
        return isTrue(input) || isFalse(input);
    }

    public static Color ConvertColor(int r, int g, int b, int a)
    {
        return new Color((float)r / 255, (float)g / 255, (float)b / 255, (float)a / 255);
    }

    public delegate int Comparison<T, U>(T x, U y);

    public static int UpperBinarySearch<T, U>(IList<T> list, U value, Comparison<T, U> comp)
    {
        if (list == null) return -1;

        int lo = 0, hi = list.Count - 1;
        while (lo < hi)
        {
            var m = lo + (hi - lo) / 2;
            if (comp(list[m], value) < 0) lo = m + 1;
            else hi = m - 1;
        }
        if (comp(list[lo], value) < 0) lo++;
        return lo;
    }
}
