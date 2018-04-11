using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextChangeExample : MonoBehaviour
{
    public string str;
    Text text;
    void Start()
    {
        text = GetComponent<Text>();
        str = EditableStringExtender.AllocateString(20);
    }
    
    void Update()
    {
        str.UnsafeClear();
        str.UnsafeAppend(System.DateTime.Now.Ticks);
        text.text = str;
        //强制重绘Text
        text.cachedTextGenerator.Invalidate();
        text.SetVerticesDirty();
    }
}
