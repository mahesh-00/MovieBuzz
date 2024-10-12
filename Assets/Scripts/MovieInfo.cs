using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DS;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MovieInfo : MonoBehaviour
{
    
    [Header("MovieInfo elements")]
    [SerializeField] private TextMeshProUGUI movieName;
    [SerializeField] private TextMeshProUGUI releaseDate;
    [SerializeField] private TextMeshProUGUI runTime;
    [SerializeField] private TextMeshProUGUI director;
    [SerializeField] private TextMeshProUGUI genre;
    [SerializeField] private TextMeshProUGUI actors;
    [SerializeField] private TextMeshProUGUI plot;
    [SerializeField] private TextMeshProUGUI language;
    [SerializeField] private TextMeshProUGUI imdbRating;
    [SerializeField] private RawImage moviePoster;

    [Header("Search Preview elements")] 
    [SerializeField] private Transform searchPreviewParent;
    [SerializeField] private GameObject searchPreviewPrefab;
    [SerializeField] private GameObject errorScreen;
    
    
    [Header("WatchList elements")]
    [SerializeField] private WatchList watchList;
    [SerializeField] private GameObject watchListScreen;
    [SerializeField] private GameObject watchListPrefab;
    [SerializeField] private GameObject watchListParent;
    
    private string omdbApiKey = "76ff5f42";
    private string omdbApiUrl = "http://www.omdbapi.com/";
    private DataStructure.MovieDetails movieDetails;
    private bool _isReqProcessed;
    private string currentMovieId;
    

    public void SearchMovie(string movieId)
    {
        string requestUrl = $"{omdbApiUrl}?apikey={omdbApiKey}&i={movieId}";
        Debug.Log(requestUrl +"movieToShow");
        StartCoroutine(DisplayMovieDetails(requestUrl));
        
    }

    private IEnumerator DisplayMovieDetails(string reqUrl)
    {
        var movieData = new CoroutineWithData(this, SendMovieRequest(reqUrl));
        yield return movieData.coroutine;
        movieDetails = JsonConvert.DeserializeObject<DataStructure.MovieDetails>((string)movieData.result);
        if (movieDetails.Response)
        {
            // Access and use movie details
            // Debug.Log($"Title: {movieDetails.Title}");
            // Debug.Log($"Director: {movieDetails.Director}");
            // Debug.Log($"Genre: {movieDetails.Genre}");
            // Debug.Log($"IMDb Rating: {movieDetails.ImdbRating}");
            UIManager.Instance.DisplayMovieInfoScreen();
            movieName.text = movieDetails.Title;
            releaseDate.text = movieDetails.Released;
            runTime.text = movieDetails.Runtime;
            director.text = movieDetails.Director;
            actors.text = movieDetails.Actors;
            genre.text = movieDetails.Genre;
            //plot.text = movieDetails.Plot;
            language.text = movieDetails.Language;
            imdbRating.text = movieDetails.ImdbRating;
            var posterImg = new CoroutineWithData(this, LoadImageFromURL(movieDetails.Poster));
            yield return posterImg.coroutine;
            moviePoster.texture = (Texture)posterImg.result;
            errorScreen.SetActive(false);
            currentMovieId = movieDetails.ImdbID;
        }
        else
        {
            errorScreen.SetActive(true);
            Debug.Log("Movie not found");
        }
    }
    
    
    //Sends web request and returns a string
    private IEnumerator SendMovieRequest(string url)
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
                // Parse and handle the response data (for simplicity, printing the raw response here)
                //Debug.Log("OMDB API Response:\n" + www.downloadHandler.text);
                // Deserialize JSON string into MovieDetails struct
                yield return www.downloadHandler.text;
               

            }
        }
    } 
    
    //Sends web request and returns a texture
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
                yield return texture;

            }
        }
    }
    
    private IEnumerator SendPreviewRequest(string url)
    {
        var searchPreviewCoroutine = new CoroutineWithData(this, SendMovieRequest(url));
        yield return searchPreviewCoroutine.coroutine;
        var searchDetails =
            JsonConvert.DeserializeObject<DataStructure.SearchResults>((string)searchPreviewCoroutine.result);
       
        if (searchDetails.Response)
        {
            foreach (var element in searchDetails.Search)
            {
                var posterImg = new CoroutineWithData(this, LoadImageFromURL(element.Poster));
                yield return posterImg.coroutine;
                if (posterImg.result != null)
                {
                    var previewGo = Instantiate(searchPreviewPrefab, searchPreviewParent).GetComponent<SearchPreview>();
                    previewGo.movieName.text = element.Title;
                    previewGo.releaseYear.text = element.Year;
                    previewGo.selectMovieBtn.onClick.AddListener(() => SearchMovie(element.imdbID));
                    previewGo.poster.texture = (Texture)posterImg.result;
                }
            }
        }
        else
        {
            errorScreen.SetActive(true);
            Debug.Log("Movie not found");
        }

        _isReqProcessed = false;
    }

    
   

    #region Button click functions
    public void DisplaySearchResult(string movieName)
    {
        UIManager.Instance.DisplaySearchMovieScreen();
        errorScreen.SetActive(false);
        if (searchPreviewParent.childCount>0)
        {
            for (int i = 0; i < searchPreviewParent.childCount; i++)
            {
                Destroy(searchPreviewParent.GetChild(i).gameObject);
            }
        }
        string requestUrl = $"{omdbApiUrl}?apikey={omdbApiKey}&s={UnityWebRequest.EscapeURL(movieName)}&type=movie";
        if (!_isReqProcessed)
        {
           
            StartCoroutine(SendPreviewRequest(requestUrl));
            _isReqProcessed = true;
        }
        else
        {
            Task.Delay(1000);
            StartCoroutine(SendPreviewRequest(requestUrl));
            _isReqProcessed = true;
        }
        
    }
    
    public void AddToWatchList()
    {
        UIManager.Instance.ShowMovieToWLScreen();
        watchList.SaveWatchListData(currentMovieId);
    }
    
    public void LoadWatchList()
    {
        StartCoroutine(DisplayWatchList());
    }

    private IEnumerator DisplayWatchList()
    {
        watchListScreen.SetActive(true);
        while (watchListParent.transform.childCount>0)
        {
            Destroy(watchListParent.transform.GetChild(0).gameObject);
            yield return new WaitForEndOfFrame();
        }
        var watchListMovies = watchList.LoadWatchListData();
        foreach (var movieId in watchListMovies.movieIdList)
        {
            var requestUrl = $"{omdbApiUrl}?apikey={omdbApiKey}&i={movieId}";
            var watchListCoroutine = new CoroutineWithData(this, SendMovieRequest(requestUrl));
            yield return watchListCoroutine.coroutine;
            var watchListMovie = JsonConvert.DeserializeObject<DataStructure.MovieDetails>((string)watchListCoroutine.result);
            if (watchListMovie.Response)
            {
                var watchListUI = Instantiate(watchListPrefab, watchListParent.transform).GetComponent<WatchListUI>();
                watchListUI.title.text = watchListMovie.Title;
                var posterImg = new CoroutineWithData(this, LoadImageFromURL(watchListMovie.Poster));
                yield return posterImg.coroutine;
                watchListUI.poster.texture = (Texture)posterImg.result;
                watchListUI.selectMovieButton.onClick.AddListener(()=>SearchMovie(watchListMovie.ImdbID));

            }
            //Debug.Log(watchListMovie.Title);

        }
    }
    #endregion
    
   
}
