using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Escena : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void Juego()
    {
        SceneManager.LoadScene("Juego");
    }

    public void Confi()
    {
        SceneManager.LoadScene("Instrucciones");
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
