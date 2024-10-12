using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchPreview : MonoBehaviour
{
    public TextMeshProUGUI movieName;
    public TextMeshProUGUI releaseYear;
    public RawImage poster;
    public Button selectMovieBtn;

    private void OnDestroy()
    {
        selectMovieBtn.onClick.RemoveAllListeners();
    }
}
