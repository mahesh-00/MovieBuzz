using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace DS
{
   public struct DataStructure
   {
      [Serializable]
      public struct MovieDetails
      {
         public string Title;
         public string Year;
         public string Rated;
         public string Released;
         public string Runtime;
         public string Genre;
         public string Director;
         public string Writer;
         public string Actors;
         public string Plot;
         public string Language;
         public string Country;
         public string Awards;
         public string Poster;
         public Rating[] Ratings;
         public string Metascore;
         public string ImdbRating;
         public string ImdbVotes;
         public string ImdbID;
         public string Type;
         public string DVD;
         public string BoxOffice;
         public string Production;
         public string Website;
         public bool Response;

         [Serializable]
         public struct Rating
         {
            public string Source;
            public string Value;
         }
      }
      
      [Serializable]
      public struct SearchPreview
      {
         public string Title { get; set; }
         public string Year { get; set; }
         public string imdbID { get; set; }
         public string Type { get; set; }
         public string Poster { get; set; }
      }

      [Serializable]
      public struct SearchResults
      {
         public SearchPreview[] Search { get; set; }
         public string totalResults { get; set; }
         public bool Response { get; set; }
      }
      
      [Serializable]
      public struct  WatchListData
      {
         public List<string> movieIdList;
      }
      [Serializable]
      public class Article
      {
         public Source source;
         public string author;
         public string title;
         public string description;
         public string url;
         public string urlToImage;
         public string publishedAt;
         public string content;
      }

      [Serializable]
      public class Source
      {
         public string id;
         public string name;
      }

      [Serializable]
      public class NewsResponse
      {
         public string status;
         public int totalResults;
         public Article[] articles;
      }

   }
}
