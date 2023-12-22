using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

public class CreateAndEditDBScript : MonoBehaviour {

	public Text DebugText;

	public GameObject itemIcon;
    public GameObject categoryItem;
    public GameObject fillerItem;

    public Button saveEditImg;
    public Button deleteImg;
    public GameObject saveImgPanel;
    public InputField saveImageName;
    public Dropdown imageCategoryDropdown;
    public Image imageToSave;

    public Transform itemIconParent;

	public string imageFolderName;

    public List<GameObject> dbIcons;

	public List<Sprite> cachedSprites;

    public List<Texture2D> objRefs;

    //public List<string> CategoryNames = new List<string>();

    List<ImageReferences> imageRefs = new List<ImageReferences>();

	//public static List<GamePiece> gamePiecesList;

	// Use this for initialization
	void Start ()
	{
//		if(File.Exists(Path.Combine(Application.persistentDataPath + "/" + imageFolderName)))
		//{
  //          DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/" + imageFolderName);
  //          FileInfo[] info = dir.GetFiles("*.png");
  //          sprites = new Sprite[info.Length];
  //          for (int i = 0; i < info.Length; i++)
  //          {
  //              Debug.Log(info[i].ToString());
  //              sprites[i] = LoadSprite(info[i].ToString());
  //          }
  //      }
		//StartSync();
	}

    public void StartSync()
    {
        var crud = new SQLcrud("ObjectList.db");
        crud.DeleteDB();
        crud.CreateDB();
        foreach(var item in dbIcons)
            Destroy(item);
        //for (int i = 0; i < objRefs.Count; i++)
        //{
        //    AddElement(AssetDatabase.GetAssetPath(objRefs[i]));
        //}

        var Objects = crud.GetObjects();
        //foreach (ImageReferences iR in Objects)
        //{
        //    Debug.Log(iR.ToString());
        //}
        //ToConsole (Objects);
        //gamePiecesList = new List<GamePiece>();
        Debug.Log("starting population");
        //PopulateItemsMenu(Objects);


    }

    public void AddElement(string imgAddress, string imageName = null, string category = null)
    {
		var crud = new SQLcrud("ObjectList.db");
		var Objects = crud.GetObjects();
        //foreach(ImageReferences iR in  Objects)
        //{
        //    Debug.Log(iR.ToString());
        //}
        ImageReferences imgRef = new ImageReferences();
        imgRef.Name = imageName != null ? imageName : Path.GetFileName(imgAddress);
        imgRef.Category = category != null ? category : null;
        imgRef.ImageAddress = imgAddress;
		crud.AddElementTodb(imgRef);
		ToConsole(Objects);
        //PopulateItemsMenu(Objects);
    }

    public void RemoveElement(string imageAddress)
    {
        var crud = new SQLcrud("ObjectList.db");
        var Objects = crud.GetObjects();
        //foreach (ImageReferences iR in Objects)
        //{
        //    Debug.Log(iR.ToString());
        //}
        crud.DeletedbElement(imageAddress);
        CreateMenu();
    }

    public void EditElement(ImageReferences imgr)
    {
        var crud = new SQLcrud("ObjectList.db");
        crud.EditdbElement(imgr);
    }

	public void PopulateItemsMenu(/*IEnumerable<ImageReferences> objects*/)
	{
        PopulateDBView();
   //     var crud = new SQLcrud("ObjectList.db");
   //     var Objects = crud.GetObjects();
   //     foreach (var item in dbIcons)
   //         Destroy(item);
   //     Debug.Log("no of images in db " + Objects.Count());
   //     foreach (var item in Objects)
   //     {
   //         ToConsole(item.ToString());
   //CreateIcon(item);
   //     }
    }

	public void AddGamePieceToList(/*GamePiece gamePiece*/)
    {
		//gamePiecesList.Add(gamePiece);
    }

    async void PopulateDBView()
    {
        await PopulateDBViewAsync();
    }

    public async Task PopulateDBViewAsync()
    {
        //string directoryPath = "/storage/emulated/0/DCIM/Camera";
        //DirectoryInfo directory = new DirectoryInfo(directoryPath);
        var crud = new SQLcrud("ObjectList.db");
        var Objects = crud.GetObjects();
        imageRefs = Objects.ToList();
        Debug.Log("loading objects");
        //foreach (var item in Objects)
        //{
        //    if (!File.Exists(item.ImageAddress))
        //        crud.DeletedbElement(item);
        //}
        
        await LoadAndCreateIconsAsync(Objects);
    }

    private async Task LoadAndCreateIconsAsync(IEnumerable<ImageReferences> files)
    {
        List<Task<Sprite>> loadingTasks = new List<Task<Sprite>>();

        foreach (var file in files)
        {
            loadingTasks.Add(LoadSpriteAsync(file.ImageAddress, file.SeriealNumber.ToString()));
        }
        Debug.Log("loaded");
        Sprite[] loadedSprites = await Task.WhenAll(loadingTasks);
        cachedSprites = loadedSprites.ToList();
        CreateMenu();
        //for (int i = 0; i < loadedSprites.Length; i++)
        //{
        //    CreateIcon(files.ElementAt(i).ImageAddress, files.ElementAt(i).Name, loadedSprites[i]);
        //}
    }

    public void CreateMenu()
    {
        foreach (var item in dbIcons)
            Destroy(item);
        BrowseImages temp = GetComponent<BrowseImages>();
        for (int i = 0; i < temp.Categories.Count; i++)
        {
            Debug.Log("Creating categories");
            CreateCategoryBar(temp.Categories[i]);
            List<ImageReferences> categoryBasedImages = new List<ImageReferences>();
            categoryBasedImages.AddRange(imageRefs.Where(x => x.Category == temp.Categories[i]));
            foreach (ImageReferences igrf in categoryBasedImages)
            {
                CreateIcon(igrf.ImageAddress, igrf.Name, cachedSprites.Find(x => x.name == igrf.SeriealNumber.ToString()));
            }
            int fillerCount = categoryBasedImages.Count % 7;
            Debug.Log(fillerCount);
            CreateFillers(7 - fillerCount);
        }
    }

    private async Task<Sprite> LoadSpriteAsync(string path, string name)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.LogWarning("File not found: " + path);
            return null;
        }

        try
        {
            Debug.Log("loading " + name);
            byte[] bytes = await Task.Run(() => File.ReadAllBytes(path)); // Read file asynchronously
            Texture2D texture = new Texture2D(900, 900, TextureFormat.RGB24, false);
            texture.LoadImage(bytes); // Load texture asynchronously
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.0f), 100.0f);
            sprite.name = name;
            return sprite;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading sprite: " + ex.Message);
            return null;
        }
    }

    public void CreateCategoryBar(string categoryName)
    {
		GameObject item = Instantiate(categoryItem,itemIconParent);
        item.GetComponentInChildren<Text>().text = categoryName;
        item.SetActive(true);
        dbIcons.Add(item);
        CreateFillers(6);
    }

    public void CreateFillers(int fillerCount)
    {
        for(int i = 0; i < fillerCount; i++)
        {
            GameObject item = Instantiate(fillerItem, itemIconParent);
            item.SetActive(true);
            dbIcons.Add(item);
        }
    }

    public void CreateIcon(string imageAddress, string name, Sprite sprite = null)
	{
		GameObject item = Instantiate(itemIcon,itemIconParent);
		//item.GetComponent<RectTransform>().anchoredPosition = itemIcon.GetComponent<RectTransform>().anchoredPosition;
		//item.GetComponent<RectTransform>().localScale = itemIcon.GetComponent<RectTransform>().localScale;
		item.SetActive(true);
        //item.transform.GetChild(0).gameObject.GetComponent<Text>().text = name != null ? name:sprite.name ;

        //this is to load a sprite from resources (Android)
        item.GetComponentInChildren<Image>().sprite = sprite == null ? LoadSprite(imageAddress) : sprite /*Resources.Load<Sprite>(imageData.ImageAddress)*/;

        //This is to load a texture from file system
        //item.GetComponent<Image>().sprite = LoadSprite(Path.Combine(Application.streamingAssetsPath, imageFolderName, imageData.ImageAddress));
        item.GetComponentInChildren<Button>().onClick.AddListener(delegate {OnIconClicked(imageAddress, sprite);});
        //GamePiece gamePieceComponent = item.GetComponent<GamePiece>();
        //gamePieceComponent.gamepieceName = propItem.Name;
        //gamePieceComponent.ID = propItem.RFID;

        //AddGamePieceToList(gamePieceComponent);
        dbIcons.Add(item);
    }
    
    public  void CreateIcon(ImageReferences imageData)
    {
        CreateIcon(imageData.ImageAddress, imageData.Name);
    }

    public void OnIconClicked(string imageAddress, Sprite sprite)
    {
        saveImgPanel.SetActive(true);
        saveEditImg.onClick.RemoveAllListeners();
        saveEditImg.onClick.AddListener(delegate { OnSaveClicked(imageAddress); });
        deleteImg.gameObject.SetActive(true);
        deleteImg.onClick.RemoveAllListeners();
        deleteImg.onClick.AddListener(delegate { OnDeleteClicked(imageAddress); });
        imageToSave.sprite = sprite;

    }

    public void OnSaveClicked(string imageAddress)
    {
        ImageReferences imgr = new ImageReferences();
        imgr.Name = saveImageName.text;
        imgr.ImageAddress = imageAddress;
        imgr.Category = imageCategoryDropdown.captionText.text;
        EditElement(imgr);
        saveImgPanel.SetActive(false);
    }

    public void OnDeleteClicked(string imageAddress)
    {
        RemoveElement(imageAddress);
        saveImgPanel?.SetActive(false); 
    }

    private void ToConsole(IEnumerable<ImageReferences> objects){
		foreach (var item in objects) 
		{
			ToConsole(item.ToString());
		}
	}
	
	private void ToConsole(string msg){
		if(DebugText)
        {
			DebugText.text = System.Environment.NewLine + msg;
		}
	
		//Debug.Log (msg);
	}

    private Sprite LoadSprite(string path)
    {
        //Debug.Log(path);
		if (string.IsNullOrEmpty(path)) return null;
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(900, 900, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Trilinear;
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.0f), 100.0f);
            return sprite;
        }
        return null;
    }

    public void SwitchScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex); 
    }

}


