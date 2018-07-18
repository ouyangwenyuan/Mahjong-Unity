using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using Excel;
using System.Data;
using System.Linq;

public class ExportExcelCode
{
    public static string DYNAMIC_EXCEL_PATH = "/Design/DynamicExcels";
    public static string STATIC_EXCEL_PATH = "/Design/StaticExcels";
    public static string STATIC_CLASS_PATH = "/Scripts/Data/Excel2CS/static/";
    public static string DYNAMIC_CLASS_PATH = "/Scripts/Data/Excel2CS/dynamic/";

    private static List<string> ingores = new List<string>()
    {
        "PlayerInfo",
        "Highscore",
        "ShopOrder",
        "LogCache"
    };
//    [MenuItem("DataTool/GenerateStaticCode")]
    public static void GenerateStaticCode()
    {
        GenerateCode(STATIC_CLASS_PATH, STATIC_EXCEL_PATH);
    }

//    [MenuItem("DataTool/GenerateDynamicCode")]
    public static void GenerateDynamicCode()
    {
        GenerateCode(DYNAMIC_CLASS_PATH, DYNAMIC_EXCEL_PATH);

        //生成动态表配置类 已在PC上测试成功 暂时不实用
        //GenerateDynamicDBInfoCode(DYNAMIC_CLASS_PATH, DYNAMIC_EXCEL_PATH);
    }

    public static void GenerateCode(string classPath,string excelPath)
    {
        FileInfo info;
        FileStream stream;
        IExcelDataReader excelReader;
        DataSet result;

        string[] files = Directory.GetFiles(Application.dataPath + excelPath, "*.xlsx", SearchOption.TopDirectoryOnly);

        try
        {
            string code1;

            foreach (string path in files)
            {
				Debug.Log(path);
                info = new FileInfo(path);
                stream = info.Open(FileMode.Open, FileAccess.Read);
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                result = excelReader.AsDataSet();
                
                int rowCount = result.Tables[0].Rows.Count;
                int colCount = result.Tables[0].Columns.Count;

                string className = info.Name.Substring(0, info.Name.Length - 5);    //去掉.xlsx

                code1 = "";
                
                code1 += "using System;\n";
                code1 += "using System.Collections;\n";
                code1 += "using System.Collections.Generic;\n";
                code1 += "using System.IO;\n";
                code1 += "using UnityEngine;\n";
                code1 += "using SQLite4Unity3d;\n\n";

                code1 += "public class " + className + "\n";
                code1 += "{\n";

                for (int col = 0; col < colCount; col++)
                {
                    string name = result.Tables[0].Rows[0][col].ToString();
                    string type = result.Tables[0].Rows[1][col].ToString();

                    if (name == "id") {
                        code1 += "    [PrimaryKey, AutoIncrement]\n";
                    }

                    code1 += "    public " + type + " " + name + " { get; set; }\n";

                }
                code1 += "\n    public " + className + "()\n";
                code1 += "    {}\n";

                code1 += "}\n";
                DirectoryInfo directory =new DirectoryInfo(Application.dataPath + classPath);
                if (!directory.Exists)
                {
                    directory.Create();
                }
                string class_file = Application.dataPath + classPath + className + ".cs";

                if (IgnoreClass(className, class_file))
                {
                    excelReader.Close();
                    stream.Close();
                    continue;
                }
                WriteClass(class_file, className, code1);
                excelReader.Close();
                stream.Close();
            }
        }
        catch (IndexOutOfRangeException exp)
        {
            Debug.LogError(exp.StackTrace);
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    static bool IgnoreClass(string className,string classFile)
    {
        return ingores.Any(className.Contains) && File.Exists(classFile);
    }

    public static void GenerateDynamicDBInfoCode(string classPath, string excelPath)
    {
        FileInfo info;
        FileStream stream;
        IExcelDataReader excelReader;
        DataSet result;

        string[] files = Directory.GetFiles(Application.dataPath + excelPath, "*.xlsx", SearchOption.TopDirectoryOnly);

        try
        {
            string code1;

            string code2;
            string code3;

            code1 = "";

            code2 = "";
            code3 = "";

            code1 += "using System;\n";
            code1 += "using System.Collections;\n";
            code1 += "using System.Collections.Generic;\n";
            code1 += "using System.IO;\n";
            code1 += "using UnityEngine;\n";
            code1 += "using SQLite4Unity3d;\n\n";

            code1 += "public class TableSQLInfo {\n";
            code1 += "  public List<string> column_list = new List<string>();\n";
            code1 += "  public string create_sql;\n";
            code1 += "  public List<string> insert_list = new List<string>();\n";
            code1 += "}\n";

            code1 += "public class DynamicDBInfo\n";
            code1 += "{\n";
            code1 += "  public Dictionary<string, TableSQLInfo> dic = new Dictionary<string, TableSQLInfo>();\n";

            code1 += "  TableSQLInfo table_info;\n";

            code1 += "  public DynamicDBInfo() {\n";

            foreach (string path in files)
            {
                code2 = "";
                code3 = "";

                Debug.Log(path);
                info = new FileInfo(path);
                stream = info.Open(FileMode.Open, FileAccess.Read);
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                result = excelReader.AsDataSet();

                int rowCount = result.Tables[0].Rows.Count;
                int colCount = result.Tables[0].Columns.Count;

                string className = info.Name.Substring(0, info.Name.Length - 5);    //去掉.xlsx

                code1 += "      table_info = new TableSQLInfo();\n";

                code2 += "\"create table " +　className + "(";

                for (int col = 0; col < colCount; col++)
                {
                    string name = result.Tables[0].Rows[0][col].ToString();
                    string type = result.Tables[0].Rows[1][col].ToString();

                    code1 += "      table_info.column_list.Add(" + "\"" + name + "\"" + ");\n";
                    code1 += "      table_info.column_list.Add(" + "\"" + type + "\"" + ");\n";

                    if (name.Equals("id"))
                    {
                        code2 += "id integer PRIMARY KEY NOT NULL"; 
                    }
                    else {
                        code2 += "," + name;
                    }
                }

                code2 += ")\";\n";

                //////////////////////////////////////////////////////////////////
                for (int i = 3; i < rowCount; i++)
                {
                    code3 += "      table_info.insert_list.Add(" + "\"insert into " + className + " values (";

                    for (int j = 0; j < colCount; j++)
                    {
                        string name = result.Tables[0].Rows[i][j].ToString();
                        string type = result.Tables[0].Rows[1][j].ToString();

                        if (type.Equals("string"))
                        {
                            if (name == "\"\"")
                            {
                                code3 += "\'\\\"\\\"\'";

                            }
                            else {
                                code3 += "\'" + name + "\'";
                            }

                        }
                        else if (type.Equals("int"))
                        {
                            if (string.IsNullOrEmpty(name))
                            {
                                code3 += 0;
                            }
                            else
                            {
                                code3 += name;
                            }
                        }
                        else if (type.Equals("float"))
                        {
                            if (string.IsNullOrEmpty(name))
                            {
                                code3 += 0.0;
                            }
                            else
                            {
                                code3 += name;
                            }
                        }

                        /*
                        if (j == colCount - 1 && i == rowCount - 1)
                        {
                            code3 += ");";
                        }
                        else 
                        */

                        if (j == colCount - 1)
                        {
                            code3 += ")\");\n";
                        }
                        else
                        {
                            code3 += ",";
                        }
                    }
                }
                /////////////////////////////////////////////////////////////////

                code1 += code3 ;

                code1 += "      table_info.create_sql = " + code2;



                code1 += "      dic.Add(" + "\"" + className + "\"" + ", table_info);\n";

                excelReader.Close();
                stream.Close();
            }

            code1 += "    }\n";
            code1 += "}\n";

            DirectoryInfo directory = new DirectoryInfo(Application.dataPath + classPath);
            if (!directory.Exists)
            {
                directory.Create();
            }
            string class_file = Application.dataPath + classPath + "DynamicDBInfo" + ".cs";

            {
                WriteClass(class_file, "DynamicDBInfo", code1);
            }

        }
        catch (IndexOutOfRangeException exp)
        {
            Debug.LogError(exp.StackTrace);
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private static void WriteClass(string path, string className, string code)
    {
        System.IO.File.WriteAllText(path, code, System.Text.UnicodeEncoding.UTF8);
    }
}
