using UnityEngine;
using System.Collections;
using DanielLochner.Assets.SimpleScrollSnap;
using DS;
using TMPro;
using UnityEngine.Networking;


public class NewsAPI : MonoBehaviour
{
    [SerializeField] private NewsData newsDataPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private SimpleScrollSnap snapScroll;
    [SerializeField] private TMP_Dropdown dropdownCategory;
    [SerializeField] private int maxLength = 2500;
    
    private DataStructure.NewsResponse _newsApiResponse;
    private NewsData newsData;
    private const string _apiKey ="d99480b9d9fe485b86a7708329f7206c";
    private string requestUrl;
    private string nextPageUrl;
    private string _nextPageId;
    private bool isFeedUpdated;
    private int pageNum = 10;
    private int previousPageNum = 0;
    
    
    private void Start()
    {
         requestUrl = $"https://newsapi.org/v2/everything?q=world&apiKey={_apiKey}&language=en";
         StartCoroutine(StartFetch());
      

    }

    private void Update()
    {
        
    }

    private IEnumerator StartFetch()
    {
        var sendWebReqCoroutine = new CoroutineWithData(this, SendWebRequest(requestUrl));
        yield return new WaitForSeconds(4);
        StartCoroutine(UpdateNewData());
    }
    
    private IEnumerator SendWebRequest(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                _newsApiResponse = JsonUtility.FromJson<DataStructure.NewsResponse>(www.downloadHandler.text);
                Debug.Log("Total Results: " + _newsApiResponse.totalResults);
                StartCoroutine(DisplayNews());
            }
        }
    }

    private IEnumerator DisplayNews()
    {
        Debug.Log("coroutine called");
        for (int i = previousPageNum; i < pageNum && i< _newsApiResponse.totalResults; i++)
        {
            
            var loadImgCoroutine = new CoroutineWithData(this,
                LoadImageFromURL(_newsApiResponse.articles[i].urlToImage));
            yield return loadImgCoroutine.coroutine;
            if (loadImgCoroutine.result != null)
            {
                //Debug.Log(_newsApiResponse.results.Length);
                newsData = Instantiate(newsDataPrefab, contentParent);

                newsData.title.text = _newsApiResponse.articles[i].title;
                var contentStr = TruncateToLength(_newsApiResponse.articles[i].content, maxLength);
                newsData.content.text = contentStr;
                //Debug.Log(_newsApiResponse.results[i].content.Length +" cont length");
              //  Debug.Log("published "+ _newsApiResponse.articles[i].publishedAt);
                newsData.url.text = _newsApiResponse.articles[i].url;
                newsData.urlButton.onClick.AddListener(() => OpenUrl());
                newsData.newsPoster.texture = (Texture)loadImgCoroutine.result;
            }
        }

        previousPageNum = pageNum;
        pageNum += 10;
        isFeedUpdated = false;
    }

    private IEnumerator UpdateNewData()
    {
        //Debug.Log("curr item "+ snapScroll.currentItem);
        if (snapScroll.SelectedPanel  == contentParent.childCount-3 && !isFeedUpdated)
        {
            
            //StartFetchNextPage();
            StartCoroutine(SendWebRequest(requestUrl));
            isFeedUpdated = true;
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(UpdateNewData());
    }

    private IEnumerator LoadImageFromURL(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading image: " + www.error);
                yield return null;
            }
            else
            {
                // Get the downloaded texture
                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                // Apply the texture to the RawImage component
                yield return texture;
            }
        }
    }
    
    private string TruncateToLength(string input, int maxLength)
    {
        if (input.Length <= maxLength)
        {
            // If the input string is already shorter than the max length, no need to truncate
            return input;
        }
        else
        {
            // Truncate the string and append "..." to indicate it has been truncated
            return input.Substring(0, maxLength) + "...";
        }
    }

    private void OpenUrl()
    {
        var uri = contentParent.GetChild(snapScroll.SelectedPanel).gameObject.GetComponent<NewsData>().url.text;
        Application.OpenURL(uri);
    }

    public void SearchNews(string newsContent)
    {
        previousPageNum = 0;
        pageNum = 10;
        foreach (Transform go in contentParent)
        {
            Destroy(go.gameObject);
        }
        requestUrl = $"https://newsapi.org/v2/everything?q={newsContent}&apiKey={_apiKey}&language=en";
        StartCoroutine(SendWebRequest(requestUrl));
    }
  
}
