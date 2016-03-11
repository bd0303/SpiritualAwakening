


//Ghost and Wolf Game Menu
//Glenn Hampton
//Created 01/14/2016


//place this into an empty game object
//this must be placed in an object that has no parent, or DontDestroyOnLoad won't work
//the object can be placed anywhere in the scene
//But it must be in the FIRST scene the game loads


using UnityEngine;
using System.Collections;
using System.Collections.Generic; //dictionary needs this to be included

public class GameMenu : MonoBehaviour {
	
	private static GameMenu FInstance; //allows test for game menu already existing
	//dictionaries need to have using.System.Collections.Generic; added above
	private static Dictionary<string, KeyCode> FKeyControls;//store the user settable control values and keep them local so outside scripts cannot change them
	public static Dictionary<string, KeyCode> Controls //user controls accessible by other scripts as a property and make it read only
	{
		get {return FKeyControls;}
	}
	
	public delegate void PauseAction();
	public delegate void UnPauseAction();
	public static event PauseAction OnPause;
	public static event UnPauseAction OnUnPause;
	
	//FOR PAUSE WINDOW
	private bool FGamePaused; //internally know when game is paused
	private bool FCanPause; //used to determine if pause window is showable. Should not allow showing during level loading
	private bool FShow;//true show window false hide it
	private float FTimeScale; //store this so timescale can be returned to normal on unpause
	private Rect FWindowRect; //where to place the window on the screen
	private float FWindowWidth=500.0f;
	private float FWindowHeight=400.0f;
	private float FButtonWidth=450.0f;
	private float FButtonHeight=50.0f;
	private float FControlButtonHeight=30.0f; //button height for control settings window
	private float FSettingsScrollHeight=20.0f;
	private Rect FLoadProgressRect;
	private float FLoadProgressWidth=500.0f;
	private float FLoadProgressHeight=25.0f;
	
	//advanced start menu items
	public Transform FLoadImage;
	public Texture2D FProgressPanel;
	public Texture2D FProgressBar;
	public int FBeginGameLevel=1; //what level to begin game on when player selects Start Game button
	public float FScrollRate=30.0f; //how fast to scroll the credits
	public Texture FTexture;//used to display a background texture on the main window
	
	private float FCurrentTime;//used to calculate deltatime for scrolling
	
	private int FWindowID; //id of the currently open window
	//window layout items
	private string FWindowTitle; //title of the currently open window
	private int FNumberOfStartButtons=3; //number of buttons on the start menu
	private int FNumberOfPauseButtons=4; //number of buttons on the pause menu
	private int FNumberOfOptionsButtons=4; //number of buttons on the options menu
	
	private int FHomeMenu;//which menu to go back to when in a nested menu
	private float FScrollY=200; //current credit scroll position
	private float FDeltaTime; //save the current delta time. delta time=0 when game is paused
	
	private string FCurrentButton;//which button is currently being edited
	private Vector2 FScrollBarPos;
	
	private Dictionary<string, KeyCode> FDefaultControls;//store the factory default control values
	
	//game settings
	private int FLoadedLevel; //currently loaded level. used to determine whether to apply settings changes or not. Do not apply on start screen
	//QualitySettings												http://docs.unity3d.com/ScriptReference/QualitySettings.html
	public int FDefaultQualityLevel;
	//Light 																http://docs.unity3d.com/ScriptReference/Light.html
	public float FDefaultLightIntensity;
	//RenderSettings 												http://docs.unity3d.com/ScriptReference/RenderSettings.html
	public float FDefaultAmbientIntensity;
	public float FDefaultReflectionIntensity;
	//shadows (part of Light)									http://docs.unity3d.com/ScriptReference/Light.html
	public LightShadows FDefaultShadowType;
	public float FDefaultShadowStrength;
	public float FDefaultShadowDistance; //(part of quality settings
	//fog (part of RenderSettings)
	public bool FDefaultFogOn;
	public float FDefaultFogDensity;
	//AudioListener													http://docs.unity3d.com/ScriptReference/AudioListener.html
	public float FDefaultAudioLevel;
	
	private int FQualityLevel;
	private float FLightIntensity;
	private float FAmbientIntensity;
	private float FReflectionIntensity;
	private LightShadows FShadowType;
	private float FShadowStrength;
	private float FShadowDistance;
	private bool FFogOn;
	private float FFogDensity;
	private float FAudioLevel;
	
	private AsyncOperation FLoadProgress = null;
	
	public float FMaxHealth=100;
	public float FMaxStamina=100;
	public float FMaxStrength=100;
	private float FCurrentHealth;
	private float FCurrentStamina;
	private float FCurrentStrength;

	public Texture FHealthGauge;
	public Texture FStaminaGauge;
	public Texture FStrengthGauge;

	//FWindowID contstants
	const int WID_START=0,
	WID_PAUSE=1,
	WID_OPTIONS=2,
	WID_GAME=3,
	WID_CONTROLLER=4,
	WID_CREDITS=5;
	
	void Awake()
	{
		//do not destroy this game object on level loading so it remains persistent.
		//all data values remain intact during load and reload of levels
		if (FInstance==null)
		{
			FInstance=this;
			DontDestroyOnLoad(gameObject);
			FLoadImage.gameObject.SetActive(false);
			CreateControlsDictionary();
			ApplySettings();
			FCurrentHealth=FMaxHealth;
			FCurrentStamina=FMaxStamina;
			FCurrentStrength=FMaxStrength;
		}
		else
		{
			DestroyImmediate(gameObject);
		}
	}
	
	// Use this for initialization
	void Start () {
		FLoadedLevel=Application.loadedLevel;
		FTimeScale=Time.timeScale;//get this so it can be set to the correct value when the start/pause window closes
		FWindowRect = new Rect ((Screen.width - FWindowWidth)/2, (Screen.height - FWindowHeight)/2, FWindowWidth, FWindowHeight);//calculate window position, this is screen center
		FLoadProgressRect = new Rect((Screen.width - FWindowWidth)/2, 10.0f, FLoadProgressWidth, FLoadProgressHeight);
		FWindowID=WID_START;
		FHomeMenu=WID_START;
		Time.timeScale=0.0f;//pause everything and show Start window
		FShow=true;
	}
	
	IEnumerator LoadLevel(int level)
	{
		FLoadImage.gameObject.SetActive(true);
		FCanPause=false; //disable pause button
		FShow=false;
		Time.timeScale=0.0f;
		FLoadProgress=Application.LoadLevelAsync(level);
		while (!FLoadProgress.isDone)
		{
			yield return FLoadProgress;
		}
		FLoadProgress=null;
	}
	
	void OnLevelWasLoaded(int level)
	{
		FLoadedLevel=Application.loadedLevel;
		ApplySettings(); //be sure to apply custom settings each time a new level is loaded
		FShow = false;
		Time.timeScale=FTimeScale; //run game
		FCanPause=true; //allow pause button to function
		FLoadImage.gameObject.SetActive(false);
	}
	
	void CreateControlsDictionary()
	{
		FDefaultControls=new Dictionary<string, KeyCode>();
		FDefaultControls.Add ("Pause", KeyCode.Escape);
		FDefaultControls.Add("Forward", KeyCode.UpArrow);
		FDefaultControls.Add ("Backward", KeyCode.DownArrow);
		FDefaultControls.Add ("Left", KeyCode.LeftArrow);
		FDefaultControls.Add ("Right", KeyCode.RightArrow);
		FDefaultControls.Add ("Jump", KeyCode.Space);
		FDefaultControls.Add("Run Assist", KeyCode.LeftShift);
		
		LoadControls();
	}
	
	void LoadControls()
	{
		FKeyControls=new Dictionary<string, KeyCode>();
		foreach (var control in FDefaultControls) //for each button in default controls dictionary, add one to the game controls dictionary, and set it to the default, if not in player preferences
		{
			//this is simply converting the player preferences string value to a key code
			//if there is no player preference for that button, it just uses the one from the default controls
			FKeyControls.Add (control.Key.ToString(), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(control.Key.ToString (), control.Value.ToString())));
		}
		FQualityLevel=PlayerPrefs.GetInt ("QualityLevel", FDefaultQualityLevel);
		FLightIntensity=PlayerPrefs.GetFloat("LightIntensity", FDefaultLightIntensity);
		FAmbientIntensity=PlayerPrefs.GetFloat ("AmbientIntensity", FDefaultAmbientIntensity);
		FReflectionIntensity=PlayerPrefs.GetFloat("ReflectionIntensity", FDefaultReflectionIntensity);
		FShadowType=(LightShadows)System.Enum.Parse (typeof(LightShadows), PlayerPrefs.GetString ("ShadowType", FDefaultShadowType.ToString()));
		FShadowStrength=PlayerPrefs.GetFloat ("ShadowStrength", FDefaultShadowStrength);
		FShadowDistance=PlayerPrefs.GetFloat ("ShadowDistance", FDefaultShadowDistance);
		FFogOn=PlayerPrefs.GetInt("FogOn", (FDefaultFogOn ? 1:0)) != 0;//FDefaultFogOn ) != 0;
		FFogDensity=PlayerPrefs.GetFloat("FogDensity", FDefaultFogDensity);
		FAudioLevel=PlayerPrefs.GetFloat("AudioLevel", FDefaultAudioLevel);
	}
	
	void SaveControls()
	{
		foreach(var control in FKeyControls) //loop through control dictionary and write each value to player preferences
		{
			PlayerPrefs.SetString(control.Key.ToString(), FKeyControls[control.Key].ToString()); 
		}
		PlayerPrefs.SetInt("QualityLevel", FQualityLevel);
		PlayerPrefs.SetFloat ("LightIntensity", FLightIntensity);
		PlayerPrefs.SetFloat("AmbientIntensity", FAmbientIntensity);
		PlayerPrefs.SetFloat ("ReflectionIntensity", FDefaultReflectionIntensity);
		PlayerPrefs.SetFloat("AudioLevel", FAudioLevel);
		PlayerPrefs.SetString("ShadowType", FShadowType.ToString());
		PlayerPrefs.SetFloat("ShadowStrength", FShadowStrength);
		PlayerPrefs.SetFloat("ShadowDistance", FShadowDistance);
		PlayerPrefs.SetInt("FogOn", (FFogOn ? 1 : 0));
		PlayerPrefs.SetFloat ("FogDensity", FFogDensity);
		PlayerPrefs.Save();
	}
	
	void ApplySettings()
	{
		if (FLoadedLevel != 0)
		{
			QualitySettings.SetQualityLevel(FQualityLevel);
			DoLights();
			QualitySettings.shadowDistance=FShadowDistance;
			RenderSettings.ambientIntensity=FAmbientIntensity;
			RenderSettings.reflectionIntensity=FReflectionIntensity;
			RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox; //kludge so reflection intensity actually gets applied
			RenderSettings.fog=FFogOn;
			RenderSettings.fogDensity=FFogDensity;
		}
		AudioListener.volume=FAudioLevel;
	}
	
	void DoLights()
	{
		Light[] lights=GameObject.FindObjectsOfType<Light>();
		if (lights.Length==0) return; //no lights to set
		foreach (Light light in lights)
		{
			light.intensity=FLightIntensity;
			light.shadows=FShadowType;
			light.shadowStrength=FShadowStrength;
		}	
	}
	
	void PauseAudio(bool pause)
	{
		AudioSource[] sources = GameObject.FindObjectsOfType<AudioSource>();
		foreach (AudioSource source in sources)
		{
			//if there are any sources that need to continue playing, they need to be filtered out here so they do not pause
			if (pause) source.Pause();
			else source.UnPause();
		}
		//do pause menu sound last so it doesn't get paused with the rest of the sources
		AudioSource menumusic=gameObject.GetComponent<AudioSource>();
		if (pause) menumusic.Play();
		else menumusic.Stop();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//no need to set this in input manager, it is in the controls dictionary now
		if (Input.GetKeyDown(FKeyControls["Pause"]))
		{
			DoPauseWindow();
			return;
		}
		
		if (FShow) return;//paused do not accept any input or do any updating
		
		//other code to do update stuff here
	}
	
	public void DoPauseWindow()
	{
		if (!FCanPause || (FHomeMenu != WID_PAUSE) || (FWindowID != WID_PAUSE)) return;  //if not on the main pause window, don't do anything, only pause window gets pause key
		if (!FShow)
		{
			FShow = true;
			if (Time.timeScale != 0.0f)
				FTimeScale=Time.timeScale;
			Time.timeScale=0.0f;
		}
		else
		{
			Time.timeScale=FTimeScale;
			FShow=false;
		}
	}
	
	void OnGUI () 
	{
		if (FLoadProgress != null)
		{
			//do progressbar
			GUI.DrawTexture(FLoadProgressRect, FProgressPanel);
			GUI.DrawTexture(new Rect(FLoadProgressRect.x,FLoadProgressRect.y,FLoadProgress.progress*FLoadProgressRect.width, FLoadProgressRect.height), FProgressBar);
		}
		if (FShow)
		{
			FDeltaTime=Time.realtimeSinceStartup-FCurrentTime; //calculate deltatime for scrolling text since Time.deltaTime = 0 when paused
			FCurrentTime=Time.realtimeSinceStartup;
			if (!FGamePaused)
			{
				FScrollBarPos=Vector2.zero;
				FGamePaused=true;
				if (OnPause != null)
				{
					OnPause();
				}
				PauseAudio(true);
			}
			if (FWindowID==WID_CONTROLLER)
			{
				//there is no testing here for duplicate keys, so be aware multiple controls may be set to the same key
				if (FCurrentButton != "")//a button has been selected to edit
				{
					Event e=Event.current;
					string tempbutton=FCurrentButton;
					FCurrentButton=""; //clear it so the window knows to re-enable the button
					//normal keyboard buttons
					if (e.isKey) FKeyControls[tempbutton]=e.keyCode;
					//modifier keys
					//Note having Sticky Keys enabled messes up the modifier button input, disable sticky keys for proper action
					else if (Input.GetKey(KeyCode.LeftShift)) FKeyControls[tempbutton]=KeyCode.LeftShift;
					else if (Input.GetKey(KeyCode.RightShift)) FKeyControls[tempbutton]=KeyCode.RightShift;
					else if (Input.GetKey(KeyCode.LeftAlt)) FKeyControls[tempbutton]=KeyCode.LeftAlt; //left/right alt actually captured by e.isKey just here for consistency
					else if (Input.GetKey(KeyCode.RightAlt)) FKeyControls[tempbutton]=KeyCode.RightAlt;
					else if (Input.GetKey(KeyCode.LeftControl)) FKeyControls[tempbutton]=KeyCode.LeftControl; //left/right ctrl actually captured by e.isKey just here for consistency
					else if (Input.GetKey(KeyCode.RightControl)) FKeyControls[tempbutton]=KeyCode.RightControl;
					//joystick buttons
					else if (Input.GetKey(KeyCode.Joystick1Button0)) FKeyControls[tempbutton]=KeyCode.Joystick1Button0;
					else if (Input.GetKey(KeyCode.Joystick1Button1)) FKeyControls[tempbutton]=KeyCode.Joystick1Button1;
					else if (Input.GetKey(KeyCode.Joystick1Button2)) FKeyControls[tempbutton]=KeyCode.Joystick1Button2;
					else if (Input.GetKey(KeyCode.Joystick1Button3)) FKeyControls[tempbutton]=KeyCode.Joystick1Button3;
					else if (Input.GetKey(KeyCode.Joystick1Button4)) FKeyControls[tempbutton]=KeyCode.Joystick1Button4;
					else if (Input.GetKey(KeyCode.Joystick1Button5)) FKeyControls[tempbutton]=KeyCode.Joystick1Button5;
					else if (Input.GetKey(KeyCode.Joystick1Button6)) FKeyControls[tempbutton]=KeyCode.Joystick1Button6;
					else if (Input.GetKey(KeyCode.Joystick1Button7)) FKeyControls[tempbutton]=KeyCode.Joystick1Button7;
					else if (Input.GetKey(KeyCode.Joystick1Button8)) FKeyControls[tempbutton]=KeyCode.Joystick1Button8;
					else if (Input.GetKey(KeyCode.Joystick1Button9)) FKeyControls[tempbutton]=KeyCode.Joystick1Button9;
					else if (Input.GetKey(KeyCode.Joystick1Button10)) FKeyControls[tempbutton]=KeyCode.Joystick1Button10;
					else if (Input.GetKey(KeyCode.Joystick1Button11)) FKeyControls[tempbutton]=KeyCode.Joystick1Button11;
					else if (Input.GetKey(KeyCode.Joystick1Button12)) FKeyControls[tempbutton]=KeyCode.Joystick1Button12;
					else if (Input.GetKey(KeyCode.Joystick1Button13)) FKeyControls[tempbutton]=KeyCode.Joystick1Button13;
					else if (Input.GetKey(KeyCode.Joystick1Button14)) FKeyControls[tempbutton]=KeyCode.Joystick1Button14;
					else if (Input.GetKey(KeyCode.Joystick1Button15)) FKeyControls[tempbutton]=KeyCode.Joystick1Button15;
					else if (Input.GetKey(KeyCode.Joystick1Button16)) FKeyControls[tempbutton]=KeyCode.Joystick1Button16;
					else if (Input.GetKey(KeyCode.Joystick1Button17)) FKeyControls[tempbutton]=KeyCode.Joystick1Button17;
					else if (Input.GetKey(KeyCode.Joystick1Button18)) FKeyControls[tempbutton]=KeyCode.Joystick1Button18;
					else if (Input.GetKey(KeyCode.Joystick1Button19)) FKeyControls[tempbutton]=KeyCode.Joystick1Button19;
					//mouse buttons
					else if (e.isMouse) //if the event was a mouse click, set the control to the mouse button
					{
						switch (e.button)
						{
						case 0: FKeyControls[tempbutton]=KeyCode.Mouse0;
							break;
						case 1: FKeyControls[tempbutton]=KeyCode.Mouse1;
							break;
						case 2: FKeyControls[tempbutton]=KeyCode.Mouse2;
							break;
						case 3: FKeyControls[tempbutton]=KeyCode.Mouse3;
							break;
						case 4: FKeyControls[tempbutton]=KeyCode.Mouse4;
							break;
						case 5: FKeyControls[tempbutton]=KeyCode.Mouse5;
							break;
						case 6: FKeyControls[tempbutton]=KeyCode.Mouse6;
							break;
						}
					}
					else FCurrentButton=tempbutton; //restore button value, it wasn't here
				}
			}
			FWindowRect = GUI.Window(FWindowID, FWindowRect, ShowPauseWindow, FWindowTitle);
		}
		else //window no longer showing
		{
			if (FGamePaused) //if the game was paused, fire the unpaused event, and unpause
			{
				ApplySettings(); //make sure any changes made get applied
				FGamePaused=false;
				if (OnUnPause != null)
				{
					OnUnPause();
				}
				PauseAudio(false);
			}
			if (FLoadedLevel != 0) //don't show stats on start/pause menu
			{
				DisplayStatistics();
			}
		}
	}

	void DisplayStatistics()
	{
		//percent current health is of maxhealth
		//currenthealth*100/maxhealth
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUI.skin.label.fontStyle=FontStyle.Bold;
		GUI.skin.label.richText=true;
		GUI.BeginGroup(new Rect(10,10, 250,105));
		GUI.Box(new Rect(0,0, 250,105), "");
		GUI.Label(new Rect(10, 10, 200, 30), "<size=20><color=white>Health</color></size>");
		GUI.DrawTexture(new Rect(110, 15, 120*(FCurrentHealth/FMaxHealth), 20), FHealthGauge);
		GUI.Label(new Rect(10, 40, 200, 30), "<size=20><color=white>Stamina</color></size>");
		GUI.DrawTexture(new Rect(110, 45, 120*(FCurrentStamina/FMaxStamina), 20), FStaminaGauge);
		GUI.Label(new Rect(10, 70, 200, 30), "<size=20><color=white>Strength</color></size>");
		GUI.DrawTexture(new Rect(110, 75, 120*(FCurrentStrength/FMaxStrength), 20), FStrengthGauge);
		GUI.EndGroup();
	}

	// DoXXXX called by character to set values
	//they could return bool ex: return (FCurrentHealth>0)
	//returned as float so other scripts can also use the values returned
	public float DoHealth(float health)
	{
		FCurrentHealth+=health;
		if (FCurrentHealth<0) FCurrentHealth=0;
		else if (FCurrentHealth>FMaxHealth) FCurrentHealth=FMaxHealth;
		return (FCurrentHealth); //return alive or dead 
	}

	public float DoStamina(float stamina)
	{
		FCurrentStamina+=stamina;
		if (FCurrentStamina<0) FCurrentStamina=0;
		else if (FCurrentStamina>FMaxStamina) FCurrentStamina=FMaxStamina;
		return (FCurrentStamina); //return refreshed or tired
	}

	public float DoStrength(float strength)
	{
		//calculate remain strength
		FCurrentStrength=FCurrentStrength+strength;
		if (FCurrentStrength<0) FCurrentStrength-=strength;//reset to original value, cannot use, not enough strength
		else if (FCurrentStrength>FMaxStrength) FCurrentStrength=FMaxStrength;
		return (FCurrentStrength); //return strong or weak
	}

	// This is the actual window.
	void ShowPauseWindow (int windowid)
	{
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.fontStyle=FontStyle.Bold;
		GUI.skin.label.richText=true;
		
		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
		GUI.skin.button.richText=true;
		
		//GUI.DrawTexture(new Rect(0,0, FWindowWidth, FWindowHeight), FTexture, ScaleMode.StretchToFill); //put background texture on all window backgrounds
		//GUI.DrawTexture(new Rect(10,10,100,100), FTexture, ScaleMode.ScaleToFit); //draw texture in a confined space
		
		if (windowid==WID_START)
		{
			//put background texture only on this window
			//You can have a different texture for each window, just make new public Texture properties and drag the desired textures onto them in the inspector
			//GUI.DrawTexture(new Rect(0,0, FWindowWidth, FWindowHeight), FTexture); 
			ShowStartWindow(); //game just started, show start menu
		}
		else if (windowid==WID_PAUSE)
		{
			ShowPauseWindow(); //player paused the game, show pause menu
		}
		else if (windowid==WID_OPTIONS)
		{
			ShowOptionsWindow(); //player chose Options, show options window
		}
		else if (windowid==WID_GAME)
		{
			ShowGameWindow();//game settings
		}
		else if (windowid==WID_CONTROLLER)
		{
			ShowControllerWindow();//player chose Controller Settings, show controller settings window
		}
		else if (windowid==WID_CREDITS)
		{
			ShowCreditsWindow(); //player chose Credits, show credits window
		}
	}
	
	void ShowStartWindow()
	{
		FWindowTitle="START";
		float windowdivider=FWindowRect.height/(FNumberOfStartButtons+2);
		float dividercenter=(windowdivider/2);
		//window label
		float y = dividercenter;
		GUI.Label(new Rect(0,y, FWindowRect.width, FButtonHeight), "<size=40><color=yellow>WELCOME</color></size>");
		
		//button1
		dividercenter-=(FButtonHeight/2);
		y=y+windowdivider+dividercenter;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=green>Begin Game</color></size>"))
		{
			FWindowID=WID_PAUSE; //game started, next time window is shown it'll be the pause window so get ready
			FHomeMenu=WID_PAUSE;
			StartCoroutine("LoadLevel",FBeginGameLevel);
		}
		
		//button2
		y=y+windowdivider+dividercenter;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=blue>Options</color></size>"))
		{
			FWindowID=WID_OPTIONS; //show options window
		}
		
		//button3
		y=y+windowdivider+dividercenter;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=red>Exit</color></size>"))
		{
			Application.Quit(); //quit the game
		}
	}
	
	void ShowPauseWindow()
	{
		FWindowTitle="PAUSED";
		float windowdivider=FWindowRect.height/(FNumberOfPauseButtons+2);
		float dividercenter=(windowdivider/2);
		
		//window label
		float y = dividercenter;
		GUI.Label(new Rect(0,y, FWindowRect.width, FButtonHeight), "<size=40><color=yellow>GAME PAUSED</color></size>"); //Main Window Label
		
		//button1
		dividercenter-=(FButtonHeight/2);
		y=y+windowdivider+dividercenter;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=green>Resume</color></size>"))
		{
			FShow = false;
			Time.timeScale=FTimeScale; //resume the game
		}
		
		//button2
		y=y+windowdivider+dividercenter;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=blue>Options</color></size>"))
		{
			FWindowID=WID_OPTIONS; //show options window
		}
		
		//button3
		y=y+windowdivider+dividercenter;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=magenta>Restart</color></size>"))
		{
			OnPause=null; //clear the event connections so they can reconnect after reloading
			OnUnPause=null; //clear the event connections so they can reconnect after reloading
			StartCoroutine("LoadLevel",Application.loadedLevel); //restart the level
		}
		
		//button4
		y=y+windowdivider+dividercenter;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=red>Exit</color></size>"))
		{
			Application.Quit(); //quit the game
		}
	}
	
	void ShowOptionsWindow()
	{
		FWindowTitle="OPTIONS";
		float windowdivider=FWindowRect.height/(FNumberOfOptionsButtons+2);
		float dividercenter=(windowdivider/2);
		
		//window label
		float y = dividercenter;
		GUI.Label(new Rect(0,y, FWindowRect.width, FButtonHeight), "<size=40><color=yellow>OPTIONS</color></size>"); //Main Window Label
		
		//button1
		dividercenter-=(FButtonHeight/2);
		y=y+windowdivider+dividercenter;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=green>Game Settings</color></size>"))
		{
			FWindowID=WID_GAME; //show game settings window
		}
		
		//button2
		y=y+windowdivider+dividercenter;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=green>Controller Setttings</color></size>"))
		{
			FWindowID=WID_CONTROLLER; //show controller settings window
		}
		
		//button3
		y=y+windowdivider+dividercenter;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=green>Credits</color></size>"))
		{
			FScrollY=200;
			FWindowID=WID_CREDITS; //show credits window
		}
		
		//button4
		y=y+windowdivider+dividercenter;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=blue>Main Menu</color></size>"))
		{
			FWindowID=FHomeMenu;
		}
	}
	
	void ShowCreditsWindow()
	{
		FWindowTitle="CREDITS";
		string credits="Game Created by Me\nSound By Me\nEffects By Me\nModels By Me\nAnimatons By Me\nCharacters By Me\nOther Random Stuff By Me";
		credits=credits+"\nArtwork By Me\nTerrains By Me\nCookies By Me\nCakes By Me\nCandy By Me\n\n\n\n\nOh Yeah . . .\n\nand Credits By Me";
		float windowdivider=FWindowRect.height/(6);//divide window into areas
		float dividercenter=(windowdivider/2);
		
		//window label
		float y = dividercenter;
		GUI.Label(new Rect(0,y, FWindowRect.width, FButtonHeight), "<size=40><color=yellow>CREDITS</color></size>"); //Main Window Label
		y+=80;
		
		//START SCROLLING TEXT
		GUI.skin.label.fontStyle=0;//turn off bold
		GUI.skin.label.fontSize=20;//set this to desired font size so calcheight returns correct value
		float textsize=GUI.skin.label.CalcHeight(new GUIContent (credits), FWindowRect.width-20);//get how tall the label is going to be
		GUI.BeginGroup(new Rect(10, y, FWindowRect.width-20, 200));//begin a GUI group to clip the label text when it scrolls 
		FScrollY-=(FScrollRate*FDeltaTime);//get the new label scrolled position
		if (FScrollY<-textsize) FScrollY=200;//put back to bottom of box if it scrolled all the text off the top
		GUI.Box(new Rect(0,0, FWindowRect.width-20, 200),"");//create a box, positioned relative to the group, to put the label in so it looks nice 
		GUI.Label(new Rect(0,FScrollY, FWindowRect.width-20, textsize), "<size=20><color=white>"+credits+"</color></size>");//place the label
		GUI.EndGroup();//close the group
		//END SCROLLING TEXT
		
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, 325, FButtonWidth, FButtonHeight), "<size=30><color=blue>Back to Options</color></size>"))
		{
			FWindowID=WID_OPTIONS; //reshow options window
		}	
	}
	
	void ShowControllerWindow()
	{
		FWindowTitle="CONTROLLER SETTINGS";
		
		//window label
		float y = 28;
		GUI.Label(new Rect(0,y, FWindowRect.width, FButtonHeight), "<size=40><color=yellow>"+FWindowTitle+"</color></size>"); //Main Window Label
		
		y=y+50;
		GUI.BeginGroup(new Rect(10, y, FWindowRect.width-20, 250));
		float scrollheight=(FKeyControls.Count+1)*(FControlButtonHeight+10);//how high to make the scroll window. all control buttons plus one for reset default button
		GUI.Box(new Rect(0,0,FWindowRect.width-20,250), ""); //place a box to make it look better
		FScrollBarPos=GUI.BeginScrollView(new Rect(0,0,FWindowRect.width-20,250), FScrollBarPos, new Rect(0,0, 400,scrollheight)); //place scroll window
		
		//place buttons
		y=10;//start position for button group
		foreach (var control in FKeyControls)
		{
			string name=control.Key.ToString ();
			if (name=="Pause") continue; //example of how to skip a button that should never be changeable
			GUI.Label(new Rect(10, y, FButtonWidth/2, FControlButtonHeight), "<size=20><color=white>"+name+"</color></size>");
			if (ShowButtonOrBox(new Rect(((FWindowRect.width/2)-20), y, FButtonWidth/2, FControlButtonHeight), control.Value.ToString(), "20",(FCurrentButton==name)))
			{
				FCurrentButton=name;
			}
			y=y+FControlButtonHeight+10; //increment to move next button down
		}
		//reset to defaults button
		if (GUI.Button(new Rect(10, y, FButtonWidth-5, FControlButtonHeight), "<size=20><color=red>Reset to Defaults</color></size>"))
		{
			//reset values here
			FCurrentButton=""; //clear button
			FKeyControls=new Dictionary<string, KeyCode>(FDefaultControls); //set controls back to default
		}
		GUI.EndScrollView(); //close the scroll view group
		GUI.EndGroup(); //close the window group
		
		//return to option window button
		y=335;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=blue>Back to Options</color></size>"))
		{
			FCurrentButton=""; //clear button
			SaveControls(); //save the values
			FWindowID=WID_OPTIONS; //reshow options window
		}
	}
	
	bool ShowButtonOrBox(Rect rect, string text, string size, bool isselected)
	{
		if (isselected) 
		{
			GUI.Box(rect, "<size="+size+"><color=lime>"+text+"</color></size>");
			return false;
		}
		else 
		{
			return GUI.Button(rect, "<size="+size+"><color=white>"+text+"</color></size>");
		}
	}
	
	void ShowGameWindow()
	{
		FWindowTitle="GAME SETTINGS";
		float sliderposadder=10.0f; //how much to add to y for slider placement 
		float y = 28;
		GUI.Label(new Rect(0,y, FWindowRect.width, FButtonHeight), "<size=40><color=yellow>"+FWindowTitle+"</color></size>"); //Main Window Label
		
		y=y+50;
		GUI.BeginGroup(new Rect(10, y, FWindowRect.width-20, 250));
		
		//NEEDS GRAPHICS CONTROLS COUNT FOR  (scrollheight=(10+1) The 10 needs to be the count
		
		
		float scrollheight=(10+1)*(FControlButtonHeight+10);//how high to make the scroll window. all control buttons plus one for reset default button
		
		GUI.Box(new Rect(0,0,FWindowRect.width-20,250), ""); //place a box to make it look better
		FScrollBarPos=GUI.BeginScrollView(new Rect(0,0,FWindowRect.width-20,250), FScrollBarPos, new Rect(0,0, 400,scrollheight)); //place scroll window
		
		//place controls
		y=10;//start position for button group
		int qualitylevel=QualitySettings.GetQualityLevel();
		string name=QualitySettings.names[qualitylevel];
		int maxqualitylevel=QualitySettings.names.Length-1;//default is not included, so ignore it
		GUI.Label(new Rect(10, y, FButtonWidth/2, FControlButtonHeight), "<size=20><color=white>"+name+"</color></size>");
		int tempqualitylevel=qualitylevel;
		FQualityLevel=(int)GUI.HorizontalSlider(new Rect(((FWindowRect.width/2)-20), y+sliderposadder, FButtonWidth/2, FSettingsScrollHeight), qualitylevel, 0, maxqualitylevel);
		if (tempqualitylevel != FQualityLevel) if (FLoadedLevel != 0) QualitySettings.SetQualityLevel(FQualityLevel);
		y=y+FControlButtonHeight+10; //increment to move next button down
		
		name="Light Intensity";
		GUI.Label(new Rect(10, y, FButtonWidth/2, FControlButtonHeight), "<size=20><color=white>"+name+"</color></size>");
		float tempintensity=FLightIntensity;
		FLightIntensity=GUI.HorizontalSlider(new Rect(((FWindowRect.width/2)-20), y+sliderposadder, FButtonWidth/2, FSettingsScrollHeight), FLightIntensity, 0.0f, 8.0f);
		if (tempintensity != FLightIntensity) if (FLoadedLevel != 0) DoLights();
		y=y+FControlButtonHeight+10; //increment to move next button down
		
		name="Ambient Intensity";
		GUI.Label(new Rect(10, y, FButtonWidth/2, FControlButtonHeight), "<size=20><color=white>"+name+"</color></size>");
		tempintensity=FAmbientIntensity;
		FAmbientIntensity=GUI.HorizontalSlider(new Rect(((FWindowRect.width/2)-20), y+sliderposadder, FButtonWidth/2, FSettingsScrollHeight), FAmbientIntensity, 0.0f, 1.0f);
		if (tempintensity != FAmbientIntensity) if (FLoadedLevel != 0) RenderSettings.ambientIntensity=FAmbientIntensity;
		y=y+FControlButtonHeight+10; //increment to move next button down
		
		name="Reflection Intensity";
		GUI.Label(new Rect(10, y, FButtonWidth/2, FControlButtonHeight), "<size=20><color=white>"+name+"</color></size>");
		tempintensity=FReflectionIntensity;
		FReflectionIntensity=GUI.HorizontalSlider(new Rect(((FWindowRect.width/2)-20), y+sliderposadder, FButtonWidth/2, FSettingsScrollHeight), FReflectionIntensity, 0.0f, 1.0f);
		if (tempintensity != FReflectionIntensity) if (FLoadedLevel != 0)
		{
			RenderSettings.reflectionIntensity=FReflectionIntensity;
			RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox; //kludge so reflection intensity actually gets applied
		}
		y=y+FControlButtonHeight+10; //increment to move next button down
		
		name="Shadow Type: "+FShadowType.ToString();
		GUI.Label(new Rect(10, y, FButtonWidth/2, FControlButtonHeight), "<size=20><color=white>"+name+"</color></size>");
		int tempshadows=(int)FShadowType; //convert shadow enum to int
		tempshadows=(int)GUI.HorizontalSlider(new Rect(((FWindowRect.width/2)-20), y+sliderposadder, FButtonWidth/2, FSettingsScrollHeight), tempshadows, 0, 2);
		if (tempshadows != (int)FShadowType) if (FLoadedLevel != 0)
		{
			FShadowType=(LightShadows)tempshadows; //convert int back to LightShadows enum
			DoLights();
		}
		y=y+FControlButtonHeight+10; //increment to move next button down
		
		name="Shadow Strength";
		GUI.Label(new Rect(10, y, FButtonWidth/2, FControlButtonHeight), "<size=20><color=white>"+name+"</color></size>");
		float tempstrength=FShadowStrength;
		FShadowStrength=GUI.HorizontalSlider(new Rect(((FWindowRect.width/2)-20), y+sliderposadder, FButtonWidth/2, FSettingsScrollHeight), FShadowStrength, 0.0f, 1.0f);
		if (tempstrength != FShadowStrength) if (FLoadedLevel != 0) DoLights();
		y=y+FControlButtonHeight+10; //increment to move next button down

		name="Shadow Distance";
		GUI.Label(new Rect(10, y, FButtonWidth/2, FControlButtonHeight), "<size=20><color=white>"+name+"</color></size>");
		float tempdistance=FShadowDistance;
		FShadowDistance=GUI.HorizontalSlider(new Rect(((FWindowRect.width/2)-20), y+sliderposadder, FButtonWidth/2, FSettingsScrollHeight), FShadowDistance, 1.0f, 300.0f);
		if (tempdistance != FShadowDistance) if (FLoadedLevel != 0) QualitySettings.shadowDistance=FShadowDistance;
		y=y+FControlButtonHeight+10; //increment to move next button down
		
		name="Fog";
		GUI.Label(new Rect(10, y, FButtonWidth/2, FControlButtonHeight), "<size=20><color=white>"+name+"</color></size>");
		bool tempfog=FFogOn;
		FFogOn=GUI.Toggle(new Rect(((FWindowRect.width/2)-20), y+sliderposadder, FButtonWidth/2, FSettingsScrollHeight), FFogOn, FFogOn.ToString());
		if (tempfog != FFogOn) if (FLoadedLevel != 0) RenderSettings.fog=FFogOn;
		y=y+FControlButtonHeight+10; //increment to move next button down
		
		name="Fog Density";
		GUI.Label(new Rect(10, y, FButtonWidth/2, FControlButtonHeight), "<size=20><color=white>"+name+"</color></size>");
		float tempdensity=FFogDensity;
		FFogDensity=GUI.HorizontalSlider(new Rect(((FWindowRect.width/2)-20), y+sliderposadder, FButtonWidth/2, FSettingsScrollHeight), FFogDensity, 0.0f, 1.0f);
		if (tempdensity != FFogDensity) if (FLoadedLevel != 0) RenderSettings.fogDensity=FFogDensity;
		y=y+FControlButtonHeight+10; //increment to move next button down
		
		name="Audio Level";
		GUI.Label(new Rect(10, y, FButtonWidth/2, FControlButtonHeight), "<size=20><color=white>"+name+"</color></size>");
		float tempaudio=FAudioLevel;
		FAudioLevel=GUI.HorizontalSlider(new Rect(((FWindowRect.width/2)-20), y+sliderposadder, FButtonWidth/2, FSettingsScrollHeight), FAudioLevel, 0.0f, 1.0f);
		if (tempaudio != FAudioLevel) AudioListener.volume=FAudioLevel;
		y=y+FControlButtonHeight+10; //increment to move next button down
		
		//reset to defaults button
		if (GUI.Button(new Rect(10, y, FButtonWidth-5, FControlButtonHeight), "<size=20><color=red>Reset to Defaults</color></size>"))
		{
			//reset values here
			FCurrentButton=""; //clear button
			FQualityLevel=FDefaultQualityLevel;
			FLightIntensity=FDefaultLightIntensity;
			FAmbientIntensity=FDefaultAmbientIntensity;
			FReflectionIntensity=FDefaultReflectionIntensity;
			FShadowType=FDefaultShadowType;
			FShadowStrength=FDefaultShadowStrength;
			FShadowDistance=FDefaultShadowDistance;
			FFogOn=FDefaultFogOn;
			FFogDensity=FDefaultFogDensity;
			FAudioLevel=FDefaultAudioLevel;
			ApplySettings();
		}
		GUI.EndScrollView(); //close the scroll view group
		GUI.EndGroup(); //close the window group
		
		//return to option window button
		y=335;
		if (GUI.Button (new Rect((FWindowRect.width-FButtonWidth)/2, y, FButtonWidth, FButtonHeight), "<size=30><color=blue>Back to Options</color></size>"))
		{
			FCurrentButton=""; //clear button
			SaveControls(); //save the values
			FWindowID=WID_OPTIONS; //reshow options window
		}
	}
}

//alternative method to handling duplicated instances in hierarchy on restart
//goes in awake
/*
		DontDestroyOnLoad(this.gameObject);
		//if it finds a duplicate of itself destroy it.
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}*/

// ***********************************************************************************\\
//FAULTY OR NON-IMPLEMENTED UNITY FEATURES

//no way to change input manager settings at runtime:
//https://feedback.unity3d.com/suggestions/scripting-expose-input-manager-

//no way to change shadow quality at runtime:
//https://feedback.unity3d.com/suggestions/allow-shadow-quality-to-be-changed-at-runtime-via-scripts

// ***********************************************************************************//


