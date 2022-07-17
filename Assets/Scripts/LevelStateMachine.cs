using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStateMachine : MonoBehaviour
{
    public int actualTurn;
    public bool isPlayerTurn;
    public int totalAttack;
    public GameState State = GameState.PlayerMoveTurn;

    public enum GameState {
        PlayerMoveTurn,
        ChangePlayerMoveToAttackTurn,
        PlayerAttackTurn,
        ChangePlayerTurnToEnemyTurn,
        EnemyMoveTurn,
        ChangeEnemyMoveToAttackTurn,
        EnemyAttackTurn,
        ChangeEnemyTurnToPlayerTurn,
        LevelFinished
    }
    public void AddAttackToStack() 
    {
    
    }

    public void ResetAttackValue()
    {
        totalAttack = 0;
    }

    public void EnableAttackGUI()
    {

    }

    public void DisableAttackGUI() 
    {
    
    }

    public void EnemyDiceRoll() 
    {
    
    }
    public void FinishTurn() 
    { 
    
    }
    public void PlayerDiceRoll()
    { 
    
    }

    public void PlayerTurnMove() 
    { 
    
    }

    public void EnemyTurnMove() 
    {
    
    }
    public void FinishLevel() 
    { 
    
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (State) 
        {
            case GameState.PlayerMoveTurn:
                PlayerTurnMove();
                break;
            case GameState.ChangePlayerMoveToAttackTurn:
                EnableAttackGUI();
                break;
            case GameState.PlayerAttackTurn:
                PlayerDiceRoll();
                break;
            case GameState.ChangePlayerTurnToEnemyTurn:
                FinishTurn();
                break;
            case GameState.EnemyMoveTurn:
                EnemyTurnMove();
                break;
            case GameState.ChangeEnemyMoveToAttackTurn:
                EnableAttackGUI();
                break;
            case GameState.EnemyAttackTurn:
                EnemyDiceRoll();
                break;
            case GameState.ChangeEnemyTurnToPlayerTurn:
                FinishTurn();
                break;
            case GameState.LevelFinished:
                //Player dead or finished Level
                FinishLevel();
                break;

        }
    }
}
