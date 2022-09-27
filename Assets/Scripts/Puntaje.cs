using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Puntaje : MonoBehaviour
{
    private int puntos;

    private int multiplicador;

    private int puntajeAlmacenado;

    private TextMeshProUGUI TextMesh;
    public bool pasoNivel_1 = true;
    public bool pasoNivel_2;
    public bool pasoNivel_3;
    public bool pasoNivel_4;
    public bool pasoNivel_5;


    private void Start()
    {
        TextMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {

        TextMesh.text = puntos.ToString("0");
    }

    public void SumatoriaPuntos(int puntosEntrada)
    {
        puntos += puntosEntrada;

        if (puntos > 300 && puntos <= 350)
        {
          SceneManager.LoadScene("Nivel2");
            pasoNivel_2 = true;

        }
        else if (puntos > 400 && puntos <= 450)
        {
          SceneManager.LoadScene("Nivel3");
          pasoNivel_3 = true;
        }
        else if (puntos > 500 && puntos <= 550)
        {
          SceneManager.LoadScene("Nivel4");
            pasoNivel_4 = true;
        }
        else if (puntos > 600 && puntos <= 650)
        {
          SceneManager.LoadScene("Nivel5");
            pasoNivel_5 = true;
        }
       /* else
        {
            SceneManager.LoadScene("Win");
        }*/
    }
 

}
/*{
    private int puntos;

    public int movimientosNecesarios;
    [SerializeField] private int goalScore;

    private TextMeshProUGUI TextMesh;

    public Contador tiempo;

    public WinCondition _winCondition;

    private void Start()
    {
        TextMesh = GetComponent<TextMeshProUGUI>();
        TextMesh.text = puntos.ToString("0");
    }

    public void SumatoriaPuntos(int puntosEntrada)
    {
        puntos += puntosEntrada;
        TextMesh.text = puntos.ToString();
    }

    public void SetCondition()
    {
        switch (_winCondition)
        {
            case WinCondition.Time:

                if (tiempo.restantes >= 1 && puntos >= goalScore)
                {
                    SceneManager.LoadScene("Nivel2");
                }
                else
                {
                    SceneManager.LoadScene("Game Over");
                }

                break;
            case WinCondition.Score:

                if (puntos >= goalScore)
                {
                    SceneManager.LoadScene("Nivel2");
                }
                else
                {
                    SceneManager.LoadScene("Game Over");
                }

                break;
            case WinCondition.mix:

                if (tiempo.restantes >= 0 && puntos >= goalScore)
                {
                    SceneManager.LoadScene("Win");
                }
                else
                {
                    SceneManager.LoadScene("Game Over");
                }

                break;
        }
    }

    public enum WinCondition
    {
        Time,
        Score,
        mix,
    }
}*/
