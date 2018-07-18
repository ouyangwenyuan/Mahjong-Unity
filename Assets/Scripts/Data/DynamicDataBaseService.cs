using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using Assets.Scripts.Data.Excel2CS;
using System;
using System.Linq;
using Assets.Scripts.Data.Excel2CS.dynamic;
using Assets.Scripts.Utils;

public class DynamicDataBaseService  {

    static DynamicDataBaseService service;

    public static DynamicDataBaseService GetInstance()
    {
        if (service == null)
        {
            //service = new DynamicDataBaseService("dynamic.db");
            service = new DynamicDataBaseService("dd");
        }

        return service;
    }

	private SQLiteConnection _connection;

    public SQLiteConnection Connection
    {
        get { return _connection; }
        set { _connection = value; }
    }

    public DynamicDataBaseService(string DatabaseName){

#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", "DB/" + DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        var filepath1 = string.Format("{0}/{1}", Application.persistentDataPath, "dynamic.db");

        Debug.Log("filepath"+filepath);

        if (!File.Exists(filepath))
        {
            if(File.Exists(filepath1)){

#if UNITY_ANDROID
 
            //var loadDb = new WWW(filepath1);  // this is the path to your StreamingAssets in android
            //Debug.Log("UNITY_ANDROID " + "jar:file://" + Application.dataPath + "!/assets/DB/" + DatabaseName);
            //Debug.Log("UNITY_ANDROID " + loadDb);
            //while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            //File.WriteAllBytes(filepath, loadDb.bytes);

            File.Copy(filepath1, filepath);
#elif UNITY_IOS

                //var loadDb = Application.dataPath + "/Raw/DB/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                //Debug.Log("UNITY_IOS " + loadDb);
                // then save to Application.persistentDataPath
                File.Copy(filepath1, filepath);

#elif UNITY_WP8

                //var loadDb = Application.dataPath + "/StreamingAssets/DB/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                //Debug.Log("UNITY_WP8 " + loadDb);
                // then save to Application.persistentDataPath
                File.Copy(filepath1, filepath);

#elif UNITY_WINRT

		//var loadDb = Application.dataPath + "/StreamingAssets/DB/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
        /Debug.Log("UNITY_WINRT " + loadDb);
		// then save to Application.persistentDataPath
		File.Copy(filepath1, filepath);

#else

	//var loadDb = Application.dataPath + "/StreamingAssets/DB/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
    
	// then save to Application.persistentDataPath
	File.Copy(filepath1, filepath);

#endif
                File.Delete(filepath1);
                
            }
            else
            {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/DB/" + DatabaseName);  // this is the path to your StreamingAssets in android
            Debug.Log("UNITY_ANDROID " + "jar:file://" + Application.dataPath + "!/assets/DB/" + DatabaseName);
            Debug.Log("UNITY_ANDROID " + loadDb);
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);

#elif UNITY_IOS

                var loadDb = Application.dataPath + "/Raw/DB/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                Debug.Log("UNITY_IOS " + loadDb);
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WP8

                var loadDb = Application.dataPath + "/StreamingAssets/DB/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                Debug.Log("UNITY_WP8 " + loadDb);
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT

		var loadDb = Application.dataPath + "/StreamingAssets/DB/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
        Debug.Log("UNITY_WINRT " + loadDb);
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);

#else

	var loadDb = Application.dataPath + "/StreamingAssets/DB/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
    
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif
            }
            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        UpdateDatabase();

        //动态更新动态表，已在PC上测试成功，暂时不使用
        /*
        DynamicDBInfo info = new DynamicDBInfo();

        List<string> be_added = new List<string>();

        foreach(KeyValuePair<string , TableSQLInfo> kvp in info.dic)
        {
            be_added.Clear();

            List<SQLite4Unity3d.SQLiteConnection.ColumnInfo> table_info = _connection.GetTableInfo(kvp.Key);

            if (table_info.Count == 0)
            {
                //没有表 创建表

                string sql_cmd = kvp.Value.create_sql;

                try
                {
                    _connection.CreateCommand(sql_cmd).ExecuteNonQuery();//每次只能执行一条SQL语句
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }

                foreach (string insert_cmd in kvp.Value.insert_list) {
                    try
                    {
                        _connection.CreateCommand(insert_cmd).ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }    
                }

            }
            else{
                //看看 有没有新字段 有的话 加进去
                for (int i = 0; i < kvp.Value.column_list.Count; i += 2 )
                {
                    string column_name = kvp.Value.column_list[i];

                    bool b_be_added = false;

                    foreach (SQLite4Unity3d.SQLiteConnection.ColumnInfo c_i in table_info)
                    {
                        if (c_i.Name.Equals(column_name))
                        {
                            b_be_added = true;

                            break;
                        }
                    }

                    if(!b_be_added){
                        be_added.Add(column_name);
                    }
                }

                if (be_added.Count > 0)
                {
                    //string alter_sql_cmd = "alter table Highscore add column headicon";
                    string alter_sql_cmd = "alter table " + kvp.Key;

                    for (int i = 0; i < be_added.Count; i++ )
                    {
                        if(i == be_added.Count - 1){
                            alter_sql_cmd += " add column " + be_added[i];
                        }
                        else{
                            alter_sql_cmd += " add column " + be_added[i] + ",";
                        }
                    }

                    try
                    {
                        _connection.CreateCommand(alter_sql_cmd).ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
        }*/
                
        Debug.Log("Final PATH: " + dbPath);     
	}

    void UpdateDatabase()
    {
        List<string> sqlList=new List<string>()
        {
            "create table LogCache(id integer PRIMARY KEY NOT NULL,JsonData,UpdateTime,Action);",
            "alter table Highscore add column deadpoint_I",
            "alter table Highscore add column deadpoint_X",
        };
        try
        {
            foreach (var sql in sqlList)
            {
                _connection.CreateCommand(sql).ExecuteNonQuery();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void ResetDatabases()
    {
        Connection.DeleteAll<PlayerInfo>();
        Connection.DeleteAll<LeaderBoard>();
        Connection.DeleteAll<Highscore>();
        Connection.DeleteAll<LogCache>();

        string sqlResetPlayerInfo = "insert into PlayerInfo values (1,1,5,'0',5,'0',50,0,0,0,0,0,'0','0','0');";
        try
        {
            _connection.CreateCommand(sqlResetPlayerInfo).ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        Debug.Log("reset complete");
    }

    public IEnumerable<FriendData> GetFriendData()
    {
        return _connection.Table<FriendData>();
    }

    public IEnumerable<Highscore> GetHighscores()
    {
        return _connection.Table<Highscore>();
    }

    public IEnumerable<PlayerInfo> GetPlayerInfo()
    {
        return _connection.Table<PlayerInfo>();
    }

    public IEnumerable<ShopOrder> GetShopOrders()
    {
        return _connection.Table<ShopOrder>();
    }

    public void InsertData(object obj)
    {
        _connection.Insert(obj);
    }

    public int UpdateData(object obj)
    {
        if (obj is PlayerInfo)
        {
            var playerInfo = obj as PlayerInfo;
            playerInfo.player_update = PlayerInfoUtil.GetTimeStamp();
            return _connection.Update(playerInfo);
        }
        return _connection.Update(obj);
    }

    public int UpdateData(object obj,bool update)
    {
        if (obj is PlayerInfo && update)
        {
            var playerInfo = obj as PlayerInfo;
            playerInfo.player_update = PlayerInfoUtil.GetTimeStamp();
            return _connection.Update(playerInfo);
        }
        return _connection.Update(obj);
    }

    public void InsertOrReplace(object obj)
    {
        _connection.InsertOrReplace(obj);
    }

    public IEnumerable<LeaderBoard> GetBoardInfo()
    {
        return _connection.Table<LeaderBoard>();
    }

    public IEnumerable<LogCache> GetLogCache()
    {
        return _connection.Table<LogCache>();
    }
}
