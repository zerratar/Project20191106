using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text lblMoney;
    [SerializeField] private TMP_Text lblShoes;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetShoes(long amount)
    {
        lblShoes.text = amount.ToString();
    }

    public void SetMoney(long amount)
    {
        lblMoney.text = amount.ToString();
    }
}
