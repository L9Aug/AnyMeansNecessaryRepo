﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SM;
using Condition;

public class PauseMenu : MonoBehaviour
{
    #region State Machine Variables

    enum PausedMachine { None, Shop, Dialog, Console, PausedMenu };
    enum PausedMenuMachine { Menu, Skills, Inventory, Map, Options };

    StateMachine PauseStateMachine;
    StateMachine PausedStateMachine;
    StateMachine PausedMenuStateMachine;

    bool isGamePaused;
    PausedMachine PausedGoto;
    PausedMenuMachine PausedMenuGoto;

    #endregion

    public GameObject PauseButtons;
    public GameObject GamePlayHUD;
    public GameObject InventoryScreen;
    public GameObject Map;
    public GameObject InventoryElements;
    public GameObject OptionsScreen;
    public GameObject SkillTreeScreen;
    public GameObject SniperScopeUI;
    public GameObject ShopScreen;

    public GameObject Player;
    public Camera MapCamera;
    Vector3 StartPos;
   
    
    // Use this for initialization
    void Start ()

    {
        disableButtons();
        SetupSateMachine();
        MapCamera.cullingMask |= (1 << 0) | (1 << 8) | (1 << 9) | (1 << 11) | (1 << 12);
    }
	
	// Update is called once per frame
	void Update ()
    {
        PauseStateMachine.SMUpdate();

        /*
        if (Input.GetButtonDown("Pause"))
        {
            if (Time.timeScale < 0.1f)
            {
                resume();
            }
            else
            {
                enableButtons();
                Time.timeScale = 0.0f;
            }
        }

        if(Input.GetButtonDown("Inventory"))
        {
            inventoryUp();
            Time.timeScale = 0.0f;
        }

        if(Input.GetButtonDown("Map"))
        {
            mapUp();
            Time.timeScale = 0.0f;
        }

        if(Input.GetButtonDown("Skills"))
        {
            SkillsUp();
            Time.timeScale = 0.0f;
        }

        if(Input.GetButtonDown("Interact"))
        {
            Shop();            
        }*/
    }

    public void DisableUI()
    {
        PauseButtons.SetActive(false);
        InventoryScreen.SetActive(false);
        OptionsScreen.SetActive(false);
        Map.SetActive(false);
        SkillTreeScreen.SetActive(false);
        SniperScopeUI.SetActive(false);
        GamePlayHUD.SetActive(false);
        ShopScreen.SetActive(false);
        
    }

    void disableButtons()// disables pause menu
    {
        DisableUI();
        GamePlayHUD.gameObject.SetActive(true);
        InventoryElements.GetComponent<RectTransform>().localPosition = new Vector3(-100, 2000, 0);
        
        
//#if !UNITY_EDITOR
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
//#endif
    }

   void enableButtons()//Function brings the pause menu up
    {
        DisableUI();
        PauseButtons.gameObject.SetActive(true);
        InventoryElements.GetComponent<RectTransform>().localPosition = new Vector3(-100,2000,0);
       
        //#if !UNITY_EDITOR
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
//#endif
    }

    void resume() //resumes game
    {
        disableButtons();
        Time.timeScale = 1.0f;

    }

    void quit() //quits game
    {
        Application.Quit();
        Debug.Log("Is Quitting");
    }

    public void inventoryUp()
    {
        DisableUI();
        InventoryScreen.gameObject.SetActive(true);
        InventoryElements.GetComponent<RectTransform>().localPosition = Vector3.zero;

        //#if !UNITY_EDITOR
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //#endif

    }

    public void mapUp()
    {
        //Displays Map section of the menu and deactivates other elements
        DisableUI();
        Map.gameObject.SetActive(true);
        InventoryElements.GetComponent<RectTransform>().localPosition = new Vector3(-100, 2000, 0);
        MapCamera.transform.position = new Vector3(Player.transform.position.x,50,Player.transform.position.z);
        MapCamera.cullingMask |= (1 << 0)|(1<<8)|(1<<9)|(1<<11)|(1<<12);

        //#if !UNITY_EDITOR
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //#endif

    }

    public void OptionsMenu()
    {
        DisableUI();
        OptionsScreen.gameObject.SetActive(true);
        InventoryElements.GetComponent<RectTransform>().localPosition = new Vector3(-100, 2000, 0);

        //#if !UNITY_EDITOR
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //#endif
    }

    public void SkillsUp()
    {
        DisableUI();
        SkillTreeScreen.gameObject.SetActive(true);
        //#if !UNITY_EDITOR
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //#endif
    }

    public void Shop()
    {
        DisableUI();
        ShopScreen.SetActive(true);
        Time.timeScale = 0.0f;

        //#if !UNITY_EDITOR
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //#endif
    }


    public void reloadCheckpoint()//reloads to checkpoint
    {
        ReloadAI();

        ReloadPlayer();

        resume();
    }

    void ReloadAI()
    {
        Base_Enemy[] Enemy = FindObjectsOfType<Base_Enemy>();

        for (int i = 0; i < Enemy.Length; i++)
        {
            if (Enemy[i] != null)
            {
                if (Enemy[i]._state == Base_Enemy.State.Dead)
                {
                    Enemy[i].GetComponent<Animator>().SetTrigger("Revived");
                }

                Enemy[i].transform.position = XMLManager.instance.enemyDB.enemList[i].enemPos;
                Enemy[i].transform.rotation = XMLManager.instance.enemyDB.enemList[i].enemyRot;
                Enemy[i].GetComponent<Base_Enemy>()._state = Base_Enemy.State.Patrol;
                Enemy[i].GetComponent<Base_Enemy>()._state = XMLManager.instance.enemyDB.enemList[i].enemyState;
                Enemy[i].GetComponent<HealthComp>().SetHealth(XMLManager.instance.enemyDB.enemList[i].enemHealth);
                Enemy[i].GetComponent<FieldOfView>().detectedtimer = XMLManager.instance.enemyDB.enemList[i].detectionTimer;
                Enemy_Patrol.detected = XMLManager.instance.enemyDB.enemList[i].detected;
                Enemy[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }

    void ReloadPlayer()
    {

        PlayerController.PC.transform.rotation = XMLManager.instance.enemyDB.PlayerRot;
        PlayerController.PC.GetComponent<HealthComp>().SetHealth(XMLManager.instance.enemyDB.PlayerHealth);
        if (XMLManager.instance.enemyDB.PlayerHealth > 0)
        {
            PlayerController.PC.Revived = true;
        }
        //UIElements.health = CheckpointScript.storedHealth;// sets health to stored value
        UIElements.xp = CheckpointScript.storedXp;//sets xp to stored value 
        PlayerController.PC.transform.position = XMLManager.instance.enemyDB.PlayerPos;//moves player to checkpoint position
    }

    #region State Machine Funcitons

    #region Transition Functions

    void SetTransitionTarget(bool PausedState, PausedMachine PausedTarget = PausedMachine.None, PausedMenuMachine PausedMenuTarget = PausedMenuMachine.Menu)
    {
        isGamePaused = PausedState;
        PausedGoto = PausedTarget;
        PausedMenuGoto = PausedMenuTarget;
    }

    void PauseGameFunc()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void ResumeGameFunc()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion

    #region Level 0 State Functions : Pause State Machine

    void BeginPlaying()
    {
        // Activate Ingame UI.
        GamePlayHUD.gameObject.SetActive(true);
    }

    void PlayingUpdate()
    {
        if (Input.GetButtonDown("Pause"))
        {
            SetTransitionTarget(true, PausedMachine.PausedMenu, PausedMenuMachine.Menu);
        }
        else if (Input.GetButtonDown("Inventory"))
        {
            SetTransitionTarget(true, PausedMachine.PausedMenu, PausedMenuMachine.Inventory);
        }
        else if (Input.GetButtonDown("Map"))
        {
            SetTransitionTarget(true, PausedMachine.PausedMenu, PausedMenuMachine.Map);
        }
        else if (Input.GetButtonDown("Skills"))
        {
            SetTransitionTarget(true, PausedMachine.PausedMenu, PausedMenuMachine.Skills);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetTransitionTarget(true, PausedMachine.Shop);
        }
        else if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            SetTransitionTarget(true, PausedMachine.Console);
        }
    }

    void EndPlaying()
    {
        // Disable In-Game UI.
        GamePlayHUD.gameObject.SetActive(false);
    }

    void BeginPaused()
    {
        // might have to call PausedStateMAchines current states entry actions.

    }

    void PausedUpdate()
    {
        // Paused State Machine Update.
        if (Input.GetButtonDown("Pause"))
        {
            SetTransitionTarget(false);
        }
        PausedStateMachine.SMUpdate();
    }

    void EndPaused()
    {
        // Force a transition on PausedStateMachine to put it to the entry state.
        if (PausedStateMachine.CurrentState.Name != "Entry State")
        {
            PausedStateMachine.ForceTransition(PausedStateMachine.CurrentState.Transitions[0]);
        }
    }

    #endregion

    #region Level 1 State Functions : Paused State Machine

    void BeginShop()
    {
        ShopScreen.SetActive(true);
    }

    void ShopUpdate()
    {

    }

    void EndShop()
    {
        ShopScreen.SetActive(false);
    }

    void BeginDialog()
    {

    }

    void DialogUpdate()
    {

    }

    void EndDialog()
    {

    }

    void BeginConsole()
    {
        ConsoleController.CC.isConsoleActive = true;
    }

    void ConsoleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            SetTransitionTarget(false);
        }
    }

    void EndConsole()
    {
        ConsoleController.CC.isConsoleActive = false;
    }

    void BeginPausedMenu()
    {

    }

    void PausedMenuUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SetTransitionTarget(true, PausedMachine.PausedMenu, PausedMenuMachine.Menu);
        }
        else if(Input.GetButtonDown("Inventory"))
        {
            SetTransitionTarget(true, PausedMachine.PausedMenu, PausedMenuMachine.Inventory);
        }
        else if (Input.GetButtonDown("Map"))
        {
            SetTransitionTarget(true, PausedMachine.PausedMenu, PausedMenuMachine.Map);
        }
        else if (Input.GetButtonDown("Skills"))
        {
            SetTransitionTarget(true, PausedMachine.PausedMenu, PausedMenuMachine.Skills);
        }
        PausedMenuStateMachine.SMUpdate();
    }

    void EndPausedMenu()
    {
        // force trans to entrry state.
        PausedMenuStateMachine.ForceTransition(PausedMenuStateMachine.CurrentState.Transitions[0]);
    }

    #endregion

    #region Level 2 State Functions : Paused Menu State Machine

    void BeginMenu()
    {
        PauseButtons.SetActive(true);
    }

    void MenuUpdate()
    {

    }

    void EndMenu()
    {
        PauseButtons.SetActive(false);
    }

    void BeginOptions()
    {
        OptionsScreen.SetActive(true);
    }

    void OptionsUpdate()
    {

    }

    void EndOptions()
    {
        OptionsScreen.SetActive(false);
    }

    void BeginSkills()
    {
        SkillTreeScreen.SetActive(true);
    }

    void SkillsUpdate()
    {

    }

    void EndSkills()
    {
        SkillTreeScreen.SetActive(false);
    }
    
    void BeginInventory()
    {
        InventoryScreen.SetActive(true);
        InventoryElements.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }

    void InventoryUpdate()
    {

    }

    void EndInventory()
    {
        InventoryScreen.SetActive(false);
        InventoryElements.GetComponent<RectTransform>().localPosition = new Vector3(-100, 2000, 0);
    }

    void BeginMap()
    {
        Map.SetActive(true);
        MapCamera.gameObject.SetActive(true);
        MapCamera.transform.position = new Vector3(Player.transform.position.x, 50, Player.transform.position.z);
    }

    void MapUpdate()
    {

    }

    void EndMap()
    {
        Map.SetActive(false);
        MapCamera.gameObject.SetActive(false);
    }
    
    #endregion

    #region State Machine Condition Functions

    bool IsGamePaused()
    {
        return isGamePaused;
    }

    bool IsShopTarget()
    {
        return PausedGoto == PausedMachine.Shop;
    }

    bool IsDialogTarget()
    {
        return PausedGoto == PausedMachine.Dialog;
    }

    bool IsConsoleTarget()
    {
        return PausedGoto == PausedMachine.Console;
    }

    bool IsPausedMenuTarget()
    {
        return PausedGoto == PausedMachine.PausedMenu;
    }

    bool IsMenuTarget()
    {
        return PausedMenuGoto == PausedMenuMachine.Menu;
    }

    bool IsOptionsTarget()
    {
        return PausedMenuGoto == PausedMenuMachine.Options;
    }

    bool IsSkillsTarget()
    {
        return PausedMenuGoto == PausedMenuMachine.Skills;
    }

    bool IsInventoryTarget()
    {
        return PausedMenuGoto == PausedMenuMachine.Inventory;
    }

    bool IsMapTarget()
    {
        return PausedMenuGoto == PausedMenuMachine.Map;
    }

    #endregion

    void SetupSateMachine()
    {
        // conditions
        BoolCondition IsGamePausedCond = new BoolCondition(IsGamePaused);
        NotCondition IsGameNotPausedCond = new NotCondition(IsGamePausedCond);

        BoolCondition IsShopTargetCond = new BoolCondition(IsShopTarget);
        BoolCondition IsDialogTargetCond = new BoolCondition(IsDialogTarget);
        BoolCondition IsConsoleTargetCond = new BoolCondition(IsConsoleTarget);
        BoolCondition IsPausedMenuTargetCond = new BoolCondition(IsPausedMenuTarget);

        BoolCondition IsMenuTargetCond = new BoolCondition(IsMenuTarget);
        BoolCondition IsOptionsTargetCond = new BoolCondition(IsOptionsTarget);
        BoolCondition IsSkillsTargetCond = new BoolCondition(IsSkillsTarget);
        BoolCondition IsInventoryTargetCond = new BoolCondition(IsInventoryTarget);
        BoolCondition IsMapTargetCond = new BoolCondition(IsMapTarget);

        // transitions
        Transition PauseGame = new Transition("Pause Game", IsGamePausedCond, PauseGameFunc);
        Transition UnPauseGame = new Transition("Un-Pause Game", IsGameNotPausedCond, ResumeGameFunc);

        Transition LeavePaused = new Transition("Leave Paused", IsGameNotPausedCond);

        Transition GotoShop = new Transition("Go To Shop", IsShopTargetCond);
        Transition GotoDialog = new Transition("Go To Dialog", IsDialogTargetCond);
        Transition GotoConsole = new Transition("Go To Console", IsConsoleTargetCond);
        Transition GotoPausedMenu = new Transition("Go To PausedMenu", IsPausedMenuTargetCond);

        Transition LeavePausedMenu = new Transition("Leave Paused Menu", IsGameNotPausedCond);

        Transition GotoMenu = new Transition("Go To Menu", IsMenuTargetCond);
        Transition GotoOptions = new Transition("Go To Options", IsOptionsTargetCond);
        Transition GotoSkills = new Transition("Go To Skills", IsSkillsTargetCond);
        Transition GotoInventory = new Transition("Go To Inventory", IsInventoryTargetCond);
        Transition GotoMap = new Transition("Go To Map", IsMapTargetCond);

        // states

        // Level 0 states : Pause State Machine.
        State GameIsPaused = new State("Game is Paused",
            new List<Transition>() { UnPauseGame },
            new List<Action>() { BeginPaused },
            new List<Action>() { PausedUpdate },
            new List<Action>() { EndPaused });

        State GameIsNotPaused = new State("Game is not paused",
            new List<Transition>() { PauseGame },
            new List<Action>() { BeginPlaying },
            new List<Action>() { PlayingUpdate },
            new List<Action>() { EndPlaying });

        // Level 1 states : Paused State Machine
        State PausedEntryState = new State("Entry State",
            new List<Transition>() { GotoShop, GotoDialog, GotoConsole, GotoPausedMenu },
            null,
            null,
            null);

        State ShopState = new State("Shop",
            new List<Transition>() { LeavePaused },
            new List<Action>() { BeginShop },
            new List<Action>() { ShopUpdate },
            new List<Action>() { EndShop });

        State DialogState = new State("Dialog",
            new List<Transition>() { LeavePaused },
            new List<Action>() { BeginDialog },
            new List<Action>() { DialogUpdate },
            new List<Action>() { EndDialog });

        State ConsoleState = new State("Console",
            new List<Transition>() { LeavePaused },
            new List<Action>() { BeginConsole },
            new List<Action>() { ConsoleUpdate },
            new List<Action>() { EndConsole });

        State PausedMenuState = new State("Paused Menu",
            new List<Transition>() { LeavePaused },
            new List<Action>() { BeginPausedMenu },
            new List<Action>() { PausedMenuUpdate },
            new List<Action>() { EndPausedMenu });

        // Level 2 States : Paused Menu State Machine
        State PausedMenuEntryState = new State("Entry State",
            new List<Transition>() { GotoMenu, GotoOptions, GotoSkills, GotoInventory, GotoMap },
            null,
            null,
            null);

        State MenuState = new State("Menu",
            new List<Transition>() { LeavePausedMenu, GotoOptions, GotoSkills, GotoInventory, GotoMap },
            new List<Action>() { BeginMenu },
            new List<Action>() { MenuUpdate },
            new List<Action>() { EndMenu });

        State OptionsState = new State("Options",
            new List<Transition>() { LeavePausedMenu, GotoMenu },
            new List<Action>() { BeginOptions },
            new List<Action>() { OptionsUpdate },
            new List<Action>() { EndOptions });

        State SkillsState = new State("Skills",
            new List<Transition>() { LeavePausedMenu, GotoMenu, GotoInventory, GotoMap },
            new List<Action>() { BeginSkills },
            new List<Action>() { SkillsUpdate },
            new List<Action>() { EndSkills });

        State InventoryState = new State("Inventory",
            new List<Transition>() { LeavePausedMenu, GotoMenu, GotoSkills, GotoMap },
            new List<Action>() { BeginInventory },
            new List<Action>() { InventoryUpdate },
            new List<Action>() { EndInventory });

        State MapState = new State("Map",
            new List<Transition>() { LeavePausedMenu, GotoMenu, GotoSkills, GotoInventory },
            new List<Action>() { BeginMap },
            new List<Action>() { MapUpdate },
            new List<Action>() { EndMap });

        //Target States
        PauseGame.SetTargetState(GameIsPaused);
        UnPauseGame.SetTargetState(GameIsNotPaused);

        LeavePaused.SetTargetState(PausedEntryState);

        GotoShop.SetTargetState(ShopState);
        GotoDialog.SetTargetState(DialogState);
        GotoConsole.SetTargetState(ConsoleState);
        GotoPausedMenu.SetTargetState(PausedMenuState);

        LeavePausedMenu.SetTargetState(PausedMenuEntryState);

        GotoMenu.SetTargetState(MenuState);
        GotoOptions.SetTargetState(OptionsState);
        GotoSkills.SetTargetState(SkillsState);
        GotoInventory.SetTargetState(InventoryState);
        GotoMap.SetTargetState(MapState);

        // initialise machines.
        PauseStateMachine = new StateMachine(null, GameIsNotPaused, GameIsPaused);
        PausedStateMachine = new StateMachine(null, PausedEntryState, ShopState, DialogState, ConsoleState, PausedMenuState);
        PausedMenuStateMachine = new StateMachine(null, PausedMenuEntryState, MenuState, OptionsState, SkillsState, InventoryState, MapState);

        PauseStateMachine.InitMachine();
        PausedStateMachine.InitMachine();
        PausedMenuStateMachine.InitMachine();
    }

    #endregion

}
