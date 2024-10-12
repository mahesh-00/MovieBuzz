using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject homeScreen, searchScreen, profileScreen, movieInfoScreen;
    
    [SerializeField] private Button homeBtn, searchBtn, profileBtn;
    [SerializeField] private Sprite home, search, profile;
    [SerializeField] private GameObject watchListAddedScreen;
    [SerializeField] private MovieInfo movieInfo;
    
    private Sprite homeDef, searchDef, profileDef;
    public enum Pages
    {
        homeScreen,
        searchScreen,
        profileScreen
    }

    public Pages currentPage;
    public static UIManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        homeBtn.onClick.AddListener(() => ScreenVisibility(Pages.homeScreen));
        searchBtn.onClick.AddListener(() => ScreenVisibility(Pages.searchScreen));
        profileBtn.onClick.AddListener(() => ScreenVisibility(Pages.profileScreen));
        homeDef = homeBtn.GetComponent<Image>().sprite;
        searchDef = searchBtn.GetComponent<Image>().sprite;
        profileDef = profileBtn.GetComponent<Image>().sprite;
        ScreenVisibility(Pages.homeScreen);
    }

    private void OnDestroy()
    {
        homeBtn.onClick.RemoveAllListeners();
        searchBtn.onClick.RemoveAllListeners();
        profileBtn.onClick.RemoveAllListeners();
    }


    private void ScreenVisibility(Enum currentPage)
    {
        DisableAllScreen();
        ChangeMenuIconToDef();
        switch (currentPage)
        {
            case Pages.homeScreen:
                homeScreen.SetActive(true);
                homeBtn.GetComponent<Image>().sprite = home;
                break;
            case Pages.searchScreen:
                searchScreen.SetActive(true);
                searchBtn.GetComponent<Image>().sprite = search;
                break;
            case Pages.profileScreen:
                profileScreen.SetActive(true);
                profileBtn.GetComponent<Image>().sprite = profile;
                movieInfo.LoadWatchList();
                break;
        }
    }

    private void DisableAllScreen()
    {
        homeScreen.SetActive(false);
        searchScreen.SetActive(false);
        profileScreen.SetActive(false);
        movieInfoScreen.SetActive(false);
    }

    public void DisplayMovieInfoScreen()
    {
        DisableAllScreen();
        movieInfoScreen.SetActive(true);
    }

    public void DisplaySearchMovieScreen()
    {
        
        homeScreen.SetActive(false);
        profileScreen.SetActive(false);
        movieInfoScreen.SetActive(false);
        searchScreen.SetActive(true);
    }

    private void ChangeMenuIconToDef()
    {
        homeBtn.GetComponent<Image>().sprite = homeDef;
        searchBtn.GetComponent<Image>().sprite = searchDef;
        profileBtn.GetComponent<Image>().sprite = profileDef;
    }

    public void ShowMovieToWLScreen()
    {
        StartCoroutine(ShowMovieToWLScreenCor());
    }

    private IEnumerator ShowMovieToWLScreenCor()
    {
        watchListAddedScreen.SetActive(true);
        yield return new WaitForSeconds(2);
        watchListAddedScreen.SetActive(false);
    }
}
