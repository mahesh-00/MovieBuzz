using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WatchListUI : MonoBehaviour
{
   public RawImage poster;
   public TextMeshProUGUI title;
   public Button selectMovieButton;

   private void OnDestroy()
   {
      selectMovieButton.onClick.RemoveAllListeners();
   }
}
