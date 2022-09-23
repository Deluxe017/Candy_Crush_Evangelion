using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        //puntos += Time.deltaTime;
        TextMesh.text = puntos.ToString("0");
    }

    public void SumatoriaPuntos(int puntosEntrada)
    {
        puntos += puntosEntrada;
    }
}