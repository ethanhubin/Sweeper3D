using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class GameLoseTip : MonoBehaviour {

    public event EventHandler HeartBtnClick;
    public event EventHandler ReplayBtnClick;
    // Use this for initialization
    void Start()
    {
        Camera.main.GetComponent<PhysicsRaycaster>().enabled = false;

        RectTransform rect = GetComponent<RectTransform>();
        rect.localScale = new Vector3(1, 0, 1);
        GetComponent<RectTransform>().DOScaleY(1, 0.3f);

        Button nextBtn = transform.Find("HeartButton").GetComponent<Button>();
        nextBtn.onClick.AddListener(onHeartbtnClick);


        Button replayBtn = transform.Find("ReplayButton").GetComponent<Button>();
        replayBtn.onClick.AddListener(onReplaybtnClick);
    }

    public void showText(string text)
    {
        transform.Find("InfoText").GetComponent<Text>().text = text;
    }

    private void onHeartbtnClick()
    {
        HeartBtnClick(this, EventArgs.Empty);
        DoRemove();
    }

    private void onReplaybtnClick()
    {
        ReplayBtnClick(this, EventArgs.Empty);
        DoRemove();
    }

    private void DoRemove()
    {
        GetComponent<RectTransform>().DOScaleY(0, 0.3f).OnComplete(onRemoveComplete);
    }

    private void onRemoveComplete()
    {
        Camera.main.GetComponent<PhysicsRaycaster>().enabled = true;
        GameObject.Destroy(gameObject);
    }
}
