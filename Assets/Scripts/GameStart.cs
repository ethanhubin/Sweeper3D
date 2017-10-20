using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameConfig.init();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void LoadClassicGame()
    {
        GameConfig.gameType = GameType.CLASSIC;
        SceneManager.LoadScene("ClassicMode");
    }

    public void LoadAdventureGame()
    {
        GameConfig.gameType = GameType.ADVENTURE;
        // SceneManager.LoadScene("AdventureMode");
    }
}
