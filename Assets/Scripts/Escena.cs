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

    public void Niveles()
    {
        SceneManager.LoadScene("Niveles");
    }

    public void Nivel1()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void Nivel2()
    {
        SceneManager.LoadScene("Nivel2");
    }
    
    public void Nivel3()
    {
        SceneManager.LoadScene("Nivel3");
    }

    public void Nivel4()
    {
        SceneManager.LoadScene("Nivel4");
    }

    public void Nivel5()
    {
        SceneManager.LoadScene("Nivel5");
    }

    public void GameOver()
    {
        SceneManager.LoadScene("Game Over");
    }
}
