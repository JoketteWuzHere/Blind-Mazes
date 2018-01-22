using KMBombInfoExtensions;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BlindMaze : MonoBehaviour
{
    private static int BlindMaze_moduleIdCounter = 1;
    private int BlindMaze_moduleId;
    public KMBombInfo BombInfo;
    public KMBombModule BombModule;
    public KMAudio KMAudio;
    public KMSelectable North;
    public KMSelectable East;
    public KMSelectable South;
    public KMSelectable West;
    public KMSelectable Reset;
    public MeshRenderer NorthMesh;
    public MeshRenderer EastMesh;
    public MeshRenderer SouthMesh;
    public MeshRenderer WestMesh;
    protected int MazeBased = 0;
    protected int MazeRot;
    private int currentMaze = -1;

    protected bool SOLVED = false;
    protected int MazeCode;
    protected int LastDigit;
    protected string CurrentP = "";
    protected int CurX;
    protected int CurY;
    protected string[,] MazeWalls = new string[5, 5];
    protected int NumNorth;
    protected int NumEast;
    protected int NumSouth;
    protected int NumWest;
    protected int MazeNumber;

    protected int REDKEY;
    protected bool NOYELLOW = true;
    protected int StartX;
    protected int StartY;

    private string TwitchHelpMessage = "Use !{0} NWSE, !{0} nwse, !{0} ULDR, or !{0} uldr to move North West South East. Use !{0} reset, or !{0} press reset to reset the maze back to starting position.";

    

    protected KMSelectable[] ProcessTwitchCommand(string TPInput)
    {
        TPInput = TPInput.ToLowerInvariant();
        if (TPInput.Equals("reset") || TPInput.Equals("press reset"))
        {
            return new KMSelectable[1] {Reset};
        }

        if (TPInput.StartsWith("move ") || TPInput.StartsWith("press ") || TPInput.StartsWith("walk ") || TPInput.StartsWith("submit "))
        {
            TPInput = TPInput.Substring(TPInput.IndexOf(" ", StringComparison.Ordinal));
        }

        List<KMSelectable> Moves = new List<KMSelectable>();
        foreach (char c in TPInput)
        {
            switch (c)
            {
                case 'n':
                case 'u':
                    Moves.Add(North);
                    break;
                case 'e':
                case 'r':
                    Moves.Add(East);
                    break;
                case 's':
                case 'd':
                    Moves.Add(South);
                    break;
                case 'w':
                case 'l':
                    Moves.Add(West);
                    break;
                case ' ':
                    break;
                default:
                    return null;
            }
        }
        return Moves.Count > 0 
            ? Moves.ToArray() 
            : null;
    }

    int GetSolvedCount()
    {
        return BombInfo.GetSolvedModuleNames().Count;
    }

    protected void Start()
    {
        BlindMaze_moduleId = BlindMaze_moduleIdCounter++;
        int ColNorth = UnityEngine.Random.Range(1, 6);
        int ColEast = UnityEngine.Random.Range(1, 6);
        int ColSouth = UnityEngine.Random.Range(1, 6);
        int ColWest = UnityEngine.Random.Range(1, 6);

        North.OnInteract += HandlePressN;
        East.OnInteract += HandlePressE;
        South.OnInteract += HandlePressS;
        West.OnInteract += HandlePressW;
        Reset.OnInteract += HandlePressReset;
        North.OnInteractEnded += HandleRelease;
        East.OnInteractEnded += HandleRelease;
        South.OnInteractEnded += HandleRelease;
        West.OnInteractEnded += HandleRelease;
        Reset.OnInteractEnded += HandleRelease;
        //check what the serial ends with and make an integer for it
        LastDigit = BombInfo.GetSerialNumberNumbers().Last();


        //Determine Values of the Knobs and Color the knobs 1RED 2GREEN 3WHITE 4GREY 5 YELLOW
        string ColNorthName = "";
        string ColEastName = "";
        string ColSouthName = "";
        string ColWestName = "";
        switch (ColNorth)
        {
            case 1:
                NumNorth = 1;
                NorthMesh.material.color = Color.red;
                REDKEY++;
                ColNorthName = "red";
                break;
            case 2:
                NumNorth = 5;
                NorthMesh.material.color = Color.green;
                ColNorthName = "green";
                break;
            case 3:
                NumNorth = 2;
                NorthMesh.material.color = Color.white;
                ColNorthName = "white";
                break;
            case 4:
                NumNorth = 2;
                NorthMesh.material.color = Color.grey;
                ColNorthName = "grey";
                break;
            case 5:
                NumNorth = 3;
                NorthMesh.material.color = Color.yellow;
                NOYELLOW = false;
                ColNorthName = "yellow";
                break;
        }

        switch (ColEast)
        {
            case 1:
                NumEast = 3;
                EastMesh.material.color = Color.red;
                REDKEY++;
                ColEastName = "red";
                break;
            case 2:
                NumEast = 1;
                EastMesh.material.color = Color.green;
                ColEastName = "green";
                break;
            case 3:
                NumEast = 5;
                EastMesh.material.color = Color.white;
                ColEastName = "white";
                break;
            case 4:
                NumEast = 5;
                EastMesh.material.color = Color.grey;
                ColEastName = "grey";
                break;
            case 5:
                NumEast = 2;
                EastMesh.material.color = Color.yellow;
                NOYELLOW = false;
                ColEastName = "yellow";
                break;
        }

        switch (ColSouth)
        {
            case 1:
                NumSouth = 3;
                SouthMesh.material.color = Color.red;
                REDKEY++;
                ColSouthName = "red";
                break;
            case 2:
                NumSouth = 2;
                SouthMesh.material.color = Color.green;
                ColSouthName = "green";
                break;
            case 3:
                NumSouth = 4;
                SouthMesh.material.color = Color.white;
                ColSouthName = "white";
                break;
            case 4:
                NumSouth = 3;
                SouthMesh.material.color = Color.grey;
                ColSouthName = "grey";
                break;
            case 5:
                NumSouth = 2;
                SouthMesh.material.color = Color.yellow;
                NOYELLOW = false;
                ColSouthName = "yellow";
                break;
        }

        switch (ColWest)
        {
            case 1:
                NumWest = 2;
                WestMesh.material.color = Color.red;
                REDKEY++;
                ColWestName = "red";
                break;
            case 2:
                NumWest = 5;
                WestMesh.material.color = Color.green;
                ColWestName = "green";
                break;
            case 3:
                NumWest = 3;
                WestMesh.material.color = Color.white;
                ColWestName = "white";
                break;
            case 4:
                NumWest = 1;
                WestMesh.material.color = Color.grey;
                ColWestName = "grey";
                break;
            case 5:
                NumWest = 4;
                WestMesh.material.color = Color.yellow;
                NOYELLOW = false;
                ColWestName = "yellow";
                break;
        }

        //Look for mazebased modules
        if (BombInfo.GetModuleNames().Contains("Mouse In The Maze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("3D Maze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("Hexamaze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("Maze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("Morse-A-Maze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("Blind Maze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("Polyhedral Maze"))
        { MazeBased++; }


        //determine rotation
        int MazeRule;
        if (BombInfo.GetBatteryCount() == 1 && BombInfo.GetBatteryHolderCount() == 0)
        {
            MazeRot = 20;
            MazeRule = 1;
        }
        else if (BombInfo.GetPorts().Distinct().Count() < 3)
        {
            MazeRot = 40;
            MazeRule = 2;
        }
        else if (BombInfo.GetSerialNumberLetters().Any("AEIOU".Contains) && BombInfo.GetOnIndicators().Contains("IND"))
        {
            MazeRot = 30;
            MazeRule = 3;
        }
        else if (REDKEY > 0 && NOYELLOW == true)
        {
            MazeRot = 40;
            MazeRule = 4;
        }
        else if (MazeBased > 2)
        {
            MazeRot = 30;
            MazeRule = 5;
        }
        else if (BombInfo.GetOffIndicators().Contains("MSA") && REDKEY > 1)
        {
            MazeRot = 20;
            MazeRule = 6;
        }
        else
        {
            MazeRot = 10;
            MazeRule = 7;
        }

        //Determine Current Position
        CurX = (NumSouth + NumNorth + 4) % 5;
        CurY = (NumEast + NumWest + 4) % 5;
        StartX = CurX;
        StartY = CurY;
        DebugLog("Maze Rotation is {0} degrees clockwise because of rule {1}", MazeRot * 9 - 90, MazeRule);
        DebugLog("North Key is {0}, making it's value {1}", ColNorthName, NumNorth);
        DebugLog("East Key is {0}, making it's value {1}", ColEastName, NumEast);
        DebugLog("South Key is {0}, making it's value {1}", ColSouthName, NumSouth);
        DebugLog("West Key is {0}, making it's value {1}", ColWestName, NumWest);
        if (NumSouth + NumNorth < 6)
        {
            DebugLog("North({0}) + South({1}) = {2}", NumNorth, NumSouth, CurX + 1);
        }
        else
        {
            DebugLog("North({0}) + South({1}) - 5 = {2}", NumNorth, NumSouth, CurX + 1);
        }

        if (NumEast + NumWest < 6)
        {
            DebugLog("East({0}) + West({1}) = {2}", NumEast, NumWest, CurY + 1);
        }
        else
        {
            DebugLog("East({0}) + West({1}) - 5 = {2}", NumEast, NumWest, CurY + 1);
        }

        DebugLog("Starting Location: [X,Y] = [{0},{1}]", CurX + 1, CurY + 1);
    }

    protected void HandleRelease()
    {
        KMAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);
    }

    protected bool ExitedMaze(string direction)
    {
        if (!CurrentP.Contains(direction.Substring(0,1))) return false;

        BombModule.HandlePass();
        SOLVED = true;
        DebugLog("Moving {0}: The module has been defused.", direction);
        return true;
    }

    protected bool HandlePressN()
    {
        KMAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        North.AddInteractionPunch(0.5f);
        if (SOLVED || ExitedMaze("North")) return false;

        if (CurrentP.Contains("U"))
        {
            DebugLog("There is a wall to the North at X = {0}, Y = {1}. Strike", CurX + 1, CurY + 1);
            BombModule.HandleStrike();
        }
        else
        {
            CurY--;
            CurrentP = MazeWalls[CurY, CurX];
            DebugLog("Moving North: X = {0}, Y = {1}", CurX + 1, CurY + 1);
        }
        return false;
    }

    protected bool HandlePressE()
    {
        KMAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        East.AddInteractionPunch(0.5f);
        if (SOLVED || ExitedMaze("East")) return false;

        if (CurrentP.Contains("R"))
        {
            DebugLog("There is a wall to the East at X = {0}, Y = {1}. Strike", CurX + 1, CurY + 1);
            BombModule.HandleStrike();
        }
        else
        {
            CurX++;
            CurrentP = MazeWalls[CurY, CurX];
            DebugLog("Moving East: X = {0}, Y = {1}", CurX + 1, CurY + 1);
        }

        return false;
    }

    protected bool HandlePressS()
    {
        KMAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        South.AddInteractionPunch(0.5f);
        if (SOLVED || ExitedMaze("South")) return false;

        if (CurrentP.Contains("D"))
        {
            DebugLog("There is a wall to the South at X = {0}, Y = {1}. Strike", CurX + 1, CurY + 1);
            BombModule.HandleStrike();
        }
        else
        {
            CurY = CurY + 1;
            CurrentP = MazeWalls[CurY, CurX];
            DebugLog("Moving South: X = {0}, Y = {1}", CurX + 1, CurY + 1);
        }
        return false;
    }

    protected bool HandlePressW()
    {
        KMAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        West.AddInteractionPunch(0.5f);
        if (SOLVED || ExitedMaze("West")) return false;

        if (CurrentP.Contains("L"))
        {
            DebugLog("There is a wall to the West at X = {0}, Y = {1}. Strike", CurX + 1, CurY + 1);
            BombModule.HandleStrike();
        }
        else
        {
            CurX = CurX - 1;
            CurrentP = MazeWalls[CurY, CurX];
            DebugLog("Moving West: X = {0}, Y = {1}", CurX + 1, CurY + 1);
        }
        return false;
    }

    protected bool HandlePressReset()
    {
        KMAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        Reset.AddInteractionPunch(0.5f);
        if (SOLVED) return false;
            
        CurX = StartX;
        CurY = StartY;
        DebugLog("Current location reset back to Starting location: X = {0}, Y = {1}", CurX + 1, CurY + 1);
        return false;
    }

    private void Update()
    {
        if (SOLVED || currentMaze == GetSolvedCount()) return;
        currentMaze = GetSolvedCount();

        MazeNumber = (LastDigit + GetSolvedCount()) % 10;
        DebugLog("The Maze Number is now {0}", MazeNumber);
        MazeCode = MazeNumber + MazeRot;

        switch (MazeCode)
        {
            case 11:
                MazeWalls = new string[5, 5] {
                    { "U D L", "U R", "N R L", "U L", "U R" },
                    { "U L", "D", "R D", "R L", "R D L" },
                    { "L", "U", "U D", "D", "U R" },
                    { "R L", "R D L", "U R L", "U D L", "R" },
                    { "D L", "U D", "R D", "U D L", "R D" }
                };
                break;
            case 21:
                MazeWalls = new string[5, 5] {
                    { "U L", "U D", "U", "U R", "U R L" },
                    { "R L", "U L D", "R", "L", "D R" },
                    { "D L", "U D R", "L R", "L D", "U D E" },
                    { "U R L", "U R L", "L", "U D", "U R" },
                    { "D L", "D", "R D", "U D L", "R D" }
                };
                break;
            case 31:
                MazeWalls = new string[5, 5] {
                    { "U L", "U D R", "U L", "U D", "U R" },
                    { "L", "U D R", "R D L", "R U L", "R L" },
                    { "L D", "U", "U D", "D", "R" },
                    { "U R L", "R L", "U L", "U", "D R" },
                    { "D L", "D R", "S R L", "D L", "R U D" }
                };
                break;
            case 41:
                MazeWalls = new string[5, 5] {
                    { "U L", "U R D", "U L", "U", "U R" },
                    { "L D", "U D", "R", "D R L", "D R L" },
                    { "W U D", "U R", "R L", "U L D", "U R" },
                    { "L U", "R", "L", "U R D", "L R" },
                    { "D L R", "L D", "D", "U D", "R D" }
                };
                break;
            case 10:
                MazeWalls = new string[5, 5] {
                    { "U L", "U", "N D R", "L U", "U R" },
                    { "L R", "D L ", "U", "D R", "L R" },
                    { "L D", "U R", "L D", "U R", "L D R" },
                    { "L U", "R", "L U R", "D L", "U R" },
                    { "D L R", "L D", "D R", "D U L", "R D" }
                };
                break;
            case 20:
                MazeWalls = new string[5, 5] {
                    { "D U L", "U R",   "L U",   "U D", "U R" },
                    { "L U",   "D",     "D R",   "L U", "R" },
                    { "L D",   "U R D", "U L",   "R",   "L E D" },
                    { "L U R", "L U",   "D R",   "D L", "R U" },
                    { "D L",   "D R",   "D U L", "D U", "R D" }
                };
                break;
            case 30:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U D R", "U L",   "U R",   "L U R" },
                    { "L D ",  "U R", "R D L", "L",      "R D" },
                    { "L R U", "L D",   "U R",   "L D",   "R U" },
                    { "L R",   "L U",   "D",     "U R",   "L R" },
                    { "D L",   "D R",   "S U L", "D",     "R D" }
                };
                break;
            case 40:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U D", "U R D", "U L",    "U R" },
                    { "L D",   "U R",   "U L",   "D R",    "R L D" },
                    { "W U R", "L",     "D R",   "U D L",  "U R" },
                    { "L",     "D R",   "U L",   "U",      "R D" },
                    { "D L",   "D U",   "D R",   "L D",    "U R D" }
                };
                break;
            case 12:
                MazeWalls = new string[5, 5] {
                    { "U L", "U R D", "N L", "U R D", "L U R" },
                    { "L D", "U D", "D", "U D", "R" },
                    { "L U D", "U", "U", "U", "D R" },
                    { "L U R", "R L", "R L", "L D", "U R" },
                    { "D L", "D R", "L D", "U D R", "L R D" }
                };
                break;
            case 22:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U R D", "L U R", "L U", "U R" },
                    { "L D",   "U D",   "R",     "L R", "L D R" },
                    { "L U",   "U D",   "R",     "L",   "U E" },
                    { "L D R", "U L",   "R",     "L R", "L D R" },
                    { "D L U",  "D R",   "L D",   "D",   "U R D" }
                };
                break;
            case 32:
                MazeWalls = new string[5, 5] {
                    { "U L R", "U L D", "U R", "L U",   "U R" },
                    { "L D",   "U R",   "L R", "L R",   "R L D" },
                    { "L U",   "D",     "D",   "D",     "R U D" },
                    { "L",     "U D",   "U",   "U D",   "R U" },
                    { "D L R", "L D U", "S R", "L D U", "R D" }
                };
                break;
            case 42:
                MazeWalls = new string[5, 5] {
                    { "U L D", "U",   "U R",   "U L",   "U R D" },
                    { "L U R", "R L", "L",     "D R",   "L U R" },
                    { "W D",   "R",   "L",     "U D",   "R D" },
                    { "U R L", "R L", "L",     "U D",   "R U" },
                    { "D L",   "D R", "L D R", "L U D", "R D" }
                };
                break;
            case 13:
                MazeWalls = new string[5, 5] {
                    { "U L D", "U R", "L N D", "U", "U R" },
                    { "L U", "D", "D U", "D R", "L R" },
                    { "L", "U R", "U L", "U", "R D" },
                    { "L D R", "L R D", "R L", "L", "U R" },
                    { "D L U", "D U", " D R", "L D R", " L D R" }
                };
                break;
            case 23:
                MazeWalls = new string[5, 5] {
                    { "U L R", "L U D", "U",   "U R", "L U R" },
                    { "L R",   "L U D", "D R", "L",   "D R" },
                    { "L D",   "U D",   "U R", "L R", "U L E" },
                    { "L U D", "U",     "R",   "L D", "R" },
                    { "D U L", "D R",   "L D", "U D", "R D" }
                };
                break;
            //TO DO: SUBTLETY ADD IN SOME RANDOM JOKE ABOUT YOUTUBE, JOKETTEWUZHERE, STATIC, AND REALITY.
            //TO DO: FIGURE OUT WHY JOKETTE PUT THAT HERE.
            case 33:
                MazeWalls = new string[5, 5] {
                    { "U R L", "U R L", "L U",   "D U",   "D U R" },
                    { "L D",   "R",     "L R",   "U R L", "U R L" },
                    { "L U",   "D",     "D R",   "L D",   "R" },
                    { "L R",   "L U",   "U D",   "U",     "R D" },
                    { "D L",   "D",     "S U R", "L D",   "U R D" }
                };
                break;
            case 43:
                MazeWalls = new string[5, 5] {
                    { "U L",   "D U", "U R", "U L",   "R U D" },
                    { "L",     "R U", "L",   "D",     "R U D" },
                    { "W R D", "R L", "L D", "D U",   "R U" },
                    { "L U",   "R",   "L U", "D U R", "R L" },
                    { "D L R", "L D", "D",   "D U R", "R L D" }
                };
                break;
            case 14:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U",     "N D",   "U R", "L U R" },
                    { "L D R", "L R",   "L U R", "L D", "R" },
                    { "L U",   "D",     "D R",   "L U", "D R" },
                    { "L R",   "U D L", "R U",   "D L", "R U" },
                    { "D L",   "U D R",     "D L",     "U D", "R D" }
                };
                break;
            case 24:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U D",     "U R", "U L D", "U R" },
                    { "L R D", "U R L", "L",   "U D",   "R" },
                    { "L U",   "D R",   "L D", "U D R", "L E" },
                    { "L R",   "L U",   "U R", "U L",   "R D" },
                    { "D L",   "D R",   "D L", "D",     "U R D" }
                };
                break;
            case 34:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U D", "U R",   "U L D", "U R"   },
                    { "L D",   "U R", "L D",   "U R D", "L R"   },
                    { "L U",   "D R", "L U",   "U",     "R D"   },
                    { "L",     "U R", "L D R", "L R",   "U R L" },
                    { "D L R", "D L",   "S U",   "D",     "R D"   }
                };
                break;
            case 44:
                MazeWalls = new string[5, 5] {
                    { "U L D", "U",     "R U", "L U",   "U R" },
                    { "L U",   "D R",   "L D", "R D",   "L R" },
                    { "W R",   "U D L", "R U", "L U",   "D R" },
                    { "L",     "U D",   "R",   "L D R", "L R U" },
                    { "D L",   "D U R", "L D", "U D",   "D R" }
                };
                break;
            case 15:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U R", "L D N", "U D",   "U R"   },
                    { "L R",   "L",   "U R D", "U L",   "D R"   },
                    { "L R",   "L",   "U D",   "",      "U R"   },
                    { "L R D", "L R", "U L",   "R D",   "L R"   },
                    { "D L U", "R D", "D L",   "D U R", "L R D" }
                };
                break;
            case 25:
                MazeWalls = new string[5, 5] {
                    { "U R L", "L U D", "U D", "U D",   "U R"   },
                    { "L D",   "U D",   "U",   "U",     "R D"   },
                    { "L U",   "U R",   "L R", "R L D", "U L E" },
                    { "L D R", "L D",   "",    "U R",   "L R"   },
                    { "D L U", "D U",   "D R", "L D",   "R D"   }
                };
                break;
            case 35:
                MazeWalls = new string[5, 5] {
                    { "U R L", "U D L", "U R",   "L U", "D U R" },
                    { "L R",   "L U",   "D R",   "L R", "L U R" },
                    { "L D",   "",      "U D",   "R", "L R"   },
                    { "L U",   "D R",   "D U L", "R",   "L R"   },
                    { "D L",   "D U",   "S U R", "L D", "R D"   }
                };
                break;
            case 45:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U R",   "U L", "U D",   "U D R" },
                    { "R L",   "D L",   "",    "U R",   "R U L" },
                    { "R W D", "U L R", "L R", "D L",   "R D" },
                    { "U L",   "D",     "D",   "D U",   "R U" },
                    { "D L",   "D U",   "D U", "D U R", "L R D" }
                };
                break;
            case 16:
                MazeWalls = new string[5, 5] {
                    { "U L", "U", "N D", "U D", "U R"},
                    { "L R", "L D R", "L U", "U R", "L R"},
                    { "L R", "U D L", "D R", "L", "R"},
                    { "L D", "U R", "U D L", "R D", "L R"},
                    { "D L U", "D", "D R U", "D L U", "R D"}
                };
                break;
            case 26:
                MazeWalls = new string[5, 5] {
                    { "U L R", "U L",   "U D", "U D", "U R" },
                    { "L",     "D R",   "L U R", "U D L", "R" },
                    { "L D R", "R L U", "L D", "R U", "L E" },
                    { "L U R", "L D",   "U", "D R", "L R" },
                    { "D L",   "D U",   "D", "D U", "R D" }
                };
                break;
            case 36:
                MazeWalls = new string[5, 5] {
                    { "U L", "U D R", "U D L", "U",     "U D R" },
                    { "L R", "U R",   "U D R", "L D",   "R U" },
                    { "L",   "R",     "L U",   "U D R", "L R" },
                    { "L R", "L D",   "D R",   "R U L", "L R" },
                    { "D L", "U D",   "S U",   "D",     "R D" }
                };
                break;
            case 46:
                MazeWalls = new string[5, 5] {
                    { "L U", "U D",   "U",     "U D",   "R U" },
                    { "L R", "U L",   "D",     "U R",   "R D L" },
                    { "W R", "D L",   "U R",   "D L R", "R U L" },
                    { "L",   "D U R", "D L R", "U L",   "R" },
                    { "L D", "D U",   "D U",   "D R",   "R D L " }
                };
                break;
            case 17:
                MazeWalls = new string[5, 5] {
                    { "U L D", "U R",   "R N L", "U L R", "L U R" },
                    { "L U",   "R",     "R L",   "L",     "D R" },
                    { "L R",   "L D",   "R",     "L D",   "U R" },
                    { "L R",   "L R U", "L D",   "U R",   "L R" },
                    { "D L R", "L D",   "U D",   "D",     "R D" }
                };
                break;
            case 27:
                MazeWalls = new string[5, 5] {
                    { "U L D", "D U", "D U", "U R", "L U R" },
                    { "L U",   "U R", "L U", "D",   "R D" },
                    { "L R",   "U L", "D",   "D U", "D U E" },
                    { "L",     "D R", "U L", "U",   "R U D" },
                    { "D L",   "D U", "D R", "D L", "R U D" }
                };
                break;
            case 37:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U",     "U D",   "U R",   "L U R" },
                    { "L R",   "D L",   "U R",   "L D R", "L R" },
                    { "L D",   "U R",   "L",     "U R",   "L R" },
                    { "L U",   "R",     "L R",   "L",     "D R" },
                    { "D L R", "L D R", "S L R", "L D",   "U R D" }
                };
                break;
            case 47:
                MazeWalls = new string[5, 5] {
                    { "U L D", "U R", "U L", "D U", "U R" },
                    { "U L D", "D",   "D R", "U L", "R" },
                    { "U W D", "U D", "U",   "D R", "L R" },
                    { "U L",   "U",   "D R", "D U L", "D R" },
                    { "D L R", "L D", "D U", "D U", "R U D" }
                };
                break;
            case 18:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U D",   "N R",   "U L",   "U R D" },
                    { "L R",   "U L",   "D",     "D",     "U R" },
                    { "L R",   "L D",   "U R D", "U L D", "R" },
                    { "L D",   "U R D", "U L",   "U R",   "L R" },
                    { "D L U", "U D",   "D R",   "L D",   "R D" }
                };
                break;
            case 28:
                MazeWalls = new string[5, 5] {
                    { "U L R", "U L",   "U D",   "U D", "U R" },
                    { "L R",   "D L R", "U L",   "U R", "L R" },
                    { "L D",   "U R",   "R D L", "L",   "D E" },
                    { "L U",   "D R",   "U R L", "L",   "U R" },
                    { "D L",   "U D",   "D",     "D R", "R L D" }
                };
                break;
            case 38:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U R",   "L U",   "D U",   "D U R" },
                    { "L R",   "D L",   "D R",   "L U D", "U R" },
                    { "L",     "U R D", "L U D", "U R",   "L R" },
                    { "L D",   "U",     "U",     "D R",   "L R" },
                    { "L D U", "D R",   "L S", "D U",   "R D" }
                };
                break;
            case 48:
                MazeWalls = new string[5, 5] {
                    { "U L R", "U L", "U",     "U D",   "U R" },
                    { "D L",   "R",   "D R L", "U L",   "D R" },
                    { "U W",   "R",   "U R L", "D L",   "U R" },
                    { "R L",   "D L", "D R",   "U R L", "L R" },
                    { "D L",   "D U", "D U",   "D R",   "L R D" }
                };
                break;
            case 19:
                MazeWalls = new string[5, 5] {
                    { "U L R", "L U D", "N R", "L U", "U R"   },
                    { "L D",   "U R",   "L R", "L R", "L R D" },
                    { "L U R", "D L",   "D",   "",    "D R"     },
                    { "L",     "U R",   "U L", "",    "U D R"     },
                    { "D L R", "D L",   "D R", "D",   "U R D"   }
                };
                break;
            case 29:
                MazeWalls = new string[5, 5] {
                    { "U L D", "U",     "U R D",   "L U", "U R" },
                    { "L U",   "R D",   "U L",   "D R", "U R L" },
                    { "L D",   "R U",   "L",     "U D", "D E" },
                    { "L U",   "",      "",      "U D", "U R" },
                    { "D L R", "L R D", "L R D", "D L U",   "R D" }
                };
                break;
            case 39:
                MazeWalls = new string[5, 5] {
                    { "U L D", "U R", "L U", "U R",   "L U R" },
                    { "L U D", "",    "D R", "D L",   "R" },
                    { "L U D", "",    "U",   "U R",   "L D R" },
                    { "L U R", "R L", "L R", "L D",   "U R" },
                    { "D L",   "D R", "L S", "D U R", "L R D" }
                };
                break;
            case 49:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U D R", "U L R", "U L R", "U R L" },
                    { "D L",   "U D",   "",      "",      "D R" },
                    { "U W",   "U D",   "R",     "L D",   "U R" },
                    { "D L R", "U L",   "R D",   "U L",   "D R" },
                    { "D L U", "R D",   "L D U", "D",     "D R U" }
                };
                break;
        }

        CurrentP = MazeWalls[CurY, CurX];
    }

    private void DebugLog(string log, params object[] args)
    {
        var logData = string.Format(log, args);
        Debug.LogFormat("[Blind Maze #{0}]: {1}", BlindMaze_moduleId, logData);
    }
}
