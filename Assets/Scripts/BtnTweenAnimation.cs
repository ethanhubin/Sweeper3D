using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BtnTweenAnimation : MonoBehaviour {
    public float start_time;
    public float duration;
    
    void Start () {
        RectTransform t = GetComponent<RectTransform>();
        Vector3 pos = t.anchoredPosition;  
        t.anchoredPosition = new Vector2(pos.x - 1080, pos.y);
        t.DOAnchorPos(pos, duration).SetDelay(start_time);        
	}
}
