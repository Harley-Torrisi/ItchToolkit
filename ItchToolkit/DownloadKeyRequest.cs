﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ItchToolkit
{
    /// <summary>
    /// Checks if a download key exists for game and returns it.
    /// </summary>
    public class DownloadKeyRequest
    {
        /// <summary>
        /// Request types compatible with download_keys API Call.
        /// </summary>
        public class RequestType
        {
            /// <summary>
            /// Detailed information on a Downlod_Key.
            /// </summary>
            public class DownloadKey
            {
                /// <summary>
                /// Date Downlod_Key was created
                /// </summary>
                public string dateCreated;
                /// <summary>
                /// Game ID Downlod_Key belongs to
                /// </summary>
                public int gameID;
                /// <summary>
                /// How many times game has been download from specific Downlod_Key
                /// </summary>
                public int downloads;
                /// <summary>
                /// Unique id for that Downlod_Key
                /// </summary>
                public int keyID;
                /// <summary>
                /// Container for information relevant to the user of said Downlod_Key
                /// </summary>
                public ItchUserRequest.RequestType.User owner;
                /// <summary>
                /// The download key itself
                /// </summary>
                public string downloadKey;
            }
            /// <summary>
            /// Types of errors that can be returned from Itch.io API Downlod_Key request
            /// </summary>
            public enum ErrorType
            {
                /// <summary>
                /// {"errors":["invalid key"]}
                /// </summary>
                InvalidKey,
                /// <summary>
                /// {"errors":["game_id must be an integer"]} or {"errors":["invalid game_id"]}
                /// </summary>
                InvalidGameID,
                /// <summary>
                /// {"errors":["no download key found"]}
                /// </summary>
                NoDownloadKey,
                /// <summary>
                /// Custom error, for when the user has passed NULL values to the request.
                /// </summary>
                DetailsMissing,
                /// <summary>
                /// No Error returned
                /// </summary>
                NULL
            }
        }
        /// <summary>
        /// Container for information relevant to requested Downlod_Key
        /// </summary>
        public RequestType.DownloadKey downloadKey;
        /// <summary>
        /// Error returned from Itch.io API Downlod_Key request
        /// </summary>
        public RequestType.ErrorType error = RequestType.ErrorType.NULL;
        /// <summary>
        /// Fetches a download key from and API call.
        /// <para></para>
        /// If it exists for game and returns it.
        /// </summary>
        /// <param name="yourKey"></param>
        /// <param name="gameID"></param>
        /// <param name="userID"></param>
        public DownloadKeyRequest(string yourKey, string gameID, string userID)
        {
            if (string.IsNullOrEmpty(yourKey) || string.IsNullOrEmpty(gameID) || string.IsNullOrEmpty(userID)) { error = RequestType.ErrorType.DetailsMissing; return; }
            string requestURL = "https://itch.io/api/1/" + yourKey + "/game/" + gameID + "/download_keys?user_id=" + userID;
            string jsonResponse = ItchGlobal.GetResposeHTTPAsync(requestURL).GetAwaiter().GetResult();
            switch (jsonResponse)
            {
                case "{\"errors\":[\"invalid key\"]}":
                    error = RequestType.ErrorType.InvalidKey;
                    break;
                case "{\"errors\":[\"game_id must be an integer\"]}":
                    error = RequestType.ErrorType.InvalidGameID;
                    break;
                case "{\"errors\":[\"invalid game_id\"]}":
                    error = RequestType.ErrorType.InvalidGameID;
                    break;
                case "{\"errors\":[\"no download key found\"]}":
                    error = RequestType.ErrorType.InvalidGameID;
                    break;
                default:
                    downloadKey = new RequestType.DownloadKey();
                    downloadKey.owner = new ItchUserRequest.RequestType.User();
                    var jsonObject = System.Json.JsonValue.Parse(jsonResponse);
                    downloadKey.dateCreated = jsonObject["download_key"].ContainsKey("created_at") ? (string)jsonObject["download_key"]["created_at"] : "";
                    downloadKey.gameID = jsonObject["download_key"].ContainsKey("game_id") ? (int)jsonObject["download_key"]["game_id"] : 0;
                    downloadKey.downloads = jsonObject["download_key"].ContainsKey("downloads") ? (int)jsonObject["download_key"]["downloads"] : 0;
                    downloadKey.keyID = jsonObject["download_key"].ContainsKey("id") ? (int)jsonObject["download_key"]["id"] : 0;
                    downloadKey.owner.avatarURL = jsonObject["download_key"]["owner"].ContainsKey("cover_url") ? (string)jsonObject["download_key"]["owner"]["cover_url"] : "";
                    downloadKey.owner.displayName = jsonObject["download_key"]["owner"].ContainsKey("display_name") ? (string)jsonObject["download_key"]["owner"]["display_name"] : "";
                    downloadKey.owner.isDeveloper = jsonObject["download_key"]["owner"].ContainsKey("developer") ? (bool)jsonObject["download_key"]["owner"]["developer"] : false;
                    downloadKey.owner.userID = jsonObject["download_key"]["owner"].ContainsKey("id") ? (int)jsonObject["download_key"]["owner"]["id"] : 0;
                    downloadKey.owner.profileURL = jsonObject["download_key"]["owner"].ContainsKey("url") ? (string)jsonObject["download_key"]["owner"]["url"] : "";
                    downloadKey.owner.isGamer = jsonObject["download_key"]["owner"].ContainsKey("gamer") ? (bool)jsonObject["download_key"]["owner"]["gamer"] : false;
                    downloadKey.owner.userName = jsonObject["download_key"]["owner"].ContainsKey("username") ? (string)jsonObject["download_key"]["owner"]["username"] : "";
                    downloadKey.owner.isPressUser = jsonObject["download_key"]["owner"].ContainsKey("press_user") ? (bool)jsonObject["download_key"]["owner"]["press_user"] : false;
                    downloadKey.downloadKey = jsonObject["download_key"].ContainsKey("key") ? (string)jsonObject["download_key"]["key"] : "";
                    break;
            }
        }
    }
}
