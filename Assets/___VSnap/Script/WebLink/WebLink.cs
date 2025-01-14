using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Collections;
using UnityEngine;

public class WebLink : MonoBehaviour
{
    [Header("Link")]
    private Dictionary<string, string> links = new Dictionary<string, string>();

    // リンクをインスペクターから設定する用のクラス
    [Serializable]
    public class Link
    {
        public string linkID;
        public string url;
    }

    [Header("Links")]
    public Link[] linkList;

    private void Start()
    {
        foreach (var link in linkList)
        {
            links.Add(link.linkID, link.url);
        }
    }

    public void OpenLink(string linkID)
    {
        if (links.ContainsKey(linkID))
        {
            Application.OpenURL(links[linkID]);
        }
        else
        {
            Debug.LogError("Link ID not found: " + linkID);
        }
    }

}