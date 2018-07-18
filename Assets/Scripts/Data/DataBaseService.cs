using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

public class DataBaseService  {

    static DataBaseService service;

    public static DataBaseService GetInstance()
    {
        if (service == null)
        {
            service = new DataBaseService("dynamic.db");
        }

        return service;
    }

	private SQLiteConnection _connection;

    public SQLiteConnection Connection
    {
        get { return _connection; }
        set { _connection = value; }
    }

    public DataBaseService(string DatabaseName){

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        Debug.Log("aaaaaaaaaaaaaaaaaaaaa");
        Debug.Log("filepath"+filepath);
        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/DB/" + DatabaseName);  // this is the path to your StreamingAssets in android
            Debug.Log("UNITY_ANDROID " + "jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);
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
    Debug.Log("CCCCCC " + loadDb);
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        
        Debug.Log("Final PATH: " + dbPath);     
	}

	public void CreateDB(){
		_connection.DropTable<Person> ();
		_connection.CreateTable<Person> ();

		_connection.InsertAll (new[]{
			new Person{
				Id = 1,
				Name = "Tom",
				Surname = "Perez",
				Age = 56
			},
			new Person{
				Id = 2,
				Name = "Fred",
				Surname = "Arthurson",
				Age = 16
			},
			new Person{
				Id = 3,
				Name = "John",
				Surname = "Doe",
				Age = 25
			},
			new Person{
				Id = 4,
				Name = "Roberto",
				Surname = "Huertas",
				Age = 37
			}
		});
	}

	public IEnumerable<Person> GetPersons(){
		return _connection.Table<Person>();
	}

	public IEnumerable<Person> GetPersonsNamedRoberto(){
		return _connection.Table<Person>().Where(x => x.Name == "Roberto");
	}

	public Person GetJohnny(){
		return _connection.Table<Person>().Where(x => x.Name == "Johnny").FirstOrDefault();
	}

    public PlayerInfo GetByID()
    {
        return _connection.Table<PlayerInfo>().Where(x => x.id == 1).FirstOrDefault();
    }

	public Person CreatePerson(){
		var p = new Person{
				Name = "Johnny",
				Surname = "Mnemonic",
				Age = 21
		};
		_connection.Insert (p);
		return p;
	}

    public IEnumerable<TalkBorder> GetTalkBorder(int chapter)
    {
        return _connection.Table<TalkBorder>().Where(x => x.chapterid == chapter);
    }

    public IEnumerable<DefaultScore> GetDefaultScore(int level)
    {
        return _connection.Table<DefaultScore>().Where(x => x.level == level);
    }

    public IEnumerable<LevelData> GetLevelData(int level)
    {
        return _connection.Table<LevelData>().Where(x => x.id == level);
    }

    public IEnumerable<ScoreRecord> GetScoreRecord()
    {
        return _connection.Table<ScoreRecord>();
    }

    public void InsertData(object obj) {
        _connection.Insert(obj);
    }

    public int updateData(object obj)
    {
        return _connection.Update(obj);
    }

    public ScoreRecord GetScoreRecordByLv(int lv) {
        return _connection.Table<ScoreRecord>().Where(x => x.id == lv).FirstOrDefault();    
    }
}
