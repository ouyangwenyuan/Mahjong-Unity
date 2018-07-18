using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System.IO;

public class StaticDataBaseService  {

    static StaticDataBaseService service;

    public static StaticDataBaseService GetInstance()
    {
        if (service == null)
        {
            //service = new StaticDataBaseService("static.db");
            service = new StaticDataBaseService("ds.db");
        }

        return service;
    }

	private SQLiteConnection _connection;

    public SQLiteConnection Connection
    {
        get { return _connection; }
        set { _connection = value; }
    }


	public StaticDataBaseService(string DatabaseName)
    {

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", "DB/" + DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);
        if (File.Exists(filepath))
        {
            File.Delete(filepath);
        }
        Debug.Log("filepath"+filepath);
        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/DB/" + DatabaseName);  // this is the path to your StreamingAssets in android
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
            //Debug.Log("Else : " + loadDb);
	        // then save to Application.persistentDataPath
	        File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }
        else{
#if UNITY_ANDROID 

            //var loadDb = new WWW(filepath);
            //while (!loadDb.isDone) { }  
            //string md5_persistent = GetMD5.GetFileMD5(loadDb.bytes);
            
            //Debug.Log(filepath);
            //Debug.Log(md5_persistent);
            
            string md5_persistent = GetMD5.GetFileMD5(filepath);
            
            Debug.Log(filepath);
            Debug.Log(md5_persistent);
    
            
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/DB/" + DatabaseName);
            while (!loadDb.isDone) { }  
            
            string md5_streaming = GetMD5.GetFileMD5(loadDb.bytes);
            Debug.Log(md5_streaming);
            
            if (!md5_persistent.Equals(md5_streaming))
            {
                Debug.Log("MD5 no same!!!");

                File.Delete(filepath);

                File.WriteAllBytes(filepath, loadDb.bytes);
            }

#elif UNITY_IOS
            string md5_persistent = GetMD5.GetFileMD5(filepath);

            string md5_streaming = GetMD5.GetFileMD5(Application.dataPath + "/Raw/DB/" + DatabaseName);

            if (!md5_persistent.Equals(md5_streaming))
            {
                File.Delete(filepath);

                var loadDb = Application.dataPath + "/Raw/DB/" + DatabaseName;

                File.Copy(loadDb, filepath);
            }
#elif UNITY_WP8
            string md5_persistent = GetMD5.GetFileMD5(filepath);

            string md5_streaming = GetMD5.GetFileMD5(Application.dataPath + "/StreamingAssets/DB/" + DatabaseName);

            if (!md5_persistent.Equals(md5_streaming))
            {
                File.Delete(filepath);

                var loadDb = Application.dataPath + "/StreamingAssets/DB/" + DatabaseName;

                File.Copy(loadDb, filepath);
            }
#elif UNITY_WINRT
            string md5_persistent = GetMD5.GetFileMD5(filepath);

            string md5_streaming = GetMD5.GetFileMD5(Application.dataPath + "/StreamingAssets/DB/" + DatabaseName);

            if (!md5_persistent.Equals(md5_streaming))
            {
                File.Delete(filepath);

                var loadDb = Application.dataPath + "/StreamingAssets/DB/" + DatabaseName;

                File.Copy(loadDb, filepath);
            }
#else
            string md5_persistent = GetMD5.GetFileMD5(filepath);

            string md5_streaming = GetMD5.GetFileMD5(Application.dataPath + "/StreamingAssets/DB/" + DatabaseName);

            if (!md5_persistent.Equals(md5_streaming))
            {
                File.Delete(filepath);

                var loadDb = Application.dataPath + "/StreamingAssets/DB/" + DatabaseName;

                File.Copy(loadDb, filepath);
            }
#endif            
        }

        var dbPath = filepath;
#endif

            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);        
        Debug.Log("Final PATH: " + dbPath);     
	}

    //Static Data

    public IEnumerable<ChapterInfo> GetChapterInfo()
    {
        return _connection.Table<ChapterInfo>();
    }

    public IEnumerable<GameStr> GetGameStr()
    {
        return _connection.Table<GameStr>();
    }

    public IEnumerable<ImgRes> GetImgRes()
    {
        return _connection.Table<ImgRes>();
    }

    public IEnumerable<Item> GetItem()
    {
        return _connection.Table<Item>();
    }

    public IEnumerable<Payment> GetPayment()
    {
        return _connection.Table<Payment>();
    }

    public IEnumerable<Shop> GetPurchase()
    {
        return _connection.Table<Shop>();
    }

    public IEnumerable<Stage> GetStages()
    {
        return _connection.Table<Stage>();
    }
    public IEnumerable<Tips> GetTips()
    {
        return _connection.Table<Tips>();
    }

    public IEnumerable<BuyItem> GetBuyItem()
    {
        return _connection.Table<BuyItem>();
    }
}
