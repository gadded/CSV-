using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OnEditor : MonoBehaviour
{
    public class ReflectConfigToCS : MonoBehaviour
    {
        [MenuItem("Tool/ͨ�������ļ����ɶ�ӦC#����")]
        public static void ConfigToCS()
        {
            ReflectConfigToCS_Windows rW = EditorWindow.GetWindow<ReflectConfigToCS_Windows>("�����ļ�ת����C#_Data");
            rW.InitConfig();
        }
    }
}
