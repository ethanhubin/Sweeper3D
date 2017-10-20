using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Text;
using System.IO;


public enum GameType{
    CLASSIC,
    ADVENTURE
}

public struct StageData
{
    public string info;
    public int width;
    public int height;
    public int mineCount;
    public int[,] mapdata;
    public StageData(int w, int h, int count, string text, int[,] data)
    {
        width = w;
        height = h;
        mineCount = count;
        info = text;
        mapdata = data;
    }
}

public class GameConfig : MonoBehaviour {
    public static List<StageData> classicStageData;
    public static List<StageData> adventureStageData;
    public static StageData currentStageData;
    public static GameType gameType;

    public static void init()
    {
        Debug.Log("init game config ");
        loadResources();
    }

    public static void SetCurrentStage(int i)
    {
        if (gameType == GameType.CLASSIC)
        {
            currentStageData = classicStageData[i];
        }
        else if(gameType == GameType.ADVENTURE)
        {
            currentStageData = adventureStageData[i];
        }       
    }

    protected static void loadResources()
    {
        TextAsset assets = Resources.Load("mapdata") as TextAsset;
        if (assets != null)
        {           
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(assets.text);
            classicStageData = GetLevelData(xmlDoc.SelectSingleNode("mapdata"), "classic");
            adventureStageData = GetLevelData(xmlDoc.SelectSingleNode("mapdata"), "adventure");
        }
    }

    protected static List<StageData> GetLevelData(XmlNode xmlDoc,string type)
    {
        List<StageData> levelData = new List<StageData>();

        XmlNodeList levelNodes = xmlDoc.SelectSingleNode(type).ChildNodes;
        foreach (XmlElement xe in levelNodes)
        {
            int w = int.Parse(xe.GetAttribute("width"));
            int h = int.Parse(xe.GetAttribute("height"));
            int count = int.Parse(xe.GetAttribute("minecount"));
            string text = xe.SelectSingleNode("info").InnerText;
            XmlNodeList rowdata = xe.SelectSingleNode("data").ChildNodes;

            
            int[,] mdata = new int[h, w];
            int j = 0;
            foreach (XmlElement row in rowdata)
            {
                string s = row.InnerText;
                for (int i = 0; i < s.Length; i++)
                {
                    mdata[j, i] = int.Parse(s.Substring(i, 1));
                }
                j++;
            }

            levelData.Add(new StageData(w, h, count, text, mdata));
        }
        Debug.Log("load level data:"+type+","+levelData.Count);
        return levelData;
    }
}
