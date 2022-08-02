using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OnEditor : MonoBehaviour
{
    public class ReflectConfigToCS : MonoBehaviour
    {
        [MenuItem("Tool/通过配置文件生成对应C#代码")]
        public static void ConfigToCS()
        {
            ReflectConfigToCS_Windows rW = EditorWindow.GetWindow<ReflectConfigToCS_Windows>("配置文件转换成C#_Data");
            rW.InitConfig();
        }
    }
}
