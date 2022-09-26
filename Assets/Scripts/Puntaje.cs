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

        if(puntos >= 100)
        {
            SceneManager.LoadScene("Win");
        }
    }

    
}