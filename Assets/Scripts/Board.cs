using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    public int altoC;
    public int anchoF;
    public int margen;
    public Tile[,] board;
    public GameObject prefiles;
    public Camera cam;
    public GameObject[] prefpuntos;
    public GameObject[] prePiezas;
    public Tile InicialsTile;
    public Tile FinalizarTile;
    [Range(0f, .5f)] public float swapTime = 3f;
    public bool puedeMoverse = true;
    public GamePieza[,] gamePieza;
    public AudioSource Source;
    public AudioClip audioFX;
    public AudioClip destroyAudio;
    public Animator animacionDestruir;

    void Start()
    {
        gamePieza = new GamePieza[anchoF, altoC];

        CreateBoard();
        CameraS();
        PiezaAleatoria();
        LLenarMatrizAleatorio();
       
    }
    void CreateBoard()
    {
        board = new Tile[anchoF, altoC];

        for (int x = 0; x < anchoF; x++)  //x para ancho
        {
            for (int y = 0; y < altoC; y++)  //y para alto
            {

                GameObject go = Instantiate(prefiles);
                go.name = "Tiles" + x + "," + y;
                go.transform.position = new Vector3(x, y, 0);
                go.transform.parent = transform;

                Tile tile = go.GetComponent<Tile>();
                tile.board = this;
                board[x, y] = tile;
                tile.Iniciador(x, y);

                //board[ancho, alto]
                //board[x,y]
                    
            }
        }
    }
    void CameraS()
    {
        cam.transform.position = new Vector3(((float)anchoF / 2) - 0.5f, ((float)altoC / 2)- 0.5f, -10);

        //float spectRatio = Screen.width / Screen.height;
        float altura = (((float) altoC / 2) + margen);
        float anchura = ((((float)anchoF / 2) + margen) / Screen.width * Screen.height);

        if(altura > anchura)
        {
            cam.orthographicSize = altura;
        }
        else
        {
            cam.orthographicSize = anchura;
        }


    }

    GameObject PiezaAleatoria()
    {

        int numAleatorio = Random.Range(0, prePiezas.Length);
        GameObject go = Instantiate(prePiezas[numAleatorio]);

        go.GetComponent<GamePieza>().boar_D = this;

        return go;


    }

     public void PiezaPosition(GamePieza go, int x, int y)
    {
        go.transform.position = new Vector3(x, y, 0f);
        go.PiezaIniciada(x, y);
        gamePieza[x, y] = go;
            
    }

    void LLenarMatrizAleatorio()
    {
        List<GamePieza> addedPieces = new List<GamePieza>();


        for (int i = 0; i < anchoF; i++)
        {
            for (int j = 0; j < altoC; j++)
            {
                if(gamePieza[i,j] == null)
                {
                    GamePieza gamePieza = LlenarMAleatoria(i, j);
                    addedPieces.Add(gamePieza);
                }
              

            }
        }
        bool estaLlena = false;
        int interacciones = 0;
        int interaccionesMaximas = 100;

        while(!estaLlena)
        {
            List<GamePieza> coincidencias = EncontrarTodasLasCoincidencias();

            if(coincidencias.Count == 0)
            {
                estaLlena = true;
                break;
            }
            else
            {
                coincidencias = coincidencias.Intersect(addedPieces).ToList();
                ReemplazarConPiezaAleatoria(coincidencias);
            }

            if( interacciones > interaccionesMaximas)
            {
                estaLlena = true;
                Debug.LogWarning("Se alcanzo el numero maximo de interacciones");
            }
            interacciones++;
        }
    }

    private void ReemplazarConPiezaAleatoria(List<GamePieza> coincidencias)
    {
        foreach (GamePieza gamePieces in coincidencias)
        {
            ClearPiezas(gamePieces.coordenadaX, gamePieces.coordenadaY);
            LlenarMAleatoria(gamePieces.coordenadaX, gamePieces.coordenadaY);
        }
    }

    GamePieza LlenarMAleatoria(int x, int y)
    {
        GameObject go = PiezaAleatoria();
        PiezaPosition(go.GetComponent<GamePieza>(), x, y);
        return go.GetComponent<GamePieza>();
    }

    public void SwitchPiezas(Tile gpStart, Tile gpEnd)
    {
        StartCoroutine(SwitchTilesCorutine(gpStart, gpEnd));
       

    }

    IEnumerator SwitchTilesCorutine(Tile gpStart, Tile gpEnd)
    {
        if(puedeMoverse)
        {
            puedeMoverse = false;

            GamePieza gpInicial = gamePieza[gpStart.indiceX, gpStart.indiceY];
            GamePieza gpFinalizar = gamePieza[gpEnd.indiceX, gpEnd.indiceY];

            if (gpInicial != null && gpFinalizar != null)
            {
                gpInicial.MovePieza(gpEnd.indiceX, gpEnd.indiceY, swapTime);
                gpFinalizar.MovePieza(gpStart.indiceX, gpStart.indiceY, swapTime);

                yield return new WaitForSeconds(swapTime);


                List<GamePieza> CoinPiezaInicial = EncontrarCoincidenciasEn(gpStart.indiceX, gpStart.indiceY);
                List<GamePieza> CoinPiezaFinal = EncontrarCoincidenciasEn(gpEnd.indiceX, gpEnd.indiceY);

                if (CoinPiezaInicial.Count == 0 && CoinPiezaFinal.Count == 0)
                {
                    gpInicial.MovePieza(gpStart.indiceX, gpStart.indiceY, swapTime);
                    gpFinalizar.MovePieza(gpEnd.indiceX, gpEnd.indiceY, swapTime);
                    yield return new WaitForSeconds(swapTime);
                    puedeMoverse = true;
                }
                else
                {
                    CoinPiezaInicial = CoinPiezaInicial.Union(CoinPiezaFinal).ToList();
                    foreach (GamePieza pieza in CoinPiezaInicial)
                    {
                        pieza.GetComponentInChildren<Animator>().SetBool("Explosion", true);
                    }   
                    yield return new WaitForSeconds(0.5f);
                    AudioSource.PlayClipAtPoint(destroyAudio, gameObject.transform.position);
                    ClearAndRefillBoard(CoinPiezaInicial);
                }

            }
        }
    }

    public void InicialTile(Tile inicial)
    {
        if(InicialsTile == null)
        {

            InicialsTile = inicial;
        }
    }

    public void FinalTile(Tile final)
    {
        if (InicialsTile != null && Vecino(InicialsTile, final) == true)
        {
            FinalizarTile = final;
        }
    }
         
    public void RelaseTile()
    {
        if(InicialsTile != null && FinalizarTile != null)
        {

            SwitchPiezas(InicialsTile, FinalizarTile);

        }

        InicialsTile = null;
        FinalizarTile = null;
    }

    bool Vecino(Tile inicial_, Tile final_)
    {
        if(Mathf.Abs(inicial_.indiceX - final_.indiceX) == 1 && (inicial_.indiceY == final_.indiceY))
        {
           
            return true;
        }

        if(Mathf.Abs(inicial_.indiceY - final_.indiceY) == 1 && (inicial_.indiceX == final_.indiceX))
        {
            return true;
        }

        return false;
    }  
    bool EstaEnRango(int _x, int _y)
    {
        return (_x < anchoF && _x >= 0 && _y >= 0 && _y < altoC);
    }
     public List<GamePieza>EncontrarCoincidencias(int startX, int startY, Vector2 direncionDeBusqueda, int cantidadMinima = 3)
    {
        //Crear una lista de coincidencias encontradas
        List<GamePieza> Coincidencias = new List<GamePieza>();

        //Crear una referencia al gamepieza inicial
        GamePieza piezaInicial = null;

        if(EstaEnRango(startX,startY))
        {
            piezaInicial = gamePieza[startX, startY];
        }
        if(piezaInicial !=null)
        {
            Coincidencias.Add(piezaInicial);
        }
        else
        {
            return null;
        }
        int siguienteX;
        int siguienteY;
        int valorMaximo = anchoF > altoC ? anchoF : altoC;

        for (int a = 1;a  <valorMaximo; a++)
        {
            siguienteX = startX + (int)Mathf.Clamp(direncionDeBusqueda.x, -1, 1) * a;
            siguienteY = startY + (int)Mathf.Clamp(direncionDeBusqueda.y, -1, 1) * a;

            if(!EstaEnRango(siguienteX,siguienteY))
            {
                break;
            }

            GamePieza siguienPieza = gamePieza[siguienteX, siguienteY];
            //Comparar las piezas iniciales y final del mismo tipo

            if(siguienPieza == null)
            {
                break;
            }
            else
            {
                if (piezaInicial.tipoFicha == siguienPieza.tipoFicha && !Coincidencias.Contains(siguienPieza))
                {
                    Coincidencias.Add(siguienPieza);
                }
                else
                {
                    break;
                }
            }          
        }
        if(Coincidencias.Count >= cantidadMinima)
        {
            return Coincidencias;
        }
        return null;
    }

    List<GamePieza> BuscaVertical(int startX, int startY, int cantidadMinima = 3)
    {
        List<GamePieza> arriba = EncontrarCoincidencias(startX, startY, Vector2.up, 2);
        List<GamePieza> abajo = EncontrarCoincidencias(startX, startY, Vector2.down, 2);

        if(arriba == null)
        {
            arriba = new List<GamePieza>();
        }
        if (abajo == null)
        {
            abajo = new List<GamePieza>();
        }

        var listasCombinadas = arriba.Union(abajo).ToList();
        return listasCombinadas.Count >= cantidadMinima ? listasCombinadas : null;
    }

    List<GamePieza> BuscaHorizontal(int startX, int startY, int cantidadMinima_ = 3)
    {
        List<GamePieza> derecho = EncontrarCoincidencias(startX, startY, Vector2.right, 2);
        List<GamePieza> izquierda = EncontrarCoincidencias(startX, startY, Vector2.left, 2);

        if (derecho == null)
        {
            derecho = new List<GamePieza>();
        }
        if (izquierda == null)
        {
            izquierda = new List<GamePieza>();
        }

        var listasCombinadas = derecho.Union(izquierda).ToList();
        return listasCombinadas.Count >= cantidadMinima_ ? listasCombinadas : null;
    }

    public void ResaltarCoincidencias()
    {
        for (int r = 0; r < anchoF; r++)
        {
            for (int c = 0; c < altoC ; c++)
            {

                ResaltarCoincidenciasEn(r,c);

            }
        }
    }

    private void ResaltarCoincidenciasEn(int r, int c)
    {
        var listasCombinadas = EncontrarCoincidenciasEn(r, c);

        if(listasCombinadas.Count > 0)
        {
            foreach(GamePieza gamePieza in listasCombinadas)
            {
                ResaltarTile(gamePieza.coordenadaX, gamePieza.coordenadaY, gamePieza.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    private List<GamePieza> EncontrarCoincidenciasEn(int r_, int c_)
    {
        List<GamePieza> horiZontal = BuscaHorizontal(r_, c_, 3);
        List<GamePieza> verTical = BuscaVertical(r_, c_, 3);

        if(horiZontal == null)
        {
            horiZontal = new List<GamePieza>();
        }

        if(verTical == null)
        {
            verTical = new List<GamePieza>();
        }

        var listasCombinadas = horiZontal.Union(verTical).ToList();
        return listasCombinadas;
    }

    List<GamePieza> EncontrarCoincidenciasEn(List<GamePieza> gamePiezas, int minLength = 3)
    {
        List<GamePieza> matches = new List<GamePieza>();

        foreach (GamePieza gp in gamePiezas)
        {
            matches = matches.Union(EncontrarCoincidenciasEn(gp.coordenadaX, gp.coordenadaY)).ToList();
        }
        return matches;
    }

    private List<GamePieza> EncontrarTodasLasCoincidencias()
    {
        List<GamePieza> todasLasCoincidencias = new List<GamePieza>();

        for (int i = 0; i < anchoF; i++)
        {
            for (int j = 0; j < altoC; j++)
            {
                var coincidencias = EncontrarCoincidenciasEn(i, j);
                todasLasCoincidencias = todasLasCoincidencias.Union(coincidencias).ToList();
            }
        }
        return todasLasCoincidencias;
    }
   
    private void ResaltarTile(int _X, int _Y, Color color)
    {
        SpriteRenderer sr = board[_X, _Y].GetComponent<SpriteRenderer>();
        sr.color = color;

    }

    void ClearBoarD()
    {
        for (int i = 0; i < anchoF; i++)
        {
            for (int j = 0; j < altoC; j++)
            {
                ClearPiezas(i,j);
            }
        }
    }

    private void ClearPiezas(int x__, int Y__)
    {
        GamePieza pieceToClear = gamePieza[x__, Y__];

        if(pieceToClear != null)
        {
            gamePieza[x__, Y__] = null;
            Destroy(pieceToClear.gameObject);
            AudioSource.PlayClipAtPoint(destroyAudio, gameObject.transform.position);
        }
    }

    private void ClearPiezas(List<GamePieza> gamePiezas)

    {
        foreach(GamePieza gp in gamePiezas)
        {
            if(gp != null)
            {
                ClearPiezas(gp.coordenadaX, gp.coordenadaY);
            }
        }
            
    }

    List<GamePieza> collapseColumna(int colum, float collapseTime = 0.1f)
    {
        List<GamePieza> movingPieces = new List<GamePieza>();

        for (int i = 0; i < altoC-1; i++)
        {
            if(gamePieza[colum,i]== null)
            {
                for (int j = i +1; j < altoC; j++)
                {
                    if (gamePieza[colum, j] != null)
                    {
                        gamePieza[colum, j].MovePieza(colum, i, collapseTime * (j-i));
                        gamePieza[colum, i] = gamePieza[colum, j];
                        gamePieza[colum, j].PiezaIniciada(colum, i);

                        if (!movingPieces.Contains(gamePieza[colum, i]))
                        {
                            movingPieces.Add(gamePieza[colum, i]);
                        }
                        gamePieza[colum, j] = null;
                        break;

                    }
                }
           
            }
           
        }
        return movingPieces;
    }

    List<GamePieza> collapseColumna(List<GamePieza> gamePiezas)
    {
        List<GamePieza> movingPieces = new List<GamePieza>();
        List<int> collumsToCollape = getColums(gamePiezas);

        foreach (int colum in collumsToCollape)
        {
            movingPieces = movingPieces.Union(collapseColumna(colum)).ToList();
        }
        return movingPieces;
    }
   
    List<int> getColums(List<GamePieza> gamePiezas)
    {
        List<int> columsIndex = new List<int>();

        foreach(GamePieza gamePiece in gamePiezas)
        {
            if(!columsIndex.Contains(gamePiece.coordenadaX))
            {
                columsIndex.Add(gamePiece.coordenadaX);
            }
        }
        return columsIndex;
    }

    void ClearAndRefillBoard(List<GamePieza>gamePiezas)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(gamePiezas));
    }

    IEnumerator ClearAndRefillBoardRoutine(List<GamePieza>gamePiezas)
    {
        yield return StartCoroutine(ClearAndCollapseColum(gamePiezas));
        yield return null;
        yield return StartCoroutine(RefillRoutine());
        puedeMoverse = true;
    }

    IEnumerator ClearAndCollapseColum(List<GamePieza>gamePiezas)
    {
        List<GamePieza> movingPieces = new List<GamePieza>();
        List<GamePieza> matches = new List<GamePieza>();

        bool isFinished = false;

        while(!isFinished)
        {
            ClearPiezas(gamePiezas);
            AudioSource.PlayClipAtPoint(audioFX, gameObject.transform.position);
            yield return new WaitForSeconds(.5f);
            movingPieces = collapseColumna(gamePiezas);

            while(!isColape(gamePiezas))
            {
                yield return new WaitForEndOfFrame();
            }
           
            matches = EncontrarCoincidenciasEn(movingPieces);

            if(matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                yield return StartCoroutine(ClearAndCollapseColum(matches));
            }
        }
    }

    IEnumerator RefillRoutine()
    {
        LLenarMatrizAleatorio();
        yield return null;
    }

    bool isColape(List<GamePieza> gamePiezas)
    {
        foreach (GamePieza gp in gamePiezas)
        {
            if(gp != null)
            {
                if(gp.transform.position.y - (float)gp.coordenadaY > 0.001f)
                {
                    return false;
                }
            }
        }
        return true;
    }  
}