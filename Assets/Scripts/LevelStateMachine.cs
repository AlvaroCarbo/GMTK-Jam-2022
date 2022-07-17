using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelStateMachine : MonoBehaviour
{
    public int actualTurn;
    public GameObject player, enemy;
    public TextMeshProUGUI turnText, turnTextBG;
    public GameObject diceHolderPlayer, diceHolderEnemy;
    public Sprite spriteDefaultDice;
    public SpriteRenderer[] spriteRendererDices;
    public bool isPlayerTurn = true;
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
    private void Awake()
    {
        actualTurn = 1;
        isPlayerTurn = true;
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
        if (isPlayerTurn)
        {
            diceHolderPlayer.SetActive(true);
           
        }
        else 
        {
            diceHolderEnemy.SetActive(true);
        }
    }

   

    public void DisableAttackGUI() 
    {
        if (isPlayerTurn)
        {
            diceHolderPlayer.SetActive(false);
            spriteRendererDices = diceHolderPlayer.FindComponentsInChildrenWithTag<SpriteRenderer>("Dice");
            for (int i = 0; i < spriteRendererDices.Length; i++)
            {
                spriteRendererDices[i].sprite = spriteDefaultDice;
            }
        }
        else
        {
            diceHolderEnemy.SetActive(false);
            spriteRendererDices = diceHolderEnemy.FindComponentsInChildrenWithTag<SpriteRenderer>("Dice");
            for (int i = 0; i < spriteRendererDices.Length; i++)
            {
                spriteRendererDices[i].sprite = spriteDefaultDice;
            }
        }
    }

    public void EnemyDiceRoll() 
    {
    
    }
    public void FinishTurn() 
    {
        DisableAttackGUI();
        actualTurn++;
        turnText.text = "TURN " + actualTurn;
        turnTextBG.text = "TURN " + actualTurn;
        isPlayerTurn = !isPlayerTurn;
        State = GameState.EnemyMoveTurn;
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
