using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 同样的字符串只会生成一份实例
/// （字符串会储存在stringPool内，不清除则无法回收）
/// </summary>
public static class PoolStringExtender
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Test/PoolStringExtender")]
    public static unsafe void DoIt()
    {
        var chars = "13123".ToCharArray();
        string a = PoolStringExtender.AllocatePoolString(chars,true);
        string b = PoolStringExtender.AllocatePoolString(chars,true);
        Debug.Log(a);
        Debug.Log(b);
        Debug.Log((object)a == (object)b);
        Debug.Log((object)a == (object)"13123");
        Debug.Log(PoolStringExtender.stringPool["13123".GetHashCode()]);
    }
#endif

    public static Dictionary<int, string> stringPool = new Dictionary<int, string>();

    public static unsafe string AllocatePoolString(char[] chars, bool intern = false, int startIndex = 0, int length = -1)
    {
        if (length == -1)
            length = chars.Length;

        fixed (char* ptr = chars)
        {
            int hash = GetHashCode(ptr + startIndex, length);
            if (stringPool.ContainsKey(hash))
            {
                return stringPool[hash];
            }
            else
            {
                string newStr = new string(chars, startIndex, length);
                if (intern)
                {
                    newStr = string.Intern(newStr);
                }
                stringPool.Add(hash,newStr);
                return newStr;
            }
        }
    }

    static unsafe int GetHashCode(char* str,int length)
    {
        char* chPtr2 = str;
        char* chPtr3 = (chPtr2 + length) - 1;
        int num = 0;
        while (chPtr2 < chPtr3)
        {
            num = ((num << 5) - num) + chPtr2[0];
            num = ((num << 5) - num) + chPtr2[1];
            chPtr2 += 2;
        }
        chPtr3++;
        if (chPtr2 < chPtr3)
        {
            num = ((num << 5) - num) + chPtr2[0];
        }
        return num;
    }
}
