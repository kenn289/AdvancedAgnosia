using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using UnityEngine.Networking;
using System.Linq;
using System;

public class SQLcrud 
{
	public SQLiteConnection _connection;
	public SQLcrud(string DatabaseName)
    {

#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName); // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
        Debug.Log("Final PATH: " + dbPath);
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

    }

    public void CreateDB()
	{
		_connection.DropTable<ImageReferences>();
		_connection.CreateTable<ImageReferences>();

		//_connection.InsertAll(new[]{
		//	new ImageReferences{
		//		Name = "Cube",
		//		SeriealNumber = 0
		//	},
		//	new ImageReferences{
		//		Name = "Sphere",
		//		SeriealNumber = 1

		//	}
		//});
	}

    public void DeleteDB()
    {
        _connection.DropTable<ImageReferences>();
    }

    public void DeletedbElement(string imageAddress)
    {
        var itemToDelete = _connection.Table<ImageReferences>().FirstOrDefault(item => item.ImageAddress == imageAddress);

        if (itemToDelete != null)
        {
            _connection.Delete(itemToDelete); // Delete the object from the database
        }
        else
            Debug.Log("NOT DELETED");
    }

    public void EditdbElement(ImageReferences imageReference)
    {
        var itemToEdit = _connection.Table<ImageReferences>().FirstOrDefault(item => item.ImageAddress == imageReference.ImageAddress);

        if (itemToEdit != null)
        {
            itemToEdit.Name = imageReference.Name;
            itemToEdit.Category = imageReference.Category;
            _connection.Update(itemToEdit);
        }
    }

    public void AddElementTodb(ImageReferences newImage)
    {
        _connection.Insert(newImage);
    }

	public IEnumerable<ImageReferences> GetObjects()
    {
        return _connection.Table<ImageReferences>();
    }
}

public class ImageReferences
{

    [AutoIncrement, PrimaryKey]
    public int SeriealNumber { get; set; }
    public string Name { get; set; }
    public string ImageAddress { get; set; }
    public string Category { get; set; }

    public override string ToString()
    {
        return string.Format("[Object: Name={0}, address={1}]", Name, ImageAddress);
    }
}
