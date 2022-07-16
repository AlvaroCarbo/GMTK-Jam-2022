using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public List<Sprite> DiceFaces = new List<Sprite> { };
    public int value;
    private void Awake()
    {
        value = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void onRollClicked() {
        this.gameObject.GetComponent<Animator>().enabled = true;
        onDiceRoll(Random.Range(1, 7));
    }

    public void onDiceRoll(int num) {
        value = num;
        StartCoroutine(RollDice());
        
    }

    public IEnumerator RollDice()
    {
        yield return new WaitForSeconds(0.5f);
        this.gameObject.GetComponent<SpriteRenderer>().sprite = DiceFaces[value - 1];
        this.gameObject.GetComponent<Animator>().enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
