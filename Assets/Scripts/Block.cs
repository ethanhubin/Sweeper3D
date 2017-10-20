using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public GameObject prefabMine;
    public GameObject prefabFlag;
    public GameObject prefabLabel;
    public GameObject prefabHover;

    public List<Material> terrains;

    public List<Sprite> numList;
    public List<Material> blockMtrs;
    public int x;
    public int y;
    public bool sweeped = false;
    public bool flaged = false;
    public bool isMine = false;
    public int num = 0;

    public GameObject flagModel;
    public GameObject hover;
    public StageManager stageManager;

    // Use this for initialization
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setTerrain(int type)
    {
        MeshRenderer r = GetComponent<MeshRenderer>();
        r.material = terrains[type];
    }

    public void setSweeped(bool bSweeped)
    {
        sweeped = bSweeped;

        float ty = sweeped ? -0.1f : 0;
        transform.DOLocalMoveY(ty, 0.2f);

        MeshRenderer r = GetComponent<MeshRenderer>();
        r.material = sweeped ? blockMtrs[1] : blockMtrs[0];
    }
    public void setFlaged(bool bFlaged)
    {
        flaged = bFlaged;
        if (flaged)
        {
            flagModel = Instantiate(prefabFlag, transform, false);
        }
        else
        {
            if (flagModel != null)
            {
                GameObject.Destroy(flagModel);
                flagModel = null;
            }
        }
    }

    public void showLabel(int number)
    {
        num = number;
        Instantiate(prefabLabel,transform,false).GetComponent<SpriteRenderer>().sprite = numList[number];        
        setSweeped(true);
    }

    public void showMine()
    {
        Instantiate(prefabMine, transform, false);
        setSweeped(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        hover = Instantiate(prefabHover, transform, false);        
        stageManager.OnBlockPointerDown(eventData,this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameObject.Destroy(hover);
        hover = null;
        stageManager.onBlockPointerUp(eventData,this);
    }
}
