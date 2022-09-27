using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Puntaje : MonoBehaviour
{
    //Variables usadas en este script
    private int puntos;

    private int multiplicador;

    private int puntajeAlmacenado;

    public int puntajeNecesario;    

    private TextMeshProUGUI TextMesh;
    public bool pasoNivel_1 = true;
    public bool pasoNivel_2;
    public bool pasoNivel_3;
    public bool pasoNivel_4;
    public bool pasoNivel_5;

    //Va a mostrar en el canva la cantidad de puntos que obtenga el jugador
    private void Start()
    {
        TextMesh = GetComponent<TextMeshProUGUI>();
    }

    //Reiniciará la puntuación cada vez que el jugador juegue cada nivel
    private void Update()
    {

        TextMesh.text = puntos.ToString("0");
    }

    //Aquí el jugador va a pasar de nivel por la puntuación que este obtenga
    public void SumatoriaPuntos(int puntosEntrada)
    {
        puntos += puntosEntrada;

        if(puntos >= puntajeNecesario)
        {
            SceneManager.LoadScene("Win");
        }
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
