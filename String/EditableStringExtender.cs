using UnityEngine;

/// <summary>
/// 修改字符串内容
/// （谨记不要超出字符串原有大小）
/// </summary>
public static class EditableStringExtender
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Test/EditableStringExtender")]
    public static unsafe void DoIt()
    {
        string a = EditableStringExtender.AllocateString(20);
        a.UnsafeAppend("abc");
        a.UnsafeAppend(123);
        Debug.Log(a);
    }
#endif

    public static string AllocateString(int length)
    {
        string str = new string((char)0, length);
        str.UnsafeClear();
        return str;
    }

    public static unsafe void UnsafeAppend(this string str, char ch)
    {
        UnsafeInsert(str, str.Length, ch);
    }
    public static unsafe void UnsafeAppend(this string str, char[] chars, int startIndex = 0, int length = -1)
    {
        UnsafeInsert(str, str.Length, chars, startIndex, length);
    }
    public static unsafe void UnsafeAppend(this string str, string chars, int startIndex = 0, int length = -1)
    {
        UnsafeInsert(str, str.Length, chars, startIndex, length);
    }
    public static unsafe void UnsafeAppend(this string str, long num)
    {
        UnsafeInsert(str, str.Length, num);
    }

    public static unsafe void UnsafeInsert(this string str, int index, char ch)
    {
        UnsafeInsert(str, index, &ch, 1);
    }
    public static unsafe void UnsafeInsert(this string str, int index, char[] chars, int startIndex = 0, int length = -1)
    {
        if (length == -1)
            length = chars.Length;
        fixed (char* ptr = chars)
        {
            UnsafeInsert(str, index, ptr + startIndex, length);
        }
    }
    public static unsafe void UnsafeInsert(this string str, int index, string chars, int startIndex = 0, int length = -1)
    {
        if (length == -1)
            length = chars.Length;
        fixed (char* ptr = chars)
        {
            UnsafeInsert(str, index, ptr + startIndex, length);
        }
    }

    public static unsafe void UnsafeInsert(this string str, int index, char* v, int length)
    {
        fixed (char* ptr = str)
        {
            char* cptr = ptr + index;
            StringCopy(cptr, cptr + length, str.Length + 1 - index);

            for (int i = 0; i < length; i++)
                *(cptr + i) = *(v + i);

            int* iptr = (int*)ptr - 1;
            *iptr = *iptr + length;
        }
    }

    public static unsafe void UnsafeInsert(this string str, int index, long num)
    {
        int length;
        LongToChars(num, out length);
        UnsafeInsert(str, index, charCache, charCache.Length - length, length);
    }

    public static unsafe void UnsafeClear(this string str)
    {
        fixed (char* ptr = str)
        {
            int* iptr = (int*)ptr - 1;
            *iptr = 0;
        }
    }

    public static unsafe void UnsafeRemove(this string str, int index = 0, int length = -1)
    {
        int strLength = str.Length;
        if (index >= strLength)
            return;

        int maxLength = strLength - index;
        if (length > maxLength || length == -1)
            length = maxLength;

        int endIndex = index + length;
        fixed (char* ptr = str)
        {
            StringCopy(ptr, ptr - length, strLength + 1 - endIndex);
            
            int* iptr = (int*)ptr - 1;
            *iptr = *iptr - length;
        }
    }

    //unsafe delegate void MemCpyImpl(byte* src, byte* dest, int len);
    //static MemCpyImpl memcpyimpl = (MemCpyImpl)Delegate.CreateDelegate(typeof(MemCpyImpl), typeof(Buffer).GetMethod("Memmove", BindingFlags.Static | BindingFlags.NonPublic));

    static unsafe void StringCopy(char* src, char* dest, int len)
    {
        if (dest < src)
        {
            for (int i = 0; i < len; i++)
            {
                *(dest + i) = *(src + i);
            }
        }
        else
        {
            for (int i = len - 1; i >= 0; i--)
            {
                *(dest + i) = *(src + i);
            }
        }
    }

    static char[] charCache = new char[20];
    static void LongToChars(long num, out int length)
    {
        int i = 0;
        int endIndex = charCache.Length - 1;
        while (num > 0)
        {
            charCache[endIndex - i] = (char)(0x30 + num % 10);
            num /= 10;
            i++;
        }
        length = i;
    }
}