using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Contador : MonoBehaviour
{
    public int minutos;
    public int segundos;
    public TMP_Text tiempo;
    public float restantes;
    public bool enMarcha;

    private void Awake()
    {
        restantes = (minutos * 60) + segundos;
    }

    private void Update()
    {
        if (enMarcha)
        {
            restantes -= Time.deltaTime;

            if(restantes < 1)
            {
                SceneManager.LoadScene("Game Over");
            }

            int tempMin = Mathf.FloorToInt(restantes / 60);
            int tempSegundos = Mathf.FloorToInt(restantes % 60);

            tiempo.text = string.Format("{00:00} : {01:00}", tempMin, tempSegundos);
        }
    }
}
