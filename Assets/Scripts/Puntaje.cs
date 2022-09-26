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
        }
        if (puntos > 400 && puntos <= 450)
        {
            SceneManager.LoadScene("Nivel3");
        }
        if (puntos > 500 && puntos <= 550)
        {
            SceneManager.LoadScene("Nivel4");
        }
        if (puntos > 600 && puntos <= 650)
        {
            SceneManager.LoadScene("Nivel5");
        }
        if (puntos > 700 && puntos <= 750)
        {
            SceneManager.LoadScene("Win");
        }
    }
 

}