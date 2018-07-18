using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using Excel;
using System.Data;
using CMD;

public class ExportExcelDB
{
    public static string STATIC_DBNAME = "ds";//"static";
    public static string DYNAMIC_DBNAME = "dd";//"dynamic";
    public static string DYNAMIC_EXCEL_PATH = "/Design/DynamicExcels";
    public static string STATIC_EXCEL_PATH = "/Design/StaticExcels";

//    [MenuItem("DataTool/GenerateStaticSQL")]
    public static void GenerateStaticSQL()
    {
        GenerateSQL(Application.dataPath + STATIC_EXCEL_PATH, STATIC_DBNAME);
    }

//    [MenuItem("DataTool/GenerateDynamicSQL")]
    public static void GenerateDynamicSQL()
    {
        GenerateSQL(Application.dataPath + DYNAMIC_EXCEL_PATH, DYNAMIC_DBNAME);
    }

    public static void GenerateSQL(string excelPath,string dbtype)
    {
        FileInfo info;
        FileStream stream;
        IExcelDataReader excelReader;
        DataSet result;

        string[] files = Directory.GetFiles(excelPath, "*.xlsx", SearchOption.TopDirectoryOnly);

        try
        {
            string code1 = "";
            //string code2 = "";

            foreach (string path in files)
            {
                info = new FileInfo(path);
                stream = info.Open(FileMode.Open, FileAccess.Read);
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                result = excelReader.AsDataSet();

                int rowCount = result.Tables[0].Rows.Count;
                int colCount = result.Tables[0].Columns.Count;

                string className = info.Name.Substring(0, info.Name.Length - 5);    //去掉.xlsx

                code1 += "drop table " + className + ";\n" ;
                code1 += "create table " + className + "(";

                for (int i = 0; i < colCount; i++) {
                    string name = result.Tables[0].Rows[0][i].ToString();

                    if (name == "id")
                    {
                        code1 += name + " integer PRIMARY KEY NOT NULL";
                    }
                    else
                    {
                        code1 += name;
                    }

                    if (i == colCount - 1)
                    {
                        code1 += ");\n";
                    }
                    else {
                        code1 += ",";
                    }
                }

                for (int i = 3; i < rowCount; i ++ )
                {
                    code1 += "insert into " + className + " values (";

                    for (int j = 0; j < colCount; j++) {

                        string name = result.Tables[0].Rows[i][j].ToString();
                        string type = result.Tables[0].Rows[1][j].ToString();

                        if (type.Equals("string"))
                        {
                            code1 += "\'" + name + "\'";
                        }
                        else if (type.Equals("int"))
                        {
                            if (string.IsNullOrEmpty(name))
                            {
                                code1 += 0;
                            }
                            else
                            {
                                code1 += name;
                            }
                        }
                        else if (type.Equals("float"))
                        {
                            if (string.IsNullOrEmpty(name))
                            {
                                code1 += 0.0;
                            }
                            else
                            {
                                code1 += name;
                            }
                        }

                        if (j == colCount - 1 && i == rowCount - 1) {
                            code1 += ");";
                        }
                        else if (j == colCount - 1)
                        {
                            code1 += ");\n";
                        }
                        else
                        {
                            code1 += ",";
                        }
                    }
                }

                excelReader.Close();
                stream.Close();
            }

            WriteClass(excelPath+"/" + dbtype + ".sql", code1);
        }
        catch (IndexOutOfRangeException exp)
        {
            Debug.LogError(exp.StackTrace);
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private static void WriteClass(string path, string code)
    {
        var utf8WithoutBom = new System.Text.UTF8Encoding(false);

        System.IO.File.WriteAllText(path, code, utf8WithoutBom);
    }

    [MenuItem("DataTool/GenerateStaticDB")]
    public static void GenerateStaticDB()
    {
        ExportExcelCode.GenerateStaticCode();
        GenerateSQL(Application.dataPath + STATIC_EXCEL_PATH, STATIC_DBNAME);
        GenerateDB(STATIC_DBNAME,STATIC_EXCEL_PATH);
    }

    [MenuItem("DataTool/GenerateDynamicDB")]
    public static void GenerateDynamicDB()
    {
        ExportExcelCode.GenerateDynamicCode();
        GenerateSQL(Application.dataPath + DYNAMIC_EXCEL_PATH, DYNAMIC_DBNAME);
        GenerateDB(DYNAMIC_DBNAME,DYNAMIC_EXCEL_PATH);
    }

    public static void GenerateDB(string DBName,string path) {
        DirectoryInfo parent = System.IO.Directory.GetParent(Application.dataPath);
        string projectPath = parent.ToString();
        
        string para = "";

        //string para1 = Application.streamingAssetsPath + "/DB/" + DBName + ".db";//"F:/workspace/MahjongMaster/Assets/StreamingAssets/DB/DB.db";
        string para1 = Application.streamingAssetsPath + "/DB/" + DBName;
        string para2 = Application.dataPath + path + "/" + DBName + ".sql";//"F:/workspace/MahjongMaster/Assets/Design/Excels/DB.sql";
        
        //Process.ProcessCommand("F:\\workspace\\MahjongMaster\\Tools\\SQLite\\sqlite-tools-win32-x86-3180000\\sqlite3.exe", para);
#if UNITY_EDITOR_OSX
        string shFile = projectPath + "/Tools/SQLite/Test/GenerateDB.sh";
        string para3 = projectPath + "/Tools/SQLite/sqlite-tools-osx-x86-3190300/sqlite3";
        para = para1 + " " + para2 + " " + para3;
		Process.ProcessCommand("chmod","777 "+shFile);
        Process.ProcessCommand(shFile, para);
#elif  UNITY_EDITOR_WIN
        string cmd = projectPath + "/Tools/SQLite/Test/GenerateDB.bat";
        string para3 = projectPath + "/Tools/SQLite/sqlite-tools-win32-x86-3180000/sqlite3";
        para = para1 + " " + para2 + " " + para3;
        Process.ProcessCommand(cmd, para);
#endif
		Debug.Log(para);
    }
}
