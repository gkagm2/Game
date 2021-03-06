﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileControl : MonoBehaviour {

    PlayerInfo playerInfo;

    MapInfo mapInfo;
    public GameObject cameraObj; // 카메라 오브젝트를 가져옴 (TileHit때문임)
    TileHit tileHit; //TileHit

    int maxTileNumber = 0; // 타일의 개수
    
    Tile[] tile; //맵에 존재하는 각 타일들

    public bool[] initTile; // 초기화 하기위해 필요한 

    public Stack<Tile> tileStack; // 타일 컴포넌트를 저장하는 Stack을 생성한다.
                           // Use this for initialization

    public int firstStartTileIndex; // 초기에 시작되는 타일의 인덱스 번호
    void Awake() {
        tileStack = new Stack<Tile>();
        mapInfo = GetComponent<MapInfo>();

        //Debug.Log("TileControl.cs " + "~~~mylevel : " + mapInfo.myLevel + "~~~myStage: " + mapInfo.myStage);

        maxTileNumber = transform.childCount; //타일의 개수를 가져옴
        tile = new Tile[maxTileNumber]; // 초기 타일들의 인스턴스 생성
        initTile = new bool[maxTileNumber]; // 초기화하기위해 필요한 정보
        for (int i = 0; i < maxTileNumber; i++) //각 타일들의 Tile 컴포넌트를 가져옴.
        {
            tile[i] = transform.GetChild(i).GetComponent<Tile>();
            initTile[i] = tile[i].check; // 초기화하기 위해 담는다
        }
        InitTiles(); // 타일 초기화.
        tileHit = cameraObj.GetComponent<TileHit>(); //TileHit 컴포넌트를 가져온다.
    }
    private void Start()
    {
        playerInfo = GameObject.Find(MyPath.gameScreen).GetComponent<PlayerInfo>();
        firstStartTileIndex = GetStartTileIndex(); // 시작하는 타일의 index를 받아온다.
        if (firstStartTileIndex < 0)
        {
            Debug.Log("타일 시작점 개수가 1개 이상임 Error");
        }
        else // 제대로 시작하는 타일의 index를 가져왔으면
        {
            //Debug.Log("level : " + gameObject.name + ", firstStartTileIndex : " + firstStartTileIndex + "tile name : " + tile[firstStartTileIndex].name);
            tileStack.Push(tile[firstStartTileIndex]); //첫번째 타일을 스택에 Push.
        }
    }
    // Update is called once per frame
    void Update() {

        //TODO : 파일에서 받아와서 써야될 듯.
        Debug.Log("---------검증하지 못한 코드.(일단 사용함)--------");
        //타일이 null일 경우
        for(int i= 0; i < tile.Length; i++)
        {
            if(tile[i] == null)
            {
                Debug.Log("타일이 Null임!!!!!!!!!!!!!!!!!!!!!!!!");
                initTile = new bool[maxTileNumber]; // 초기화하기위해 필요한 정보
                for (int j = 0; j < maxTileNumber; j++) //각 타일들의 Tile 컴포넌트를 가져옴.
                {
                    tile[j] = transform.GetChild(j).GetComponent<Tile>();
                    initTile[j] = tile[j].check; // 초기화하기 위해 담는다
                }
                InitTiles(); // 타일 초기화.
            }

        }


        if (CheckAllTilesTouched()) //모든 타일들이 터치되면
        {
            Debug.Log("(1)All tiles Touched");

            InitTiles(); //타일들을 초기화 한다.

            ScreenManager screenManager = GameObject.Find(MyPath.gameScreen).GetComponent<ScreenManager>();

            // 현재 레벨에서 가지고 있는 별의 개수가 레벨의 최대 별의 개수보다 작으면
            if (playerInfo.currentHaveStarsPerLevel[playerInfo.currentLevel - 1] <= playerInfo.maxStarsPerLevel[playerInfo.currentLevel - 1])
            {
                // 현재 하고 있는 스테이지가 성공했던 스테이면
                if(playerInfo.GetLevelStageInfoObjSuccessValue(playerInfo.currentLevel,playerInfo.currentStage))
                {
                    Debug.Log("(2)이미 한 스테이지임 성공 팝업창 뜸");
                    screenManager.OpenSuccessPopup(); // 성공 팝업 창이 뜬다.
                }
                // 진행중인 스테이지를 깬 것이였으면
                else if (playerInfo.currentLevel == playerInfo.myProgressLevel && playerInfo.currentStage == playerInfo.myProgressStage)
                {
                    // 해당 레벨의 마지막 스테이지를 깬 것이였으면
                    if (playerInfo.GetLevelMaxStageNumber(playerInfo.currentLevel) == playerInfo.currentStage && playerInfo.GetLevelStageInfoObjSuccessValue(playerInfo.currentLevel, playerInfo.currentStage) == false) //해당 스테이지가 각 레벨의 마지막 스테이지이고  성공하지 않은 스테이지면
                    {
                        if (playerInfo.currentLevel < playerInfo.GetLevelCount()) //다음 레벨이 있을 경우
                        {
                            Debug.Log("(2)다음레벨 잠금 해제");
                            playerInfo.NextLevelSetting(); // 다음 레벨에 대한 플레이어 정보 세팅하기
                            
                            screenManager.OpenAllStageSuccessPopup(); // 모든 스테이지를 성공했다는 팝업 창이 뜬다.
                        }
                        else // 다음 레벨이 없을 경우
                        {
                            Debug.Log("(2)모든 스테이지를 다 깼다.");
                            // TODO : 팝업창 만들어야 함
                        }
                    }
                    else //해당 레벨의 마지막이 아닌 스테이지를 깬 것이면
                    {
                        Debug.Log("(2)성공 팝업창 뜸");
                        playerInfo.myProgressStage += 1; //스테이지 진행도를 1 올림.
                        screenManager.OpenSuccessPopup(); // 성공 팝업창이뜬다.
                    }
                }
            }
            else // 진행 상태의  Stage의 번호가 각 레벨에 해당하는 별의 수가 넘어가면
            {
                Debug.Log("꽉 채웠음");
            }
            Debug.Log("(3) 이 부분 뜨나?");

            // 이번 스테이지의 정보를 성공으로 바꿈.
            playerInfo.SetLevelStageInfoObjSuccessValue(playerInfo.currentLevel, playerInfo.currentStage, true);
        }

            if (TouchedTilesCount() >= 2) //체크된 개수가 2개 이상이면
        {
            for(int i=0; i < maxTileNumber; i++) //전체 타일을 검사하기 위해 돌림
            {
                //체크가 되어있는데 스텍에 안들어가 있으면
                if (tile[i].CheckTouched() && !tileStack.Contains(tile[i])) 
                {
                    tileStack.Push(tile[i]); // 스텍에 푸쉬한다.
                }
            }
        }
    }

    // 타일 초기화
    public void InitTiles()
    {
        // 타일을 복사한다.
        // 각 타일의 정보를 가져옴.
        for (int i = 0; i < tile.Length; i++)
        {
            if(tile[i].check != initTile[i]){ //초기 값이랑 다르면
                tile[i].ChangeColor(); //색을 바꾼다.
            }
        }
        TileStackInit(); //타일 스텍 초기화
    }

    // 타일을 담는 스택 초기화
    public void TileStackInit()
    {
        
        tileStack.Clear(); // 타일 스택을 모두 없애고
        tileStack.Push(tile[firstStartTileIndex]); // 시작 타일을 스택에 집어넣는다.
    }


    // 모든 타일들이 터치되었는지 확인하는 함수.
    public bool CheckAllTilesTouched()    {
        int touchCount = 0;
        for(int i = 0; i < maxTileNumber; i++)
        {
            if (tile[i].CheckTouched()) //터치되었으면 
            {
                ++touchCount; //터치한 개수를 올림.
            }
        }
        return touchCount == maxTileNumber; //터치한 개수와 최대 타일 개수와 같으면 true
    }

    // 터치된 타일의 개수를 세줌.
    public int TouchedTilesCount()
    {
        int touchCount = 0;
        for(int i = 0; i < maxTileNumber; i++)
        {
            if (tile[i].CheckTouched()) //터치되었으면
            {
                ++touchCount;
            }
        }
        return touchCount;
    }
    
    // 첫번 째 타일의 인덱스를 구하기
    public int GetStartTileIndex()
    {
        int indexNumber = 0;
        int touchedCount = 0;
        for (int i = 0; i < maxTileNumber; i++)
        {
            if (tile[i].CheckTouched()) //터치되었으면
            {
                if(touchedCount >= 2) //2 이상이면
                {
                    return -1; //0을 리턴한다.
                }
                ++touchedCount; // 터치 카운터를 1 올림
                indexNumber = i; // 인덱스를 집어넣는다.
            }
        }
        return indexNumber;
    }

   
}
