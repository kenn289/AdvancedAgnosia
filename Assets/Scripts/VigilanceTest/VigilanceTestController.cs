using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VigilanceTestController : MonoBehaviour
{
    public List<GameObject> tokens;

    List<GameObject> ImageIcons = new List<GameObject>();

    public List<ImageReferences> images = new List<ImageReferences>();

    public List<Sprite> sprites;

    public GameObject itemIcon;

    public GameObject imageIcon;

    public Dropdown CategoryDropdown;

    public List<Dropdown> objectDropdowns;

    public string currentCategory => CategoryDropdown.captionText.text;

    Transform itemIconParent => itemIcon.transform.parent;

    public string sampleQuestion;

    public Text questionText;

    public int totalCount = 20;

    public float rigthnessFraction = .2f;

    public List<string> correctValue;

    public int[] vigilance = { 0, 0 };

    public List<VigilanceToken> vigilanceTokens = new List<VigilanceToken>();

    List<ImageReferences> correctImages = new List<ImageReferences>();

    bool spritesLoaded = false;

    private void Start()
    {
        List<string> categoryNames = new List<string>();
        categoryNames.Add(BrowseImages.ObjectCategories.number.ToString());
        categoryNames.Add(BrowseImages.ObjectCategories.alphabet.ToString());
        categoryNames.Add(BrowseImages.ObjectCategories.objects.ToString());
        CategoryDropdown.AddOptions(categoryNames);
    }

    public void ComputeTestFactors()
    {
        spritesLoaded = false;
        images.Clear();
        var crud = new SQLcrud("ObjectList.db");
        var Objects = crud.GetObjects();
        foreach(var item in Objects)
        {
            if(item.Category == currentCategory)
                images.Add(item);
        }
        rigthnessFraction *= totalCount;
        List<string> availiableValues = new List<string>();
        for (int j = 0; j < images.Count; j++)
        {
            availiableValues.Add(images[j].Name);
        }
        StartCoroutine(WaitToLoadSprites());
    }

    public void SetParameters()
    {
        correctImages = images.FindAll(x => correctValue.Contains(x.Name));
        PopulateTokens();
        //StartCoroutine(WaitToLoadSprites());
    }

    public IEnumerator WaitToLoadSprites()
    {
        LoadSprites();
        while(!spritesLoaded)
        {
            yield return new WaitForEndOfFrame();
        }
        PopulateImageSelection();
        //PopulateTokens();
    }

    public async Task PopulateSpritesAsync()
    {
        try
        {
            await LoadAndCreateIconsAsync(images); // Load and create icons for the first 20 images
        }
        catch (Exception ex)
        {
            Debug.LogError("Error while populating folders: " + ex.Message);
        }
    }

    private async Task LoadAndCreateIconsAsync(IEnumerable<ImageReferences> files)
    {
        List<Task<Sprite>> loadingTasks = new List<Task<Sprite>>();

        foreach (var file in files)
        {
            loadingTasks.Add(LoadSpriteAsync(file.ImageAddress, file.SeriealNumber.ToString()));
        }

        Sprite[] loadedSprites = await Task.WhenAll(loadingTasks);

        sprites.Clear();
        sprites = loadedSprites.ToList();

        spritesLoaded = true;
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

    public List<VigilanceToken> ShuffleList(List<VigilanceToken> objs)
    {
        List<VigilanceToken> list = new List<VigilanceToken>();
        List<VigilanceToken> originalList = new List<VigilanceToken>();
        originalList.AddRange(objs);
        VigilanceToken temp = null;
        for(int i = 0; i < objs.Count; i++)
        {
            temp = new VigilanceToken();
            temp = originalList[UnityEngine.Random.Range(0, originalList.Count - 1)];
            list.Add(temp);
            originalList.Remove(temp);
        }
        return list;
    }

    async void LoadSprites()
    {
        await LoadAndCreateIconsAsync(images);
    }

    public void SetVigilanceFactor(string token)
    {
        if(!correctValue.Contains(token))
            correctValue.Add(token);
        //questionText.text = sampleQuestion + " " + token;
    }

    public void PopulateImageSelection()
    {
        correctValue.Clear();
        foreach(GameObject gO in ImageIcons)
            Destroy(gO);
        foreach(ImageReferences imgr in images)
        {
            CreateSelectionImage(sprites.Find(x => x.name == imgr.SeriealNumber.ToString()), imgr.Name);
        }
    }

    public void CreateSelectionImage(Sprite sprite, string token)
    {
        GameObject item = Instantiate(imageIcon, imageIcon.transform.parent);
        item.SetActive(true);
        //item.transform.GetChild(0).gameObject.GetComponent<Text>().text = name;

        //this is to load a sprite from resources (Android)
        item.GetComponent<Image>().sprite = sprite /*Resources.Load<Sprite>(imageData.ImageAddress)*/;
        item.GetComponent<Button>().onClick.AddListener(delegate { SetVigilanceFactor(token); item.GetComponent<Button>().interactable = false; });
        ImageIcons.Add(item);
        //Debug.Log("creating icon " + token);
    }

    public void PopulateTokens()
    {
        VigilanceToken temp = new VigilanceToken();
        List<VigilanceToken> correctTokens = new List<VigilanceToken>();
        List<VigilanceToken> wrongTokens = new List<VigilanceToken>();
        List<VigilanceToken> activeTokens = new List<VigilanceToken>();
        int randNum = 0;
        foreach (ImageReferences imgr in images)
        {
            temp = new VigilanceToken();
            temp.sprite = sprites.Find(x=>x.name == imgr.SeriealNumber.ToString());
            temp.tokenValue = imgr.SeriealNumber.ToString();
            vigilanceTokens.Add(temp);
            if(correctImages.Contains(imgr))
            {
                correctTokens.Add(temp);
            }
        }
        Debug.Log(correctValue.ToString());
        int maxVal = vigilanceTokens.Count - 1;
        for(int  i = 0; i < (int)rigthnessFraction; i++)
        {
            randNum = UnityEngine.Random.Range(0, correctImages.Count);
            activeTokens.Add(correctTokens[randNum]);
        }
        Debug.Log(correctTokens.Count());
        foreach(var item in vigilanceTokens)
        {
            if(!correctTokens.Contains(item))
            {
                wrongTokens.Add(item);  
            }
        }
        for (int i = 0; i < totalCount - (int)rigthnessFraction; i++)
        {
            randNum = UnityEngine.Random.Range(0, wrongTokens.Count);
            activeTokens.Add(wrongTokens[randNum]);
        }
        activeTokens = ShuffleList(activeTokens);
        Debug.Log(activeTokens.Count);
        foreach(VigilanceToken token in activeTokens)
            CreateToken(token);
    }

    public void CreateToken(VigilanceToken vT)
    {
        CreateToken(vT.sprite, vT.tokenValue);
    }

    public void CreateToken(Sprite sprite, string token)
    {
        GameObject item = Instantiate(itemIcon, itemIconParent);
        item.SetActive(true);
        //item.transform.GetChild(0).gameObject.GetComponent<Text>().text = name;

        //this is to load a sprite from resources (Android)
        item.GetComponent<Image>().sprite = sprite /*Resources.Load<Sprite>(imageData.ImageAddress)*/;
        item.GetComponent<Button>().onClick.AddListener(delegate { CheckTokenResult(token); item.GetComponent<Button>().interactable = false; });
        tokens.Add(item);
    }

    public void CheckTokenResult(string token)
    {
        if (correctValue.Contains(token))
        {
            vigilance[0]++;
            Debug.Log("CorrectResponse");
        }    
        else
        {
            Debug.Log("WrongResponse");
            vigilance[1]++;
        }
    }

    public class VigilanceToken
    {
        public string tokenValue;
        public Sprite sprite;
        public bool ReturnTokenComaprison(string token)
        {
            if(this.tokenValue == token) 
                return true;
            else
                return false;
        }
    }

    public void SwitchScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}


