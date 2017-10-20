using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Xml;
using System.Text;
using System.IO;
using UnityEngine.SceneManagement;

public class StageLoader : MonoBehaviour
{
    public Transform uiCanvas;
    public Transform stageCanvas;

    public Text mineCountLabel;
    public Text timeLabel;

    public GameObject prefabStageManager;
    public GameObject prefabWinTip;
    public GameObject prefabLoseTip;

    private StageManager stageManager;    

    [HideInInspector]
    public float timeSpend;
    public bool isPlaying;
    // Use this for initialization
    void Start () {        
        newStage();
    }

    // Update is called once per frame
    void Update () {
        if (isPlaying)
        {
            timeSpend += Time.deltaTime;
            int hour = (int)timeSpend / 3600;
            int minute = ((int)timeSpend - hour * 3600) / 60;
            int second = (int)timeSpend - hour * 3600 - minute * 60;
            int millisecond = (int)((timeSpend - (int)timeSpend) * 100);

            timeLabel.text = string.Format("{0:D2}:{1:D2}:{2:D2}", minute, second, millisecond);
        }
    }

    public void ContinueStage()
    {
        isPlaying = true;
    }
    public void PauseStage()
    {
        isPlaying = false;
    }

    public void ReplayStage()
    {        
        removeStage();
        newStage();
    }

    public void ExitStage()
    {
        removeStage();
        SceneManager.LoadScene("Start");
    }

    public void NextStage()
    {
        removeStage();
        newStage();
    }
    
    private void newStage()
    {
        //Vector3 pos = stageCanvas.transform.position;

        stageManager = Instantiate(prefabStageManager,stageCanvas).GetComponent<StageManager>();

        //stageManager.transform.position = new Vector3(pos.x, pos.y,10);
        
        
        stageManager.WalkOnMine += GameManager_WalkOnMine;
        stageManager.FlagOnMine += GameManager_FlagOnMine;
        stageManager.GameWin += StageManager_GameWin;

        stageManager.NewStage(GameConfig.currentStageData);
        
        timeSpend = 0;
        isPlaying = true;

        updateGameInfo();
    }

    private void updateGameInfo()
    {
        mineCountLabel.text = stageManager.flagedCount + "/" + GameConfig.currentStageData.mineCount;
    }

    private void StageManager_GameWin(object sender, System.EventArgs e)
    {
        Debug.Log("Game Win!");        
        showWinTip();
    }

    private void GameManager_FlagOnMine(object sender, System.EventArgs e)
    {
        Debug.Log("Flag On Mine!");
        updateGameInfo();
    }

    private void GameManager_WalkOnMine(object sender, System.EventArgs e)
    {
        Debug.Log("Walk On Mine!");
        showLoseTip();
        updateGameInfo();
    }

    private void showLoseTip()
    {
        GameLoseTip loseTip = Instantiate(prefabLoseTip, uiCanvas).GetComponent<GameLoseTip>();
        loseTip.ReplayBtnClick += LoseTip_ReplayBtnClick;
        loseTip.HeartBtnClick += LoseTip_HeartBtnClick;

        loseTip.showText("mine : " + stageManager.flagedCount + "/" + GameConfig.currentStageData.mineCount + "    time : " + (int)timeSpend + "s");

        PauseStage();
    }

    private void LoseTip_HeartBtnClick(object sender, System.EventArgs e)
    {
        ContinueStage();
        stageManager.checkWin();
    }

    private void LoseTip_ReplayBtnClick(object sender, System.EventArgs e)
    {
        ReplayStage();
    }

    private void showWinTip()
    {
        GameWinTip tip = Instantiate(prefabWinTip, uiCanvas).GetComponent<GameWinTip>();
        tip.ReplayBtnClick += Tip_ReplayBtnClick;
        tip.NextBtnClick += Tip_NextBtnClick;
        tip.showText("moves : "+ stageManager.moves +"    time : "+ (int)timeSpend+"s");
        PauseStage();
    }

    private void Tip_NextBtnClick(object sender, System.EventArgs e)
    {
        NextStage();
    }

    private void Tip_ReplayBtnClick(object sender, System.EventArgs e)
    {
        ReplayStage();
    }

    public void removeStage()
    {
        stageManager.WalkOnMine -= GameManager_WalkOnMine;
        stageManager.FlagOnMine -= GameManager_FlagOnMine;
        stageManager.GameWin -= StageManager_GameWin;

        Destroy(stageManager.gameObject);
        stageManager = null;
    }
}