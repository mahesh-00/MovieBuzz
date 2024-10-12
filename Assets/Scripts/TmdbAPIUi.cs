using System.Collections;
using System.Collections.Generic;
using DS;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TmdbAPIUi : MonoBehaviour
{
    private string omdbApiKey = "76ff5f42";

    private string tmdbApiUrl =
        "https://api.themoviedb.org/3/search/movie?query=war&include_adult=false&language=en-US&page=1";
    private DataStructure.MovieDetails movieDetails;
    
   

    [SerializeField] private TMP_InputField movieNameSearchInput,movieYearInput;
    [SerializeField] private TextMeshProUGUI 
        movieName,
        releaseDate,
        runTime,
        director,
        genre,
        actors,
        plot,
        language,
        imdbRating;

    [SerializeField] private GameObject errorScreen;
    [SerializeField] private RawImage moviePoster;

    // Start is called before the first frame update
    void Start()
    {
        // Search for the movie "Nanban"
       // SearchMovie("Nanban");
    }

    public void SearchMovie()
    {
        string requestUrl = $"{tmdbApiUrl}?apikey={omdbApiKey}&t={UnityWebRequest.EscapeURL(movieNameSearchInput.text)}&y={UnityWebRequest.EscapeURL(movieYearInput.text)}";

        StartCoroutine(SendRequest(requestUrl));
        
    }

    IEnumerator SendRequest(string url)
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
                Debug.Log("OMDB API Response:\n" + www.downloadHandler.text);
                // Deserialize JSON string into MovieDetails struct
                movieDetails = JsonConvert.DeserializeObject<DataStructure.MovieDetails>(www.downloadHandler.text);
                if (movieDetails.Response)
                {
                    DisplayMovieDetails();
                }
                else
                {
                    errorScreen.SetActive(true);
                    Debug.Log("Movie not found");
                }

            }
        }
    }
    
    IEnumerator LoadImageFromURL(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading image: " + www.error);
            }
            else
            {
                // Get the downloaded texture
                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                // Apply the texture to the RawImage component
                moviePoster.texture = texture;
            }
        }
    }
    private void DisplayMovieDetails()
    {
        // Access and use movie details
        // Debug.Log($"Title: {movieDetails.Title}");
        // Debug.Log($"Director: {movieDetails.Director}");
        // Debug.Log($"Genre: {movieDetails.Genre}");
        // Debug.Log($"IMDb Rating: {movieDetails.ImdbRating}");

        movieName.text = movieDetails.Title;
        releaseDate.text = movieDetails.Released;
        runTime.text = movieDetails.Runtime;
        director.text = movieDetails.Director;
        actors.text = movieDetails.Actors;
        genre.text = movieDetails.Genre;
        //plot.text = movieDetails.Plot;
        language.text = movieDetails.Language;
        imdbRating.text = movieDetails.ImdbRating;
        StartCoroutine(LoadImageFromURL(movieDetails.Poster));
        errorScreen.SetActive(false);
    } 
    
   
}
