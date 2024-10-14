using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UnityEngine.AI;
using DG.Tweening;
using Random = UnityEngine.Random;

public static class Utilities
{
    /// <summary>
    /// Returns shuffled array
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Returns random item from the array
    /// </summary>
    /// <param name="canBeNull">Need to return default value?</param>
    public static T GetRandomValue<T>(this IList<T> list, bool canBeNull = false)
    {
        if (canBeNull)
        {
            int index = UnityEngine.Random.Range(0, list.Count + 1);
            if (index == list.Count) return default(T);
            else return list[index];
        }
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Returns random item from the array
    /// </summary>
    public static List<T> GetRandomValues<T>(this IList<T> list, int count)
    {
        if (count >= list.Count) return new List<T>(list);
        int num;
        List<int> possible = new List<int>();
        for (int i = 0; i < list.Count; ++i)
        {
            possible.Add(i);
        }
        List<T> result = new List<T>();

        for (int i = 0; i < count; i++)
        {
            num = possible.GetRandomValue();
            possible.Remove(num);
            result.Add(list[num]);
        }
        return result;
    }

    public static TKey GetRandomKey<TKey, TValue>(this Dictionary<TKey, TValue>.KeyCollection keys)
    {
        TKey[] array = new TKey[keys.Count];
        keys.CopyTo(array, 0);
        if (keys.Count > 0)
            return array.GetRandomValue();
        else return default;
    }

    public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
    {
        List<T> result = new List<T>(listToClone.Count);
        foreach (var item in listToClone)
        {
            result.Add((T)item.Clone());
        }
        return result;
    }

    public static bool FullyContainedIn<T>(this IList<T> smallerList, IList<T> biggerList, Dictionary<T, int> lookUp)
    {
        if (smallerList == null || biggerList == null || smallerList.Count != biggerList.Count)
            return false;
        if (smallerList.Count == 0)
            return true;
        int count = 0;
        lookUp.Clear();
        // create index for the first list
        for (int i = 0; i < smallerList.Count; i++)
        {
            if (!lookUp.TryGetValue(smallerList[i], out count))
                lookUp.Add(smallerList[i], 1);
            else
                lookUp[smallerList[i]] = count + 1;
        }
        for (int i = 0; i < biggerList.Count; i++)
        {
            count = 0;
            if (!lookUp.TryGetValue(biggerList[i], out count))
            {
                // early exit as the current value in B doesn't exist in the lookUp (and not in ListA)
                return false;
            }
            count--;
            if (count <= 0)
                lookUp.Remove(biggerList[i]);
            else
                lookUp[biggerList[i]] = count;
        }
        // if there are remaining elements in the lookUp, that means ListA contains elements that do not exist in ListB
        return lookUp.Count == 0;
    }

    /// <summary>
    /// Метод получения координат точек соответствующих кривой Безье в Global Space
    /// </summary>
    /// <param name="startPosition">Координаты начальной точки</param>
    /// <param name="finishPosition">Координаты конечной точки</param>
    /// <param name="height">Высота Арки</param>
    /// <param name="pointsNum">Кол-во точек</param>
    /// <param name="absoluteHeight">Высота абсолютная величина, если нет, то задана для 10 едениц расстояния</param>
    /// <returns></returns>
    public static Vector3[] GetPointsOfArc(Vector3 startPosition, Vector3 finishPosition, float height, int pointsNum, bool absoluteHeight = true)
    {
        Vector3[] result = new Vector3[pointsNum + 1];

        if (!absoluteHeight)
            height = (startPosition - finishPosition).magnitude * height / 10;
        // опорная точка, считаем что это точка на середине отрезка между началом и концом, на высоте height
        Vector3 middlePoint = new Vector3(((startPosition.x + finishPosition.x) / 2), ((startPosition.y + finishPosition.y) / 2) + height, ((startPosition.z + finishPosition.z) / 2));

        for (int i = 0; i <= pointsNum; i++)
            result[i] = GetPointOfBezierCurveByIndex(i, startPosition, finishPosition, middlePoint, pointsNum);

        return result;
    }

    public static HashSet<T> GetInSphere<T>(Vector3 position, float radius, int layerMask)
    {
        Collider[] cols = Physics.OverlapSphere(position, radius, layerMask);
        HashSet<T> set = new HashSet<T>();
        T other;
        for (int i = 0; i < cols.Length; i++)
        {
            other = cols[i].GetComponentInParent<T>();
            if (other == null) continue;
            set.Add(other);
        }
        return set;
    }

    /// <summary>
    /// Метод получения координат одной точки на кривой Безье
    /// </summary>
    /// <param name="i">Индекс точки</param>
    /// <param name="startPosition">Начальная точка кривой</param>
    /// <param name="finishPosition">Конечная точка кривой</param>
    /// <param name="middlePoint">Опорная точка</param>
    /// <param name="pointsNum">кол-во точек</param>
    /// <returns>Координаты точки</returns>
    public static Vector3 GetPointOfBezierCurveByIndex(int i, Vector3 startPosition, Vector3 finishPosition, Vector3 middlePoint, int pointsNum)
    {
        float t = i * (1f / pointsNum);
        return ((1 - t) * (1 - t) * startPosition) + (2 * t * (1 - t) * middlePoint) + (t * t * finishPosition);
    }

    public static Vector3 GetPointInsideTheCircle(float radius = 1, float restrictedRadius = 0, float height = 0)
    {
        Vector3 result = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up) * Vector3.right * UnityEngine.Random.Range(restrictedRadius, radius);
        result.y = height;
        return result;
    }

    public static Vector3 GetPointInsideTheCircle(Vector3 initialPosition, float radius = 1, float restrictedRadius = 0, float height = 0)
    {
        Vector3 result = GetPointInsideTheCircle(radius, restrictedRadius, height) + initialPosition;
        result.y = height;
        return result;
    }

    public static float GetValueClampedIn(float minBound, float maxBound, float value, float minValue, float maxValue)
    {
        return maxBound - (((maxBound - minBound) * (maxValue - value)) / (maxValue - minValue));
    }

    public static float GetTwoSignificantDigits(float value)
    {
        float result = value;

        if (value < 10 && value > -10)
            return (float)Math.Round(value, 1);
        else
        {
            int degreeOf10 = (int)Math.Pow(10, ((int)Math.Log10(Math.Abs(value))));
            return Mathf.Round((float)Math.Round(value / degreeOf10, 1) * degreeOf10);
        }
    }

    public static string GetTimeString(float value) => GetTimeString((int)value);

    public static string GetTimeString(int value)
    {
        if (value >= 86400)
            return (value / 86400) + "d" + ((value % 86400 > 3600) ? " " + ((value % 86400) / 3600) + "h" : "");
            //return TimeSpan.FromSeconds(value).ToString(@"d\dhh\h");
        if (value >= 3600)
            return (value / 3600) + "h" + ((value % 3600 > 60) ? " " + ((value % 3600) / 60) + "m" : "");
            //return TimeSpan.FromSeconds(value).ToString(@"hh\h");
        if (value >= 60)
            return (value / 60) + "m " + value % 60 + "s";
            //return TimeSpan.FromSeconds(value).ToString(@"mm\m");

        return value + "s";
    }

    private static List<string> notations = new() { "", "k", "m", "b", "t", "q" };
    private static string additionalNotation = "a";
    /// <summary>
    /// Метод возвращает число, преобразованное в строку вида 000.00n
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetNotesString(double value)
    {
        int magnitude = (int)Math.Log10(value);

        int order = magnitude / 3;
        if (order < 1)
        {
            return System.Math.Round(value).ToString();
            //return value.ToString();
        }

        while (order >= notations.Count)
        {
            ExtendNotations();
        }

        int numOfDigits = magnitude + 1;
        int doubleOfDigitsInLastOrder = 3 - (2 * numOfDigits % 3);

        StringBuilder sb = new StringBuilder();
        sb.Append(value.ToString("0." + new string('#', 339)).Substring(0, doubleOfDigitsInLastOrder + 2));
        sb.Insert(doubleOfDigitsInLastOrder, ".");
        sb.Append(notations[order]);
        return sb.ToString();
    }

    private static void ExtendNotations()
    {
        string nextNotation = GetNextValueForNotations(additionalNotation);
        notations.Add(nextNotation);
        additionalNotation = nextNotation;
    }

    private static string GetNextValueForNotations(string input)
    {
        StringBuilder sb = new StringBuilder(input);
        for (int i = sb.Length - 1; i >= 0; i--)
        {
            if (sb[i] == 'z')
            {
                sb[i] = 'a';
                if (i == 0)
                {
                    sb.Insert(0, 'a');
                    break;
                }
            }
            else
            {
                sb[i]++;
                break;
            }
        }

        return sb.ToString();
    }

    public static double RoundOff(double number, int interval){
        
        int remainder = (int) (number % interval);
        number += (remainder < interval / 2) ? -remainder : (interval - remainder);
        return number;
    }

#if UNITY_EDITOR
    public static void ClearLog()
    {
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
#endif
}
[System.Serializable]
public class Pair<T1, T2>
{
    public T1 Value1;
    public T2 Value2;
}

[System.Serializable]
public class Trio<T1, T2, T3>
{
    public T1 Value1;
    public T2 Value2;
    public T3 Value3;
}
