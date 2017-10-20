using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassicSelect : MonoBehaviour {

	
	void Start () {
		
	}
	
	public void LoadGameStage(int level)
    {

        GameConfig.SetCurrentStage(level);
        SceneManager.LoadScene("GameStage");
    }
}
