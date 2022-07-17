using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelStateMachine : MonoBehaviour
{
    public static LevelStateMachine Instance;
    public int actualTurn;
    public GameObject player, enemy;
    public TextMeshProUGUI turnText, turnTextBG, dmgPlayerText, dmgEnemyText;
    public GameObject diceHolderPlayer, diceHolderEnemy;
    public DiceRoller[] dicesOnUse;
    public Sprite spriteDefaultDice;
    public SpriteRenderer[] spriteRendererDices;
    public bool isPlayerTurn = true, canFinishTurn;
    public int totalAttack;
    public GameState State = GameState.PlayerMoveTurn;

    public EntityController enemySelected;

    public enum GameState
    {
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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        actualTurn = 1;
        canFinishTurn = true;
        isPlayerTurn = true;
    }

    public void OnAttackMade()
    {
        if (isPlayerTurn)
        {
            enemySelected.DecreaseHealth(totalAttack);
            DisableAttackGUI();
            player.GetComponentInChildren<Animator>().SetTrigger("Attack");
        }
        else
        {
           
            enemy.GetComponentInChildren<Animator>().SetTrigger("Attack");
            StartCoroutine(ApplyDMGEnemy());
            
        }
    }
    public IEnumerator ApplyDMGEnemy() 
    {
        yield return new WaitForSeconds(2f);
        actualTurn++;
        player.GetComponent<EntityController>().DecreaseHealth(totalAttack / 2);
        State = GameState.ChangeEnemyTurnToPlayerTurn;
    }
    public void AddAttackToStack(int value)
    {
        totalAttack += value;
        if (isPlayerTurn)
        {
            dmgPlayerText.text = "Your Attack: " + totalAttack;
        }
        else
        {
            dmgEnemyText.text = "Enemy Attack: " + totalAttack / 2;
        }
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
            dmgPlayerText.text = "Your Attack: ";
            totalAttack = 0;
            for (int i = 0; i < spriteRendererDices.Length; i++)
            {
                if (dicesOnUse.Length != 0)
                {
                    dicesOnUse[i].ableToRoll = true;
                }

                spriteRendererDices[i].sprite = spriteDefaultDice;
            }
        }
        else
        {
            diceHolderEnemy.SetActive(false);
            spriteRendererDices = diceHolderEnemy.FindComponentsInChildrenWithTag<SpriteRenderer>("Dice");
            dmgEnemyText.text = "Enemy Attack: ";
            totalAttack = 0;
            for (int i = 0; i < spriteRendererDices.Length; i++)
            {
                if (dicesOnUse.Length != 0)
                {
                    dicesOnUse[i].ableToRoll = true;
                }

                spriteRendererDices[i].sprite = spriteDefaultDice;
            }
        }
    }

    public void FindDices()
    {
        if (isPlayerTurn)
        {
            dicesOnUse = diceHolderPlayer.FindComponentsInChildrenWithTag<DiceRoller>("Dice");
        }
        else
        {
            dicesOnUse = diceHolderEnemy.FindComponentsInChildrenWithTag<DiceRoller>("Dice");
        }
    }

    public void EnemyDiceRoll()
    {
        for (int i = 0; i < dicesOnUse.Length; i++)
        {
            dicesOnUse[i].onRollClicked();
        }

        StartCoroutine(CountEnemyDmg());
    }

    public IEnumerator CountEnemyDmg()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < dicesOnUse.Length; i++)
        {
            if (dicesOnUse[i].value != 0 && dicesOnUse[i].hasRolled)
            {
                AddAttackToStack(dicesOnUse[i].value);
                dicesOnUse[i].hasRolled = false;
            }
        }

        //Need stop doing that once done
        OnAttackMade();
    }

    public void FinishTurn()
    {
        State = GameState.PlayerMoveTurn;
        DisableAttackGUI();
        turnText.text = "TURN " + actualTurn;
        turnTextBG.text = "TURN " + actualTurn;

    }
    public void PassTurn() { 

        if (isPlayerTurn) { 
        State = GameState.EnemyMoveTurn;
        DisableAttackGUI();
        player.GetComponent<EntityController>().ResetMovePoints();
        canFinishTurn = false;
        isPlayerTurn = !isPlayerTurn;
        }
    }
    public void PlayerDiceRoll()
    {
        for (int i = 0; i < dicesOnUse.Length; i++)
        {
            if (dicesOnUse[i].value != 0 && dicesOnUse[i].hasRolled)
            {
                AddAttackToStack(dicesOnUse[i].value);
                dicesOnUse[i].hasRolled = false;
            }
        }
    }

    public void PlayerTurnMove()
    {
        canFinishTurn = true;
    }

    public void EnemyTurnMove()
    {
        enemy.GetComponent<PathfinderTest>().FindPath();
        
        //If near enough player will try to fight him, if not then moves to random place and pass turn
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
                FindDices();
                State = GameState.PlayerAttackTurn;
                break;
            case GameState.PlayerAttackTurn:
                PlayerDiceRoll();
                break;
            case GameState.ChangePlayerTurnToEnemyTurn:
                PassTurn();
                break;
            case GameState.EnemyMoveTurn:
                EnemyTurnMove();
                break;
            case GameState.ChangeEnemyMoveToAttackTurn:
                EnableAttackGUI();
                FindDices();
                State = GameState.EnemyAttackTurn;
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