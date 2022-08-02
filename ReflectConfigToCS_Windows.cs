using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
internal class ReflectConfigToCS_Windows : EditorWindow
{
    private string JsonFileName;
    private string PathFileName;
    string biaoti =
@"/************************************************************************
该文件是通过自动生成的，禁止手动修改
作者：
日期：#1
*************************************************************************/";
    string selectFileStr;
    string selectFolder;
    ///
    /// 初始化配置数据
    ///
    public void InitConfig()
    {

    }

    private void OnGUI()
    {
        GUILayout.Label("请输入文件名：");
        JsonFileName = EditorGUILayout.TextField(JsonFileName);
        if (GUILayout.Button("请选择一个配置文件"))
        {
            selectFileStr = EditorUtility.OpenFilePanel("", Application.dataPath, "csv");
            //UnityEngine.Debug.Log(selectFileStr);
        }
        if (GUILayout.Button("请选择存储位置："))
        {
            selectFolder = EditorUtility.OpenFolderPanel("", Application.dataPath, "");
        }
        if (GUILayout.Button("生成"))
        {
            if (string.IsNullOrEmpty(JsonFileName))
            {
                UnityEngine.Debug.Log("未设置配置文件名字");
                return;
            }
            if (string.IsNullOrEmpty(selectFileStr))
            {
                UnityEngine.Debug.Log("未选择配置文件");
                return;
            }
            UnityEngine.Debug.Log(selectFileStr);
            ReaderConfigFile(selectFileStr);
        }
    }

    /// <summary>
    /// 读取配置文件
    /// </summary>
    /// <param name="path">文件路径</param>
    void ReaderConfigFile(string path)
    {
        string[] fileStr = File.ReadAllLines(path);
        PathFileName = path;
        //UnityEngine.Debug.Log(fileStr);
        CreateCS(fileStr);
    }

    /// <summary>
    /// 创建C#文件
    /// </summary>
    /// <param name="reflectFileName">反射成CS的文件名</param>
    void CreateCS(string[] reflectFileName)
    {
        /************ 写入配置路径位置与创建的文件写入流 ************/
        string CSPath = $"{selectFolder + "/"}{JsonFileName}.cs";
        StreamWriter sw = new StreamWriter(CSPath);

        /************ 设置一些写入的格式符与变量 ************/
        //写入的行以\为换行符  \t==tab
        string tabKey = "\t";
        //参数名称
        string[] argumentName = reflectFileName[0].Split(',');
        //参数类型
        string[] argumentType = reflectFileName[1].Split(',');

        //string[] argumentList = reflectFileName[1].Split(',');

        string time = DateTime.Now.ToString();
        //替换为标题中的具体时间
        sw.WriteLine(biaoti.Replace("#1", time));
        //写入头文件
        sw.WriteLine(GetImport());
        /************ 正式在配置流文件里开始写入代码配置 ************/
        sw.WriteLine($"public class {JsonFileName}");
        sw.WriteLine("{");
        //遍历参数列表，生成配置
        for (int i = 0; i < argumentType.Length; i++)
        {
            argumentType[i] = argumentType[i].ToLower();
            sw.WriteLine($"{tabKey}public {argumentType[i]} {argumentName[i]};");
        }

        sw.WriteLine("}");

        //生成解析csv文件函数
        sw.WriteLine($"public class JsonToCsv{JsonFileName}");
        sw.WriteLine("{");
        sw.WriteLine($"{tabKey}public List<{JsonFileName}> JsonToCsvOpen(List<{JsonFileName}> list)");
        sw.WriteLine($"{tabKey}" + "{");
        sw.WriteLine($"{tabKey}{tabKey}string json = \"{PathFileName}\";");
        sw.WriteLine($"{tabKey}{tabKey}string[] fileStr = File.ReadAllLines(json);");
        sw.WriteLine($"{tabKey}{tabKey}for (int i = 3; i < fileStr.Length; i++)" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}string[] list_open = fileStr[i].Split(',');");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}{JsonFileName} jsons = new {JsonFileName}();");
        for (int i = 0; i < argumentType.Length; i++)
        {
            //当前不同类型表头定义不同类型
            if (argumentType[i] == "int")
            {
                sw.WriteLine($"{tabKey}{tabKey}{tabKey}jsons.{argumentName[i]} = int.Parse(list_open[{i}]);");
            }
            else if (argumentType[i] == "string")
            {
                sw.WriteLine($"{tabKey}{tabKey}{tabKey}jsons.{argumentName[i]} = list_open[{i}];");
            }
            else if (argumentType[i] == "float")
            {
                sw.WriteLine($"{tabKey}{tabKey}{tabKey}jsons.{argumentName[i]} = float.Parse(list_open[{i}]);");
            }
        }
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}list.Add(jsons);" + "}");
        sw.WriteLine($"{tabKey}{tabKey}return list;");
        sw.WriteLine($"{tabKey}" + "}");
        sw.WriteLine("}");



        sw.WriteLine($"public class {JsonFileName}Manager");
        sw.WriteLine("{");

        sw.WriteLine($"{tabKey}public static List<{JsonFileName}> {JsonFileName}_list = new List<{JsonFileName}>();");
        sw.WriteLine($"{tabKey}public {JsonFileName}Manager()"+"{");
        sw.WriteLine($"{tabKey}{tabKey}JsonToCsv{JsonFileName} jsonTo=new JsonToCsv{JsonFileName}();");
        sw.WriteLine($"{tabKey}{tabKey}jsonTo.JsonToCsvOpen({JsonFileName}_list);");
        sw.WriteLine($"{tabKey}" + "}");

        //查找对应的元素
        sw.WriteLine($"{tabKey}public static {JsonFileName} GetData(int id)" + "{");
        sw.WriteLine($"{tabKey}{tabKey}for (int i = 0; i < {JsonFileName}_list.Count; i++)" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}if ({JsonFileName}_list[i].Id==id)");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}{tabKey}return {JsonFileName}_list[i];" + "}");
        sw.WriteLine($"{tabKey}{tabKey}return null;" + "}");
        //添加元素
        sw.WriteLine($"{tabKey}public static void AddData({JsonFileName} data)" + "{");
        sw.WriteLine($"{tabKey}{tabKey}if(data==null)" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}return;" + "}");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}if ({JsonFileName}_list.Contains(data))" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}{tabKey}{JsonFileName}_list.Add(data);");
        sw.WriteLine($"{tabKey}{tabKey}" + "}");
        sw.WriteLine("}");
        //删除元素
        sw.WriteLine($"{tabKey}public static void RevData({JsonFileName} data)" + "{");
        sw.WriteLine($"{tabKey}{tabKey}for (int i = 0; i < {JsonFileName}_list.Count; i++)" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}if ({JsonFileName}_list[i].Id==data.Id)");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}{tabKey}{JsonFileName}_list.Remove(data);" + "}");
        sw.WriteLine("}");
        //修改元素
        sw.WriteLine($"{tabKey}public static void ChangeData({JsonFileName} data)" + "{");
        sw.WriteLine($"{tabKey}{tabKey}for (int i = 0; i < {JsonFileName}_list.Count; i++)" + "{");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}if ({JsonFileName}_list[i].Id==data.Id)");
        sw.WriteLine($"{tabKey}{tabKey}{tabKey}{tabKey}{JsonFileName}_list[i]=data;" + "}");
        sw.WriteLine("}");
        sw.WriteLine("}");
        sw.Flush();
        sw.Close();
        AssetDatabase.Refresh();
        Process.Start(CSPath);
    }
    /// <summary>
    /// 加载调用数据
    /// </summary>
    /// <returns></returns>
    string GetImport()
    {
        string importStr = null;
        importStr += $"using UnityEngine;\r\n";
        importStr += $"using UnityEngine.UI;\r\n";
        importStr += $"using System;\r\n";
        importStr += $"using System.Collections;\r\n";
        importStr += $"using UnityEditor;\r\n";
        importStr += $"using System.IO;\r\n";
        importStr += $"using System.Collections.Generic;\r\n";
        return importStr;
    }


}