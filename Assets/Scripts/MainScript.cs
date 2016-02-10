using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum GameScreen
{
    Menu,
    Game,
    Edit,
    Win,
    End,
    Scores,
    Network,
    NewLevel,
    ChooseMapPackConstruction,
    Continue,
    EnterIP,
    Construction,
    LocalGame,
    NetworkLobby,
    Tanks
}

public enum Bonus
{
    Life,
    Upgrade,
    Bomb,
    Freeze,
    Fort,
    Shield,
    FullUpgrade,
    Ship
}

public enum LabObject
{ 
    Empty, //00
    EnemyRespawn, //01
    PlayerRespawn, //02
    Bricks, //03
    Concrete, //04
    Water, //05
    Forest, //06
    Ice, //07
    Fortress //08
}

public class Player
{
    public int lifes;
    public int number;
    public int[] kills;
    public int[] viewKills;
    public int scores;
    public int[] scores1;
    public int rank;

    public Player()
    {
        kills = new int[5];
        viewKills = new int[5];
        scores1 = new int[5];
        scores = 0;
        lifes = 0;
        number = 0;
        rank = 0;
    }
}

public class Flower
{
    public float X;
    public float Y;
    public float curX;
    public float curY;
    public float a;
    public float time;
    public float timeEnd;
    public string txt;
    public int state;
    public bool special;

    public Flower()
    {
        time = 0.0f;
        curX = 0.0f;
        curY = 0.0f;
        X = 0.0f;
        Y = 0.0f;
        a = 0.0f;
        txt = string.Empty;
        state = 0;
        special = false;
    }
}

public class TankInfo
{
    public string name;
    public float width;
    public float length;
    public float height;
    public float speed;
    public float bulletSpeed;
    public int armour;
    public int ammo;

    public TankInfo(string n, float w, float l, float h, float s, float bs, int ar, int am)
    {
        name = n;
        width = w;
        length = l;
        height = h;
        speed = s;
        bulletSpeed = bs;
        armour = ar;
        ammo = am;
    }
}

public class MainScript : MonoBehaviour
{
    public string version;
    public string about1;
    public string about2;



    public bool debug;

    public bool noSound = false;

    public GameObject infoCamera;
    public GameObject infoTank;
    public int currentInfoTank = 0;

    private TankInfo[] tankInfo = new TankInfo[] {
        //           width   length  height  speed bulletspeed armour ammo
        new TankInfo("Player Rank 1", 1.625f, 1.625f, 0.875f, 3.0f, 7.5f, 1, 1),
        new TankInfo("Player Rank 2", 1.625f, 2.000f, 0.875f, 3.0f, 15.0f, 1, 1),
        new TankInfo("Player Rank 3", 1.625f, 1.875f, 0.875f, 3.0f, 15.0f, 1, 2),
        new TankInfo("Player Rank 4", 1.750f, 1.875f, 1.000f, 3.0f, 15.0f, 1, 2),

        new TankInfo("Enemy Rank 1", 1.625f, 1.875f, 0.875f, 2.0f, 7.5f, 1, 1),
        new TankInfo("Enemy Rank 2", 1.625f, 1.875f, 0.875f, 4.0f, 7.5f, 1, 1),
        new TankInfo("Enemy Rank 3", 1.625f, 1.875f, 0.875f, 2.0f, 15.0f, 1, 1),
        new TankInfo("Enemy Rank 4", 1.750f, 1.875f, 1.000f, 2.0f, 7.5f, 4, 1)
    };

    public PoolObjects smallExplosions;
    public PoolObjects bigExplosions;
    public PoolObjects spawns;
    public PoolObjects spawnSprites;

    public Transform bullet;

    public Material bricksMat;

    public Material concreteMat;

    public Transform bigExplosion;

    public Transform explosion;

    public Texture whiteSquare;

    public Texture tankTexture;

    public Texture star;

    public Texture gameNameTex;

    public Transform[] prefabs;

    public Transform tankPrefab;

    public int currentLevel;

    public int[,] labyrinth;

    public int[,] labyrinth2;

    public Transform[] planePrefab;

    public Transform[,] labTransforms;

    public GameScreen gameScreen;

    private GameScreen newGameScreen;

    public Transform labParent;

    public Camera mainCamera;

    public Transform bonus;

    public GUISkin gameSkin;

    public Texture playerTank;
    public Texture enemyTank;
    public Texture[] paints;
    public int cameraMode;
    private int sizeLabX;
    private int sizeLabY;
    private float respawnTimer;

    public float respawnRate;
    private float salutTimer;
    private float salutRate = 1.0f;

    public int enemysScreen;

    public int numberSpawnedEnemy;

    public int numberEnemyTanks;
    private int allEnemy = 20;
    private float originalWidth = 640.0f;
    private float originalHeight = 480.0f;
    private Vector3 scale;
    private float scaleX;
    private Matrix4x4 scaleMatrix;
    private string stringToEdit;
    private int currentSelectV;
    private bool firstMoveAxeV;
    private int oldAxe;
    private int currentSelectH;
    private int oldSelectH;
    private int oldSelectV;
    private bool firstMoveAxeH;
    private int oldAxeH;
    private Vector3 planeHitPoint;

    public int difficulty;
    private Transform planeTransform;

    public Transform spawnPrefab;

    public Fade fade;
    private bool fading;
    public Texture[] bonusTextures;

    public int freezeEnemies;
    public int freezePlayers;
 
    private int[,] blocks;
    private int currentScore;

    public bool gameOver;
    private bool showLabelPressStartButton;
    private float alphaLabelPressStartButton;
    private bool directionLabelPressStartButton;

    public bool pause;

    public float otherTime;

    public float otherDelta;
    private bool showWindowConnect;
    private bool showWindowError;
    private bool showWindowErrorConnect;
    
    public NetworkView nv;
    private string labelError;
    [HideInInspector]
    public Transform[] tanksTransforms;
    [HideInInspector]
    public TankScript[] tanksScripts;
    [HideInInspector]
    public Renderer[] tanksRenderers;
    [HideInInspector]
    public Collider[] tanksColliders;

    [HideInInspector]
    public NetworkRigidbody[] nrbs;

    [HideInInspector]
    public Transform[] bulletTransforms;
    [HideInInspector]
    public BulletScript[] bulletScripts;
    [HideInInspector]
    public Transform[] bulletsExplosionsTransforms;
    //[HideInInspector]
    //public Detonator[] bulletsDetonators;
    [HideInInspector]
    public Transform[] tanksExplosionsTransforms;
    //[HideInInspector]
    //public Detonator[] tanksDetonators;

    public Transform bullets;

    public Transform spawnsRoot;

    public Transform explosions;

    public Transform tanksRoot;

    public AudioClip newLevelAudio;

    public AudioClip menuSound;

    public AudioClip motorSound;
    
    private int[] typesTanks;
    private HostData[] hostData;
    public bool noTanks;
    private string[] levels;
   
    public Mesh[] playerMeshes;

    public Mesh[] enemyMeshes;

    public Material[] mats;

    public Material[] matsTrucks;

    public Texture[] textures;
    private int hiScore;
    private int currentMapPack;
    private FileInfo[] fileInfoMapPacksArray;

    public int playersCount = 1;

    public Texture trucks;

    public Player[] playersList;

    public AudioClip menuMusic;

    public AudioClip[] combatMusic;

    public AudioClip continueMusic;

    public AudioSource music;

    public Transform[] playersCameraViews;
    private int indexAnimTank;
    private float timerAnimTank;

    public Texture[] animTankTextures;

    public AudioClip[] soundsMotors;

    public Transform bonusTransform;
    private BonusScript bonusScript;

    private string[] mainMenuStrings = new string[]
    {
        "LOCAL GAME",
        "NETWORK GAME",
        "CONSTRUCTION",
        "TANKS",
        "EXIT"
    };
        
    private string[] choosePlayersStrings = new string[]
    {
        "1 PLAYER",
        "2 PLAYERS",
        "3 PLAYERS",
        "4 PLAYERS",
        "BACK"
    };

    private string[] networkMenuStrings = new string[]
    {
        "IP_LABEL",
        "CONNECT",
        "SERVER",
        "BACK",
        "IP_CONNECT"
    };
    
    private string[] chooseMapPackMenuStrings = new string[]
    {
        "NEW",
        "EDIT",
        "DELETE"
    };
    
    private string[] difficultyMenuStrings = new string[] 
    {
        "EASY",
        "NORMAL",
        "HARD"
    };
    
    private string[] continueMenuStrings = new string[]
    {
        "CONTINUE",
        "MENU"
    };
    
    private string[] difficultyInfo = new string[]
    {
        "99 LIVES, INFINITE CONTINUES, NO BONUSES FOR CPU",
        "3 LIVES, NO CONTINUES, NO BONUSES FOR CPU",
        "3 LIVES, NO CONTINUES, CPU MAY COLLECT BONUSES"
    };
    private string[] difficultyInfo1 = new string[]
    {
        "LIFES: 99", "LIFES: 3", "LIFES: 3"
    };
    private string[] difficultyInfo2 = new string[]
    {
        "CONTINUES: 99", "CONTINUES: 0", "CONTINUES: 0"
    };
    private string[] difficultyInfo3 = new string[]
    {
        "NO BONUSES FOR CPU", "NO BONUSES FOR CPU", "CPU MAY COLLECT BONUSES"
    };

    //Количество строк всегда равняется количеству позиций курсора.
    private string[] newLocalGameStrings = new string[]
    { 
        "NUMBER OF PLAYERS: ",
        "MAP PACK: ",
        "DIFFICULTY: ",
        "START",
        "BACK"
    };
    private int[] newLocalGameRectCursor = new int[]
    { 
        32,
        64,
        96,
        304,
        336
    };

    private List<Flower> flowerList = new List<Flower>();
    private float freezeRate = 1.0f;
    private float freezePlayersRate = 1.0f;
    private float scoresRate = 0.125f;
    private Rect rectWindowConnect = new Rect(140.0f, 180.0f, 360.0f, 120.0f);
    private string connectToIP = "127.0.0.1";
    
    private Vector3[] bonusPositions = new Vector3[]
    {
        new Vector3(10.0f, 0.0f, 22.0f),
        new Vector3(22.0f, 0.0f, 10.0f),
        new Vector3(22.0f, 0.0f, 22.0f),
        new Vector3(10.0f, 0.0f, 10.0f)
    };
    
    private int[] scores = new int[4] { 100, 200, 300, 400 };
    private int[,] randomTypeTank = new int[15, 4]
    {
        {  95, 100, 100, 100},
        {  90, 100, 100, 100},
        {  85, 100, 100, 100},
        {  80, 100, 100, 100},
        {  75, 100, 100, 100},
        {  70,  95, 100, 100},
        {  65,  90, 100, 100},
        {  60,  85, 100, 100},
        {  55,  80, 100, 100},
        {  50,  75, 100, 100},
        {  45,  70,  95, 100},
        {  40,  65,  90, 100},
        {  35,  60,  85, 100},
        {  30,  55,  80, 100},
        {  25,  50,  75, 100}
    };
    private string codeNet = "Tnks_234fFW3n";
    //private string endLevel = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000333333003300330033330000003333330033003300333300000033000000333033003300330000330000003330330033003300003333330033033300330033000033333300330333003300330000330000003300330033003300003300000033003300330033000033333300330033003333000000333333003300330033330000444444444444444444444444444444444444444444444444444400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
    //private string defaultLevel = "";
    private string fileToLoad = string.Empty;
    [HideInInspector]
    public int allTanks = 24;
    private int allTanksx2 = 48;
    private Rect rectWindowChooseSize = new Rect(140.0f, 180.0f, 360.0f, 120.0f);

    private string[] playersIPs = { "", "", "", "" };
    private int[] playersPings = { 0, 0, 0, 0 };

    private string[] chooseSizeStrings = new string[] {"13x13", "17x13"};
    
    private Rect rectWindowEnterName = new Rect(140.0f, 180.0f, 360.0f, 120.0f);
    private string newMapPackName = "NEW_MAP_PACK";
    private int maxSymbol = 12;
    private int[] symbols = new int[37]
    {
        32,
        48,
        49,
        50,
        51,
        52,
        53,
        54,
        55,
        56,
        57,
        65,
        66,
        67,
        68,
        69,
        70,
        71,
        72,
        73,
        74,
        75,
        76,
        77,
        78,
        79,
        80,
        81,
        82,
        83,
        84,
        85,
        86,
        87,
        88,
        89,
        90
    };
    
    public Vector3[] playerRespawns = new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
    public Vector3[] enemyRespawns = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
    
    private Color[] colors = new Color[] {Color.yellow, Color.green, Color.red, Color.blue};
    
    private float timerRateAnimTank = 0.02f;

    private float freezeEnemiesTimer = 0.0f;
    private float freezePlayersTimer = 0.0f;
    private float scoresTimer = 0.0f;
    private bool showWindowEnterName = false;
    private bool showWindowChooseSize = false;

    private string[] objTags = new string[] {
        "Empty", //0
        "EnemyRespawn", //1
        "PlayerRespawn", //2
        "Bricks", //3
        "Concrete", //4
        "Water", //5
        "Forest", //6
        "Ice", //7
        "Fortress" //8
    };

    //Ликвидация горизонтального отступа. Указывать в GUI.Label и подобных вместо 0.
    //private int 0 = 0;

    //Текущий лист серверов.
    private int hostDataList = 0;

    private Vector2[] cursorPositionsNetwork = new Vector2[]
    {
        new Vector2(208, 16),  //0
        new Vector2(208, 48),  //1
        new Vector2(208, 80),  //2
        new Vector2(208, 112),  //3
        new Vector2(208, 144),  //4
        new Vector2(208, 176),  //5
        new Vector2(208, 208),  //6
        new Vector2(208, 240),  //7
        new Vector2(208, 272),  //8
        new Vector2(208, 304),  //9
        new Vector2(208, 336),  //10
        new Vector2(144, 384),  //11
        new Vector2(400, 384),  //12
        new Vector2(224, 416)  //13
    };

    public string currentMapPackStr = "";

    private float playerInfoTimer = 0.0f;
    private float playerInfoTimerRate = 1.0f;

    private float updateServersInfoTimer = 0.0f;
    private float updateServersInfoTimerRate = 1.0f;

    public AudioSource audioSmallExplosion;
    public AudioSource audioBigExplosion;

    private List<NetworkPlayer> networkPlayers;

    public int[] ipNumbers = new int[4]{0,0,0,0};
    
    //public int currentPlayerSpawn = -1;
    //public int currentEnemySpawn = -1;
    //public int currentEnemyTypeSpawn = -1;
    //public int currentEnemyPlaceSpawn = -1;

    //-------------------------------------------------------------------------------------------------------------

    private int sw2 = Screen.width / 2;
    private int sh2 = Screen.height / 2;
    private int sw = Screen.width;
    private int sh = Screen.height;

    private const int fontSize = 32;

    public AudioSource buttonMoveSound;

    private bool stageClear = false;

    // 11
    // 11 - блок респауна врагов

    // 88
    // 88 - крепость

    // 22
    // 2Х - респаун игрока, где Х - номер.

    // 33
    // 33 - кирпичный блок

    // 44
    // 44 - бетонный блок

    private string defaultLevel = "1100000000001100000000001111000000000011000000000011000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003333000000000002200000022038830220000002224000000210388302200000023";

    private int[] scoresToLifes = { 0, 0, 0, 0 };

    private int valueLife = 10000;

    public AudioSource giveLifeSound;

    private void Awake()
    {
        connectToIP = ipNumbers[0].ToString("D3") + "." +
            ipNumbers[1].ToString("D3") + "." +
            ipNumbers[2].ToString("D3") + "." +
            ipNumbers[3].ToString("D3");
    }



    private void Start()
    {
        //nv.RPC()

        enemyRespawns = new Vector3[3] 
        {
            new Vector3(3,0,28),
            new Vector3(15,0,28),
            new Vector3(27,0,28)
        };

        //Debug.Log((11 / 2).ToString());

        labyrinth = new int[26, 26];
        labTransforms = new Transform[13, 13];

        networkPlayers = new List<NetworkPlayer>();

        scaleX = (float)Screen.height / originalHeight;

        playersList = new Player[4];
        playersList[0] = new Player();
        playersList[1] = new Player();
        playersList[2] = new Player();
        playersList[3] = new Player();
        hostData = new HostData[0];

        bonusScript = bonusTransform.GetComponent<BonusScript>();
        //MasterServer.ipAddress = "37.140.199.168";
        //MasterServer.port = 23466;
        bulletTransforms = new Transform[allTanksx2];
        bulletScripts = new BulletScript[allTanksx2];
        for (int i = 0; i < allTanksx2; i++)
        {
            bulletTransforms[i] = (Transform)Instantiate(bullet, Vector3.zero, Quaternion.identity);
            bulletScripts[i] = bulletTransforms[i].gameObject.GetComponent<BulletScript>();
            bulletTransforms[i].collider.enabled = false;
            bulletTransforms[i].renderer.enabled = false;
            bulletTransforms[i].parent = bullets;
        }
        //bulletsExplosionsTransforms = new Transform[allTanksx2];
        //bulletsDetonators = new Detonator[allTanksx2];
        //for (int index = 0; index < allTanksx2; ++index)
        //{
        //    bulletsExplosionsTransforms[index] = (Transform)Instantiate(explosion, Vector3.zero, Quaternion.Euler(270.0f, 90.0f, 0.0f));
        //    bulletsDetonators[index] = bulletsExplosionsTransforms[index].gameObject.GetComponent<Detonator>();
        //    bulletsExplosionsTransforms[index].parent = explosions;
        //}
        //tanksExplosionsTransforms = new Transform[allTanks];
        //tanksDetonators = new Detonator[allTanks];
        
        //for (int i = 0; i < allTanks; i++)
        //{
        //    tanksExplosionsTransforms[i] = (Transform)Instantiate(bigExplosion, Vector3.zero, Quaternion.Euler(270f, 90f, 0.0f));
        //    tanksDetonators[i] = tanksExplosionsTransforms[i].gameObject.GetComponent<Detonator>();
            
        //    tanksExplosionsTransforms[i].parent = explosions;
        //}

        nrbs = new NetworkRigidbody[allTanks];

        CreatePoolTanks();
        nv = this.GetComponent<NetworkView>();
        LoadOptions();
        fade.fadeState = FadeState.FromFade;
        currentSelectV = 0;
        gameScreen = GameScreen.Menu;
        cameraMode = 0;
        //labyrinth = new int[sizeLabX, sizeLabY];
        labTransforms = new Transform[sizeLabX, sizeLabY];
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        otherDelta = pause ? 0.0f : deltaTime;
        otherTime += otherDelta;

        if (currentSelectV != oldSelectV)
        {
            oldSelectV = currentSelectV;
            OnChangeVerticalSelect();
        }
        if (currentSelectH != oldSelectH)
        {
            oldSelectH = currentSelectH;
            OnChangeHorizontalSelect();
        }

        if (gameScreen == GameScreen.Network)
        {
            if (updateServersInfoTimer < Time.time)
            {
                updateServersInfoTimer = Time.time + updateServersInfoTimerRate;

                UpdateServers();
            }
        }

        if (playerInfoTimer < Time.time)
        {
            playerInfoTimer = Time.time + playerInfoTimerRate;

            if (Network.isServer)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (networkPlayers.Count > i)
                    {
                        playersIPs[i + 1] = networkPlayers[i].ipAddress;
                        playersPings[i + 1] = Network.GetLastPing(networkPlayers[i]);
                    }
                    else
                    {
                        playersIPs[i + 1] = "---.---.---.---";
                        playersPings[i + 1] = 0;
                    }
                }

                nv.RPC(
                    "UpdateNetPlayersInfo",
                    RPCMode.Others,
                    playersIPs[1],
                    playersIPs[2],
                    playersIPs[3],
                    playersPings[1],
                    playersPings[2],
                    playersPings[3]);
            }
        }
        
        if (showLabelPressStartButton || pause)
        {
            if (directionLabelPressStartButton)
            {
                alphaLabelPressStartButton += 5f * deltaTime;
            }
            else
            {
                alphaLabelPressStartButton -= 5f * deltaTime;
            }
            
            if (alphaLabelPressStartButton >= 1.0f)
            {
                directionLabelPressStartButton = false;
            }
            else if (alphaLabelPressStartButton <= 0.0f)
            {
                directionLabelPressStartButton = true;
            }
        }
        UpdateFlowers();

        //Обработка событий при смене текущего экрана.
        if (fade.fadeState == FadeState.Nothing && fading)
        {
            gameScreen = newGameScreen;
            fading = false;
            fade.fadeState = FadeState.FromFade;
            if (newGameScreen == GameScreen.Game)
            {
                sizeLabX = 26;
                sizeLabY = 26;

                stageClear = false;

                if (Network.isServer)
                {
                    levels = File.ReadAllLines(fileInfoMapPacksArray[currentMapPack].FullName);
                    //sizeLabX = int.Parse(levels[0]) * 2;
                    //sizeLabY = int.Parse(levels[1]) * 2;
                    if (levels == null)
                    {
                        //LoadDefaultLevel();
                        levels[2] = defaultLevel;
                    }
                }

                
                if (Network.isServer)
                {
                    nv.RPC("LoadLabyrinthFromString", RPCMode.All, levels[currentLevel + 2]);
                }


            }
            else if (newGameScreen == GameScreen.Tanks)
            {
                infoCamera.SetActive(true);
                infoTank.SetActive(true);
                currentSelectV = 0;
                currentSelectH = 0;
            }
            else if (newGameScreen == GameScreen.EnterIP)
            {
                currentSelectV = 0;
                currentSelectH = 0;
            }
            else if (newGameScreen == GameScreen.LocalGame)
            {
                fileInfoMapPacksArray = new DirectoryInfo("./MapPacks/").GetFiles();
                currentMapPackStr = fileInfoMapPacksArray[currentMapPack].Name;
                currentSelectV = 3;
                currentSelectH = 1;
            }
            else if (newGameScreen == GameScreen.Network)
            {
                UpdateServers();
                currentSelectV = 1;
                currentSelectH = 0;
            }
            else if (newGameScreen == GameScreen.NetworkLobby)
            {
                fileInfoMapPacksArray = new DirectoryInfo("./MapPacks/").GetFiles();
                currentMapPackStr = fileInfoMapPacksArray[currentMapPack].Name;

                if (Network.isClient)
                {
                    currentSelectV = 6;
                }
                else if (Network.isServer)
                {
                    currentSelectV = 3;
                }
                currentSelectH = 0;
            }
            else if (newGameScreen == GameScreen.Menu)
            {
                infoCamera.SetActive(false);
                infoTank.SetActive(false);
                ClearLabyrinth();
                showLabelPressStartButton = false;
                currentSelectV = 0;
                currentSelectH = 1;
            }
            else if (newGameScreen == GameScreen.Continue)
            {
                ClearLabyrinth();
                showLabelPressStartButton = false;
                currentSelectV = 0;
                currentSelectH = 1;
            }
            else if (newGameScreen == GameScreen.Scores)
            {
                currentScore = 0;
                ClearLabyrinth();
                showLabelPressStartButton = true;
            }
            else if (newGameScreen == GameScreen.Construction)
            {
                levels = File.ReadAllLines(fileToLoad);
                sizeLabX = int.Parse(levels[0]) * 2;
                sizeLabY = int.Parse(levels[1]) * 2;
                //labyrinth = new int[sizeLabX, sizeLabY];
                labTransforms = new Transform[sizeLabX / 2, sizeLabY / 2];
                currentSelectV = 0;
                currentSelectH = 1;
                LoadLabyrinthFromString(levels[currentLevel + 2]);
                if (sizeLabY == 26)
                    SpawnPlane(0);
                if (sizeLabY == 34)
                    SpawnPlane(1);
            }
            else if (newGameScreen == GameScreen.NewLevel)
            {
                if (!noSound) 
                {
                    music.Play(); 
                }
                
                showLabelPressStartButton = true;
                currentSelectV = 0;
                currentSelectH = 1;
            }
            else if (newGameScreen == GameScreen.ChooseMapPackConstruction)
            {
                fileInfoMapPacksArray = new DirectoryInfo("./MapPacks/").GetFiles();
                currentSelectV = 0;
                currentSelectH = 1;
            }
            else if (newGameScreen == GameScreen.End)
            {
                SpawnPlane(0);
                showLabelPressStartButton = true;
                tanksTransforms[0].position = new Vector3(7f, 0.0f, 8f);
                tanksTransforms[0].rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                Renderer renderer1 = tanksTransforms[0].renderer;
                renderer1.material.color = Color.yellow;
                renderer1.enabled = true;
                tanksTransforms[0].renderer.enabled = true;
                if (playersCount == 2.0)
                {
                    tanksTransforms[1].position = new Vector3(23f, 0.0f, 8f);
                    tanksTransforms[1].rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    Renderer renderer2 = tanksTransforms[1].renderer;
                    renderer2.material.color = Color.green;
                    renderer2.enabled = true;
                    tanksTransforms[1].renderer.enabled = true;
                }
                currentSelectV = 0;
                currentSelectH = 1;
            }
        }

        //Обработка кадров каждого из игровых экранов.
        switch (gameScreen)
        {
            case GameScreen.Tanks:
            {
                ProcessInput(0, 1);
                break;
            }
            case GameScreen.NetworkLobby:
            {
                //ProcessInput(7, 0);
                if (Network.isServer)
                {
                    ProcessInput(6, 0);
                }
                else
                {
                    ProcessInput(0, 0);
                    //if ()
                }
                break;
            }
            case GameScreen.LocalGame:
            {
                ProcessInput(newLocalGameStrings.Length - 1, 0);
                break;
            }
            case GameScreen.Menu:
            {
                UpdateAnimTank();
                ProcessInput(mainMenuStrings.Length - 1, 0);
                break;
            }
            case GameScreen.Game:
            {
                UpdateGameScreen();
                break;
            }
            case GameScreen.Scores:
            {
                UpdateScoresScreen();
                break;
            }
            case GameScreen.EnterIP:
            {
                
                if (currentSelectH >= 0 && currentSelectH <= 3)
                {
                    ipNumbers[currentSelectH] = 255 - currentSelectV;
                    connectToIP = ipNumbers[0].ToString("D3") + "." +
                        ipNumbers[1].ToString("D3") + "." +
                        ipNumbers[2].ToString("D3") + "." +
                        ipNumbers[3].ToString("D3");
                }
                ProcessInput(255, 5);
                break;
            }
            case GameScreen.Network:
            {
                ProcessInput(cursorPositionsNetwork.Length - 1, 0);
                break;
            }
            case GameScreen.NewLevel:
            {
                if (!Network.isServer && !Network.isClient)
                {
                    if (Input.GetButtonDown("P1Fire1") || Input.GetButtonDown("P2Fire1") || (Input.GetButtonDown("P3Fire1") || Input.GetButtonDown("P4Fire1")))
                    {
                        PressFireOnNewLevel();
                    }
                }
                else if ((Network.isServer || Network.isClient) && (Input.GetButtonDown("P1Fire1") || Input.GetButtonDown("P2Fire1") || (Input.GetButtonDown("P3Fire1") || Input.GetButtonDown("P4Fire1"))))
                {
                    nv.RPC("PressFireOnNewLevel", RPCMode.All);
                }
                if (!Network.isServer && !Network.isClient)
                {
                    if (!Input.GetButtonDown("P1Start") && !Input.GetButtonDown("P2Start") && (!Input.GetButtonDown("P3Start") && !Input.GetButtonDown("P4Start")))
                    {
                        break;
                    }
                    SetScreen(GameScreen.Game);
                    showLabelPressStartButton = false;
                    break;
                }
                else
                {
                    if (!Network.isServer && !Network.isClient || !Input.GetButtonDown("P1Start") && !Input.GetButtonDown("P2Start") && (!Input.GetButtonDown("P3Start") && !Input.GetButtonDown("P4Start")))
                    {
                        break;
                    }
                    nv.RPC("PressStart", RPCMode.All);
                    break;
                }
            }
            case GameScreen.ChooseMapPackConstruction:
            {
                if (showWindowEnterName && !showWindowChooseSize)
                {
                    ProcessInputEnterName();
                    break;
                }
                else if (!showWindowEnterName && showWindowChooseSize)
                {
                    ProcessInputChooseSize();
                    break;
                }
                else
                {
                    if (showWindowEnterName || showWindowChooseSize)
                        break;
                    ProcessInputChooseMapPackConstruction();
                    break;
                }
            }
            case GameScreen.Continue:
            {
                UpdateAnimTank();
                ProcessInputMenuContinue();
                break;
            }
        }
    }

    private void UpdateGameScreen()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    if (playersCameraViews[0].camera.enabled)
        //        playersCameraViews[0].camera.enabled = false;
        //    else
        //        playersCameraViews[0].camera.enabled = true;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    if (playersCameraViews[1].camera.enabled)
        //        playersCameraViews[1].camera.enabled = false;
        //    else
        //        playersCameraViews[1].camera.enabled = true;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    if (playersCameraViews[2].camera.enabled)
        //        playersCameraViews[2].camera.enabled = false;
        //    else
        //        playersCameraViews[2].camera.enabled = true;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    if (playersCameraViews[3].camera.enabled)
        //        playersCameraViews[3].camera.enabled = false;
        //    else
        //        playersCameraViews[3].camera.enabled = true;
        //}
        if (Input.GetButtonDown("P1Start") || Input.GetButtonDown("P2Start"))
        {
            pause = !pause;
        }
        if (freezeEnemies > 0 && otherTime > freezeEnemiesTimer)
        {
            freezeEnemiesTimer = otherTime + freezeRate;
            --freezeEnemies;
        }
        
        if (freezePlayers > 0.0f && otherTime > freezePlayersTimer)
        {
            freezePlayersTimer = otherTime + freezePlayersRate;
            freezePlayers--;
        }

        if (numberSpawnedEnemy < allEnemy &&
            enemysScreen < playersCount * 2.0f + 2.0f &&
            respawnTimer < otherTime)
        {
            RespawnEnemy();
        }
        if (numberEnemyTanks <= 0 && stageClear == false)
        {
            stageClear = true;
            Debug.Log(Time.time + " : Invoke(\"StageClearDelay\"), 5.0f);");
            Invoke("StageClearDelay", 5.0f);
            
            //nv.RPC("StageClear", RPCMode.All);
            
        }
        //if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("P1Camera") || Input.GetButtonDown("P2Camera"))
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeCameraMode();
        }

        //Здесь клавиши для дебага
        //if (!debug)
        //{
        //    return;
        //}
        if (Input.GetKeyDown(KeyCode.Home))
        {
            int pos = Random.Range(0, 4);
            int num1 = Random.Range(0, 8);
            if (Network.isServer)
            {
                nv.RPC("SpawnBonus", RPCMode.All, bonusPositions[pos], num1);
            }
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            if (Network.isServer)
            {
                nv.RPC("SpawnBonus", RPCMode.All, tanksScripts[0].transform.position, (int)Bonus.Bomb);
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (Network.isServer)
            {
                nv.RPC("SpawnBonus", RPCMode.All, tanksScripts[0].transform.position, (int)Bonus.Fort);
            }
        }
        if (Input.GetKeyDown(KeyCode.PageUp) && tanksScripts[0] != null)
        {
            //if (tanksScripts[0].invincible == 0)
            //{
            //    ShowFlower(tanksTransforms[0].position, "INVINCIBLE");
            //    tanksScripts[0].invincible = 999999;
            //}
            //else
            //{
            //    tanksScripts[0].invincible = 0;
            //}
        }
        if (Input.GetKeyDown(KeyCode.End))
        {
            playersList[0].kills[0] = 20;
            playersList[0].kills[1] = 20;
            playersList[0].kills[2] = 20;
            playersList[0].kills[3] = 20;
            playersList[0].kills[4] = 20;
            nv.RPC("NextLevel", RPCMode.All);
        }
    }

    private void StageClearDelay()
    {
        Debug.Log(Time.time + " : StageClear()");
        nv.RPC("StageClear", RPCMode.All);
    }

    private void UpdateScoresScreen()
    {
        
            
                
        if (currentScore < 5)
        {
            //if (Time.time > scoresTimer)
            //{
                //scoresTimer = Time.time + scoresRate;
                //buttonMoveSound.Play();

                        
                for (int i = 0; i < 4; i++)
                {
                    Debug.Log(Time.time + " : currentScore = " + currentScore.ToString());
                    while (playersList[i].scores1[currentScore] != playersList[i].kills[currentScore] * (currentScore + 1) * 100)
                    {


                        if (Time.time > scoresTimer)
                        {
                            scoresTimer = Time.time + scoresRate;
                            buttonMoveSound.Play();

                            playersList[i].scores1[currentScore] += (currentScore + 1) * 100;
                            playersList[i].scores += playersList[i].scores1[currentScore];
                            if (playersList[i].scores / (float)valueLife > scoresToLifes[i])
                            {
                                giveLifeSound.Play();
                                playersList[i].lifes++;
                                scoresToLifes[i]++;
                            }
                        }
                    }
                            
                }
                currentScore++;

            //}
                    
        }
                
            
            
        
        if (currentScore == 5)
        {
            scoresTimer = Mathf.Infinity;
            currentScore++;
            showLabelPressStartButton = true;
        }
        if (Input.GetButtonDown("P1Start") || Input.GetButtonDown("P2Start") || Input.GetButtonDown("P3Start") || Input.GetButtonDown("P4Start"))
        {
            nv.RPC("PressStart", RPCMode.All);
        }
    }

    private void RespawnEnemy()
    {
        

        respawnRate = ((190.0f - currentLevel * 4.0f - (playersCount - 1.0f) * 20.0f) / 60.0f);
        respawnTimer = otherTime + respawnRate;
        int r = Random.Range(0, 3);
        if (!noTanks && !fading)
        {
            if (Network.isServer)
            {
                if (difficulty == 0 || difficulty == 1)
                {
                    if (numberSpawnedEnemy < allTanks - 4)
                    {
                        nv.RPC("SpawnSpawnEnemy", RPCMode.All, r, numberSpawnedEnemy + 4, typesTanks[numberSpawnedEnemy + 4]);
                        Debug.Log(Time.time.ToString("F2") + " : RespawnEnemy();");
                    }
                }
                else
                {
                    nv.RPC("SpawnSpawnEnemy", RPCMode.All, 0, numberSpawnedEnemy + 4, typesTanks[numberSpawnedEnemy + 4]);
                    nv.RPC("SpawnSpawnEnemy", RPCMode.All, 1, numberSpawnedEnemy + 4, typesTanks[numberSpawnedEnemy + 4]);
                    nv.RPC("SpawnSpawnEnemy", RPCMode.All, 2, numberSpawnedEnemy + 4, typesTanks[numberSpawnedEnemy + 4]);
                    Debug.Log(Time.time.ToString("F2") + " : RespawnTripleEnemy();");
                }
            }
        }
    }

    void MyLabel(int x, int y, string text, TextAnchor textAnchor)
    {
        TextAnchor textAnchorBackup = GUI.skin.label.alignment;
        
        GUI.skin.label.alignment = textAnchor;
        
        switch (textAnchor)
        {
            case TextAnchor.UpperCenter:
            {
                GUI.Label(new Rect(x - (text.Length * fontSize) / 2, y + 2, text.Length * fontSize, fontSize), text);
                break;
            }
            case TextAnchor.MiddleCenter:
            {
                GUI.Label(new Rect(x - (text.Length * fontSize) / 2, y - fontSize / 2, text.Length * fontSize, fontSize), text);
                break;
            }
            case TextAnchor.LowerCenter:
            {
                GUI.Label(new Rect(x - (text.Length * fontSize) / 2, y - fontSize - 2, text.Length * fontSize, fontSize), text);
                break;
            }
            case TextAnchor.UpperLeft:
            {
                GUI.Label(new Rect(x, y, text.Length * fontSize, fontSize), text);
                break;
            }
            case TextAnchor.UpperRight:
            {
                GUI.Label(new Rect(x - text.Length * fontSize, y, text.Length * fontSize, fontSize), text);
                break;
            }
            case TextAnchor.LowerRight:
            {
                GUI.Label(new Rect(x - text.Length * fontSize, y - fontSize - 2, text.Length * fontSize, fontSize), text);
                break;
            }
        }
        GUI.skin.label.alignment = textAnchorBackup;
    }

    void MyLabel(int x, int y, string text, TextAnchor textAnchor, Color color)
    {
        Color colorBackup = GUI.color;

        GUI.color = color;

        MyLabel(x, y, text, textAnchor);

        GUI.color = colorBackup;
    }

    void MyLabel(int x, int y, string text, TextAnchor textAnchor, Color color, Color shadowColor)
    {
        Color colorBackup = GUI.color;

        GUI.color = shadowColor;
        MyLabel(x + 2, y - 2, text, textAnchor);

        GUI.color = color;
        MyLabel(x, y, text, textAnchor);

        GUI.color = colorBackup;
    }

    private void OnGUI()
    {
        sw2 = Screen.width / 2;
        sh2 = Screen.height / 2;
        sw = Screen.width;
        sh = Screen.height;

        GUI.skin = gameSkin;

        //float 16 = (Screen.height / 480.0f) * 21.0f;
        //GUI.skin.label.16 = Mathf.CeilToInt(16);

        //scale.x = (float)Screen.width / originalWidth; // calculate hor scale
        //scale.y = Screen.height / originalHeight; // calculate vert scale
        //scale.x = scale.y; // this will keep your ratio base on Vertical scale
        //scale.z = 1.0f;
        //float scaleX = scale.x; // store this for translate
        ////Matrix4x4 svMat = GUI.matrix; // save current matrix
        //// substitute matrix - only scale is altered from standard
        ////GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);

        //if (scale.x <= 0.0f)
        //{
        //    scale.x = 0.00001f;
        //}

        //if (scale.y <= 0.0f)
        //{
        //    scale.y = 0.00001f;
        //}

        //GUI.matrix = Matrix4x4.TRS(new Vector3((scaleX - scale.y) / 2.0f * originalWidth, 0.0f, 0.0f), Quaternion.identity, scale);

        

        //Отклонение графики, чтобы интерфейс всегда был в центре.
        //Всегда нужно вставлять вместо нуля в GUI.Label и подобные.

        //0 = Mathf.CeilToInt((Screen.width / 2.0f) / scale.y - originalWidth / 2.0f);
        //Debug.Log("(originalWidth / 2.0f) * scale.y = " + ((originalWidth / 2.0f) * scale.y).ToString());
        //Debug.Break();

        //Тест для выравнивания текста
        //GUI.DrawTexture(new Rect(0,0,16,16), whiteSquare);
        //GUI.Label(new Rect(0, 16, 64, 16), "0000");
        //for (int i = 0; i < 30; i++)
        //{
        //    GUI.Label(new Rect(0 - 64, i * 16, 640 + 128, 16), "----0123456789012345678901234567890123456789----");
        //    //GUI.Label(new Rect(0, i * 16, 640 + 128, 16), "----0123456789012345678901234567890123456789----");
        //}

        //Имеем текстовое поле размером 40х30.



        if (showWindowChooseSize)
        {
            rectWindowChooseSize = GUI.Window(0, rectWindowChooseSize, new GUI.WindowFunction(DoWindowChooseSize), string.Empty);
        }
        if (showWindowEnterName)
        {
            rectWindowEnterName = GUI.Window(0, rectWindowEnterName, new GUI.WindowFunction(DoWindowEnterName), string.Empty);
        }
        if (showWindowConnect)
        {
            rectWindowConnect = GUI.Window(0, rectWindowConnect, new GUI.WindowFunction(DoWindowConnect), string.Empty);
        }
        if (showWindowError)
        {
            rectWindowConnect = GUI.Window(0, rectWindowConnect, new GUI.WindowFunction(DoWindowError), string.Empty);
        }
        if (showWindowErrorConnect)
        {
            rectWindowConnect = GUI.Window(0, rectWindowConnect, new GUI.WindowFunction(DoWindowErrorConnect), string.Empty);
        }
        if (showLabelPressStartButton)
        {
            //GUI.color = new Color(1.0f, 1.0f, 1.0f, alphaLabelPressStartButton);
            //GUI.Label(new Rect(sw - 160, sh - 432, 288, 16), "PRESS START BUTTON");
            //GUI.color = Color.white;

            MyLabel(sw2, sh2 + fontSize * 6, "PRESS START BUTTON", TextAnchor.MiddleCenter, new Color(1.0f, 1.0f, 1.0f, alphaLabelPressStartButton));
        }
        
        //Цветы - всплывающие сообщения.
        for (int i = 0; i < flowerList.Count; ++i)
        {
            //Тень
            //GUI.color = new Color(0.5f, 0.5f, 0.5f, flowerList[i].a);
            //GUI.Label(new Rect(flowerList[i].X + 2, (flowerList[i].curY + flowerList[i].Y + 2), flowerList[i].txt.Length * fontSize, fontSize), flowerList[i].txt);
            
            //Текст
            //GUI.color = new Color(1.0f, 1.0f, 1.0f, flowerList[i].a);
            //GUI.Label(new Rect(flowerList[i].X, flowerList[i].curY + flowerList[i].Y, flowerList[i].txt.Length * fontSize, fontSize), flowerList[i].txt);

            MyLabel((int)flowerList[i].X, (int)(flowerList[i].curY + flowerList[i].Y), flowerList[i].txt, TextAnchor.MiddleCenter);

            //GUI.color = Color.white;
        }
        switch (gameScreen)
        {
            //               1         2         3
            //     0123456789012345678901234567890123456789
            //    ##########################################
            // 01 #       >NUMBER OF PLAYERS: 1            #
            // 03 #                                        #
            // 05 #        MAP PACK: BATTLE CITY           #
            // 07 #                                        #
            // 09 #        DIFFICULTY: EASY                #
            // 11 #                                        #
            // 13 #        INFO:                           #
            // 15 #            INFINITE LIVES              #
            // 17 #            INFINITE CONTINUES          #
            // 19 #            NO BONUSES FOR CPU          #
            // 21 #                                        #
            // 23 #       START                            #
            // 25 #                                        #
            // 27 #       BACK                             #
            // 29 #                                        #
            //    ##########################################
            case GameScreen.LocalGame:
            {
                string[] menuStrings = { 
                    "NUMBER OF PLAYERS: " + playersCount,
                    "MAP PACK: " + currentMapPackStr,
                    "DIFFICULTY: " + difficultyMenuStrings[difficulty],
                    "START",
                    "BACK"
                };

                string cursorString = ">";

                for (int i = 0; i < menuStrings.Length; i++)
                {
                    if (i == currentSelectV) cursorString = ">";
                    else cursorString = "";

                    if (currentSelectV == i) MyLabel(sw2, sh2 - fontSize + i * fontSize, cursorString + menuStrings[i], TextAnchor.MiddleCenter, Color.yellow);
                    else MyLabel(sw2, sh2 - fontSize + i * fontSize, cursorString + menuStrings[i], TextAnchor.MiddleCenter);
                }

                MyLabel(sw2, sh2 + 5 * fontSize, difficultyInfo1[difficulty], TextAnchor.MiddleCenter);
                MyLabel(sw2, sh2 + 6 * fontSize, difficultyInfo2[difficulty], TextAnchor.MiddleCenter);
                MyLabel(sw2, sh2 + 7 * fontSize, difficultyInfo3[difficulty], TextAnchor.MiddleCenter);

                break;
            }
            case GameScreen.Menu:
            {
                //Максимально набранное количество очков
                MyLabel(sw2, 0, "HIGH SCORE: " + hiScore.ToString("D6"), TextAnchor.UpperCenter, Color.white);
                
                //Размер картинки 1024х128
                GUI.DrawTexture(new Rect(sw2 - 512, 128, 1024, 128), gameNameTex);
                
                string cursorString = ">";

                for (int i = 0; i < mainMenuStrings.Length; i++)
                {
                    if (i == currentSelectV) cursorString = ">";
                    else cursorString = "";

                    if (currentSelectV == i) MyLabel(sw2, sh2 - fontSize + i * fontSize, cursorString + mainMenuStrings[i], TextAnchor.MiddleCenter, Color.yellow);
                    else if (i == 1 || i == 2) MyLabel(sw2, sh2 - fontSize + i * fontSize, cursorString + mainMenuStrings[i], TextAnchor.MiddleCenter, Color.gray);
                    else MyLabel(sw2, sh2 - fontSize + i * fontSize, cursorString + mainMenuStrings[i], TextAnchor.MiddleCenter, Color.white);
                }

                //Курсор в виде танчика
                //GUI.DrawTexture(new Rect(sw2 - 128, sh2 - 32 + currentSelectV * 32, 32, 32), animTankTextures[indexAnimTank], ScaleMode.StretchToFill);

                MyLabel(sw2, sh - fontSize, about1, TextAnchor.LowerCenter, Color.white);
                MyLabel(sw2, sh, about2, TextAnchor.LowerCenter, Color.white);
                MyLabel(sw, sh, version, TextAnchor.LowerRight, Color.white);
                break;
            }
            //               1         2         3
            //     0123456789012345678901234567890123456789
            //    ##########################################
            // 01 #  DIMENSIONS:                           #
            // 03 #  WIDTH: 1.00 M                         #
            // 05 #  HEIGHT: 1.00 M                        #
            // 07 #  LENGTH: 1.00 M                        #
            // 09 #                                        #
            // 11 #                                        #
            // 13 #                                        #
            // 15 #                                        #
            // 17 #                                        #
            // 19 #                                        #
            // 21 #                                        #
            // 23 #                                        #
            // 25 #                                        #
            // 27 #        >BACK             NEXT          #
            // 29 #                                        #
            //    ##########################################
            case GameScreen.Tanks:
            {
                string txtInfo1 = "DIMENSIONS:\n\nWIDTH: " + tankInfo[currentInfoTank].width.ToString("F3") + " M"
                    + "\n\nHEIGHT: " + tankInfo[currentInfoTank].height.ToString("F3") + " M"
                    + "\n\nLENGTH: " + tankInfo[currentInfoTank].length.ToString("F3") + " M";
                string txtInfo2 = "SPEED: " + (tankInfo[currentInfoTank].speed * 60 * 60 / 1000).ToString("F1") + " KM/H"
                    + "\n\nBULLET SPEED: " + (tankInfo[currentInfoTank].bulletSpeed).ToString("F1") + " M/S"
                    + "\n\nARMOUR: " + tankInfo[currentInfoTank].armour.ToString()
                    + "\n\nAMMO: " + tankInfo[currentInfoTank].ammo.ToString();
                GUI.Label(new Rect(16, 16, 16 * 32, 16 * 7), txtInfo1);
                GUI.Label(new Rect(sw2, 16, 16 * 32, 16 * 7), txtInfo2);
                GUI.Label(new Rect(sw2 - 16 * 3, sh - 16 * 4, 16 * 32, 16), tankInfo[currentInfoTank].name);
                GUI.Label(new Rect(sw2 - 16 * 6, sh - 16 * 2, 16 * 4, 16), "BACK");
                GUI.Label(new Rect(sw2 + 16 * 6, sh - 16 * 2, 16 * 4, 16), "NEXT");

                if (currentSelectH == 0)
                {
                    GUI.Label(new Rect(sw2 - 16 * 7, sh - 16 * 2, 16, 16), ">");
                }
                else {
                    GUI.Label(new Rect(sw2 + 16 * 5, sh - 16 * 2, 16 * 4, 16), ">");
                }

                break;
            }
            case GameScreen.Game:
            {
                //GUI_HiScore();
                //GUI_Stars();
                //GUI_Scores();
                //GUI_GreyBack();
                //GUI_EnemyTanks();
                //GUI.color = Color.grey;
                //GUI.DrawTexture(new Rect(0 + 576, 16, 64, 192), whiteSquare, ScaleMode.StretchToFill);

                int j = 0;
                for (int i = 0; i < numberEnemyTanks; i++)
                {
                    GUI.color = Color.white;

                    if (i % 2 == 0)
                    {
                        j++;
                        //GUI.Box(new Rect(sw - fontSize * 1.5f, 16 + j * 16, 16, 16), enemyTank);
                        GUI.DrawTexture(new Rect(sw - fontSize * 2, fontSize + j * fontSize, fontSize, fontSize), enemyTank, ScaleMode.StretchToFill);
                    }
                    else
                    {
                        //GUI.Box(new Rect(sw - fontSize, 16 + j * 16, 16, 16), enemyTank);
                        GUI.DrawTexture(new Rect(sw - fontSize, fontSize + j * fontSize, fontSize, fontSize), enemyTank, ScaleMode.StretchToFill);
                    }
                }

                GUI.color = Color.white;
                //GUI.color = Color.grey;
                //GUI.DrawTexture(new Rect(0 + 576, 208, 80, 208), whiteSquare, ScaleMode.StretchToFill);
                for (int i = 0; i < 4; i++)
                {
                    //GUI.color = Color.white;
                    //GUI.Box(new Rect(sw - fontSize * 1.5f, 240 + i * 48, 16, 16), playerTank);
                    GUI.DrawTexture(new Rect(sw - fontSize * 2, fontSize * 15 + i * fontSize * 3, fontSize, fontSize), playerTank, ScaleMode.StretchToFill);
                    
                    //GUI.color = Color.black;
                    //GUI.Label(new Rect(sw - fontSize, fontSize * 15 + i * fontSize * 2, fontSize * 2, fontSize), playersList[i].lifes.ToString());
                    if (playersList[i] != null)
                    {
                        if (playersList[i].lifes != null)
                        {
                            MyLabel(sw - fontSize, fontSize * 15 + i * fontSize * 3, playersList[i].lifes.ToString(), TextAnchor.UpperLeft, Color.black);
                        }
                    }
                    //GUI.Label(new Rect(0 + 608, 240 + i * 48, 32, 16), "99");
                    //GUI.Label(new Rect(sw - fontSize * 2, fontSize * 15 + i * fontSize * 2 - 16, fontSize * 2, fontSize), (i + 1).ToString("D1") + "P");
                    MyLabel(sw - fontSize * 2, fontSize * 15 + i * fontSize * 3 - fontSize, (i + 1).ToString("D1") + "P", TextAnchor.UpperLeft, Color.black);
                }

                GUI.color = Color.white;

                if (pause)
                {
                    //GUI.color = new Color(1f, 1f, 1f, alphaLabelPressStartButton);
                    //GUI.Label(new Rect(0 + 280f, 240f, 256f, 16f), "PAUSE");
                    //GUI.color = Color.white;
                    MyLabel(sw2,sh2,"PAUSE",TextAnchor.MiddleCenter);
                }
                //GUI.color = Color.grey;
                //GUI.Label(new Rect(0 + 580f, 14f, 32f, 16f), numberEnemyTanks.ToString("D2"));
                //GUI.Box(new Rect(0 + 614f, 14f, 16f, 16f), tankTexture);
                //GUI.color = Color.white;
                //GUI.Label(new Rect(0 + 578f, 16f, 32f, 16f), numberEnemyTanks.ToString("D2"));
                //GUI.Box(new Rect(0 + 612f, 16f, 16f, 16f), tankTexture);
                break;
            }
            case GameScreen.End:
            {
                GUI.Label(new Rect(0 + 256f, 240f, 64f, 16f), "CONGRATULATIONS!");
                GUI.Label(new Rect(0 + 300f, 224f, 64f, 16f), "YOU WIN!");
                break;
            }
            case GameScreen.Scores:
            {
                //GUI_HiScore();
                //GUI_Scores();
                int num = fontSize * 7;
                for (int i = 0; i < 4; i++)
                {
                    GUI.color = colors[i];
                    GUI.Label(new Rect(sw2 - fontSize * 14 + num * i, sh2 - fontSize * 4, 128, 16), "P" + (i + 1).ToString());
                    //GUI.Label(new Rect(sw2 - num * i, sh2 - fontSize * 3, 128, 16), playersList[i].viewKills[0].ToString());
                    //GUI.Label(new Rect(sw2 - num * i, sh2 - fontSize * 2, 128, 16), playersList[i].viewKills[1].ToString());
                    //GUI.Label(new Rect(sw2 - num * i, sh2 - fontSize * 1, 128, 16), playersList[i].viewKills[2].ToString());
                    //GUI.Label(new Rect(sw2 - num * i, sh2 - fontSize * 0, 128, 16), playersList[i].viewKills[3].ToString());
                    GUI.Label(new Rect(sw2 - fontSize * 14 + num * i, sh2 - fontSize * 3, 128, 16), playersList[i].scores1[0].ToString());
                    GUI.Label(new Rect(sw2 - fontSize * 14 + num * i, sh2 - fontSize * 2, 128, 16), playersList[i].scores1[1].ToString());
                    GUI.Label(new Rect(sw2 - fontSize * 14 + num * i, sh2 - fontSize * 1, 128, 16), playersList[i].scores1[2].ToString());
                    GUI.Label(new Rect(sw2 - fontSize * 14 + num * i, sh2 - fontSize * 0, 128, 16), playersList[i].scores1[3].ToString());
                    GUI.Label(new Rect(sw2 - fontSize * 14 + num * i, sh2 + fontSize * 1, 128, 16), playersList[i].scores1[4].ToString());
                    GUI.Label(new Rect(sw2 - fontSize * 14 + num * i, sh2 + fontSize * 2, 128, 16), "----------------------------");
                    GUI.Label(new Rect(sw2 - fontSize * 14 + num * i, sh2 + fontSize * 3, 128, 16), playersList[i].scores.ToString());
                }
                break;
            }
            //  0123456789012345678901234567890123456789
            // ##########################################
            //0#           >servers: 1-10               #
            //1#                                        #
            //2#            255.255.255.255             #
            //3#                                        #
            //4#            255.255.255.255             #
            //5#                                        #
            //6#           >255.255.255.255             #
            //7#                                        #
            //8#            255.255.255.255             #
            //9#                                        #
            //0#            255.255.255.255             #
            //1#                                        #
            //2#            255.255.255.255             #
            //3#                                        #
            //4#            255.255.255.255             #
            //5#                                        #
            //6#            255.255.255.255             #
            //7#                                        #
            //8#            255.255.255.255             #
            //9#                                        #
            //0#            255.255.255.255             #
            //1#                                        #
            //2#          >back           host          #
            //3#                                        #
            //4#             direct connect             #
            //5#                                        #
            //6#    open port 25001 for host server     #
            //7#                                        #
            //8#                                        #
            //9#                                        #
            // ##########################################
            //Экран сетевой игры.
            case GameScreen.Network:
            {
                GUI.Label(new Rect(0 + 400, 16, 240, 16), 
                   // "SERVERS: " + (hostDataList * 10).ToString("D2") + "-" + (((hostDataList + 1) * 10) - 1).ToString("D2"));
                    "SERVERS: " + hostData.Length.ToString());
                GUI.Label(new Rect(0 + 224, 16, 240, 16), "PAGE " + (hostDataList + 1).ToString());
                //if (hostData.Length != 0)
                //{
                    for (int i = hostDataList * 10; i < (hostDataList + 1) * 10; i++)
                    {
                        if (i <= hostData.Length - 1)
                        {
                            GUI.Label(new Rect(0 + 224, (48 + 32 * i) - hostDataList * 320, 240, 16), hostData[i].gameName);
                        }
                        else
                        {
                            GUI.Label(new Rect(0 + 224, (48 + 32 * i) - hostDataList * 320, 240, 16), "--------------");
                        }
                    }
                //}
                GUI.Label(new Rect(0 + 160, 384, 64, 16), "BACK");
                //GUI.Label(new Rect(0 + 288, 384, 64, 16), "JOIN");
                GUI.Label(new Rect(0 + 416, 384, 64, 16), "HOST");
                GUI.Label(new Rect(0 + 240, 416, 208, 16), "CONNECT TO IP");
                GUI.Label(new Rect(0 + 64, 448, 496, 16), "OPEN PORT 25001 FOR HOST SERVER");
                
                //Вертикальный курсор
                GUI.Label(new Rect(
                    0 + cursorPositionsNetwork[currentSelectV].x, 
                    cursorPositionsNetwork[currentSelectV].y, 16, 16), ">");
                
                //Горизонтальный курсор
                //if (currentSelectH != 3)
                //{
                //    GUI.Label(new Rect(0 + (128 * currentSelectH + 144), 384, 16, 16), ">");
                //}
                //else
                //{
                //    GUI.Label(new Rect(0 + 224, 416, 16, 16), ">");
                //}
                break;
            }
            //  0123456789012345678901234567890123456789
            // ##########################################
            //0#                                        #
            //1# NAME\IP          PING     COMMAND      #
            //2#                                        #
            //3# HOST             000 MS                #
            //4#                                        #
            //5# 255.255.255.255  050 MS  >KICK         #
            //6#                                        #
            //7# ---.---.---.--- --- MS    KICK         #
            //8#                                        #
            //9# ---.---.---.--- --- MS    KICK         #
            //0#                                        #
            //1#                                        #
            //2#                                        #
            //3#       >MAP PACK: BATTLE CITY           #
            //4#                                        #
            //5#        DIFFICULTY: HARD                #
            //6#                                        #
            //7#        START                           #
            //8#                                        #
            //9#        BACK                            #
            //0#                                        #
            //1#                                        #
            //2#                                        #
            //3#                                        #
            //4#                                        #
            //5#                                        #
            //6#                                        #
            //7#                                        #
            //8#                                        #
            //9#                                        #
            // ##########################################
            //Экран сетевого лобби.
            case GameScreen.NetworkLobby:
            {
                GUI.Label(new Rect(0 + 16, 16, 112, 16),"NAME\\IP");
                
                GUI.Label(new Rect(0 + 304, 16, 64, 16),"PING");
                
                GUI.Label(new Rect(0 + 448, 16, 112, 16), "COMMAND");
                GUI.Label(new Rect(0 + 208, 208, 160 + currentMapPackStr.Length * 16, 16), "MAP PACK: " + currentMapPackStr);
                GUI.Label(new Rect(0 + 208, 240, 176, 16), "DIFFICULTY: ");
                GUI.Label(new Rect(0 + 208, 272, 80, 16), "START");
                GUI.Label(new Rect(0 + 208, 304, 64, 16), "BACK");
                GUI.Label(new Rect(0 + 448, 80, 64, 16), "KICK");
                GUI.Label(new Rect(0 + 448, 112, 64, 16), "KICK");
                GUI.Label(new Rect(0 + 448, 144, 64, 16), "KICK");

                GUI.Label(new Rect(0 + 16, 48, 64, 16), "HOST");
                GUI.Label(new Rect(0 + 16, 80, 240, 16), playersIPs[1]);
                GUI.Label(new Rect(0 + 16, 112, 240, 16), playersIPs[2]);
                GUI.Label(new Rect(0 + 16, 144, 240, 16), playersIPs[3]);

                GUI.Label(new Rect(0 + 304, 48, 96, 16), "000 MS");
                GUI.Label(new Rect(0 + 304, 80, 96, 16), playersPings[1].ToString("D3") + " MS");
                GUI.Label(new Rect(0 + 304, 112, 96, 16), playersPings[2].ToString("D3") + " MS");
                GUI.Label(new Rect(0 + 304, 144, 96, 16), playersPings[3].ToString("D3") + " MS");

                if (currentSelectV < 3)
                {
                    GUI.Label(new Rect(0 + 432, 80 + currentSelectV * 32, 16, 16), ">");
                }
                else
                {
                    GUI.Label(new Rect(0 + 192, 112 + currentSelectV * 32, 16, 16), ">");
                }

                break;
            }
            //Экран номера уровня.
            case GameScreen.NewLevel:
            {
                //GUI.color = Color.grey;
                //GUI.Label(new Rect(0 + 256 + 2, 224 + 2, 128, 16), "LEVEL " + (currentLevel + 1).ToString());
                //GUI.color = Color.white;
                //GUI.Label(new Rect(0 + 256, 224, 128, 16), "LEVEL " + (currentLevel + 1).ToString());
                MyLabel(sw2, sh2, "LEVEL " + (currentLevel + 1).ToString(), TextAnchor.MiddleCenter, Color.white, Color.gray);
                break;
            }
            case GameScreen.ChooseMapPackConstruction:
            {
                string name2 = fileInfoMapPacksArray[currentMapPack].Name;
                GUI.Label(new Rect((float)(originalWidth / 2.0 - 144.0), (float)(originalHeight / 2.0 - 128.0), 288f, 320f), "SELECT MAP PACK:");
                GUI.Label(new Rect(originalWidth / 2f - (float)(name2.Length * 16 / 2), (float)(originalHeight / 2.0 - 64.0), (float)(name2.Length * 16), 320f), fileInfoMapPacksArray[currentMapPack].Name);
                GUI.Label(new Rect((float)(originalWidth / 2.0 - 200.0), originalHeight / 2f, 400f, 320f), "PRESS UP OR DOWN BUTTON");
                GUI.Label(new Rect(160f, 384f, 64f, 16f), "NEW");
                GUI.Label(new Rect(288f, 384f, 64f, 16f), "EDIT");
                GUI.Label(new Rect(416f, 384f, 64f, 16f), "DELETE");
                if (showWindowEnterName)
                    break;
                GUI.Label(new Rect((float)(128 * currentSelectH + 144), 384f, 16f, 16f), ">");
                break;
            }
            case GameScreen.Continue:
            {
                float width4 = 400f;
                GUI.BeginGroup(new Rect((float)(originalWidth / 2.0 - 64.0), (float)(originalHeight / 2.0 - 64.0), width4, 320f));
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUI.color = Color.white;
                for (int index1 = 0; index1 < continueMenuStrings.Length; ++index1)
                {
                    string text = continueMenuStrings[index1];
                    GUILayoutOption[] guiLayoutOptionArray = new GUILayoutOption[2];
                    int index2 = 0;
                    GUILayoutOption guiLayoutOption1 = GUILayout.Width(width4);
                    guiLayoutOptionArray[index2] = guiLayoutOption1;
                    int index3 = 1;
                    GUILayoutOption guiLayoutOption2 = GUILayout.Height(32f);
                    guiLayoutOptionArray[index3] = guiLayoutOption2;
                    GUILayout.Label(text, guiLayoutOptionArray);
                }
                GUILayout.EndVertical();
                GUI.EndGroup();
                GUI.Box(new Rect(208f, (float)(32.0 * currentSelectV + originalHeight / 2.0 - 8.0 - 64.0), 32f, 32f), animTankTextures[indexAnimTank]);
                break;
            }
            //               1         2         3
            //     0123456789012345678901234567890123456789
            //    ##########################################
            // 01 #                                        #
            // 03 #                                        #
            // 05 #                                        #
            // 07 #                                        #
            // 09 #            ˄                           #
            // 11 #           255.255.255.255              #
            // 13 #            ˅                           #
            // 15 #                                        #
            // 17 #          BACK        >JOIN             #
            // 19 #                                        #
            // 21 #                                        #
            // 23 #                                        #
            // 25 #                                        #
            // 27 #                                        #
            // 29 #                                        #
            //    ##########################################
            case GameScreen.EnterIP:
            {
                GUI.color = Color.white;

                //Надписи
                GUI.Label(
                    new Rect(0 + 10 * 16, 11 * 16, (12 + 3) * 16, 16),
                    connectToIP);
                GUI.Label(
                    new Rect(0 + 10 * 16, 17 * 16, 4 * 16, 1 * 16),
                    "BACK");
                GUI.Label(
                    new Rect(0 + 23 * 16, 17 * 16, 4 * 16, 1 * 16),
                    "JOIN");

                //Курсоры
                if (currentSelectH >= 0 && currentSelectH <= 3)
                {
                    GUI.Label(
                        new Rect(0 + (11 + currentSelectH * 4) * 16, 10 * 16, 1 * 16, 1 * 16),
                        "˄");
                    GUI.Label(
                        new Rect(0 + (11 + currentSelectH * 4) * 16, 12 * 16, 1 * 16, 1 * 16),
                        "˅");
                }
                else if (currentSelectH == 4)
                {
                    GUI.Label(
                        new Rect(0 + 9 * 16, 17 * 16, 1 * 16, 1 * 16),
                        ">");    
                }
                else if (currentSelectH == 5)
                {
                    GUI.Label(
                        new Rect(0 + 22 * 16, 17 * 16, 1 * 16, 1 * 16),
                        ">");
                }

                break;
            }
            case GameScreen.Construction:
            {
                GUI.Label(new Rect(0, 0, 320, 32), "CONSTRUCTION");
                break;
            }
        }
    }

    private void GUI_HiScore()
    {
        //GUI.color = Color.grey;
        //GUI.Label(new Rect(0 + 546, -2, 96, 16), hiScore.ToString("D6"));
        GUI.color = Color.white;
        //GUI.Label(new Rect(0 + 544, 0, 96, 16), hiScore.ToString("D6"));
        GUI.Label(new Rect(Screen.width - 96, 0, 96, 16), hiScore.ToString("D6"));
    }
    private void GUI_Stars()
    {
        //int num = 128;
        //for (int index = 0; index < playersCount; ++index)
        //{
        //    if (tanksScripts[index].rank > 0)
        //    {
        //        GUI.Box(new Rect(0 + (48 + num * index), 16f, 16f, 16f), star);
        //    }
        //    if (tanksScripts[index].rank > 1)
        //    {
        //        GUI.Box(new Rect(0 + (64 + num * index), 16f, 16f, 16f), star);
        //    }
        //    if (tanksScripts[index].rank > 2)
        //    {
        //        GUI.Box(new Rect(0 + (80 + num * index), 16f, 16f, 16f), star);
        //    }
        //}
    }
    private void GUI_Scores()
    {
        int num = 128;
        for (int i = 0; i < 4; ++i)
        {
            if (difficulty == 1 || difficulty == 2)
            {
                GUI.color = Color.grey;
                GUI.Box(new Rect(0 + (34 + num * i), 14, 16, 16), tankTexture);
                GUI.color = colors[i];
                GUI.Box(new Rect(0 + (32 + num * i), 16, 16, 16), tankTexture);
                GUI.color = Color.grey;
                GUI.Label(new Rect(0 + (2 + num * i), 14, 32, 16), playersList[i].lifes.ToString("D2"));
                GUI.color = colors[i];
                GUI.Label(new Rect(0 + (0 + num * i), 16, 32, 16), playersList[i].lifes.ToString("D2"));
            }
            GUI.color = Color.grey;
            GUI.Label(new Rect(0 + (2 + num * i), -2, 96, 16), playersList[i].scores.ToString("D6"));
            GUI.color = colors[i];
            GUI.Label(new Rect(0 + (0 + num * i), 0, 96, 16), playersList[i].scores.ToString("D6"));
        }
    }
    private void GUI_PlayerLifes()
    {

    }
    private void GUI_EnemyTanks()
    {

    }
    private void GUI_GreyBack()
    {
        GUI.color = Color.grey;
        GUI.DrawTexture(new Rect(0 + 576, 0, 16 * 5, 480), whiteSquare, ScaleMode.StretchToFill);
        GUI.color = Color.white;
    }

    private void OnApplicationQuit()
    {
        SaveOptions();
        Network.Disconnect();
        //MasterServer.UnregisterHost();
    }

    private void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.RegistrationSucceeded)
        {
            Debug.Log(Time.time.ToString() + " : Server registered");
        }
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            hostData = MasterServer.PollHostList();
            Debug.Log(Time.time.ToString() + " : Host list received. Length is: " + hostData.Length);
            for (int i = 0; i < hostData.Length; i++)
            {
                for (int j = 0; j < hostData[i].ip.Length; j++)
                {
                    Debug.Log(Time.time.ToString() + " : hostData[" + i.ToString() + "]=" + hostData[i].ip[j].ToString());
                }
            }
        }
    }

    private void UpdateServers()
    {
        //MasterServer.RequestHostList(codeNet);
    }

    private void UpdateAnimTank()
    {
        if (Time.time <= timerAnimTank)
            return;
        timerAnimTank = Time.time + timerRateAnimTank;
        ++indexAnimTank;
        if (indexAnimTank <= animTankTextures.Length - 1)
            return;
        indexAnimTank = 0;
    }

    [RPC]
    public void UpdateNetPlayersInfo(string ip1, string ip2, string ip3, int ping1, int ping2, int ping3)
    {
        playersIPs[1] = ip1;
        playersIPs[2] = ip2;
        playersIPs[3] = ip3;

        playersPings[1] = ping1;
        playersPings[2] = ping2;
        playersPings[3] = ping3;
    }

    [RPC]
    void PressFireOnNewLevel()
    {
        ++currentLevel;
        if (currentLevel <= levels.Length - 3)
            return;
        currentLevel = 0;
    }
    [RPC]
    private void PressStart()
    {
        if (audio.isPlaying)
        {
            audio.Stop();
        }
        if (gameScreen == GameScreen.Scores)
        {
            SetScreen(GameScreen.NewLevel);
        }
        else if (gameScreen == GameScreen.NewLevel)
        {
            SetScreen(GameScreen.Game);
        }
        showLabelPressStartButton = false;
    }

    private void OnConnectedToServer()
    {
        //tanksTransforms[0].gameObject.GetComponent<PlayerInput>().remote = true;
        //for (int index = 2; index < 22; index++)
        //{
        //    tanksTransforms[index].gameObject.GetComponent<AIInput>().remote = true;
        //}
        //showWindowConnect = false;
        //SetScreen(GameScreen.NewLevel);
        //Debug.Log((object)"This CLIENT has connected to a server");
    }
    private void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        SetScreen(GameScreen.Menu);

        //if (gameOver)
        //{
        //    return;
        //}
        //tanksTransforms[0].gameObject.GetComponent<PlayerInput>().remote = false;
        //for (int i = 2; i < 22; i++)
        //{
        //    //tanksTransforms[i].gameObject.GetComponent<AIInput>().remote = true;
        //}
        //labelError = "Server shut down";
        //showWindowError = true;
        //StartBackToMenu();
        //Debug.Log("This CLIENT has disconnected from a server OR this SERVER was just shut down");
    }
    private void OnFailedToConnect(NetworkConnectionError error)
    {
        showWindowErrorConnect = true;
        Debug.Log((object)("Could not connect to server: " + (object)error));
    }
    private void OnPlayerConnected(NetworkPlayer player)
    {


        //MasterServer.UnregisterHost();
        //tanksTransforms[1].gameObject.GetComponent<PlayerInput>().remote = true;
        //showWindowConnect = false;
        //SetScreen(GameScreen.NewLevel);
        //object[] objArray = new object[4];
        //int index1 = 0;
        //string str1 = "Player connected from: ";
        //objArray[index1] = (object)str1;
        //int index2 = 1;
        //string ipAddress = player.ipAddress;
        //objArray[index2] = (object)ipAddress;
        //int index3 = 2;
        //string str2 = ":";
        //objArray[index3] = (object)str2;
        //int index4 = 3;
        //// ISSUE: variable of a boxed type
        //int local = player.port;
        //objArray[index4] = (object)local;
        //Debug.Log((object)string.Concat(objArray));

        //SetScreen(GameScreen.);
        networkPlayers.Add(player);

        //nv.RPC("SpawnPlayer", RPCMode.All, networkPlayers.Count + 1);
    }
    private void OnServerInitialized()
    {
        Debug.Log((object)"Server initialized and ready");
    }
    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        //if (gameOver)
        //    return;
        //tanksTransforms[1].gameObject.GetComponent<PlayerInput>().remote = false;
        //StartBackToMenu();
        //object[] objArray = new object[4];
        //int index1 = 0;
        //string str1 = "Player disconnected from: ";
        //objArray[index1] = (object)str1;
        //int index2 = 1;
        //string ipAddress = player.ipAddress;
        //objArray[index2] = (object)ipAddress;
        //int index3 = 2;
        //string str2 = ":";
        //objArray[index3] = (object)str2;
        //int index4 = 3;
        //// ISSUE: variable of a boxed type
        //int local = player.port;
        //objArray[index4] = (object)local;
        //Debug.Log((object)string.Concat(objArray));
        //labelError = "Player disconnected";
        //showWindowError = true;

        networkPlayers.Remove(player);
    }

    private void DoWindowChooseSize(int windowID)
    {
        GUI.Label(new Rect(32f, 32f, 192f, 16f), chooseSizeStrings[currentSelectV]);
        GUI.Label(new Rect(96f, 80f, 192f, 16f), "PRESS UP OR DOWN BUTTON");
    }
    private void DoWindowEnterName(int windowID)
    {
        GUI.Label(new Rect(32f, 32f, 192f, 16f), newMapPackName);
        GUI.Label(new Rect((float)(32 + currentSelectH * 17), 48f, 16f, 16f), "^");
        GUI.Label(new Rect(96f, 80f, 192f, 16f), "ENTER NAME");
    }
    private void DoWindowConnect(int windowID)
    {
        GUI.Label(new Rect(20f, 30f, 320f, 16f), "Waiting other player...");
        if (!GUI.Button(new Rect(100f, 60f, 160f, 40f), "Cancel"))
            return;
        Network.Disconnect();
        showWindowConnect = false;
    }
    private void DoWindowError(int windowID)
    {
        GUI.Label(new Rect(40f, 30f, 320f, 16f), labelError);
    }
    private void DoWindowErrorConnect(int windowID)
    {
        GUI.Label(new Rect(60f, 30f, 320f, 16f), "Error connect");
    }

    private void CreatePoolTanks()
    {
        tanksTransforms = new Transform[allTanks];
        tanksScripts = new TankScript[allTanks];
        tanksRenderers = new Renderer[allTanks];
        tanksColliders = new Collider[allTanks];
        for (int i = 0; i < allTanks; ++i)
        {
            tanksTransforms[i] = (Transform)Instantiate(tankPrefab, Vector3.zero, Quaternion.Euler(0.0f, 0.0f, 0.0f));
            tanksScripts[i] = tanksTransforms[i].gameObject.GetComponent<TankScript>();
            tanksScripts[i].number = i;
            nrbs[i] = tanksTransforms[i].gameObject.GetComponent<NetworkRigidbody>();
            if (i >= 0 && i <= 3)
            {
                tanksScripts[i].ally = "Player";
                tanksScripts[i].fireRate = 0.3f;
                tanksTransforms[i].gameObject.AddComponent<PlayerInput>();
                tanksTransforms[i].gameObject.GetComponent<PlayerInput>().number = i + 1;
                playersList[i].number = i + 1;
            }
            else
            {
                tanksTransforms[i].audio.enabled = false;
                tanksScripts[i].ally = "Enemy";
                tanksScripts[i].fireRate = 0.4f;
                tanksTransforms[i].gameObject.AddComponent<AIInput>();
                tanksTransforms[i].gameObject.GetComponent<AIInput>().number = i;
            }
            tanksRenderers[i] = tanksTransforms[i].GetComponent<Renderer>();
            tanksRenderers[i].materials[1].SetTexture("_MainTex", trucks);
            tanksColliders[i] = tanksTransforms[i].GetComponent<Collider>();
            tanksTransforms[i].tag = "Tank";
            tanksTransforms[i].parent = tanksRoot;
            tanksRenderers[i].enabled = false;
            tanksColliders[i].enabled = false;
        }
    }
    private void ResetPoolTanks()
    {
        typesTanks = new int[allTanks];
        typesTanks[0] = tanksScripts[0].rank;
        typesTanks[1] = tanksScripts[1].rank;
        typesTanks[2] = tanksScripts[2].rank;
        typesTanks[3] = tanksScripts[3].rank;
        for (int index1 = 4; index1 < allTanks; ++index1)
        {
            int num1 = 0;
            int index2 = currentLevel;
            if (index2 > randomTypeTank.GetLength(0))
                index2 = randomTypeTank.GetLength(0) - 1;
            int num2 = UnityEngine.Random.Range(0, 100);
            if (num2 <= randomTypeTank[index2, 0])
                num1 = 0;
            else if (num2 > randomTypeTank[index2, 0] && num2 <= randomTypeTank[index2, 1])
                num1 = 1;
            else if (num2 > randomTypeTank[index2, 1] && num2 <= randomTypeTank[index2, 2])
                num1 = 2;
            else if (num2 > randomTypeTank[index2, 2] && num2 <= randomTypeTank[index2, 3])
                num1 = 3;
            typesTanks[index1] = num1;
        }
        for (int index = 0; index < allTanks; ++index)
        {
            if (index >= 0 && index <= 3)
            {
                tanksTransforms[index].gameObject.GetComponent<MeshFilter>().mesh = playerMeshes[typesTanks[index]];
                tanksScripts[index].invincible = 3;
            }
            else
            {
                tanksTransforms[index].gameObject.GetComponent<MeshFilter>().mesh = enemyMeshes[typesTanks[index]];
                if (index == 6 || index == 13 || index == 20)
                    tanksScripts[index].special = true;
            }
        }
    }

    [RPC]
    private void StageClear()
    {
        ShowFlower(new Vector3(15f, 0.0f, 16f), "STAGE CLEAR");
        CancelInvoke();
        Invoke("NextLevel", 5f);
        //gameScreen = GameScreen.Win;
    }

    private void ProcessInputMenuContinue()
    {
        float axis = Input.GetAxis("P1Vertical");
        if (oldAxe != Mathf.RoundToInt(axis))
            firstMoveAxeV = false;
        if (axis >= -0.5 && axis <= 0.5)
            oldAxe = 0;
        if (axis > 0.5 && !firstMoveAxeV)
        {
            --currentSelectV;
            firstMoveAxeV = true;
            oldAxe = 1;
            if (currentSelectV < 0)
                currentSelectV = continueMenuStrings.Length - 1;
            audio.PlayOneShot(menuSound);
        }
        if (axis < -0.5 && !firstMoveAxeV)
        {
            ++currentSelectV;
            firstMoveAxeV = true;
            oldAxe = -1;
            if (currentSelectV > continueMenuStrings.Length - 1)
                currentSelectV = 0;
            audio.PlayOneShot(menuSound);
        }
        if (!Input.GetButtonDown("P1Start") && !Input.GetButtonDown("P1Fire1"))
            return;
        GUI_Action_Continue_Menu(currentSelectV);
    }
    private void ProcessInputChooseMapPackConstruction()
    {
        float axis1 = Input.GetAxis("P1Vertical");
        if (oldAxe != Mathf.RoundToInt(axis1))
            firstMoveAxeV = false;
        if (axis1 >= -0.5 && axis1 <= 0.5)
            oldAxe = 0;
        if (axis1 > 0.5 && !firstMoveAxeV)
        {
            --currentSelectV;
            firstMoveAxeV = true;
            oldAxe = 1;
            if (currentSelectV < 0)
                currentSelectV = fileInfoMapPacksArray.Length - 1;
            audio.PlayOneShot(menuSound);
            currentMapPack = currentSelectV;
        }
        if (axis1 < -0.5 && !firstMoveAxeV)
        {
            ++currentSelectV;
            firstMoveAxeV = true;
            oldAxe = -1;
            if (currentSelectV > fileInfoMapPacksArray.Length - 1)
                currentSelectV = 0;
            audio.PlayOneShot(menuSound);
            currentMapPack = currentSelectV;
        }
        float axis2 = Input.GetAxis("P1Horizontal");
        if (oldAxeH != Mathf.RoundToInt(axis2))
            firstMoveAxeH = false;
        if (axis2 >= -0.5 && axis2 <= 0.5)
            oldAxeH = 0;
        if (axis2 > 0.5 && !firstMoveAxeH)
        {
            ++currentSelectH;
            firstMoveAxeH = true;
            oldAxeH = 1;
            if (currentSelectH < 0)
                currentSelectH = chooseMapPackMenuStrings.Length - 1;
            if (currentSelectH > chooseMapPackMenuStrings.Length - 1)
                currentSelectH = 0;
            audio.PlayOneShot(menuSound);
        }
        if (axis2 < -0.5 && !firstMoveAxeH)
        {
            --currentSelectH;
            firstMoveAxeH = true;
            oldAxeH = -1;
            if (currentSelectH < 0)
                currentSelectH = chooseMapPackMenuStrings.Length - 2;
            if (currentSelectH > chooseMapPackMenuStrings.Length - 2)
                currentSelectH = 0;
            audio.PlayOneShot(menuSound);
        }
        if (!Input.GetButtonDown("P1Start") && !Input.GetButtonDown("P1Fire1"))
            return;
        GUI_Action_ChooseMapPackConstruction(currentSelectV, currentSelectH);
    }
    private void ProcessInputEnterName()
    {
        float axis1 = Input.GetAxis("P1Vertical");
        if (oldAxe != Mathf.RoundToInt(axis1))
            firstMoveAxeV = false;
        if (axis1 >= -0.5 && axis1 <= 0.5)
            oldAxe = 0;
        if (axis1 > 0.5 && !firstMoveAxeV)
        {
            --currentSelectV;
            firstMoveAxeV = true;
            oldAxe = 1;
            if (currentSelectV < 0)
                currentSelectV = symbols.Length - 1;
            audio.PlayOneShot(menuSound);
            char[] chArray = newMapPackName.ToCharArray();
            chArray[currentSelectH] = (char)symbols[currentSelectV];
            newMapPackName = new string(chArray);
        }
        if (axis1 < -0.5 && !firstMoveAxeV)
        {
            ++currentSelectV;
            firstMoveAxeV = true;
            oldAxe = -1;
            if (currentSelectV > symbols.Length - 1)
                currentSelectV = 0;
            audio.PlayOneShot(menuSound);
            char[] chArray = newMapPackName.ToCharArray();
            chArray[currentSelectH] = (char)symbols[currentSelectV];
            newMapPackName = new string(chArray);
        }
        float axis2 = Input.GetAxis("P1Horizontal");
        if (oldAxeH != Mathf.RoundToInt(axis2))
            firstMoveAxeH = false;
        if (axis2 >= -0.5 && axis2 <= 0.5)
            oldAxeH = 0;
        if (axis2 > 0.5 && !firstMoveAxeH)
        {
            ++currentSelectH;
            firstMoveAxeH = true;
            oldAxeH = 1;
            if (currentSelectH < 0)
                currentSelectH = maxSymbol;
            if (currentSelectH > maxSymbol)
                currentSelectH = 0;
            audio.PlayOneShot(menuSound);
        }
        if (axis2 < -0.5 && !firstMoveAxeH)
        {
            --currentSelectH;
            firstMoveAxeH = true;
            oldAxeH = -1;
            if (currentSelectH < 0)
                currentSelectH = maxSymbol;
            if (currentSelectH > maxSymbol)
                currentSelectH = 0;
            audio.PlayOneShot(menuSound);
        }
        if (!Input.GetButtonDown("P1Start") && !Input.GetButtonDown("P1Fire1"))
            return;
        GUI_Action_EnterName();
    }
    private void ProcessInputChooseSize()
    {
        float axis = Input.GetAxis("P1Vertical");
        if (oldAxe != Mathf.RoundToInt(axis))
            firstMoveAxeV = false;
        if (axis >= -0.5 && axis <= 0.5)
            oldAxe = 0;
        if (axis > 0.5 && !firstMoveAxeV)
        {
            --currentSelectV;
            firstMoveAxeV = true;
            oldAxe = 1;
            if (currentSelectV < 0)
                currentSelectV = chooseSizeStrings.Length - 1;
            audio.PlayOneShot(menuSound);
        }
        if (axis < -0.5 && !firstMoveAxeV)
        {
            ++currentSelectV;
            firstMoveAxeV = true;
            oldAxe = -1;
            if (currentSelectV > chooseSizeStrings.Length - 1)
                currentSelectV = 0;
            audio.PlayOneShot(menuSound);
        }
        if (!Input.GetButtonDown("P1Start") && !Input.GetButtonDown("P1Fire1"))
            return;
        GUI_Action_ChooseSize(currentSelectV);
    }

    void OnChangeVerticalSelect()
    {
        buttonMoveSound.Play();
        if (gameScreen == GameScreen.EnterIP)
        {
            if (currentSelectH >= 0 && currentSelectH <= 3)
            {
                SaveOptions();
            }
        }
    }
    void OnChangeHorizontalSelect()
    {
        buttonMoveSound.Play();
        if (gameScreen == GameScreen.EnterIP)
        {
            if (currentSelectH >= 0 && currentSelectH <= 3)
            {
                currentSelectV = 255 - ipNumbers[currentSelectH];
            }
        }
    }
    void ReleaseInput()
    {
        firstMoveAxeV = false;
        firstMoveAxeH = false;
    }

    //Функция отслеживания ввода для перемещения по меню. Доработки более не требует.
    private void ProcessInput(int maxVertical, int maxHorizontal)
    {
        float timeToRelease = 0.2f;

        

        if (!IsInvoking("ReleaseInput"))
        {
            //Вертикальный курсор
            float axis = Input.GetAxis("P1Vertical");
            if (oldAxe != Mathf.RoundToInt(axis))
            {
                firstMoveAxeV = false;
            }
            if (axis >= -0.5f && axis <= 0.5f)
            {
                oldAxe = 0;
            }
            if (axis > 0.5f && !firstMoveAxeV)
            {
                currentSelectV--;
                firstMoveAxeV = true;
                oldAxe = 1;
                if (currentSelectV < 0)
                {
                    currentSelectV = maxVertical;
                }
                if (!IsInvoking("ReleaseInput"))
                {
                    Invoke("ReleaseInput", timeToRelease);
                }
                audio.PlayOneShot(menuSound);
            }
            if (axis < -0.5f && !firstMoveAxeV)
            {
                currentSelectV++;
                firstMoveAxeV = true;
                oldAxe = -1;
                if (currentSelectV > maxVertical)
                {
                    currentSelectV = 0;
                }
                if (!IsInvoking("ReleaseInput"))
                {
                    Invoke("ReleaseInput", timeToRelease);
                }
                audio.PlayOneShot(menuSound);
            }

            //Горизонтальный курсор
            float axis2 = Input.GetAxis("P1Horizontal");
            if (oldAxeH != Mathf.RoundToInt(axis2))
            {
                firstMoveAxeH = false;
            }
            if (axis2 >= -0.5f && axis2 <= 0.5f)
            {
                oldAxeH = 0;
            }
            if (axis2 > 0.5f && !firstMoveAxeH)
            {
                currentSelectH++;
                firstMoveAxeH = true;
                oldAxeH = 1;
                if (currentSelectH < 0)
                {
                    currentSelectH = maxHorizontal;
                }
                if (currentSelectH > maxHorizontal)
                {
                    currentSelectH = 0;
                }
                if (!IsInvoking("ReleaseInput"))
                {
                    Invoke("ReleaseInput", timeToRelease);
                }
                audio.PlayOneShot(menuSound);
            }
            if (axis2 < -0.5f && !firstMoveAxeH)
            {
                currentSelectH--;
                firstMoveAxeH = true;
                oldAxeH = -1;
                if (currentSelectH < 0)
                {
                    currentSelectH = maxHorizontal;
                }
                if (currentSelectH > maxHorizontal)
                {
                    currentSelectH = 0;
                }
                if (!IsInvoking("ReleaseInput"))
                {
                    Invoke("ReleaseInput", timeToRelease);
                }
                audio.PlayOneShot(menuSound);
            }

            //Действие при нажатии
            if (Input.GetButtonDown("P1Start") || Input.GetButtonDown("P1Fire1"))
            {
                GUI_Action(currentSelectV, currentSelectH);
            }
        }
    }

    //Функция реакции интерфейса на ввод.
    private void GUI_Action(int verticalSelect, int horizontalSelect)
    {
        switch (gameScreen)
        {
            case GameScreen.Tanks:
            {
                switch (horizontalSelect)
                {
                    //0-3 ip numbers
                    case 0:
                    {
                        SetScreen(GameScreen.Menu);
                        break;
                    }
                    case 1:
                    {
                        currentInfoTank++;
                        if (currentInfoTank > 7)
                        {
                            currentInfoTank = 0;
                        }
                        if (currentInfoTank < 4)
                        {
                            infoTank.GetComponent<MeshFilter>().mesh = playerMeshes[currentInfoTank];
                        }
                        else {
                            infoTank.GetComponent<MeshFilter>().mesh = enemyMeshes[currentInfoTank - 4];
                        }
                        break;
                    }
                }
                break;
            }
            case GameScreen.EnterIP:
            {
                switch (horizontalSelect)
                {
                    //0-3 ip numbers
                    case 0:
                    {
                        //ipNumbers[0] = verticalSelect;
                        //connectToIP = ipNumbers[0].ToString("D3") + "." +
                        //    ipNumbers[1].ToString("D3") + "." +
                        //    ipNumbers[2].ToString("D3") + "." +
                        //    ipNumbers[3].ToString("D3");
                        break;
                    }
                    case 1:
                    {
                        //ipNumbers[1] = verticalSelect;
                        //connectToIP = ipNumbers[0].ToString("D3") + "." +
                        //    ipNumbers[1].ToString("D3") + "." +
                        //    ipNumbers[2].ToString("D3") + "." +
                        //    ipNumbers[3].ToString("D3");
                        break;
                    }
                    case 2:
                    {
                        //ipNumbers[2] = verticalSelect;
                        //connectToIP = ipNumbers[0].ToString("D3") + "." +
                        //    ipNumbers[1].ToString("D3") + "." +
                        //    ipNumbers[2].ToString("D3") + "." +
                        //    ipNumbers[3].ToString("D3");
                        break;
                    }
                    case 3:
                    {
                        //ipNumbers[3] = verticalSelect;
                        //connectToIP = ipNumbers[0].ToString("D3") + "." +
                        //    ipNumbers[1].ToString("D3") + "." +
                        //    ipNumbers[2].ToString("D3") + "." +
                        //    ipNumbers[3].ToString("D3");
                        break;
                    }
                    //BACK
                    case 4:
                    {
                        //nv.RPC("SetScreenInt", RPCMode.All, (int)GameScreen.Menu);
                        SetScreen(GameScreen.Menu);
                        break;
                    }
                    //JOIN
                    case 5:
                    {
                        //nv.RPC("SetScreenInt", RPCMode.All, (int)GameScreen.NetworkLobby);
                        Network.Connect(connectToIP, 25001);
                        SetScreen(GameScreen.NetworkLobby);
                        break;
                    }
                }
                break;
            }
            case GameScreen.NetworkLobby:
            {
                switch (verticalSelect)
                {
                    //Kick Player 2
                    case 0:
                    {
                        break;
                    }
                    //Kick Player 3
                    case 1:
                    {
                        break;
                    }
                    //Kick Player 4
                    case 2:
                    {
                        break;
                    }
                    //MAP PACK
                    case 3:
                    {
                        if (Network.isServer)
                        {
                            currentMapPack++;
                            if (currentMapPack > fileInfoMapPacksArray.Length - 1)
                            {
                                currentMapPack = 0;
                            }
                            currentMapPackStr = fileInfoMapPacksArray[currentMapPack].Name;
                            nv.RPC("SetMapPackString", RPCMode.Others, currentMapPackStr);
                        }
                        break;
                    }
                    //DIFFICULTY
                    case 4:
                    {
                        break;
                    }
                    //START
                    case 5:
                    {
                        nv.RPC("SetScreenInt", RPCMode.All, (int)GameScreen.NewLevel);
                        break;
                    }
                    //BACK
                    case 6:
                    {
                        //MasterServer.UnregisterHost();
                        Network.Disconnect();
                        SetScreen(GameScreen.Menu);
                        break;
                    }
                }
                break;
            }
            case GameScreen.Network:
            {
                //1-10 - список серверов.
                if (verticalSelect >= 1 && verticalSelect <= 10)
                {
                    if (hostData.Length >= verticalSelect + hostDataList * 10)
                    {
                        Network.Connect(hostData[verticalSelect - 1 + hostDataList * 10]);
                        //gameScreen = GameScreen.NetworkLobby;
                        SetScreen(GameScreen.NetworkLobby);
                    }
                    //showWindowConnect = true;
                    currentLevel = 0;
                    ResetPlayers();
                }

                switch (verticalSelect)
                {   
                    //SERVERS
                    case 0:
                    {
                        hostDataList++;
                        if (hostDataList > 9)
                        {
                            hostDataList = 0;
                        }
                        break;
                    }
                    //BACK
                    case 11:
                    {
                        SetScreen(GameScreen.Menu);
                        break;
                    }
                    //HOST
                    case 12:
                    {
                        Network.InitializeServer(3, 25001, false);
                        //showWindowConnect = true;
                        //currentLevel = 0;
                        //ResetPlayers();
                        //MasterServer.RegisterHost(codeNet, UnityEngine.Random.Range(0, 10000).ToString("D4") + "-"
                        //    + UnityEngine.Random.Range(0, 10000).ToString("D4") + "-"
                        //    + UnityEngine.Random.Range(0, 10000).ToString("D4"), "");
                        SetScreen(GameScreen.NetworkLobby);
                        break;
                    }
                    //CONNECT TO IP
                    case 13:
                    {
                        SetScreen(GameScreen.EnterIP);
                        break;
                    }
                }
                break;
            }
            case GameScreen.LocalGame:
            {
                switch (verticalSelect)
                {
                    //NUMBERS OF PLAYERS
                    case 0:
                    {
                        playersCount++;
                        if (playersCount > 4)
                        {
                            playersCount = 1;
                        }
                        break;
                    }
                    //MAP PACK
                    case 1:
                    {
                        currentMapPack++;
                        if (currentMapPack > fileInfoMapPacksArray.Length - 1)
                        {
                            currentMapPack = 0;
                        }
                        currentMapPackStr = fileInfoMapPacksArray[currentMapPack].Name;
                        break;
                    }
                    //DIFFICULTY
                    case 2:
                    {
                        difficulty++;
                        if (difficulty > 2)
                        {
                            difficulty = 0;
                        }
                        break;
                    }
                    //START
                    case 3:
                    {
                        if (difficulty == 0)
                        {
                            playersList[0].lifes = 99;
                            playersList[1].lifes = 99;
                            playersList[2].lifes = 99;
                            playersList[3].lifes = 99;
                        }
                        if (difficulty == 1 || difficulty == 2)
                        {
                            playersList[0].lifes = 3;
                            playersList[1].lifes = 3;
                            playersList[2].lifes = 3;
                            playersList[3].lifes = 3;
                        }
                        
                        scoresToLifes[0] = 1;
                        scoresToLifes[1] = 1;
                        scoresToLifes[2] = 1;
                        scoresToLifes[3] = 1;

                        Network.InitializeServer(0, 25001, false);
                        SetScreen(GameScreen.NewLevel);
                        break;
                    }
                    //BACK
                    case 4:
                    {
                        SetScreen(GameScreen.Menu);
                        break;
                    }
                }
                break;
            }
            case GameScreen.Menu:
            {
                switch (verticalSelect)
                {
                    //NEW LOCAL GAME
                    case 0:
                    {
                        //SetScreen(GameScreen.ChooseNumberPlayers);
                        SetScreen(GameScreen.LocalGame);
                        currentLevel = 0;
                        ResetPlayers();
                        break;
                    }
                    //NEW NETWORK GAME
                    case 1:
                    {
                        //currentLevel = 0;
                        //SetScreen(GameScreen.Network);
                        
                        //MasterServer.ClearHostList();
                        //MasterServer.RequestHostList(codeNet);
                        break;
                    }
                    //CONSTRUCTION
                    case 2:
                    {
                        //SetScreen(GameScreen.ChooseMapPackConstruction);
                        break;
                    }
                    //TANKS
                    case 3:
                    {
                        SetScreen(GameScreen.Tanks);
                        break;
                    }
                    //EXIT
                    case 4:
                    {
                        Application.Quit();
                        break;
                    }
                }
                break;
            }
        }
    }
    
    private void GUI_Action_ChooseMapPackConstruction(int n, int m)
    {
        switch (m)
        {
            case 0:
                showWindowEnterName = true;
                break;
            case 1:
                SetScreen(GameScreen.Construction);
                break;
        }
    }
    private void GUI_Action_Continue_Menu(int n)
    {
        switch (n)
        {
            case 0:
                --currentLevel;
                ResetPlayers();
                SetScreen(GameScreen.NewLevel);
                break;
            case 1:
                SetScreen(GameScreen.Menu);
                break;
        }
    }
    private void GUI_Action_EnterName()
    {
        showWindowEnterName = false;
        showWindowChooseSize = true;
    }
    private void GUI_Action_ChooseSize(int n)
    {
        switch (n)
        {
            case 0:
                sizeLabX = 26;
                sizeLabY = 26;
                MainScript mainScript1 = this;
                string[] strArray1 = new string[3];
                int index1 = 0;
                string str1 = "13";
                strArray1[index1] = str1;
                int index2 = 1;
                string str2 = "13";
                strArray1[index2] = str2;
                int index3 = 2;
                string str3 = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
                strArray1[index3] = str3;
                mainScript1.levels = strArray1;
                break;
            case 1:
                sizeLabX = 26;
                sizeLabY = 34;
                MainScript mainScript2 = this;
                string[] strArray2 = new string[3];
                int index4 = 0;
                string str4 = "13";
                strArray2[index4] = str4;
                int index5 = 1;
                string str5 = "17";
                strArray2[index5] = str5;
                int index6 = 2;
                string str6 = "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
                strArray2[index6] = str6;
                mainScript2.levels = strArray2;
                break;
        }
        string path = "./MapPacks/" + newMapPackName + ".maps";
        File.WriteAllLines(path, levels);
        SetScreen(GameScreen.Construction);
        showWindowChooseSize = false;
        fileToLoad = path;
    }

    private void UpdateFlowers()
    {
        float deltaTime = Time.deltaTime;
        for (int i = 0; i < flowerList.Count; ++i)
        {
            if (!flowerList[i].special)
            {
                if (flowerList[i].state == 0)
                {
                    flowerList[i].curY -= 50f * deltaTime;
                    flowerList[i].a += 5f * deltaTime;
                }
                if (flowerList[i].a >= 1.0f && flowerList[i].state == 0)
                    flowerList[i].state = 1;
                if (flowerList[i].state == 1)
                    flowerList[i].time += 1f * deltaTime;
                if (flowerList[i].time >= flowerList[i].timeEnd && flowerList[i].state == 1)
                    flowerList[i].state = 2;
                if (flowerList[i].state == 2)
                {
                    flowerList[i].a -= 5f * deltaTime;
                    flowerList[i].curY -= 50f * deltaTime;
                }
                if (flowerList[i].a <= 0.0f && flowerList[i].state == 2)
                    flowerList.Remove(flowerList[i]);
            }
            else if (flowerList[i].special)
            {
                if (flowerList[i].state == 0)
                    flowerList[i].a += 5f * deltaTime;
                if (flowerList[i].a >= 1.0f && flowerList[i].state == 0)
                    flowerList[i].state = 1;
                if (flowerList[i].state == 1)
                    flowerList[i].time += 1f * deltaTime;
                if (flowerList[i].time >= flowerList[i].timeEnd && flowerList[i].state == 1)
                    flowerList[i].state = 2;
                if (flowerList[i].state == 2)
                    flowerList[i].a -= 5f * deltaTime;
                if (flowerList[i].a <= 0.0f && flowerList[i].state == 2)
                    flowerList[i].state = 0;
            }
        }
    }
    public void ShowFlower(string msg1)
    {
        Vector3 vector3 = mainCamera.WorldToScreenPoint(new Vector3(15.0f, 0.0f, 16.0f));
        Flower flower = new Flower();
        float num = (float)Screen.width / Screen.height;
        vector3.x = (vector3.x / scaleX * num * 0.75f - (scaleX - scale.y) / 2.5f * originalWidth);
        vector3.y = vector3.y / scale.y;
        flower.X = vector3.x - (msg1.Length * 16 / 2);
        flower.Y = originalHeight - vector3.y;
        flower.txt = msg1;
        flower.timeEnd = 1.0f;
        flowerList.Add(flower);
    }
    public void ShowFlower(Vector3 pos, string msg1)
    {
        Vector3 vector3 = mainCamera.WorldToScreenPoint(pos);
        Flower flower = new Flower();
        float num = (float)Screen.width / Screen.height;
        vector3.x = (float)(vector3.x / scaleX * num * 0.75f - (scaleX - scale.y) / 2.5f * originalWidth);
        vector3.y = vector3.y / scale.y;
        flower.X = vector3.x - (float)(msg1.Length * 16.0f / 2.0f);
        flower.Y = originalHeight - vector3.y;
        flower.txt = msg1;
        flower.timeEnd = 1.0f;
        flowerList.Add(flower);
    }
    public void ShowFlower(Vector3 pos, string msg1, float time)
    {
        Vector3 vector3 = mainCamera.WorldToScreenPoint(pos);
        Flower flower = new Flower();
        float num = (float)Screen.width / (float)Screen.height;
        vector3.x = (float)(vector3.x / scaleX * num * 0.75f - (scaleX - scale.y) / 2.5f * originalWidth);
        vector3.y = vector3.y / scale.y;
        flower.X = vector3.x - (float)(msg1.Length * 16.0f / 2.0f);
        flower.Y = originalHeight - vector3.y;
        flower.txt = msg1;
        flower.timeEnd = time;
        flowerList.Add(flower);
    }

    private void ResetPlayers()
    {
        //for (int i = 0; i < playersList.Length; i++)
        //{
        //    playersList[i].scores = 0;
        //    playersList[i].lifes = 3;
        //    for (int j = 0; j < 5; j++)
        //    {
        //        playersList[i].kills[j] = 0;
        //        playersList[i].scores1[j] = 0;
        //    }
        //    tanksScripts[i].SetRank(0);
        //    tanksScripts[i].waterWalking = false;
        //}
    }

    private void Salut()
    {
        UnityEngine.Object.Instantiate((UnityEngine.Object)explosion, new Vector3(UnityEngine.Random.Range(0.0f, 30f), 0.0f, UnityEngine.Random.Range(0.0f, 30f)), Quaternion.identity);
    }

    public void SetColorByHitPoints(int numberTank, int hitPoints)
    {
        Renderer renderer = tanksTransforms[numberTank].gameObject.renderer;
        if (hitPoints == 4)
        {
            renderer.material.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        }
        else if (hitPoints == 3)
        {
            renderer.material.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
        }
        else if (hitPoints == 2)
        {
            renderer.material.color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        }
        else if (hitPoints == 1)
        {
            renderer.material.color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        }
    }

    //numberTank - номер танка, в который попали.
    //otherTank - номер танка, который выстрелил.
    [RPC] 
    public void ProcessCollision(int numberTank, int otherTank)
    {
        Debug.Log(Time.time.ToString("F2") + " : ProcessCollision(" + numberTank + "," + otherTank + ");");

        string ally = tanksScripts[numberTank].ally;
        int invincible = tanksScripts[numberTank].invincible;
        int hitPoints = tanksScripts[numberTank].hitPoints;
        int type = tanksScripts[numberTank].type;
        bool special = tanksScripts[numberTank].special;
        string otherAlly = tanksScripts[otherTank].ally;
        if (invincible == 0)
        {
            if (hitPoints > 1)
            {
                if (tanksScripts[numberTank].waterWalking)
                {
                    tanksScripts[numberTank].SetWaterWalking(false);
                }
                hitPoints--;
                SetColorByHitPoints(numberTank, hitPoints);
            }
            else
            {
                //tanksExplosionsTransforms[numberTank].position = tanksTransforms[numberTank].position;
                //tanksDetonators[numberTank].Explode();

                //if (!noSound)
                //{
                //    tanksExplosionsTransforms[numberTank].audio.Play();
                //}

                bigExplosions.ShowAtPosition(tanksTransforms[numberTank].position);
                //Debug.Break();

                if (ally == "Player")
                {
                    Player player = playersList[numberTank];
                    tanksScripts[numberTank].SetRank(0);
                    if (player.lifes > 0)
                    {
                        player.lifes--;
                        nv.RPC("SpawnSpawnPlayer", RPCMode.All, numberTank);
                    }
                    else if (!gameOver)
                    {
                        StartBackToMenu();
                    }
                }
                else if (ally == "Enemy")
                {
                    ShowFlower(tanksTransforms[numberTank].position, scores[type].ToString());
                    if (otherAlly == "Player")
                    {
                        Player player = playersList[otherTank];
                        player.scores += scores[type];
                        if (hiScore < player.scores)
                        {
                            hiScore = player.scores;
                        }
                        player.kills[type]++;
                    }
                    enemysScreen--;
                    numberEnemyTanks--;
                    if (special)
                    {
                        int pos = Random.Range(0, 4);
                        int selectedType = 0;

                        if (difficulty == 0 || difficulty == 1)
                        {
                            selectedType = Random.Range(0, 5);
                        }
                        else if (difficulty == 2)
                        {
                            selectedType = Random.Range(0, 7);
                        }
                        if (Network.isServer)
                        {
                            nv.RPC("SpawnBonus", RPCMode.All, bonusPositions[pos], selectedType);
                        }
                    }
                }
                tanksRenderers[numberTank].enabled = false;
                tanksColliders[numberTank].enabled = false;

                //tanksTransforms[numberTank].position = Vector3.zero;
                //tanksTransforms[numberTank].rotation = Quaternion.identity;
                //tanksTransforms[numberTank].gameObject.SetActive(false);
            }
        }
        tanksScripts[numberTank].hitPoints = hitPoints;
    }

    //Поскольку игрок появляется не сразу, а сначала появляется мерцание,
    // пришлось создать специальный объект Spawn,
    // который создаёт игрока по истечении времени.
    [RPC]
    public void SpawnSpawnPlayer(int number)
    {
        GameObject spawnGO = spawns.ShowAtPosition(playerRespawns[number]);
        Spawn spawnScript = spawnGO.GetComponent<Spawn>();
        spawnScript.number = number;
        
        GameObject spawnSprite = spawnSprites.ShowAtPosition(playerRespawns[number]);
    }
    [RPC]
    public void SpawnSpawnEnemy(int r, int number, int type)
    {
        //Это спрайт мерцания.
        GameObject spawnSprite = spawnSprites.ShowAtPosition(enemyRespawns[r]);

        //Это специальный объект для создания объекта по истечении времени.
        // Потом лучше будет переписать под использование функции Invoke. Или нет.
        //Spawn учитывает нажатие кнопки паузы, а Invoke не будет.
        GameObject spawnGO = spawns.ShowAtPosition(enemyRespawns[r]);
            
        Spawn spawnScript = spawnGO.GetComponent<Spawn>();
        spawnScript.number = number;
        spawnScript.r = r;
        spawnScript.type = type;
    }
    [RPC]
    void SpawnPlayer(int number)
    {
        tanksTransforms[number].position = playerRespawns[number];
        tanksTransforms[number].rotation = Quaternion.identity;

        //ms.playersCameraViews[number].parent = ms.tanksTransforms[number];
        //ms.playersCameraViews[number].localPosition = new Vector3(0.0f, 2f, -2f);
        //ms.playersCameraViews[number].localRotation = Quaternion.Euler(30f, 0.0f, 0.0f);

        Renderer r = tanksRenderers[number];
        r.materials[0] = mats[0];
        r.materials[1] = matsTrucks[0];
        r.materials[2] = matsTrucks[1];
        r.materials[0].color = colors[number];
        r.materials[1].color = colors[number];
        r.materials[2].color = colors[number];
        r.enabled = true;

        tanksColliders[number].enabled = true;

        tanksScripts[number].SetInvincibility(3);

        tanksScripts[number].speedTank = 3;
    }
    [RPC]
    public void SpawnBullet(int numberTank, bool second, string ally, float bulletSpeed, Vector3 pos, int rot)
    {
        if (!second)
        {
            Transform tmpTransform = bulletTransforms[numberTank];
            if (numberTank < 4)
            {
                if (!noSound)
                {
                    tmpTransform.audio.Play();
                }
            }
            tmpTransform.position = pos;
            tmpTransform.rotation = Quaternion.Euler(0.0f, rot * 90.0f, 0.0f);
            tmpTransform.gameObject.renderer.enabled = true;
            tmpTransform.gameObject.collider.enabled = true;
            tmpTransform.rigidbody.angularVelocity = Vector3.zero;
            tmpTransform.rigidbody.velocity = Vector3.zero;
            tmpTransform.rigidbody.AddForce(tmpTransform.forward * bulletSpeed, ForceMode.VelocityChange);
            BulletScript bulletScript = tmpTransform.GetComponent<BulletScript>();
            bulletScript.number = numberTank;
            bulletScript.direction = rot;
            bulletScript.ally = ally;
            tmpTransform.parent = bullets;
            if (!(ally == "Player") || tanksScripts[numberTank].rank <= 2)
            {
                return;
            }
            tmpTransform.transform.tag = "SpecialBullet";
        }
        else
        {
            Transform tmpTransform = bulletTransforms[numberTank + allTanks];
            if (numberTank < 4)
            {
                if (!noSound)
                {
                    tmpTransform.audio.Play();
                }
            }
            tmpTransform.position = pos;
            tmpTransform.rotation = Quaternion.Euler(0.0f, rot * 90.0f, 0.0f);
            tmpTransform.gameObject.renderer.enabled = true;
            tmpTransform.gameObject.collider.enabled = true;
            tmpTransform.rigidbody.AddForce(tmpTransform.forward * bulletSpeed, ForceMode.VelocityChange);
            BulletScript bulletScript = tmpTransform.GetComponent<BulletScript>();
            bulletScript.number = numberTank;
            bulletScript.ally = ally;
            bulletScript.direction = rot;
            tmpTransform.parent = bullets;
            if (!(ally == "Player") || tanksScripts[numberTank].rank <= 2)
            {
                return;
            }
            tmpTransform.tag = "SpecialBullet";
        }
    }
    [RPC]
    public void SpawnBonus(Vector3 pos, int type)
    {
        bonusScript.Show(pos, (Bonus)type);
        //bonusScript.Show(bonusPositions[pos], (Bonus)type);
    }
    [RPC]
    private void SpawnEnemy(int r, int number, int type)
    {
        tanksTransforms[number].position = enemyRespawns[r];

        AIInput ai = tanksTransforms[number].GetComponent<AIInput>();

        float num = (float)((190.0f - currentLevel * 4.0f - (playersCount - 1.0f) * 20.0f) / 60.0f);

        ai.oldPosition = transform.position;
        ai.timerModeRate = num * 8.0f;
        ai.timerMode = otherTime + ai.timerModeRate;
        ai.mode = 0;

        tanksScripts[number].type = type;
        tanksScripts[number].speedTank = type != 1 ? 2 : 4;
        tanksScripts[number].bulletSpeed = type != 2 ? 7.5f : 15.0f;

        if (type == 3)
        {
            tanksScripts[number].hitPoints = 4;
            tanksScripts[number].speedTank = 2;
        }
        else
        {
            tanksScripts[number].hitPoints = 1;
        }

        Renderer rend = tanksRenderers[number];
        rend.materials[0] = mats[0];
        rend.materials[0].color = Color.grey;
        rend.materials[1] = matsTrucks[0];
        rend.materials[1].color = Color.grey;
        rend.materials[2] = matsTrucks[0];
        rend.materials[2].color = Color.black;
        rend.enabled = true;

        tanksColliders[number].enabled = true;

        numberSpawnedEnemy++;
        enemysScreen++;
    }
    private void SpawnPlane(int n)
    {
        planeTransform = (Transform)UnityEngine.Object.Instantiate((UnityEngine.Object)planePrefab[n], new Vector3(15f, 0.0f, 16f), Quaternion.identity);
    }

    private void StartGame()
    {
        for (int i = 4; i < allTanks; i++)
        {
            tanksScripts[i].waterWalking = false;
        }
        //if (currentLevel <= levels.Length)
        //{
        gameOver = false;
        numberSpawnedEnemy = 0;
        enemysScreen = 0;
        numberEnemyTanks = allEnemy;
        respawnTimer = otherTime;
        //if (!Network.isClient)
        //{
        //    LoadLabyrinthFromString(levels[currentLevel + 1]);
        //}
        for (int number = 0; number < playersCount; number++)
        {
            nv.RPC("SpawnSpawnPlayer", RPCMode.All, number);
        }
        SpawnPlane(0);
        //}
        //else
        //{
        //    SetScreen(GameScreen.End);
        //}
    }

    [RPC]
    public void SetBulletPosRot(int bullet, Vector3 pos, int rot)
    {
        bulletTransforms[bullet].position = pos;
        bulletTransforms[bullet].rotation = Quaternion.Euler(0.0f, rot * 90.0f, 0.0f);
    }

    [RPC]
    public void SetTankPosRot(int tank, Vector3 pos, int rot)
    {
        nrbs[tank].SetPosition(tank, pos, Quaternion.Euler(0, rot * 90, 0), Vector3.zero, Vector3.zero, Network.time);
        tanksScripts[tank].direction = rot;
        //tanksTransforms[tank].position = pos;
        //tanksTransforms[tank].rotation = rot;
    }

    [RPC]
    private void ReceiveInput(Vector3 pos, int tank, int x, int y, bool fire, bool fire2)
    {
        Debug.Log((object)(Time.time.ToString() + " : ReceiveInput()"));
        tanksTransforms[tank].position = pos;
        tanksScripts[tank].inputX = x;
        tanksScripts[tank].inputY = y;
        tanksScripts[tank].fireInput = fire;
        tanksScripts[tank].fire2Input = fire2;
    }
    [RPC]
    private void ReceiveClientInput(Vector3 pos, int tank, int x, int y, bool fire, bool fire2)
    {
        //Debug.Log((object) (Time.time.ToString() + " : ReceiveClientInput()"));
        //NetworkView networkView = nv;
        //string name = "ReceiveInput";
        //int num = 2;
        //object[] objArray = new object[6];
        //int index1 = 0;
        //// ISSUE: variable of a boxed type
        //Vector3 local1 =  pos;
        //objArray[index1] = (object) local1;
        //int index2 = 1;
        //// ISSUE: variable of a boxed type
        //int local2 =  tank;
        //objArray[index2] = (object) local2;
        //int index3 = 2;
        //// ISSUE: variable of a boxed type
        //int local3 =  x;
        //objArray[index3] = (object) local3;
        //int index4 = 3;
        //// ISSUE: variable of a boxed type
        //int local4 =  y;
        //objArray[index4] = (object) local4;
        //int index5 = 4;
        //// ISSUE: variable of a boxed type
        //bool local5 =  (bool) (fire ? 1 : 0);
        //objArray[index5] = (object) local5;
        //int index6 = 5;
        //// ISSUE: variable of a boxed type
        //bool local6 =  (bool) (fire2 ? 1 : 0);
        //objArray[index6] = (object) local6;
        //networkView.RPC(name, (RPCMode) num, objArray);
    }

    public void OnChangeInput(Vector3 pos, int tank, int x, int y, bool fire, bool fire2)
    {
        //if (Network.isClient)
        //{
        //    NetworkView networkView = nv;
        //    string name = "ReceiveClientInput";
        //    int num = 1;
        //    object[] objArray = new object[6];
        //    int index1 = 0;
        //    // ISSUE: variable of a boxed type
        //    Vector3 local1 = pos;
        //    objArray[index1] = (object)local1;
        //    int index2 = 1;
        //    // ISSUE: variable of a boxed type
        //    int local2 = tank;
        //    objArray[index2] = (object)local2;
        //    int index3 = 2;
        //    // ISSUE: variable of a boxed type
        //    int local3 = x;
        //    objArray[index3] = (object)local3;
        //    int index4 = 3;
        //    // ISSUE: variable of a boxed type
        //    int local4 = y;
        //    objArray[index4] = (object)local4;
        //    int index5 = 4;
        //    // ISSUE: variable of a boxed type
        //    bool local5 = (bool)(fire ? 1 : 0);
        //    objArray[index5] = (object)local5;
        //    int index6 = 5;
        //    // ISSUE: variable of a boxed type
        //    bool local6 = (bool)(fire2 ? 1 : 0);
        //    objArray[index6] = (object)local6;
        //    networkView.RPC(name, (RPCMode)num, objArray);
        //}
        //else
        //{
        //    if (!Network.isServer)
        //        return;
        //    NetworkView networkView = nv;
        //    string name = "ReceiveInput";
        //    int num = 1;
        //    object[] objArray = new object[6];
        //    int index1 = 0;
        //    // ISSUE: variable of a boxed type
        //    Vector3 local1 = pos;
        //    objArray[index1] = (object)local1;
        //    int index2 = 1;
        //    // ISSUE: variable of a boxed type
        //    int local2 = tank;
        //    objArray[index2] = (object)local2;
        //    int index3 = 2;
        //    // ISSUE: variable of a boxed type
        //    int local3 = x;
        //    objArray[index3] = (object)local3;
        //    int index4 = 3;
        //    // ISSUE: variable of a boxed type
        //    int local4 = y;
        //    objArray[index4] = (object)local4;
        //    int index5 = 4;
        //    // ISSUE: variable of a boxed type
        //    bool local5 = (bool)(fire ? 1 : 0);
        //    objArray[index5] = (object)local5;
        //    int index6 = 5;
        //    // ISSUE: variable of a boxed type
        //    bool local6 = (bool)(fire2 ? 1 : 0);
        //    objArray[index6] = (object)local6;
        //    networkView.RPC(name, (RPCMode)num, objArray);
        //}
    }

    [RPC]
    private void NextLevel()
    {
        //currentLevel++;
        SetScreen(GameScreen.Scores);
    }

    private void ChangeCameraMode()
    {
        cameraMode++;
        if (cameraMode > 2)
        {
            cameraMode = 0;
        }
        ShowFlower(new Vector3(15.0f, 0.0f, 16.0f), "Camera " + cameraMode.ToString());
        if (cameraMode == 0)
        {
            mainCamera.transform.SetParent(null);
            mainCamera.isOrthoGraphic = true;
            mainCamera.orthographicSize = 16f;
            mainCamera.nearClipPlane = -100f;
            mainCamera.transform.position = new Vector3(15f, 20f, 17f);
            mainCamera.transform.rotation = Quaternion.Euler(90f, 0.0f, 0.0f);
        }
        else if (cameraMode == 1)
        {
            mainCamera.transform.SetParent(null);
            mainCamera.isOrthoGraphic = true;
            mainCamera.orthographicSize = 16f;
            mainCamera.nearClipPlane = -100f;
            mainCamera.transform.position = new Vector3(15f, 0.0f, 16f);
            mainCamera.transform.rotation = Quaternion.Euler(45.0f, 45.0f, 0.0f);
        }
        else if (cameraMode == 2)
        {
            mainCamera.transform.SetParent(tanksTransforms[0]);
            mainCamera.isOrthoGraphic = false;
            mainCamera.fieldOfView = 60.0f;
            mainCamera.nearClipPlane = 0.1f;
            mainCamera.transform.localPosition = Vector3.zero - Vector3.up * 0.5f;
            mainCamera.transform.localRotation = Quaternion.identity;
        }
    }

    public void StartBackToMenu()
    {
        ShowFlower(new Vector3(15f, 0.0f, 16f), "GAME OVER", 5f);
        SaveOptions();
        gameOver = true;
        Invoke("BackToMenu", 5f);
    }

    private void BackToMenu()
    {
        showWindowError = false;
        showWindowErrorConnect = false;
        if (difficulty == 1 || difficulty == 2)
            SetScreen(GameScreen.Menu);
        else
            SetScreen(GameScreen.Continue);
        if (!Network.isServer && !Network.isClient)
            return;
        Network.Disconnect();
    }

    public void SetScreen(GameScreen gScr)
    {
        if (fading)
            return;
        fade.fadeState = FadeState.ToFade;
        newGameScreen = gScr;
        fading = true;
    }
    [RPC]
    public void SetScreenInt(int gScr)
    {
        if (fading)
        {
            return;
        }
        fade.fadeState = FadeState.ToFade;
        newGameScreen = (GameScreen)gScr;
        fading = true;
    }

    [RPC]
    public void DestroyAllEnemy()
    {
        for (int i = 0; i < tanksScripts.Length; i++)
        {
            if (tanksScripts[i].ally == "Enemy" && tanksRenderers[i].enabled)
            {
                numberEnemyTanks--;
                //tanksExplosionsTransforms[i].position = tanksTransforms[i].position;
                //tanksDetonators[i].Explode();
                bigExplosions.ShowAtPosition(tanksTransforms[i].position);
                tanksColliders[i].enabled = false;
                tanksRenderers[i].enabled = false;
            }
        }
        enemysScreen = 0;
    }
    [RPC]
    public void DestroyAllPlayers()
    {
        for (int i = 0; i < tanksScripts.Length; i++)
        {
            if (tanksScripts[i].ally == "Player" && tanksRenderers[i].enabled)
            {
                tanksExplosionsTransforms[i].position = tanksTransforms[i].position;
                //tanksDetonators[i].Explode();
                tanksColliders[i].enabled = false;
                tanksRenderers[i].enabled = false;
                if (difficulty == 2)
                {
                    playersList[i].lifes--;
                }
            }
        }
    }

    [RPC]
    public void FreezeAllPlayers()
    {
        freezePlayers = 6;
    }
    [RPC]
    public void FreezeAllEnemy()
    {
        freezeEnemies = 10;
    }

    public void UpgradeFort()
    {
        SetConcreteFort();
        Invoke("SetBricksFort", 16.0f);
        Invoke("SetConcreteFort", 17.0f);
        Invoke("SetBricksFort", 18.0f);
        Invoke("SetConcreteFort", 19.0f);
        Invoke("SetBricksFort", 20.0f);
    }
    private void SetConcreteFort()
    {
        //Убираем блоки вокруг крепости.
        ClearUpgradeFort();

        //Этот блок сверху крепости.
        SetLabObject(11, 6, LabObject.Concrete, "0011001100110011");

        //Этот блок сверху и слева от крепости.
        SetLabObject(11, 5, LabObject.Concrete, "1111111100110011");

        //Этот блок сверху и справа от крепости.
        SetLabObject(11, 7, LabObject.Concrete, "0011001111111111");

        //Этот блок внизу и слева от крепости.
        SetLabObject(12, 5, LabObject.Concrete, "1111111100000000");
        
        //Этот блок внизу и справа от крепости.
        SetLabObject(12, 7, LabObject.Concrete, "0000000011111111");
        
    }
    private void SetBricksFort()
    {
        //Убираем блоки вокруг крепости.
        ClearUpgradeFort();

        //Этот блок сверху крепости.
        SetLabObject(11, 6, LabObject.Concrete, "0011001100110011");

        //Этот блок сверху и слева от крепости.
        SetLabObject(11, 5, LabObject.Concrete, "1111111100110011");

        //Этот блок сверху и справа от крепости.
        SetLabObject(11, 7, LabObject.Concrete, "0011001111111111");

        //Этот блок внизу и слева от крепости.
        SetLabObject(12, 5, LabObject.Concrete, "1111111100000000");

        //Этот блок внизу и справа от крепости.
        SetLabObject(12, 7, LabObject.Concrete, "0000000011111111");
    }
    public void ClearUpgradeFort()
    {
        //Этот блок сверху крепости.
        DeleteLabObject(11, 6);

        //Этот блок сверху и слева от крепости.
        DeleteLabObject(11, 5);

        //Этот блок сверху и справа от крепости.
        DeleteLabObject(11, 7);

        //Этот блок внизу и слева от крепости.
        DeleteLabObject(12, 5);

        //Этот блок внизу и справа от крепости.
        DeleteLabObject(12, 7);
    }

    public void DeleteLabObject(int x, int y)
    {
        if (labTransforms[x, y] != null)
        {
            Destroy(labTransforms[x, y].gameObject);
            labTransforms[x, y] = null;
        }
        else
        {
            Debug.Log(Time.time + " : блок " + x.ToString() + ", " + y.ToString() + " не существует.");
        }
    }
    public void SetLabObject(int x, int y, LabObject type, string blocks)
    {
        if (blocks.Length != 16)
        {
            Debug.Log(Time.time + " : wrong length of parametr, must be equal 16.");
        }

        labTransforms[x, y] = (Transform)Instantiate(prefabs[(int)type], new Vector3(y * 2.0f + 3.0f, 0.0f, 28.0f - x * 2.0f), Quaternion.identity);
        labTransforms[x, y].parent = labParent;

        Bricks bricks = labTransforms[x, y].GetComponent<Bricks>();
        bricks.x = x;
        bricks.y = y;
        SetBlockState(x, y, blocks);
    }

    [RPC]
    public void SetMapPackString(string strMapPack)
    {
        currentMapPackStr = strMapPack;
    }

    [RPC]
    public void SetBlockState(int x, int y, string strPar)
    {
        Debug.Log("SetBlockState(" + x.ToString() + "," + y.ToString() + "," + strPar + ");");
        if (labTransforms[x, y] == null)
        {
            return;
        }
        Bricks bricks = labTransforms[x, y].GetComponent<Bricks>();
        for (int i = 0; i < 16; i++)
        {
            if (strPar[i] == '1')
            {
                bricks.destroyedBlocks[i] = true;
            }
            else
            {
                bricks.destroyedBlocks[i] = false;
            }
        }
    }

    public int GetLabyrinth(Vector3 pos)
    {
        int index1 = Mathf.FloorToInt(pos.x) - 2;
        int index2 = 28 - Mathf.FloorToInt(pos.z);
        if (index2 < sizeLabX && index1 < sizeLabY && (index2 >= 0 && index1 >= 0))
            return labyrinth[index2, index1];
        else
            return -1;
    }
    private void SaveLabyrinth(string filename)
    {

    }
    private void LoadLabyrinth(string filename)
    {
        ClearLabyrinth();
        //string[] strArray1 = new string[sizeLabX];
        string dataPath = Application.dataPath;
        int startIndex1 = dataPath.LastIndexOf("/");
        string[] strArray2 = File.ReadAllLines(dataPath.Remove(startIndex1) + "/Levels/" + filename + ".lab");
        for (int index = 0; index < sizeLabX; ++index)
        {
            for (int startIndex2 = 0; startIndex2 < sizeLabY; startIndex2++)
            {
                labyrinth[index, startIndex2] = int.Parse(strArray2[index].Substring(startIndex2, 1));
            }
        }
        SetLabyrinth();
    }
    [RPC]
    public void LoadLabyrinthFromString(string toRead)
    {
        ClearLabyrinth();
        labyrinth = new int[26, 26];
        labTransforms = new Transform[13, 13];
        for (int x = 0; x < 26; x++)
        {
            for (int y = 0; y < 26; y++)
            {
                labyrinth[x, y] = int.Parse(toRead.Substring(y + x * sizeLabY, 1));
            }
        }
        SetLabyrinth();
        ResetPoolTanks();
        currentLevel++;
        StartGame();
    }
    private void ClearLabyrinth()
    {
        for (int i = 1; i < 9; i++)
        {
            GameObject[] objList = GameObject.FindGameObjectsWithTag(objTags[i]);
            for (int j = 0; j < objList.Length; j++)
            {
                Destroy(objList[j]);
            }
        }
        
        for (int i = 0; i < allTanks; i++)
        {
            tanksColliders[i].enabled = false;
            tanksRenderers[i].enabled = false;
            tanksTransforms[i].GetChild(0).gameObject.SetActive(false);
            tanksTransforms[i].GetChild(1).gameObject.SetActive(false);
            //tanksTransforms[i].GetComponentInChildren<Renderer>().enabled = false;
            //tanksTransforms[i].GetComponentInChildren<Collider>().enabled = false;
        }
        
        GameObject[] objList2 = GameObject.FindGameObjectsWithTag("Spawn");
        for (int j = 0; j < objList2.Length; j++)
        {
            Destroy(objList2[j]);
        }

        if (planeTransform != null)
        {
            Destroy(planeTransform.gameObject);
        }
    }
    //Сравниваем 4 элемента в лабиринте по двум координатам
    private bool QuadraticEqual(int ix, int iy, int iResult)
    {
        if (labyrinth[ix, iy] == iResult &&
            labyrinth[ix + 1, iy] == iResult &&
            labyrinth[ix, iy + 1] == iResult &&
            labyrinth[ix + 1, iy + 1] == iResult)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void SetLabyrinth()
    {
        
        int ix = 0;
        while (ix < 26)
        {
            int iy = 0;
            while (iy < 26)
            {
                if (labyrinth[ix, iy] == 2 && 
                    labyrinth[ix + 1, iy] == 2 && 
                    labyrinth[ix, iy + 1] == 2 && 
                    labyrinth[ix + 1, iy + 1] == 1)
                {
                    playerRespawns[0] = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                }
                else if (
                    labyrinth[ix, iy] == 2 && 
                    labyrinth[ix + 1, iy] == 2 && 
                    labyrinth[ix, iy + 1] == 2 &&
                    labyrinth[ix + 1, iy + 1] == 2)
                {
                    playerRespawns[1] = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                }
                else if (
                    labyrinth[ix, iy] == 2 &&
                    labyrinth[ix + 1, iy] == 2 &&
                    labyrinth[ix, iy + 1] == 2 &&
                    labyrinth[ix + 1, iy + 1] == 3)
                {
                    playerRespawns[2] = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                }
                else if (
                    labyrinth[ix, iy] == 2 &&
                    labyrinth[ix + 1, iy] == 2 &&
                    labyrinth[ix, iy + 1] == 2 &&
                    labyrinth[ix + 1, iy + 1] == 4)
                {
                    playerRespawns[3] = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                }
                else if (
                    QuadraticEqual(ix, iy, (int)LabObject.Fortress) ||
                    QuadraticEqual(ix, iy, (int)LabObject.EnemyRespawn) ||
                    QuadraticEqual(ix, iy, (int)LabObject.PlayerRespawn) ||
                    QuadraticEqual(ix, iy, (int)LabObject.Water) ||
                    QuadraticEqual(ix, iy, (int)LabObject.Ice) ||
                    QuadraticEqual(ix, iy, (int)LabObject.Forest))
                {
                    Vector3 position = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                    //Debug.Log("labTransforms.GetRank = " + labTransforms.GetLength(0) + " , " + labTransforms.GetLength(1));
                    //Debug.Log("index1 = " + index1.ToString() + " , " + "index2 = " + index2.ToString());
                    labTransforms[ix / 2, iy / 2] = (Transform)Instantiate(prefabs[labyrinth[ix, iy]], position, Quaternion.identity);
                    if (labyrinth[ix, iy] == (int)LabObject.Fortress)
                    {
                        labTransforms[ix / 2, iy / 2].rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    }
                    if ((labyrinth[ix, iy] == (int)LabObject.PlayerRespawn ||
                         labyrinth[ix, iy] == (int)LabObject.EnemyRespawn) &&
                         gameScreen == GameScreen.Game)
                    {
                        labTransforms[ix / 2, iy / 2].renderer.enabled = false;
                        labTransforms[ix / 2, iy / 2].parent = spawnsRoot;
                    }
                }
                else
                {
                    if ((
                        labyrinth[ix, iy] == 3 || 
                        labyrinth[ix, iy] == 4) && 
                        labyrinth[ix + 1, iy] == 0 && ((
                        labyrinth[ix, iy + 1] == 3 ||
                        labyrinth[ix, iy + 1] == 4) &&
                        labyrinth[ix + 1, iy + 1] == 0))
                    {
                        Vector3 position = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                        labTransforms[ix / 2, iy / 2] = (Transform)Instantiate(prefabs[labyrinth[ix, iy]], position, Quaternion.identity);
                        Bricks bricks = labTransforms[ix / 2, iy / 2].GetComponent<Bricks>();
                        bricks.x = ix / 2;
                        bricks.y = iy / 2;
                        
                        bricks.destroyedBlocks[0] = true;
                        bricks.destroyedBlocks[1] = true;
                        bricks.destroyedBlocks[4] = true;
                        bricks.destroyedBlocks[5] = true;
                        bricks.destroyedBlocks[9] = true;
                        bricks.destroyedBlocks[13] = true;
                        bricks.destroyedBlocks[8] = true;
                        bricks.destroyedBlocks[12] = true;

                        labTransforms[ix / 2, iy / 2].parent = labParent;
                    }
                    if ((
                        labyrinth[ix, iy] == 3 ||
                        labyrinth[ix, iy] == 4) && (
                        labyrinth[ix + 1, iy] == 3 ||
                        labyrinth[ix + 1, iy] == 4) && (
                        labyrinth[ix, iy + 1] == 0 && 
                        labyrinth[ix + 1, iy + 1] == 0))
                    {
                        Vector3 position = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                        labTransforms[ix / 2, iy / 2] = (Transform)UnityEngine.Object.Instantiate((UnityEngine.Object)prefabs[labyrinth[ix, iy]], position, Quaternion.identity);
                        Bricks bricks = labTransforms[ix / 2, iy / 2].GetComponent<Bricks>();
                        bricks.x = ix / 2;
                        bricks.y = iy / 2;
                        
                        bricks.destroyedBlocks[9] = true;
                        bricks.destroyedBlocks[13] = true;
                        bricks.destroyedBlocks[8] = true;
                        bricks.destroyedBlocks[12] = true;
                        
                        bricks.destroyedBlocks[10] = true;
                        bricks.destroyedBlocks[11] = true;
                        bricks.destroyedBlocks[14] = true;
                        bricks.destroyedBlocks[15] = true;
                        
                        labTransforms[ix / 2, iy / 2].parent = labParent;
                    }
                    if (labyrinth[ix, iy] == 0 && labyrinth[ix + 1, iy] == 0 && (labyrinth[ix, iy + 1] == 3 || labyrinth[ix, iy + 1] == 4) && (labyrinth[ix + 1, iy + 1] == 3 || labyrinth[ix + 1, iy + 1] == 4))
                    {
                        Vector3 position = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                        labTransforms[ix / 2, iy / 2] = (Transform)UnityEngine.Object.Instantiate((UnityEngine.Object)prefabs[labyrinth[ix, iy + 1]], position, Quaternion.identity);
                        Bricks bricks = labTransforms[ix / 2, iy / 2].GetComponent<Bricks>();
                        bricks.x = ix / 2;
                        bricks.y = iy / 2;

                        bricks.destroyedBlocks[0] = true;
                        bricks.destroyedBlocks[1] = true;
                        bricks.destroyedBlocks[4] = true;
                        bricks.destroyedBlocks[5] = true;

                        bricks.destroyedBlocks[3] = true;
                        bricks.destroyedBlocks[7] = true;
                        bricks.destroyedBlocks[2] = true;
                        bricks.destroyedBlocks[6] = true;
                        
                        labTransforms[ix / 2, iy / 2].parent = labParent;
                    }
                    if (labyrinth[ix, iy] == 0 && (labyrinth[ix + 1, iy] == 3 || labyrinth[ix + 1, iy] == 4) && (labyrinth[ix, iy + 1] == 0 && (labyrinth[ix + 1, iy + 1] == 3 || labyrinth[ix, iy + 1] == 4)))
                    {
                        Vector3 position = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                        labTransforms[ix / 2, iy / 2] = (Transform)UnityEngine.Object.Instantiate((UnityEngine.Object)prefabs[labyrinth[ix + 1, iy]], position, Quaternion.identity);
                        Bricks bricks = labTransforms[ix / 2, iy / 2].GetComponent<Bricks>();
                        bricks.x = ix / 2;
                        bricks.y = iy / 2;

                        bricks.destroyedBlocks[3] = true;
                        bricks.destroyedBlocks[7] = true;
                        bricks.destroyedBlocks[2] = true;
                        bricks.destroyedBlocks[6] = true;

                        bricks.destroyedBlocks[10] = true;
                        bricks.destroyedBlocks[11] = true;
                        bricks.destroyedBlocks[14] = true;
                        bricks.destroyedBlocks[15] = true;
                        
                        labTransforms[ix / 2, iy / 2].parent = labParent;
                    }
                    //33 - кирпичи, 44 - бетон
                    //33            44 
                    if (labyrinth[ix, iy] == 3 && labyrinth[ix + 1, iy] == 3 && (labyrinth[ix, iy + 1] == 3 && labyrinth[ix + 1, iy + 1] == 3)
                        || labyrinth[ix, iy] == 4 && labyrinth[ix + 1, iy] == 4 && (labyrinth[ix, iy + 1] == 4 && labyrinth[ix + 1, iy + 1] == 4))
                    {
                        Vector3 position = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                        //Debug.Log("labyrinth[index1, index2] = " + labyrinth[index1, index2].ToString());
                        labTransforms[ix / 2, iy / 2] = (Transform)Instantiate(prefabs[labyrinth[ix, iy]], position, Quaternion.identity);
                        //Debug.Break();
                        Bricks bricks = labTransforms[ix / 2, iy / 2].GetComponent<Bricks>();
                        
                        bricks.x = ix / 2;
                        bricks.y = iy / 2;
                        
                        labTransforms[ix / 2, iy / 2].parent = labParent;
                    }
                    if (labyrinth[ix, iy] == 3 && labyrinth[ix + 1, iy] == 0 && (labyrinth[ix, iy + 1] == 0 && labyrinth[ix + 1, iy + 1] == 0) || labyrinth[ix, iy] == 4 && labyrinth[ix + 1, iy] == 0 && (labyrinth[ix, iy + 1] == 0 && labyrinth[ix + 1, iy + 1] == 0))
                    {
                        Vector3 position = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                        labTransforms[ix / 2, iy / 2] = (Transform)UnityEngine.Object.Instantiate((UnityEngine.Object)prefabs[labyrinth[ix, iy]], position, Quaternion.identity);
                        Bricks bricks = labTransforms[ix / 2, iy / 2].GetComponent<Bricks>();
                        bricks.x = ix / 2;
                        bricks.y = iy / 2;

                        bricks.destroyedBlocks[0] = true;
                        bricks.destroyedBlocks[1] = true;
                        bricks.destroyedBlocks[4] = true;
                        bricks.destroyedBlocks[5] = true;

                        bricks.destroyedBlocks[9] = true;
                        bricks.destroyedBlocks[13] = true;
                        bricks.destroyedBlocks[8] = true;
                        bricks.destroyedBlocks[12] = true;

                        bricks.destroyedBlocks[10] = true;
                        bricks.destroyedBlocks[11] = true;
                        bricks.destroyedBlocks[14] = true;
                        bricks.destroyedBlocks[15] = true;

                        labTransforms[ix / 2, iy / 2].parent = labParent;
                    }
                    if (labyrinth[ix, iy] == 0 && labyrinth[ix + 1, iy] == 0 && (labyrinth[ix, iy + 1] == 3 && labyrinth[ix + 1, iy + 1] == 0) || labyrinth[ix, iy] == 0 && labyrinth[ix + 1, iy] == 0 && (labyrinth[ix, iy + 1] == 4 && labyrinth[ix + 1, iy + 1] == 0))
                    {
                        Vector3 position = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                        labTransforms[ix / 2, iy / 2] = (Transform)UnityEngine.Object.Instantiate((UnityEngine.Object)prefabs[labyrinth[ix, iy + 1]], position, Quaternion.identity);
                        Bricks bricks = labTransforms[ix / 2, iy / 2].GetComponent<Bricks>();
                        bricks.x = ix / 2;
                        bricks.y = iy / 2;

                        bricks.destroyedBlocks[0] = true;
                        bricks.destroyedBlocks[1] = true;
                        bricks.destroyedBlocks[4] = true;
                        bricks.destroyedBlocks[5] = true;

                        bricks.destroyedBlocks[3] = true;
                        bricks.destroyedBlocks[7] = true;
                        bricks.destroyedBlocks[2] = true;
                        bricks.destroyedBlocks[6] = true;

                        bricks.destroyedBlocks[9] = true;
                        bricks.destroyedBlocks[13] = true;
                        bricks.destroyedBlocks[8] = true;
                        bricks.destroyedBlocks[12] = true;

                        labTransforms[ix / 2, iy / 2].parent = labParent;
                    }
                    if (labyrinth[ix, iy] == 0 &&
                        labyrinth[ix + 1, iy] == 3 &&
                        (labyrinth[ix, iy + 1] == 0 &&
                        labyrinth[ix + 1, iy + 1] == 0)
                        ||
                        labyrinth[ix, iy] == 0 &&
                        labyrinth[ix + 1, iy] == 4 &&
                        (labyrinth[ix, iy + 1] == 0 &&
                        labyrinth[ix + 1, iy + 1] == 0))
                    {
                        Vector3 position = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                        labTransforms[ix / 2, iy / 2] = (Transform)UnityEngine.Object.Instantiate((UnityEngine.Object)prefabs[labyrinth[ix + 1, iy]], position, Quaternion.identity);
                        Bricks bricks = labTransforms[ix / 2, iy / 2].GetComponent<Bricks>();
                        bricks.x = ix / 2;
                        bricks.y = iy / 2;

                        bricks.destroyedBlocks[3] = true;
                        bricks.destroyedBlocks[7] = true;
                        bricks.destroyedBlocks[2] = true;
                        bricks.destroyedBlocks[6] = true;

                        bricks.destroyedBlocks[9] = true;
                        bricks.destroyedBlocks[13] = true;
                        bricks.destroyedBlocks[8] = true;
                        bricks.destroyedBlocks[12] = true;

                        bricks.destroyedBlocks[10] = true;
                        bricks.destroyedBlocks[11] = true;
                        bricks.destroyedBlocks[14] = true;
                        bricks.destroyedBlocks[15] = true;

                        labTransforms[ix / 2, iy / 2].parent = labParent;
                    }
                    if (labyrinth[ix, iy] == 0 && labyrinth[ix + 1, iy] == 0 && (labyrinth[ix, iy + 1] == 0 && labyrinth[ix + 1, iy + 1] == 3) || labyrinth[ix, iy] == 0 && labyrinth[ix + 1, iy] == 0 && (labyrinth[ix, iy + 1] == 0 && labyrinth[ix + 1, iy + 1] == 4))
                    {
                        Vector3 position = new Vector3((float)iy + 3f, 0.0f, 28f - (float)ix);
                        labTransforms[ix / 2, iy / 2] = (Transform)UnityEngine.Object.Instantiate((UnityEngine.Object)prefabs[labyrinth[ix + 1, iy + 1]], position, Quaternion.identity);
                        Bricks bricks = labTransforms[ix / 2, iy / 2].GetComponent<Bricks>();
                        bricks.x = ix / 2;
                        bricks.y = iy / 2;

                        bricks.destroyedBlocks[0] = true;
                        bricks.destroyedBlocks[1] = true;
                        bricks.destroyedBlocks[4] = true;
                        bricks.destroyedBlocks[5] = true;

                        bricks.destroyedBlocks[3] = true;
                        bricks.destroyedBlocks[7] = true;
                        bricks.destroyedBlocks[2] = true;
                        bricks.destroyedBlocks[6] = true;

                        bricks.destroyedBlocks[10] = true;
                        bricks.destroyedBlocks[11] = true;
                        bricks.destroyedBlocks[14] = true;
                        bricks.destroyedBlocks[15] = true;

                        labTransforms[ix / 2, iy / 2].parent = labParent;
                    }
                }
                iy += 2;
            }
            ix += 2;
        }
    }

    private void LoadOptions()
    {
        if (PlayerPrefs.HasKey("hiScore"))
        {
            hiScore = PlayerPrefs.GetInt("hiScore");
        }
        if (PlayerPrefs.HasKey("cameraMode"))
        {
            hiScore = PlayerPrefs.GetInt("cameraMode");
        }
        if (PlayerPrefs.HasKey("ipNumber1"))
        {
            ipNumbers[0] = PlayerPrefs.GetInt("ipNumber1");
        }
        if (PlayerPrefs.HasKey("ipNumber2"))
        {
            ipNumbers[1] = PlayerPrefs.GetInt("ipNumber2");
        }
        if (PlayerPrefs.HasKey("ipNumber3"))
        {
            ipNumbers[2] = PlayerPrefs.GetInt("ipNumber3");
        }
        if (PlayerPrefs.HasKey("ipNumber4"))
        {
            ipNumbers[3] = PlayerPrefs.GetInt("ipNumber4");
        }
    }
    private void SaveOptions()
    {
        PlayerPrefs.SetInt("hiScore", hiScore);
        PlayerPrefs.SetInt("cameraMode", cameraMode);
        PlayerPrefs.SetInt("ipNumber1", ipNumbers[0]);
        PlayerPrefs.SetInt("ipNumber2", ipNumbers[1]);
        PlayerPrefs.SetInt("ipNumber3", ipNumbers[2]);
        PlayerPrefs.SetInt("ipNumber4", ipNumbers[3]);
        PlayerPrefs.Save();
    }
}
