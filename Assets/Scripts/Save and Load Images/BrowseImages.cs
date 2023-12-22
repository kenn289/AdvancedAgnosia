using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using System.Linq;
using System;
using UnityEngine.Android;
using System.Threading.Tasks;

public class BrowseImages : MonoBehaviour
{
    public enum ImageExtensions
    {
        png,
        jpg,
        jpeg,
    }

    public enum ObjectCategories
    {
        number,
        alphabet,
        objects
    }

    public List<string> Categories = new List<string>();

    public GameObject saveImgPanel;
    public InputField saveImageName;
    public Dropdown imageCategoryDropdown;
    public Button saveImageButton;
    public Button deleteImageButton;
    public Image imageToSave;
    public GameObject itemIcon;

    bool holdingData = false;

    List<FileInfo> imageFiles = new List<FileInfo>();
    public List<string> images = new List<string>();
    Transform itemIconParent;
    CreateAndEditDBScript makeIconsDude => GetComponent<CreateAndEditDBScript>();

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(PopulateFolders());

        imageCategoryDropdown.AddOptions(Categories);
        //RequestExternalStoragePermission();
        itemIconParent  = itemIcon.transform.parent;
    }

    //public IEnumerator PopulateFolders()
    //{
    //    DirectoryInfo directory = new DirectoryInfo("/storage/emulated/0/DCIM/Camera");

    //    if(directory.Exists)
    //    {
    //        var testdir = directory.GetFiles().Where(x => x.Extension.Contains("jpg"));
    //        foreach (var file in testdir)
    //        {
    //            imageFiles.Add(file);
    //        }
    //        imageFiles.OrderBy(x => x.CreationTime);
    //        for (int i = 0; i < imageFiles.Count; i++)
    //        {
    //            images.Add(imageFiles[i].FullName);
    //        }
    //        Debug.Log("Collected data " + directory.FullName);
    //        yield return new WaitForEndOfFrame();

    //        if (imageFiles.Count > 0)
    //        {
    //            for (int i = 0; i < 20; i++)
    //            {
    //                CreateIcon(images[i], imageFiles[i].Name);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Directory does not exist!");
    //    }
    //}

    //public void CreateIcon(string imageAddress, string name)
    //{
    //    GameObject item = Instantiate(itemIcon, itemIconParent);
    //    item.SetActive(true);
    //    item.transform.GetChild(0).gameObject.GetComponent<Text>().text = name;

    //    //this is to load a sprite from resources (Android)
    //    item.GetComponent<Image>().sprite = LoadSprite(imageAddress);
    //    item.GetComponent<Button>().onClick.AddListener(delegate { saveImgPanel.SetActive(true); imageToSave.sprite = LoadSprite(imageAddress); saveImageButton.onClick.AddListener(delegate { AddImageToDB(imageAddress, saveImageName.text); }); });
    //}

    //private Sprite LoadSprite(string path)
    //{
    //    Debug.Log(path);
    //    if (string.IsNullOrEmpty(path)) return null;
    //    if (File.Exists(path))
    //    {
    //        byte[] bytes = File.ReadAllBytes(path);
    //        Texture2D texture = new Texture2D(900, 900, TextureFormat.RGB24, false);
    //        texture.filterMode = FilterMode.Trilinear;
    //        texture.LoadImage(bytes);
    //        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.0f), 100.0f);
    //        return sprite;
    //    }
    //    return null;
    //}

    public async Task PopulateFoldersAsync()
    {
        string directoryPathDCIM = "/storage/emulated/0/DCIM/Camera";
        string directoryPathDownloads = "/storage/emulated/0/Download";
        DirectoryInfo directoryDCIM = new DirectoryInfo(directoryPathDCIM);
        DirectoryInfo directoryDownloads = new DirectoryInfo(directoryPathDownloads);

        if (!directoryDCIM.Exists || !directoryDownloads.Exists)
        {
            Debug.Log("Directory does not exist");
            return;
        }

        if(imageFiles.Count > 0 && holdingData)
        {
            return;
        }

        try
        {
            holdingData = false;
            imageFiles = new List<FileInfo>();
            imageFiles = directoryDCIM.GetFiles()
                .Where(x => x.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)|| x.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.CreationTime)
                .ToList();

            //Debug.Log("Collected data from " + directoryDCIM.FullName);

            imageFiles.AddRange(directoryDownloads.GetFiles()
                .Where(x => x.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || x.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.CreationTime)
                .ToList());

            Debug.Log("Collected data from " + directoryDownloads.FullName);

            await Task.Yield(); // Yield control back to the main thread

            imageFiles.Sort((p1, p2) => p2.CreationTime.CompareTo(p1.CreationTime));

            await LoadAndCreateIconsAsync(imageFiles.Take(30)); // Load and create icons for the first 20 images
        }
        catch (Exception ex)
        {
            Debug.LogError("Error while populating folders: " + ex.Message);
        }
    }

    private async Task LoadAndCreateIconsAsync(IEnumerable<FileInfo> files)
    {
        List<Task<Sprite>> loadingTasks = new List<Task<Sprite>>();

        foreach (var file in files)
        {
            loadingTasks.Add(LoadSpriteAsync(file.FullName, file.Name));
        }

        Sprite[] loadedSprites = await Task.WhenAll(loadingTasks);

        for (int i = 0; i < loadedSprites.Length; i++)
        {
            CreateIcon(loadedSprites[i], files.ElementAt(i).Name);
        }
        holdingData = true;
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
            return sprite;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading sprite: " + ex.Message);
            return null;
        }
    }

    private void CreateIcon(Sprite sprite, string name)
    {
        GameObject item = Instantiate(itemIcon, itemIconParent);
        item.SetActive(true);
        item.GetComponentInChildren<Text>().text = name;

        if (sprite != null)
        {
            item.GetComponent<Image>().sprite = sprite;
        }
        else
        {
            // Handle error or placeholder sprite if loading failed
        }

        // Add click listener
        item.GetComponent<Button>().onClick.AddListener(() => OnIconClick(sprite, name));
    }

    // Additional method for handling icon click
    private void OnIconClick(Sprite sprite, string name)
    {
        saveImgPanel.SetActive(true);
        deleteImageButton.gameObject.SetActive(false);
        imageToSave.sprite = sprite;
        saveImageButton.onClick.RemoveAllListeners();
        saveImageButton.onClick.AddListener(delegate { AddImageToDB((imageFiles.Find(x => x.Name == name).FullName), saveImageName.text, imageCategoryDropdown.captionText.text); });
        // Implement icon click behavior here
    }

    public void AddImageToDB(string ImageAddress, string ImageName, string CategoryName)
    {
        Debug.Log(ImageName + " added to db");
        makeIconsDude.AddElement(ImageAddress, ImageName, CategoryName);
    }

    //private void TraverseDirDFS(DirectoryInfo dir, string spaces)
    //{
    //    try
    //    {
    //        var testdir = dir.GetFiles().Where(x => x.Extension.Contains("jpg"));
    //        foreach (var file in testdir)
    //        {
    //            imageFiles.Add(file);
    //        }

    //        DirectoryInfo[] children = dir.GetDirectories();

    //        Debug.Log($"Access Granted ");

    //        foreach (DirectoryInfo child in children)
    //        {
    //            // Recursively traverse directories, handle potential exceptions
    //            TraverseDirDFS(child, spaces + "  ");
    //        }
    //    }
    //    catch (UnauthorizedAccessException ex)
    //    {
    //        // Handle the exception (print a message, log it, or take necessary action)
    //        Debug.Log($"Access denied: {ex.Message}");
    //    }
    //}

    public List<DirectoryInfo> UpdateDirectoryList(DirectoryInfo root)
    {
        List<DirectoryInfo> folders = new List<DirectoryInfo>();
        if (root.Exists)
        {
            foreach (DirectoryInfo folder in root.GetDirectories())
            {
                //Debug.Log(folder.Name + " added");
                folders.Add(folder);
                folders.AddRange(UpdateDirectoryList(folder));
            }
        }
        return folders;
    }

    public void RequestExternalStoragePermission()
    {
        StartCoroutine(RequestPermissionRoutine());
    }

    private IEnumerator RequestPermissionRoutine()
    {
        // Check if permission is granted
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            // Request permission asynchronously
            Permission.RequestUserPermission(Permission.ExternalStorageRead);

            // Wait for a short duration before checking permission again
            float elapsedTime = 0f;
            while (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) && elapsedTime < 5f)
            {
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // Check if permission is now granted
            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                // Permission granted, proceed with accessing the directory
                PopulateFolders();
                yield return null;
                //yield return StartCoroutine(PopulateFolders());
            }
            else
            {
                // Permission not granted within the allotted time, handle accordingly
                Debug.Log("Permission denied for external storage read.");
            }
        }
        else
        {
            // Permission was already granted, proceed with accessing the directory
            PopulateFolders();
            yield return null;
        }
    }

    async void PopulateFolders()
    {
        await PopulateFoldersAsync();
    }
}
