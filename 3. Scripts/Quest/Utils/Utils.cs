using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public static class Utils
{
    public static T GetAPIData<T>(string s) where T : new()
    {
        T data = new T();
        data = JsonUtility.FromJson<T>(s);

        return data;
    }

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.datas;
    }
    
    private static readonly char[] moneyUnits = GameConstants.UI.MONEY_UNITS.ToCharArray();
    
    public static string MoneyFormat(BigInteger money)
    {
        if (money < GameConstants.UI.MONEY_UNIT_THRESHOLD)
            return money.ToString();

        int unitIndex = 0;
        BigInteger baseValue = money;

        while (baseValue >= GameConstants.UI.MONEY_UNIT_THRESHOLD && unitIndex < moneyUnits.Length)
        {
            baseValue /= GameConstants.UI.MONEY_UNIT_THRESHOLD;
            unitIndex++;
        }
        
        BigInteger divisor = BigInteger.Pow(GameConstants.UI.MONEY_UNIT_THRESHOLD, unitIndex);
        
        decimal decimalValue = (decimal)money / (decimal)divisor;

        // 반올림 대신 내림 처리 (소수점 3자리에서 내림 후 2자리까지만 표시)
        decimal flooredValue = Math.Floor(decimalValue * (decimal)Math.Pow(10, GameConstants.UI.MONEY_DECIMAL_PLACES)) / (decimal)Math.Pow(10, GameConstants.UI.MONEY_DECIMAL_PLACES);

        string formattedValue = flooredValue.ToString($"F{GameConstants.UI.MONEY_DECIMAL_PLACES}");

        string alphabet = unitIndex > 0 ? moneyUnits[unitIndex - 1].ToString() : "";

        return formattedValue + alphabet;
    }
    
    public static IEnumerator BezierMove(GameObject prefab, Vector3 startPosition, Vector3 endPos, float duration)
    {
        var product = ObjectPoolManager.Instance.GetObject(prefab, startPosition ,Quaternion.Euler(0, 0, Random.Range(-90f, 90f)));
        
        var wait = new WaitForFixedUpdate();

        var time = 0f;
        while (time < duration)
        {
            var midPos = (startPosition + endPos) * 0.5f;
            var dir = (endPos - startPosition).normalized;
            var perp = Vector3.Cross(dir, Vector3.forward).normalized;
            var p1 = midPos - perp * 0.5f;
            time += Time.fixedDeltaTime;
            float s = Mathf.Clamp01(time / duration);
            product.transform.position = QuadraticBezier(s, startPosition, p1, endPos);
            yield return wait;
        }
        
        ObjectPoolManager.Instance.ReturnObject(product);

        product.transform.position = endPos;
    }
    
    public static IEnumerator BezierMove(GameObject prefab, Vector3 startPosition, Transform endTransform, float duration)
    {
        var product = ObjectPoolManager.Instance.GetObject(prefab, startPosition ,Quaternion.Euler(0, 0, Random.Range(-90f, 90f)));
        
        var wait = new WaitForFixedUpdate();

        var time = 0f;
        while (time < duration)
        {
            var midPos = (startPosition + endTransform.position) * 0.5f;
            var dir = (endTransform.position - startPosition).normalized;
            var perp = Vector3.Cross(dir, Vector3.forward).normalized;
            var p1 = midPos - perp * 0.5f;
            time += Time.fixedDeltaTime;
            float s = Mathf.Clamp01(time / duration);
            product.transform.position = QuadraticBezier(s, startPosition, p1, endTransform.position);
            yield return wait;
        }
        
        ObjectPoolManager.Instance.ReturnObject(product);

        product.transform.position = endTransform.position;
    }
    
    private static Vector3 QuadraticBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1f - t;
        return u*u*p0 + 2f*u*t*p1 + t*t*p2;
    }
}



[System.Serializable]
public class Wrapper<T>
{
    public T[] datas;
}