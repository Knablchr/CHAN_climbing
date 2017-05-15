/**Definition of the class Entity and CPhantom
* Prefix: P_
* @author Ramon Molla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public const int X_DIM = 0;
    public const int Y_DIM = 1;
    public const int TOTAL_DIM = 2;

    public int      CurrentState;
    public int      Type;
    public float    TimeOut;
    public float[]  Position;
    public char     Direction;	//N, S, W, E Cardinal points
    public string   Name;

    void Start () {
        //Two dimensions initialization
        Position = new float[2];
    }

    public bool SamePosition(float[] Pos) {
        for (int Dim = X_DIM; Dim < TOTAL_DIM; Dim++)
            if (Position[Dim] != Pos[Dim])
                return false;

        return true;
    }
};

public class Game: MonoBehaviour {

    public enum PACMan_CHARS
    {
        CHAR_PHANTOM,
        CHAR_PACMAN,
        CHAR_MAX
    };

    public static string[] PACMan_NAMES = { "PHANTOM", "PACMAN" };

    //Global variables
    //total amount of characters in the same level: 4 pahntoms and one PAC-Man
    public static int TOTAL_CHARS = 5;
    public static int CHAR_PACMAN = 4;

    //Time out for every timeable event. Typically getting out from home and pill effect
    public static float[] TO              = { 5, 10 };
    public static float[] Initial         = { 0,  0 },
                          HomePosition    = { 0, -1 };

    //Scene Graph
    public static Entity[] Characters;

    void OnEnable(){
        Characters = new Entity[TOTAL_CHARS];
    }

    public static bool SuperPACMan() { return (int)PacMan.PACManStates.PMS_SUPERPACMAN == Game.Characters[Game.CHAR_PACMAN].CurrentState; }
    public static bool PACManDied()  { return (int)PacMan.PACManStates.PMS_DIED == Game.Characters[Game.CHAR_PACMAN].CurrentState; }
}

public class PacMan : Entity {

    //PacMan states
    public enum PACManStates
    {
        PMS_START,
        PMS_EATING,
        PMS_SUPERPACMAN,
        PMS_DIED,
        PMS_MAX_STATES
    };

    // Use this for initialization
    void Start () {
        Name = Game.PACMan_NAMES[(int) Game.PACMan_CHARS.CHAR_PACMAN];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

class CPhantom : Entity
{
    //Phantoms states
    enum PhantomStates
    {
        PS_START,
        PS_AT_HOME,
        PS_OUT_HOME,
        PS_WANDER,
        PS_CHASE,
        PS_RUN_AWAY,
        PS_GO_HOME,
        PS_MAX_STATES
    };

    enum PhantomTimeOuts
    {
        PS_AT_HOME_TO,
        PS_PILL_TO,
        PS_MAX_TO
    };

    void Start()
    {
        Direction = 'N';
        CurrentState = (int)PhantomStates.PS_AT_HOME;
        TimeOut = 0.0f;
        Name = Game.PACMan_NAMES[(int)Game.PACMan_CHARS.CHAR_PHANTOM];
    }

    void UpdatePosition() { }
    //Behaviour methods. State actions
    void Die() { }
    void EnterHome() { }
    void MoveOutHome() { }
    bool PACManNear() { return true; }
    void PACManFollow() { }
    void RunAway() { }
    void GoHome() { }
    void MoveToHome() { }

    //Virtual method to manage any collision
    void OnCollisionEnter(Collision collision)
    {
        if (Game.PACMan_NAMES[(int)Game.PACMan_CHARS.CHAR_PACMAN] == collision.collider.gameObject.name)
            CurrentState = (int) PhantomStates.PS_GO_HOME;
    }

    //Virtual method to manage the phantom behaviour
    void Update()
    {
        switch (CurrentState)
        {
            case (int) PhantomStates.PS_START:
                TimeOut = Time.time;
                CurrentState = (int)PhantomStates.PS_AT_HOME;
                MoveToHome();
                break;
            case (int)PhantomStates.PS_AT_HOME:
                if (Time.time - TimeOut >= Game.TO[(int)PhantomTimeOuts.PS_AT_HOME_TO])
                    CurrentState = (int)PhantomStates.PS_OUT_HOME;
                break;
            case (int)PhantomStates.PS_OUT_HOME:
                MoveOutHome();
                if (SamePosition(Game.Initial))
                    CurrentState = (int)PhantomStates.PS_WANDER;
                break;
            case (int)PhantomStates.PS_WANDER:
                UpdatePosition();
                if (PACManNear())
                    CurrentState = (int)PhantomStates.PS_CHASE;
                if (Game.SuperPACMan())
                    CurrentState = (int)PhantomStates.PS_RUN_AWAY;
                if (Game.PACManDied())
                    CurrentState = (int)PhantomStates.PS_START;
                break;
            case (int)PhantomStates.PS_CHASE:
                PACManFollow();
                if (Game.SuperPACMan())
                    CurrentState = (int)PhantomStates.PS_RUN_AWAY;
                if (Game.PACManDied())
                    CurrentState = (int)PhantomStates.PS_START;
                break;
            case (int)PhantomStates.PS_RUN_AWAY:
                if (Game.SuperPACMan())
                    RunAway();
                else
                    CurrentState = (int)PhantomStates.PS_WANDER;
                break;
            case (int)PhantomStates.PS_GO_HOME:
                GoHome();
                if (SamePosition(Game.HomePosition))
                    CurrentState = (int)PhantomStates.PS_START;
                break;
            default:
                break;
        };
    }

};
