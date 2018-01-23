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

    protected bool SOLVED = true;
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

    public string TwitchHelpMessage = "Use !{0} NWSE, !{0} nwse, !{0} ULDR, or !{0} uldr to move North West South East.";

    protected KMSelectable[] ProcessTwitchCommand(string TPInput)
    {
        string tpinput = TPInput.ToLowerInvariant();
        bool Incomp = false;
        List<KMSelectable> Moves = new List<KMSelectable>();
        foreach (char c in tpinput)
        {
            if (c == 'n' || c == 'u')
            {
                Moves.Add(North);
            }
            else if (c == 'e' || c == 'r')
            {
                Moves.Add(East);
            }
            else if (c == 's' || c == 'd')
            {
                Moves.Add(South);
            }
            else if (c == 'w' || c == 'l')
            {
                Moves.Add(West);
            }
            else if (c == ' ')
            {
            }
            else
            {
                Moves.Clear();
                Incomp = true;
            }
        }
        if (Incomp == false)
        {
            KMSelectable[] MovesArray = Moves.ToArray();
            return MovesArray;
        }
        else
        {
            return null;
        }
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
        //check what the serial ends with and make an integer for it
        LastDigit = BombInfo.GetSerialNumberNumbers().Last();


        //Determine Values of the Knobs and Color the knobs 1RED 2GREEN 3WHITE 4GREY 5 YELLOW
        string ColNorthName = "";
        string ColEastName = "";
        string ColSouthName = "";
        string ColWestName = "";
        if (ColNorth == 1)
        {
            NumNorth = 1;
            NorthMesh.material.color = Color.red;
            REDKEY++;
            ColNorthName = "red";
        }
        if (ColNorth == 2)
        {
            NumNorth = 5;
            NorthMesh.material.color = Color.green;
            ColNorthName = "green";
        }
        if (ColNorth == 3)
        {
            NumNorth = 2;
            NorthMesh.material.color = Color.white;
            ColNorthName = "white";
        }
        if (ColNorth == 4)
        {
            NumNorth = 2;
            NorthMesh.material.color = Color.grey;
            ColNorthName = "grey";
        }
        if (ColNorth == 5)
        {
            NumNorth = 3;
            NorthMesh.material.color = Color.yellow;
            NOYELLOW = false;
            ColNorthName = "yellow";
        }

        if (ColEast == 1)
        {
            NumEast = 3;
            EastMesh.material.color = Color.red;
            REDKEY++;
            ColEastName = "red";
        }
        if (ColEast == 2)
        {
            NumEast = 1;
            EastMesh.material.color = Color.green;
            ColEastName = "green";
        }
        if (ColEast == 3)
        {
            NumEast = 5;
            EastMesh.material.color = Color.white;
            ColEastName = "white";
        }
        if (ColEast == 4)
        {
            NumEast = 5;
            EastMesh.material.color = Color.grey;
            ColEastName = "grey";
        }
        if (ColEast == 5)
        {
            NumEast = 2;
            EastMesh.material.color = Color.yellow;
            NOYELLOW = false;
            ColEastName = "yellow";
        }

        if (ColSouth == 1)
        {
            NumSouth = 3;
            SouthMesh.material.color = Color.red;
            REDKEY++;
            ColSouthName = "red";
        }
        if (ColSouth == 2)
        {
            NumSouth = 2;
            SouthMesh.material.color = Color.green;
            ColSouthName = "green";
        }
        if (ColSouth == 3)
        {
            NumSouth = 4;
            SouthMesh.material.color = Color.white;
            ColSouthName = "white";
        }
        if (ColSouth == 4)
        {
            NumSouth = 3;
            SouthMesh.material.color = Color.grey;
            ColSouthName = "grey";
        }
        if (ColSouth == 5)
        {
            NumSouth = 2;
            SouthMesh.material.color = Color.yellow;
            NOYELLOW = false;
            ColSouthName = "yellow";
        }

        if (ColWest == 1)
        {
            NumWest = 2;
            WestMesh.material.color = Color.red;
            REDKEY++;
            ColWestName = "red";
        }
        if (ColWest == 2)
        {
            NumWest = 5;
            WestMesh.material.color = Color.green;
            ColWestName = "green";
        }
        if (ColWest == 3)
        {
            NumWest = 3;
            WestMesh.material.color = Color.white;
            ColWestName = "white";
        }
        if (ColWest == 4)
        {
            NumWest = 1;
            WestMesh.material.color = Color.grey;
            ColWestName = "grey";
        }
        if (ColWest == 5)
        {
            NumWest = 4;
            WestMesh.material.color = Color.yellow;
            NOYELLOW = false;
            ColWestName = "yellow";
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

        DebugLog("[X,Y] = [{0},{1}]", CurX + 1, CurY + 1);
    }

    protected bool HandlePressN()
    {
        KMAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        North.AddInteractionPunch(0.5f);

        if (SOLVED)
        {
            if (CurrentP.Contains("N"))
            {
                {
                    BombModule.HandlePass();
                    SOLVED = false;
                    DebugLog("The module has been defused.");
                }
            }
            else
            {
                if (CurrentP.Contains("U"))
                {
                    BombModule.HandleStrike();
                }
                else
                {
                    CurY--;
                    DebugLog("X = {0}, Y = {1}", CurX + 1, CurY + 1);
                }

            }
        }
        return false;
    }

    protected bool HandlePressE()
    {
        KMAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        East.AddInteractionPunch(0.5f);

        if (SOLVED)
        {
            if (CurrentP.Contains("E"))
            {
                BombModule.HandlePass();
                SOLVED = false;
                DebugLog("The module has been defused.");
            }
            else
            {
                if (CurrentP.Contains("R"))
                {
                    BombModule.HandleStrike();
                }
                else
                {
                    CurX++;
                    DebugLog("X = {0}, Y = {1}", CurX + 1, CurY + 1);
                }
            }
        }

        return false;
    }

    protected bool HandlePressS()
    {
        KMAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        South.AddInteractionPunch(0.5f);

        if (SOLVED)
        {
            if (CurrentP.Contains("S"))
            {
                BombModule.HandlePass();
                SOLVED = false;
                DebugLog("The module has been defused.");
            }
            else
            {
                if (CurrentP.Contains("D"))
                {
                    BombModule.HandleStrike();
                }
                else
                {
                    CurY = CurY + 1;
                    DebugLog("X = {0}, Y = {1}", CurX + 1, CurY + 1);
                }

            }
        }
        return false;
    }

    protected bool HandlePressW()
    {
        KMAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        West.AddInteractionPunch(0.5f);

        if (SOLVED)
        {
            if (CurrentP.Contains("W"))
            {
                BombModule.HandlePass();
                SOLVED = false;
                DebugLog("The module has been defused.");
            }
            else
            {
                if (CurrentP.Contains("L"))
                {
                    BombModule.HandleStrike();
                }
                else
                {
                    CurX = CurX - 1;
                    DebugLog("X = {0}, Y = {1}", CurX + 1, CurY + 1);
                }


            }
        }
        return false;
    }

    protected bool HandlePressReset()
    {
        KMAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        Reset.AddInteractionPunch(0.5f);

        if (!SOLVED)
        { }
        else
        {
            CurX = StartX;
            CurY = StartY;
        }

        return false;
    }

    private void Update()
    {
        MazeNumber = (LastDigit + GetSolvedCount()) % 10;
        if (currentMaze != GetSolvedCount() && !SOLVED)
        {
            currentMaze = GetSolvedCount();
            DebugLog("The Maze Number is now {0}", MazeNumber);
        }
        MazeCode = MazeNumber + MazeRot;
        if (MazeCode == 11)
        {
            MazeWalls = new string[5, 5] {
                 { "U D L", "U R", "N R L", "U L", "U R" },
                 { "U L", "D", "R D", "R L", "R D L" },
                 { "L", "U", "U D", "D", "U R" },
                 { "R L", "R D L", "U R L", "U D L", "R" },
                 { "D L", "U D", "R D", "U D L", "R D" }
            };
        }
        if (MazeCode == 21)
        {
            MazeWalls = new string[5, 5] {
                 { "U L", "U D", "U", "U R", "U R L" },
                 { "R L", "U L D", "R", "L", "D R" },
                 { "D L", "U D R", "L R", "L D", "U D E" },
                 { "U R L", "U R L", "L", "U D", "U R" },
                 { "D L", "D", "R D", "U D L", "R D" }
            };
        }
        if (MazeCode == 31)
        {
            MazeWalls = new string[5, 5] {
                 { "U L", "U D R", "U L", "U D", "U R" },
                 { "L", "U D R", "R D L", "R U L", "R L" },
                 { "L D", "U", "U D", "D", "R" },
                 { "U R L", "R L", "U L", "U", "D R" },
                 { "D L", "D R", "S R L", "D L", "R U D" }
            };
        }
        if (MazeCode == 41)
        {
            MazeWalls = new string[5, 5] {
                 { "U L", "U R D", "U L", "U", "U R" },
                 { "L D", "U D", "R", "D R L", "D R L" },
                 { "W U D", "U R", "R L", "U L D", "U R" },
                 { "L U", "R", "L", "U R D", "L R" },
                 { "D L R", "L D", "D", "U D", "R D" }
            };
        }
        if (MazeCode == 10)
        {
            MazeWalls = new string[5, 5] {
                 { "U L", "U", "N D R", "L U", "U R" },
                 { "L R", "D L ", "U", "D R", "L R" },
                 { "L D", "U R", "L D", "U R", "L D R" },
                 { "L U", "R", "L U R", "D L", "U R" },
                 { "D L R", "L D", "D R", "D U L", "R D" }
            };
        }
        if (MazeCode == 20)
        {
            MazeWalls = new string[5, 5] {
                 { "D U L", "U R",   "L U",   "U D", "U R" },
                 { "L U",   "D",     "D R",   "L U", "R" },
                 { "L D",   "U R D", "U L",   "R",   "L E D" },
                 { "L U R", "L U",   "D R",   "D L", "R U" },
                 { "D L",   "D R",   "D U L", "D U", "R D" }
            };
        }
        if (MazeCode == 30)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U D R", "U L",   "U R",   "L U R" },
                 { "L D ",  "U R", "R D L", "L",      "R D" },
                 { "L R U", "L D",   "U R",   "L D",   "R U" },
                 { "L R",   "L U",   "D",     "U R",   "L R" },
                 { "D L",   "D R",   "S U L", "D",     "R D" }
            };
        }
        if (MazeCode == 40)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U D", "U R D", "U L",    "U R" },
                 { "L D",   "U R",   "U L",   "D R",    "R L D" },
                 { "W U R", "L",     "D R",   "U D L",  "U R" },
                 { "L",     "D R",   "U L",   "U",      "R D" },
                 { "D L",   "D U",   "D R",   "L D",    "U R D" }
            };
        }
        if (MazeCode == 12)
        {
            MazeWalls = new string[5, 5] {
                 { "U L", "U R D", "N L", "U R D", "L U R" },
                 { "L D", "U D", "D", "U D", "R" },
                 { "L U D", "U", "U", "U", "D R" },
                 { "L U R", "R L", "R L", "L D", "U R" },
                 { "D L", "D R", "L D", "U D R", "L R D" }
            };
        }
        if (MazeCode == 22)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U R D", "L U R", "L U", "U R" },
                 { "L D",   "U D",   "R",     "L R", "L D R" },
                 { "L U",   "U D",   "R",     "L",   "U E" },
                 { "L D R", "U L",   "R",     "L R", "L D R" },
                 { "D L U",  "D R",   "L D",   "D",   "U R D" }
            };
        }
        if (MazeCode == 32)
        {
            MazeWalls = new string[5, 5] {
                 { "U L R", "U L D", "U R", "L U",   "U R" },
                 { "L D",   "U R",   "L R", "L R",   "R L D" },
                 { "L U",   "D",     "D",   "D",     "R U D" },
                 { "L",     "U D",   "U",   "U D",   "R U" },
                 { "D L R", "L D U", "S R", "L D U", "R D" }
            };
        }
        if (MazeCode == 42)
        {
            MazeWalls = new string[5, 5] {
                 { "U L D", "U",   "U R",   "U L",   "U R D" },
                 { "L U R", "R L", "L",     "D R",   "L U R" },
                 { "W D",   "R",   "L",     "U D",   "R D" },
                 { "U R L", "R L", "L",     "U D",   "R U" },
                 { "D L",   "D R", "L D R", "L U D", "R D" }
            };
        }
        if (MazeCode == 13)
        {
            MazeWalls = new string[5, 5] {
                 { "U L D", "U R", "L N D", "U", "U R" },
                 { "L U", "D", "D U", "D R", "L R" },
                 { "L", "U R", "U L", "U", "R D" },
                 { "L D R", "L R D", "R L", "L", "U R" },
                 { "D L U", "D U", " D R", "L D R", " L D R" }
            };
        }
        if (MazeCode == 23)
        {
            MazeWalls = new string[5, 5] {
                 { "U L R", "L U D", "U",   "U R", "L U R" },
                 { "L R",   "L U D", "D R", "L",   "D R" },
                 { "L D",   "U D",   "U R", "L R", "U L E" },
                 { "L U D", "U",     "R",   "L D", "R" },
                 { "D U L", "D R",   "L D", "U D", "R D" }
            };
        }
        //TO DO: SUBTLETY MENTION A (GREEN) THUMB AND A (SHIFT) KEY AND (FIVE) STRAWBERRIES ON A CHOCOLATE CAKE
        if (MazeCode == 33)
        {
            MazeWalls = new string[5, 5] {
                 { "U R L", "U R L", "L U",   "D U",   "D U R" },
                 { "L D",   "R",     "L R",   "U R L", "U R L" },
                 { "L U",   "D",     "D R",   "L D",   "R" },
                 { "L R",   "L U",   "U D",   "U",     "R D" },
                 { "D L",   "D",     "S U R", "L D",   "U R D" }
            };
        }
        if (MazeCode == 43)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "D U", "U R", "U L",   "R U D" },
                 { "L",     "R U", "L",   "D",     "R U D" },
                 { "W R D", "R L", "L D", "D U",   "R U" },
                 { "L U",   "R",   "L U", "D U R", "R L" },
                 { "D L R", "L D", "D",   "D U R", "R L D" }
            };
        }
        if (MazeCode == 14)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U",     "N D",   "U R", "L U R" },
                 { "L D R", "L R",   "L U R", "L D", "R" },
                 { "L U",   "D",     "D R",   "L U", "D R" },
                 { "L R",   "U D L", "R U",   "D L", "R U" },
                 { "D L",   "U D R",     "D L",     "U D", "R D" }
            };
        }
        if (MazeCode == 24)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U D",     "U R", "U L D", "U R" },
                 { "L R D", "U R L", "L",   "U D",   "R" },
                 { "L U",   "D R",   "L D", "U D R", "L E" },
                 { "L R",   "L U",   "U R", "U L",   "R D" },
                 { "D L",   "D R",   "D L", "D",     "U R D" }
            };
        }
        if (MazeCode == 34)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U D", "U R",   "U L D", "U R"   },
                 { "L D",   "U R", "L D",   "U R D", "L R"   },
                 { "L U",   "D R", "L U",   "U",     "R D"   },
                 { "L",     "U R", "L D R", "L R",   "U R L" },
                 { "D L R", "D L",   "S U",   "D",     "R D"   }
            };
        }
        if (MazeCode == 44)
        {
            MazeWalls = new string[5, 5] {
                 { "U L D", "U",     "R U", "L U",   "U R" },
                 { "L U",   "D R",   "L D", "R D",   "L R" },
                 { "W R",   "U D L", "R U", "L U",   "D R" },
                 { "L",     "U D",   "R",   "L D R", "L R U" },
                 { "D L",   "D U R", "L D", "U D",   "D R" }
            };
        }
        if (MazeCode == 15)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U R", "L D N", "U D",   "U R"   },
                 { "L R",   "L",   "U R D", "U L",   "D R"   },
                 { "L R",   "L",   "U D",   "",      "U R"   },
                 { "L R D", "L R", "U L",   "R D",   "L R"   },
                 { "D L U", "R D", "D L",   "D U R", "L R D" }
            };
        }
        if (MazeCode == 25)
        {
            MazeWalls = new string[5, 5] {
                 { "U R L", "L U D", "U D", "U D",   "U R"   },
                 { "L D",   "U D",   "U",   "U",     "R D"   },
                 { "L U",   "U R",   "L R", "R L D", "U L E" },
                 { "L D R", "L D",   "",    "U R",   "L R"   },
                 { "D L U", "D U",   "D R", "L D",   "R D"   }
            };
        }
        if (MazeCode == 35)
        {
            MazeWalls = new string[5, 5] {
                 { "U R L", "U D L", "U R",   "L U", "D U R" },
                 { "L R",   "L U",   "D R",   "L R", "L U R" },
                 { "L D",   "",      "U D",   "R", "L R"   },
                 { "L U",   "D R",   "D U L", "R",   "L R"   },
                 { "D L",   "D U",   "S U R", "L D", "R D"   }
            };
        }
        if (MazeCode == 45)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U R",   "U L", "U D",   "U D R" },
                 { "R L",   "D L",   "",    "U R",   "R U L" },
                 { "R W D", "U L R", "L R", "D L",   "R D" },
                 { "U L",   "D",     "D",   "D U",   "R U" },
                 { "D L",   "D U",   "D U", "D U R", "L R D" }
            };
        }
        if (MazeCode == 16)
        {
            MazeWalls = new string[5, 5] {
                 { "U L", "U", "N D", "U D", "U R"},
                 { "L R", "L D R", "L U", "U R", "L R"},
                 { "L R", "U D L", "D R", "L", "R"},
                 { "L D", "U R", "U D L", "R D", "L R"},
                 { "D L U", "D", "D R U", "D L U", "R D"}
            };
        }
        if (MazeCode == 26)
        {
            MazeWalls = new string[5, 5] {
                 { "U L R", "U L",   "U D", "U D", "U R" },
                 { "L",     "D R",   "L U R", "U D L", "R" },
                 { "L D R", "R L U", "L D", "R U", "L E" },
                 { "L U R", "L D",   "U", "D R", "L R" },
                 { "D L",   "D U",   "D", "D U", "R D" }
            };
        }
        if (MazeCode == 36)
        {
            MazeWalls = new string[5, 5] {
                 { "U L", "U D R", "U D L", "U",     "U D R" },
                 { "L R", "U R",   "U D R", "L D",   "R U" },
                 { "L",   "R",     "L U",   "U D R", "L R" },
                 { "L R", "L D",   "D R",   "R U L", "L R" },
                 { "D L", "U D",   "S U",   "D",     "R D" }
            };
        }
        if (MazeCode == 46)
        {
            MazeWalls = new string[5, 5] {
                 { "L U", "U D",   "U",     "U D",   "R U" },
                 { "L R", "U L",   "D",     "U R",   "R D L" },
                 { "W R", "D L",   "U R",   "D L R", "R U L" },
                 { "L",   "D U R", "D L R", "U L",   "R" },
                 { "L D", "D U",   "D U",   "D R",   "R D L " }
            };
        }
        if (MazeCode == 17)
        {
            MazeWalls = new string[5, 5] {
                 { "U L D", "U R",   "R N L", "U L R", "L U R" },
                 { "L U",   "R",     "R L",   "L",     "D R" },
                 { "L R",   "L D",   "R",     "L D",   "U R" },
                 { "L R",   "L R U", "L D",   "U R",   "L R" },
                 { "D L R", "L D",   "U D",   "D",     "R D" }
            };
        }
        if (MazeCode == 27)
        {
            MazeWalls = new string[5, 5] {
                 { "U L D", "D U", "D U", "U R", "L U R" },
                 { "L U",   "U R", "L U", "D",   "R D" },
                 { "L R",   "U L", "D",   "D U", "D U E" },
                 { "L",     "D R", "U L", "U",   "R U D" },
                 { "D L",   "D U", "D R", "D L", "R U D" }
            };
        }
        if (MazeCode == 37)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U",     "U D",   "U R",   "L U R" },
                 { "L R",   "D L",   "U R",   "L D R", "L R" },
                 { "L D",   "U R",   "L",     "U R",   "L R" },
                 { "L U",   "R",     "L R",   "L",     "D R" },
                 { "D L R", "L D R", "S L R", "L D",   "U R D" }
            };
        }
        if (MazeCode == 47)
        {
            MazeWalls = new string[5, 5] {
                 { "U L D", "U R", "U L", "D U", "U R" },
                 { "U L D", "D",   "D R", "U L", "R" },
                 { "U W D", "U D", "U",   "D R", "L R" },
                 { "U L",   "U",   "D R", "D U L", "D R" },
                 { "D L R", "L D", "D U", "D U", "R U D" }
            };
        }
        if (MazeCode == 18)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U D",   "N R",   "U L",   "U R D" },
                 { "L R",   "U L",   "D",     "D",     "U R" },
                 { "L R",   "L D",   "U R D", "U L D", "R" },
                 { "L D",   "U R D", "U L",   "U R",   "L R" },
                 { "D L U", "U D",   "D R",   "L D",   "R D" }
            };
        }
        if (MazeCode == 28)
        {
            MazeWalls = new string[5, 5] {
                 { "U L R", "U L",   "U D",   "U D", "U R" },
                 { "L R",   "D L R", "U L",   "U R", "L R" },
                 { "L D",   "U R",   "R D L", "L",   "D E" },
                 { "L U",   "D R",   "U R L", "L",   "U R" },
                 { "D L",   "U D",   "D",     "D R", "R L D" }
            };
        }
        if (MazeCode == 38)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U R",   "L U",   "D U",   "D U R" },
                 { "L R",   "D L",   "D R",   "L U D", "U R" },
                 { "L",     "U R D", "L U D", "U R",   "L R" },
                 { "L D",   "U",     "U",     "D R",   "L R" },
                 { "L D U", "D R",   "L S", "D U",   "R D" }
            };
        }
        if (MazeCode == 48)
        {
            MazeWalls = new string[5, 5] {
                 { "U L R", "U L", "U",     "U D",   "U R" },
                 { "D L",   "R",   "D R L", "U L",   "D R" },
                 { "U W",   "R",   "U R L", "D L",   "U R" },
                 { "R L",   "D L", "D R",   "U R L", "L R" },
                 { "D L",   "D U", "D U",   "D R",   "L R D" }
            };
        }
        if (MazeCode == 19)
        {
            MazeWalls = new string[5, 5] {
                 { "U L R", "L U D", "N R", "L U", "U R"   },
                 { "L D",   "U R",   "L R", "L R", "L R D" },
                 { "L U R", "D L",   "D",   "",    "D R"     },
                 { "L",     "U R",   "U L", "",    "U D R"     },
                 { "D L R", "D L",   "D R", "D",   "U R D"   }
            };
        }
        if (MazeCode == 29)
        {
            MazeWalls = new string[5, 5] {
                 { "U L D", "U",     "U R D",   "L U", "U R" },
                 { "L U",   "R D",   "U L",   "D R", "U R L" },
                 { "L D",   "R U",   "L",     "U D", "D E" },
                 { "L U",   "",      "",      "U D", "U R" },
                 { "D L R", "L R D", "L R D", "D L U",   "R D" }
            };
        }
        if (MazeCode == 39)
        {
            MazeWalls = new string[5, 5] {
                 { "U L D", "U R", "L U", "U R",   "L U R" },
                 { "L U D", "",    "D R", "D L",   "R" },
                 { "L U D", "",    "U",   "U R",   "L D R" },
                 { "L U R", "R L", "L R", "L D",   "U R" },
                 { "D L",   "D R", "L S", "D U R", "L R D" }
            };
        }
        if (MazeCode == 49)
        {
            MazeWalls = new string[5, 5] {
                 { "U L",   "U D R", "U L R", "U L R", "U R L" },
                 { "D L",   "U D",   "",      "",      "D R" },
                 { "U W",   "U D",   "R",     "L D",   "U R" },
                 { "D L R", "U L",   "R D",   "U L",   "D R" },
                 { "D L U", "R D",   "L D U", "D",     "D R U" }
            };
        }

        CurrentP = MazeWalls[CurY, CurX];
    }

    private void DebugLog(string log, params object[] args)
    {
        var logData = string.Format(log, args);
        Debug.LogFormat("[Blind Maze #{0}]: {1}", BlindMaze_moduleId, logData);
    }
}
