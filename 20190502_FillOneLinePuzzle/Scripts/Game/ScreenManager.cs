﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenManager : MonoBehaviour {

    /// <summary>
    /// ///////////////나중에 지우기////////////////////////////////////////////
    /// </summary>
    
    // Main button control
    public Camera camera; //현재 보고있는 화면

    // Screen (Scene)
    

    public GameObject gameScreen;
    short screenNumber;

    
    public  class LevelStageInfo
    {
        string level;
        string stage;
    }
    LevelStageInfo levelStageInfo;
    /// <summary>
    /// //////////////////////////////////////////////////////////////////////
    /// </summary>



    public GameObject mainScreen; // main scene의 Object를 가져옴

    Stack<GameObject> screenStack = new Stack<GameObject>(); // 씬을 저장할 Stack 생성



    // Use this for initialization
    void Start () {
        screenStack.Push(mainScreen); // mainScreen을 push 함. (최초로 켜지는 씬)

    }

    // Update is called once per frame
    void Update () {
        
	}

    /// <summary>
    /// MainScreen Buttons
    /// </summary>
    /// 
    // ShareScreen Buttons

    public void GotoShopScreen() //shopScreen으로 가는 버튼
    {
        if(screenStack.Peek().name != "ShopScreen") //현재 씬이 ShopScreen이 아니면
        {
            GameObject shopScreen = gameObject.transform.Find("ShopScreen").gameObject;//shopScreen을 찾아라
            if (shopScreen) // shopScreen이 존재하면
            {
                Debug.Log("ShopScreen이 SetActive true 됨");
                screenStack.Peek().SetActive(false); // 현재 스크린을 안보이게 하고
                screenStack.Push(shopScreen); // Screen을 Push 함
                screenStack.Peek().SetActive(true); //이후 shopScreen을 보이게 함.
            }
            else
            {
                Debug.Log("shopScreen을 찾지 못했습니다.");
            }
        }
    }

    // 샵에서 뒤로가기 버튼
    public void ShopScreen_BackBtn()
    {
        PlayerInfo playerInfo = GetComponent<PlayerInfo>();
        if (playerInfo) // 가져오면
        {
            Debug.Log("현레 스 : " + playerInfo.currentLevel + " ," + playerInfo.currentStage);
            if (playerInfo.currentLevel != 0 && playerInfo.currentStage != 0)
            {
                Debug.Log("1이상이땔");
                GameObject scene = screenStack.Pop(); //현재 스크린을 Pop하고
                scene.SetActive(false); // 현재 스크린을 안보이게 한다.
            }
            else
            {
                Debug.Log("0,0dlfE");
                GameObject scene = screenStack.Pop(); //현재 스크린을 Pop하고
                scene.SetActive(false); // 현재 스크린을 안보이게 한다.
                screenStack.Peek().SetActive(true); // 이전 스크린을 보이게 한다.
            }
        }

        
    }
    public void GotoLevelsScreen() //levelsScreen으로 가는 버튼
    {
        PlayerInfo playerInfo = GetComponent<PlayerInfo>();

        if(playerInfo.currentLevel != 0 && playerInfo.currentStage != 0) // 퀴드 푸는 도중에 levels창으로 갈 경우
        {
            while (screenStack.Count > 2) //Levels창만 남기고 다 Pop한다.
            {
                GameObject obj = screenStack.Pop(); //pop 한뒤
                                                    //Debug.Log("Pop한 Obj의 이름 : " + obj.name);
                obj.SetActive(false);  // 모두 setActive false로 바꿈.
            }
            playerInfo.currentLevel = playerInfo.currentStage = 0;
            screenStack.Peek().SetActive(true); // 마지막 screen을 보여줌
        }
        else //main창에서 levels 창으로 갈 경우
        {
            GameObject levelsScreen = gameObject.transform.Find("LevelsScreen").gameObject; // levelsScreen을 찾아라
            if (levelsScreen) // levelsScreen이 존재하면
            {
                ChangeScreen(levelsScreen); //화면에 보여주는 스크린을 바꿈
            }
        }
        
        
    }
    public void GotoStageScreen() // stageScreen으로 가는 버튼
    {
        GameObject stageScreen = gameObject.transform.Find("StageScreen").gameObject; // stageScreen을 찾아라
        if (stageScreen) // stageScreen이 존재하면
        {
            ChangeScreen(stageScreen); //화면에 보여주는 스크린을 바꿈

            PlayerInfo playerInfo = GetComponent<PlayerInfo>(); // playerInfo 컴포넌트를 가져옴.
            if (playerInfo) // 가져오는데 성공하면
            {
                GameObject[] stagesLevelObj = new GameObject[playerInfo.GetLevelCount()];
                for (int i = 0; i < playerInfo.GetLevelCount(); i++) {
                    stagesLevelObj[i] = new GameObject();
                    stagesLevelObj[i] = transform.Find("StageScreen/StagesScrollView/Level" + (i+1)).gameObject;
                    if (playerInfo.currentLevel != i + 1)
                    {
                        stagesLevelObj[i].SetActive(false);
                    }
                    else
                    {
                        stagesLevelObj[i].SetActive(true);
                    }
                }
            }
        }
    }

    // 레벨 스크린에서 레벨 버튼을 눌렸을 경우
    public void LevelsScreen_LevelBtn(short level)
    {
        GameObject levelBtnLock = GameObject.Find(MyPath.levelScreen_Level + level +"/Score/Lock");// 레벨 버튼의 Lock을 찾는다
        if (levelBtnLock) // 레벨 버튼의 Lock을 찾았으면
        {
            if (levelBtnLock.activeSelf == true) // 잠겨있으면
            {
                return; //실행 중단.
            }
        }
        PlayerInfo playerInfo = GetComponent<PlayerInfo>(); //플레이어 정보를 가져온다
        if (playerInfo) // 가져왔으면
        {
            playerInfo.currentLevel = level; //현재 선택한 레벨을 설정해줌
            GotoStageScreen(); // stageScreen으로 가는 버튼
            StageScreen_LevelActive(playerInfo.currentLevel); //스테이지 스크린에서 클릭된 레벨의 Stage를 설정한다.
        }
    }


    // 스테이지 스크린에서 클릭된 레벨의 Stage를 설정한다.
    public void StageScreen_LevelActive(short choicedStageLevel)
    {
        GameObject stageScreen_levelObj = GameObject.Find(MyPath.stageScreen_levels + choicedStageLevel);
        if (stageScreen_levelObj)
        {
            stageScreen_levelObj.SetActive(true);
        }
        else
        {
            Debug.Log("못찾음");
        }
    }

    // 화면에 보여주는 스크린을 파라미터 오브젝트(스크린으)로 바꿈 (상점에 이용)
    void ChangeScreen(GameObject obj) 
    {
        screenStack.Peek().SetActive(false); // 현재 스크린을 안보이게 하고
        screenStack.Push(obj); // Screen을 Push 함
        screenStack.Peek().SetActive(true); //이후 shopScreen을 보이게 함.
    }

    // ShareScreen Buttons
    public void BackBtn() //뒤로 가는 버튼
    {
        PlayerInfo playerInfo = GetComponent<PlayerInfo>();
        if (playerInfo) // 가져오면
        {
            GameObject scene = screenStack.Pop(); //현재 스크린을 Pop하고
            scene.SetActive(false); // 현재 스크린을 안보이게 한다.
            screenStack.Peek().SetActive(true); // 이전 스크린을 보이게 한다.
        }
        
    }



    // 팝업창을 없앤다.
    public void ClosePopup(GameObject obj)
    {
        obj.SetActive(false);
    }

    // 팝업창을 띄운다.
    public void OpenPopup(GameObject obj)
    {
        obj.SetActive(true);
    }
    

    // 성공 팝업 창 열기
    public void OpenSuccessPopup()
    {

        GameObject successPopup = gameObject.transform.Find("SuccessPopup").gameObject; // successPopup을 찾아라
        if(successPopup) // successPopup이 존재하면
        {
            //Debug.Log("successPopup이 뜬다.!");
            GameScreen_HideScreen(); // 게임 스크린을 숨긴다
            OpenPopup(successPopup); // 팝업창을 띄운다.
        }
    }

    // 성공 팝업 창 닫기
    public void CloseSuccessPopup()
    {
        GameObject successPopup = gameObject.transform.Find("SuccessPopup").gameObject; // successPopup을 찾아라
        if (successPopup) // successPopup이 존재하면
        {
            //Debug.Log("successPopup이 닫힌다.!");
            ClosePopup(successPopup); // 팝업창을 없앤다.
        }
    }
    // Success 창을 종료한다.
    public void SuccessPopup_ClosePopup()
    {
        GameObject successPopup = gameObject.transform.Find("SuccessPopup").gameObject;

        if (successPopup)
        {
            ClosePopup(successPopup);
        }

    }

    // Success 창에서 Main Menu창으로 넘어간다.
    public void SuccessPopup_OnClickMainMenuBtn(int popupNumber)
    {
        GotoMainMenuScreen(); // 메인메뉴 창으로 간다.
        switch (popupNumber)
        {
            case 1: // 1이면 성공 팝업 창
                SuccessPopup_ClosePopup(); //팝업 창을 종료
                break;
            case 2: // 2이면 모든 스테이지 성공 팝업 창
                AllStageSuccessPopup_ClosePopup(); // 팝업 창을 종료
                break;
        }
        GameScreen_HideScreen(); // 게임 스크린창을 숨김

        PlayerInfo playerInfo = GetComponent<PlayerInfo>(); // 현재 레벨과 스테이지 상태를 0으로 바꿈
        playerInfo.currentLevel = playerInfo.currentStage = 0;
    }
    // Success 창에서 Share 버튼을 클릭 시
    public void SuccessPopup_OnClickShareBtn()
    {
        Share();
    }
    // Success 창에서 NextStage 버튼을 클릭 시
    public void SuccessPopup_OnClickNextStageBtn()
    {
        PlayerInfo playerInfo = GetComponent<PlayerInfo>(); // 플레이어 정보 가져옴


        
        // 현재 레벨에서 가지고 있는 별의 개수가 레벨의 최대 별의 개수보다 작으면
        if (playerInfo.currentHaveStarsPerLevel[playerInfo.currentLevel - 1] <= playerInfo.maxStarsPerLevel[playerInfo.currentLevel - 1])
        {
            Debug.Log("(1.1)");
            // 스테이지의 마지막부분이면
            if (playerInfo.currentStage == playerInfo.GetLevelMaxStageNumber(playerInfo.currentLevel))
            {
                Debug.Log("(2)");

                // 레벨이 추가 될 수 있으면
                if (playerInfo.currentLevel < playerInfo.GetLevelCount())
                {
                    Debug.Log("(3)");

                    // 현재 레벨과 스테이지를 변경.
                    playerInfo.currentLevel += 1;
                    playerInfo.currentStage = 1;
                }
            }
            else
            {
                Debug.Log("(2.1)");
                playerInfo.currentStage += 1; // 현재 스테이지를 1 올림.
            }

            Debug.Log("(1.1 last");
            SuccessPopup_ClosePopup(); //success 팝업 창을 종료시킨다.
                                        //Debug.Log("현재 stack 맨 위에 있는 것 : " + screenStack.Peek());
                                        //GameScreen_HideScreen();   // 숨겼다가 
            GameScreen_ShowScreen(playerInfo.currentLevel, playerInfo.currentStage); // 다음 레벨과 스테이지의 타일을 보여줌

        }
        
        else // 다음 스테이지가 존재하지 않음.
        {
            Debug.Log("(1.2)");
            Debug.Log("별의 개수를 꽉 채웠음");
        }

    }



    // 메뉴 팝업창 열기
    public void OpenMenuPopup()
    {
        GameObject menuPopup = gameObject.transform.Find("MenuPopup").gameObject; // menuPopup을 찾아라
        if (menuPopup) // menuPopup이 존재하면
        {
            OpenPopup(menuPopup); // 팝업창을 띄운다.

        }
    }

    /// 메뉴 팝업창의 버튼

    // Menu Popup창을 종료한다.
    public void MenuPopup_OnClickResumeBtn()
    {
        GameObject menuPopup = gameObject.transform.Find("MenuPopup").gameObject; // menuPopup을 찾아라
        if (menuPopup)
        {
            ClosePopup(menuPopup);
        }
    }

    // Menu Popup 창에서 Stage 선택창으로 돌아간다.
    public void MenuPopup_OnClickSelectStageBtn()
    {
        GameObject menuPopup = gameObject.transform.Find("MenuPopup").gameObject; // menuPopup을 찾아라
        if (menuPopup)
        {
            GameScreen_HideScreen(); // 게임 스크린을 숨김
            MenuPopup_OnClickResumeBtn(); // 메뉴 팝업창을 종료
            screenStack.Peek().SetActive(true); // stage 창을 보이게 한다.

            PlayerInfo playerInfo = GetComponent<PlayerInfo>();
            playerInfo.currentStage = 0; // 현재 스테이지 상태를 0으로 바꿈
        }
    }


    // Menu Popup 창에서 Main Menu창으로 넘어간다.
    public void MenuPopup_OnClickMainMenu()
    {
        GotoMainMenuScreen(); // 메인메뉴 창으로 간다.
        MenuPopup_OnClickResumeBtn(); //메인 메뉴 창을 종료
        GameScreen_HideScreen(); // 게임 스크린창을 숨김

        PlayerInfo playerInfo = GetComponent<PlayerInfo>(); // 현재 레벨과 스테이지 상태를 0으로 바꿈
    }
    
    // 메인 메뉴 창으로 가기
    public void GotoMainMenuScreen()
    {
        //TODO : 스택에 있는 것들을 모두 다 빼고 MainMenu창인 스택만 남기는 것을 구현해야 함.
        
        while(screenStack.Count > 1) //Main창만 남기고 다 Pop한다.
        {
            GameObject obj = screenStack.Pop(); //pop 한뒤
            //Debug.Log("Pop한 Obj의 이름 : " + obj.name);
            obj.SetActive(false);  // 모두 setActive false로 바꿈.
        }
        PlayerInfo playerInfo = GetComponent<PlayerInfo>();
        playerInfo.currentLevel = playerInfo.currentStage = 0;
        screenStack.Peek().SetActive(true); // 마지막 screen을 보여줌
    }


    // GameScreen

    // StageScreen에서 GameScreen으로 넘어갈 때
    public void GameScreen_ShowScreen(int level= 0 , int stage =0)
    {
        if (level == 0 || stage == 0) // 레벨과 스테이지가 제대로 입력이 안들어오면
        {
            //Debug.Log("레벨과 스테이지 입력 안함 Error");
            return; // 함수 종료
        }
        

        GameObject gameScreen = GameObject.Find(MyPath.gameScreen + "GameScreen/"); // GameScreen 오브젝트를 가져옴
        screenStack.Peek().SetActive(false); // 현재 active되어있는 screen(stageScreen)을 false로 바꾼다. 
        


        PlayerInfo playerInfo = GetComponent<PlayerInfo>();
        GameObject map = GameObject.Find(MyPath.gameScreen_Maps + "LS" + level + "_" + stage); //레벨과 스테이지의 Object를 가져옴
        if (gameScreen && map && playerInfo) // 가져왔으면
        {
            // level과 stage의 정보를 가진 객체들을 불러와
            for (int i = 0; i < playerInfo.levelStageInfo.Length; i++)
            {
                playerInfo.levelStageInfo[i].obj.SetActive(false); // 나머지는 다 안보이게 하고

                // 해당 레벨과 스테이지의 Tile만 보이게 한다.
                if (playerInfo.levelStageInfo[i].level == level && playerInfo.levelStageInfo[i].stage == stage)
                {
                    if (playerInfo.levelStageInfo[i].obj.GetComponent<TileControl>())
                    {
                        //찾음
                        playerInfo.levelStageInfo[i].obj.GetComponent<TileControl>().InitTiles(); //타일 초기화.
                    }
                    else
                    {
                        Debug.Log("못찾음!");
                    }
                    playerInfo.levelStageInfo[i].obj.SetActive(true); // 타일을 보여줌
                }
            }
            gameScreen.transform.position = new Vector3(0, 0, 0); // 위치를 바꿔 화면에 보이게 한다.
        }
        else
        {
            if (!gameScreen)
            {
                Debug.Log("GameScreen을 찾지 못함");
            }
            if (!map)
            {
                //BackBtn();
                Debug.Log("Map을 찾지 못함");
            }

            if (!playerInfo)
            {
                Debug.Log("PlayerInfo를 찾지 못함");
            }
            playerInfo.currentLevel = playerInfo.currentStage = 0;
        }
    }

    // GameScreen을 종료할 때
    public void GameScreen_HideScreen()
    {
        GameObject gameScreen = GameObject.Find(MyPath.gameScreen + "GameScreen/"); // GameScreen 오브젝트를 가져옴
        if (gameScreen) // 가져왔으면
        {
            gameScreen.transform.position = new Vector3(0, 940.0f, 0); // 안보이게 숨김
        }
        else
        {
            Debug.Log("GameScreen 못가져옴 Error!");
        }
    }

    public void OpenAllStageSuccessPopup()
    {
        GameObject allStageSuccessPopup = gameObject.transform.Find("AllStageSuccessPopup").gameObject; // successPopup을 찾아라
        if (allStageSuccessPopup) // successPopup이 존재하면
        {
            Debug.Log("OpenAllStageSuccessPopup 뜸");
            PlayerInfo playerInfo = GetComponent<PlayerInfo>();
            //Debug.Log("successPopup이 뜬다.!");
            GameScreen_HideScreen(); // 게임 스크린을 숨긴다
            OpenPopup(allStageSuccessPopup); // 팝업창을 띄운다.
        }
    }

    public void AllStageSuccessPopup_GotoLevelsScreenBtn()
    {
        AllStageSuccessPopup_ClosePopup();
        GotoLevelsScreen();
    }

    // AllStageSuccess 팝업 창을 종료한다.
    public void AllStageSuccessPopup_ClosePopup()
    {
        GameObject allStageSuccessPopup = gameObject.transform.Find("AllStageSuccessPopup").gameObject;

        if (allStageSuccessPopup)
        {
            ClosePopup(allStageSuccessPopup);
        }
    }


    // 공유 하기 
    public void Share()
    {
        // https://www.youtube.com/watch?v=E4E4EfkGs0Y
        //StartCoroutine(ShareScreenShot());

        Debug.Log("공유하기~");
    }

    //IEnumerator ShareScreenShot()
    //{
    //    yield return new WaitForEndOfFrame();
    //    ScreenCapture.CaptureScreenshot("screenshot.png", 2);
    //    string destination = Path.Combine(Application.persistentDataPath, "screenshot.png");

    //    yield return new WaitForSecondsRealtime(0.3f);

    //    if (!Application.isEditor)
    //    {
            
    //        //AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
    //        //AndroidJavaObject intentObject = new AndroidJavaClass("android.content.Intent");
    //        //intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
    //        //AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
    //        //AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
    //        //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
    //        //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Can you beat my score?");
    //        //intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
    //        //AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //        //AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");


    //    }
    //}
}
