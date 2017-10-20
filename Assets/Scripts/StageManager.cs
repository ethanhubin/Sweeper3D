using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageManager : MonoBehaviour
{
    public event EventHandler WalkOnMine;
    public event EventHandler FlagOnMine;
    public event EventHandler GameWin;

    [HideInInspector]
    public bool bDragable;

    public int[,] currentStageMapdata;

    [HideInInspector]
    public int flagedCount;

    [HideInInspector]
    public int moves;

    public GameObject prefabBlock;
    private List<Block> blockList;
    private Block[,] blockTable;    

    private int[,] dir = new int[8, 2]
    {
        { -1,-1},
        { 0,-1},
        { 1,-1},
        { 1,0},
        { 1,1},
        { 0,1},
        { -1,1},
        { -1,0}
    };

    // Use this for initialization
    void Start () {
   
	}

    public void NewStage(StageData levelData)
    {
        currentStageMapdata = levelData.mapdata;
        createMap(currentStageMapdata);        
        genaratonMineData(0, 0, levelData.mineCount);
        moves = 0;
    }

    void createMap(int[,] mapdata)
    {
        int h = mapdata.GetLength(0);
        int w = mapdata.GetLength(1);
        Debug.Log("create map :" + h + "x" + w);

        blockList = new List<Block>();
        blockTable = new Block[h, w];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (mapdata[y, x] >=0 )
                {
                    GameObject b = Instantiate(prefabBlock, transform, false);
                    b.transform.localPosition = new Vector3(x, 0, y);
                    Block mb = b.GetComponent<Block>();
                    mb.setTerrain(mapdata[y,x]);
                    mb.stageManager = this;
                    blockList.Add(mb);
                    blockTable[y, x] = mb;
                    mb.isMine = false;
                    mb.x = x;
                    mb.y = y;
                }
            }
        }
    }

    void genaratonMineData(int x, int y, int mineCount)
    {
        int m = 0;
        while (m < mineCount)
        {
            int r = UnityEngine.Random.Range(0, blockList.Count);
            Block b = blockList[r];
            if ((b.x != x || b.y != y) && (!b.isMine))
            {
                b.isMine = true;
                m++;
            }
        }
    }

    private Block getBlockAt(int x,int y)
    {
        if (x>=0 && x < blockTable.GetLength(1) && y>=0 && y < blockTable.GetLength(0)) return blockTable[y, x];
        else return null;
    }

    private int getMineCountArround(Block block)
    {
        int m = 0;
        for (int d = 0; d < 8; d++)
        {
            Block b = getBlockAt(block.x + dir[d, 0], block.y + dir[d, 1]);
            if (b != null && b.isMine) m++;
        }
        return m;
    }

    public void sweepAt(Block block)
    {
        if (block.sweeped) return;
        moves++;

        if (!block.isMine)
        {
            //没雷
            mineSweep(block);

            checkWin();
        }
        else
        {
            walkOnMine(block);
            //有雷
        }

        Debug.Log("sweep At:" + block.x + "," + block.y);
                
    }

    public void flagAt(Block block)
    {
        if (!block.flaged)
        {
            block.setFlaged(true);
            flagedCount++;
            FlagOnMine(this, EventArgs.Empty);
            checkWin();
        }
    }

    public void cancelFlagAt(Block block)
    {
        if (block.flaged)
        {
            block.setFlaged(false);
            flagedCount--;
            FlagOnMine(this, EventArgs.Empty);
        }
    }


    private void mineSweep(Block block)
    {
        if (block.sweeped) return;

        int m = getMineCountArround(block);
        if (block.flaged) cancelFlagAt(block);
        block.showLabel(m);

        if (m == 0)
        {
            for (int d = 0; d < 8; d++)
            {
                Block b = getBlockAt(block.x + dir[d, 0], block.y + dir[d, 1]);
                if (b != null) mineSweep(b);
            }
        }
    }

    private void sweepAround(Block block)
    {

        int flagednum = 0;
        for (int d = 0; d < 8; d++)
        {
            Block b = getBlockAt(block.x + dir[d, 0], block.y + dir[d, 1]);
            if (b != null)
            {
                if (b.flaged) flagednum++;
                if (b.sweeped && b.isMine) flagednum++;
            }
        }

        if(flagednum == block.num)
        {
            for (int d = 0; d < 8; d++)
            {
                Block b = getBlockAt(block.x + dir[d, 0], block.y + dir[d, 1]);
                if (b != null)
                {
                    if (!b.sweeped && !b.flaged) sweepAt(b);
                }
            }
        }        
    }

    public void checkWin()
    {
        bool win = true;
        foreach(Block b in blockList)
        {
            if (!b.sweeped && !b.flaged) win = false; 
        }

        if (win) GameWin(this, EventArgs.Empty);
    }


    private void walkOnMine(Block block)
    {
        block.showMine();
        flagedCount++;
        WalkOnMine(this, EventArgs.Empty);
    }

    // Update is called once per frame
    void Update () {
	}

    private Coroutine longClickCheck;
    private bool longClicked;
    private Block clickBlock;
    private float clickTime;
    private Vector2 clickPos;
    private bool draged;

    private Vector3 _vec3TargetScreenSpace;// 目标物体的屏幕空间坐标  
    private Vector3 _vec3TargetWorldSpace;// 目标物体的世界空间坐标      
    private Vector3 _vec3MouseScreenSpace;// 鼠标的屏幕空间坐标  
    private Vector3 _vec3Offset;// 偏移  
    
    public void OnBlockPointerDown(PointerEventData eventData,Block block)
    {

            clickBlock = block;            
            longClicked = false;
            clickTime = Time.time;
            clickPos = eventData.position;
            draged = false;

            // 把目标物体的世界空间坐标转换到它自身的屏幕空间坐标   
            _vec3TargetScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
            // 存储鼠标的屏幕空间坐标（Z值使用目标物体的屏幕空间坐标）   
            _vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);
            // 计算目标物体与鼠标物体在世界空间中的偏移量   
            _vec3Offset = transform.position - Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace);

            longClickCheck = StartCoroutine("LongClickCheck");
        
    }

    IEnumerator LongClickCheck()
    {
        while (Input.GetMouseButton(0))
        {
            if (Vector2.Distance(Input.mousePosition, clickPos) > 20.0f)
            {
                draged = true;

                // 存储鼠标的屏幕空间坐标（Z值使用目标物体的屏幕空间坐标）  

                _vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);

                // 把鼠标的屏幕空间坐标转换到世界空间坐标（Z值使用目标物体的屏幕空间坐标），加上偏移量，以此作为目标物体的世界空间坐标  

                _vec3TargetWorldSpace = Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace) + _vec3Offset;

                // 更新目标物体的世界空间坐标   

                transform.position = _vec3TargetWorldSpace;
                
            }


            if(((Time.time - clickTime) > 0.5f) && (!draged) )
            {
                // long clicked
                longClicked = true;
                onBlockLongClick(clickBlock);
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }                
    }


    
    public void onBlockLongClick(Block block)
    {
        if (!block.sweeped)
        {
            if (block.flaged) cancelFlagAt(block);
            else flagAt(block);
        }
    }

    public void onBlockPointerUp(PointerEventData eventData, Block block)
    {
        StopCoroutine(longClickCheck);

        if((!longClicked) && (!draged))
        {
            if (block.sweeped)
            {
                if (block.num > 0 && !block.isMine)
                {
                    sweepAround(block);
                }
            }else if (!block.flaged)
            {
                sweepAt(block);
            }


        }
        
    }
}
