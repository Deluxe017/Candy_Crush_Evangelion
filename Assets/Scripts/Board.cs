using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using TMPro;

public class Board : MonoBehaviour
{
    //Variables usadas

    public int alto;
    public int ancho;

    public int borderSize;

    public GameObject tilePrefab;
    public GameObject[] gamePiecesPrefabs;

    public float swapTime = .3f;

    Escena escena_;

    public Puntaje m_puntaje;

    Tile[,] m_allTiles;
    GamePiece[,] m_allGamePieces;

    [SerializeField] Tile piezaInicial;
    [SerializeField] Tile piezaFinal;

    bool m_playerInputEnabled = true;

    public int cantidadMovimientos;
    public TextMeshProUGUI moves;

    Transform tileParent;
    Transform gamePieceParent;

    int myCount = 0;

    public int movimientoMaximo;

    public AudioClip musica;
    public AudioClip audioFX;

    //Se calcula el ancho y el alto del board, la cámara se acomoda automáticamente con la resolución de aspecto del juego
    private void Start()
    {
        SetParents();

        moves.text = "Movimientos: " + cantidadMovimientos.ToString();

        m_allTiles = new Tile[alto, ancho];
        m_allGamePieces = new GamePiece[alto, ancho];

        SetupTiles();
        SetupCamera();
        FillBoard(10, .5f);
    }

    private void SetParents()
    {
        if (tileParent == null)
        {
            tileParent = new GameObject().transform;
            tileParent.name = "Tiles";
            tileParent.parent = this.transform;
        }

        if (gamePieceParent == null)
        {
            gamePieceParent = new GameObject().transform;
            gamePieceParent.name = "GamePieces";
            gamePieceParent = this.transform;
        }
    }

    //La cámara se acomoda con la resolución de aspecto
    private void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(alto - 1) / 2f, (float)(ancho - 1) / 2f, -10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)ancho / 2f + (float)borderSize;
        float horizontalSize = ((float)alto / 2f + (float)borderSize) / aspectRatio;
        Camera.main.orthographicSize = verticalSize > horizontalSize ? verticalSize : horizontalSize;
    }


    //Se instancian las ubicaciones de las fichas
    private void SetupTiles()
    {
        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector2(i, j), Quaternion.identity);
                tile.name = $"Tile({i},{j})";

                if (tileParent != null)
                {
                    tile.transform.parent = tileParent;
                }
                m_allTiles[i, j] = tile.GetComponent<Tile>();
                m_allTiles[i, j].Init(i, j, this);
            }


        }




    }

    //Agrega aleatoriamente fichas al Board
    private void FillBoard(int falseOffset = 0, float moveTime = .1f)
    {
        List<GamePiece> addedPieces = new List<GamePiece>();

        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                if (m_allGamePieces[i, j] == null)
                {
                    if (falseOffset == 0)
                    {
                        GamePiece piece = FillRandomAt(i, j);
                        addedPieces.Add(piece);
                    }
                    else
                    {
                        GamePiece piece = FillRandomAt(i, j, falseOffset, moveTime);
                        addedPieces.Add(piece);
                    }
                }


            }
        }
        //Interacciones máximas
        int maxIterations = 20;
        //Números de interacciones
        int iterations = 0;

        bool isFilled = false;

        //Si se encuentra algún match, las fichas de destruirán
        while (!isFilled)
        {
            List<GamePiece> matches = FindAllMatches();

            if (matches.Count == 0)
            {
                isFilled = true;
                break;
            }
            else
            {
                matches = matches.Intersect(addedPieces).ToList();

                if (falseOffset == 0)
                {
                    ReplaceWithRandom(matches);
                }
                else
                {
                    ReplaceWithRandom(matches, falseOffset, moveTime);
                }
            }

            if (iterations > maxIterations)
            {
                isFilled = true;
                Debug.LogWarning($"Se alcanzó el máximo de interacciones");
            }

            iterations++;
        }
    }

    public void ClickedTile(Tile tile)
    {
        if (piezaInicial == null)
        {
            piezaInicial = tile;
        }
    }

    //Cambiará de posición la pieza inicial con la pieza final
    public void DragToTile(Tile tile)
    {
        if (piezaInicial != null && IsNextTo(tile, piezaInicial))
        {
            piezaFinal = tile;
        }
    }

    public void ReleaseTile()
    {
        if (piezaInicial != null && piezaFinal != null)
        {
            SwitchTiles(piezaInicial, piezaFinal);
        }
        piezaInicial = null;
        piezaFinal = null;
    }

    private void SwitchTiles(Tile m_clickedTile, Tile m_targetTile)
    {
        StartCoroutine(SwitchTilesRoutine(m_clickedTile, m_targetTile));
    }

    //Encontrará los matches en coordenadas X y Y
    IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        if (m_playerInputEnabled)
        {
            GamePiece clickedPiece = m_allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
            GamePiece targetPiece = m_allGamePieces[targetTile.xIndex, targetTile.yIndex];

            if (clickedPiece != null && targetPiece != null)
            {

                clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                targetPiece.Move(clickedPiece.xIndex, clickedPiece.yIndex, swapTime);

                yield return new WaitForSeconds(swapTime);

                List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
                List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.xIndex, targetTile.yIndex);

                

                if (clickedPieceMatches.Count == 0 && targetPieceMatches.Count == 0)
                {
                    clickedPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
                    targetPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);                  

                    yield return new WaitForSeconds(swapTime);

                }
                else
                {
                    CleatAndRefillBoard(clickedPieceMatches.Union(targetPieceMatches).ToList());
                    AudioSource.PlayClipAtPoint(audioFX, gameObject.transform.position);
                }
                cantidadMovimientos--;
                moves.text = "Movimientos: " + cantidadMovimientos.ToString();


            }
        }
        if(cantidadMovimientos <= 0)
        {
            SceneManager.LoadScene("Game Over");
        }


    }


    private void CleatAndRefillBoard(List<GamePiece> gamePieces)
    {
        myCount = 0;
        StartCoroutine(ClearAndRefillRoutine(gamePieces));
    }

    List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLenght = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece startPiece = null;

        if (IsWithBounds(startX, startY))
        {
            startPiece = m_allGamePieces[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }



        int nextX;
        int nextY;

        int maxValue = alto > ancho ? alto : ancho;

        for (int i = 1; i < maxValue; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithBounds(nextX, nextY))
            {
                break;
            }

            GamePiece nextPiece = m_allGamePieces[nextX, nextY];

            if (nextPiece == null)
            {
                break;
            }
            else
            {
                if (nextPiece.tipoFicha == startPiece.tipoFicha && !matches.Contains(nextPiece))
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minLenght)
        {
            return matches;
        }
        else
        {
            return null;
        }
    }

    //Se encuentra los matches verticalmente
    List<GamePiece> FindVerticalMatches(int startX, int startY, int minLenght = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, Vector2.up, 2);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, Vector2.down, 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }
        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();
        return combinedMatches.Count >= minLenght ? combinedMatches : null;
    }

    //Se encuentra los matches horizontalmente
    List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLenght = 3)
    {
        List<GamePiece> rightMatches = FindMatches(startX, startY, Vector2.right, 2);
        List<GamePiece> leftMatches = FindMatches(startX, startY, Vector2.left, 2);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }
        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();
        return combinedMatches.Count >= minLenght ? combinedMatches : null;
    }

    //Se encuentra los matches, ya sea horizontal y verticalmente
    private List<GamePiece> FindMatchesAt(int x, int y, int minLenght = 3)
    {
        List<GamePiece> horizontalMatches = FindHorizontalMatches(x, y, minLenght);
        List<GamePiece> verticalMatches = FindVerticalMatches(x, y, minLenght);

        if (horizontalMatches == null)
        {
            horizontalMatches = new List<GamePiece>();


        }
        if (verticalMatches == null)
        {
            verticalMatches = new List<GamePiece>();

        }
        var MatchesCombinados = horizontalMatches.Union(verticalMatches).ToList();
        if(horizontalMatches.Count != 0 && verticalMatches.Count != 0)
        {
            int cantidadPuntos = 1000;

            m_puntaje.SumatoriaPuntos(cantidadPuntos);

        }

        var combinedMatches = horizontalMatches.Union(verticalMatches).ToList();
        

        return combinedMatches;
    }

    public bool Figure(int x, int y, int minLenght = 3)
    {
        List<GamePiece> horizontalMatches = FindHorizontalMatches(x, y, minLenght);
        List<GamePiece> verticalMatches = FindVerticalMatches(x, y, minLenght);

        if (horizontalMatches == null)
        {
            horizontalMatches = new List<GamePiece>();
        }
        if (verticalMatches == null)
        {
            verticalMatches = new List<GamePiece>();
        }
        if (horizontalMatches.Count == 0 && verticalMatches.Count != 0)
        {
            return false;
        }
        if(horizontalMatches.Count != 0 && verticalMatches.Count == 0)
        {

            return false;
        }
        if (horizontalMatches.Count != 0 && verticalMatches.Count != 0)
        {
            
            return true;
        }
        else
        {
            return false;
        }
    }


        List<GamePiece> FindMatchesAt(List<GamePiece> gamePieces, int minLenght = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        foreach (GamePiece piece in gamePieces)
        {
            matches = matches.Union(FindMatchesAt(piece.xIndex, piece.yIndex, minLenght)).ToList();
        }

        return matches;
    }

    //Revisa las fichas que estén al lado de cada una
    private bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex)
        {

            return true;
        }

        if (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.xIndex)
        {
            return true;
        }

        return false;
    }

    //Se encuentran todos los matches
    private List<GamePiece> FindAllMatches()
    {
        List<GamePiece> combinedMatches = new List<GamePiece>();

        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                var matches = FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }

        return combinedMatches;
    }

    //Se cambia el color del Tile cuando se encuentran los matches
    void HighlightTileOff(int x, int y)
    {
        SpriteRenderer spriteRender = m_allTiles[x, y].GetComponent<SpriteRenderer>();
        spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 0);
    }

    //Se cambia el color del Tile cuando se encuentran los matches
    void HighlightTileOn(int x, int y, Color col)
    {
        SpriteRenderer spriteRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
        spriteRenderer.color = col;
    }

    void HighlightMatchesAt(int x, int y)
    {
        HighlightTileOff(x, y);
        var combinedMatches = FindMatchesAt(x, y);

        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece piece in combinedMatches)
            {
                HighlightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }


    void HighlightMatches()
    {
        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                HighlightMatchesAt(i, j);
            }
        }
    }

    void HighlightPieces(List<GamePiece> gamepieces)
    {
        foreach (GamePiece piece in gamepieces)
        {
            if (piece != null)
            {
                HighlightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = m_allGamePieces[x, y];

        if (pieceToClear != null)
        {
            m_allGamePieces[x, y] = null;

            Destroy(pieceToClear.gameObject);
        }

        HighlightTileOff(x, y);
    }

    void ClearPieceAt(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                ClearPieceAt(piece.xIndex, piece.yIndex);
            }
        }

    }

    void ClearBoard()
    {
        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                ClearPieceAt(i, j);
            }
        }
    }

    //Se obtienen las fichas de manera aleatoria
    GameObject GetRandomPiece()
    {
        int randomInx = Random.Range(0, gamePiecesPrefabs.Length);

        if (gamePiecesPrefabs[randomInx] == null)
        {
            Debug.LogWarning($"La clase Board en el array de prefabs en la posicion {randomInx} no contiene una pieza valida");
        }

        return gamePiecesPrefabs[randomInx];
    }

    public void placeGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning($"gamePiece invalida");
            return;
        }

        gamePiece.transform.position = new Vector2(x, y);
        gamePiece.transform.rotation = Quaternion.identity;

        if (IsWithBounds(x, y))
        {
            m_allGamePieces[x, y] = gamePiece;
        }

        gamePiece.SetCoord(x, y);
    }

    private bool IsWithBounds(int x, int y)
    {
        return (x >= 0 && x < alto && y >= 0 && y < ancho);
    }

    //No permite cambiar de posición las fichas del mismo tipo
    GamePiece FillRandomAt(int x, int y, int falseOffset = 0, float moveTime = .1f)
    {
        GamePiece randomPiece = Instantiate(GetRandomPiece(), Vector2.zero, Quaternion.identity).GetComponent<GamePiece>();

        if (randomPiece != null)
        {
            randomPiece.Init(this);
            placeGamePiece(randomPiece, x, y);

            if (falseOffset != 0)
            {
                randomPiece.transform.position = new Vector2(x, y + falseOffset);
                randomPiece.Move(x, y, moveTime);
            }

            randomPiece.transform.parent = gamePieceParent;
        }

        return randomPiece;
    }

    //Reemplaza las fichas de manera aleatoria
    void ReplaceWithRandom(List<GamePiece> gamePieces, int falseOffset = 0, float moveTime = .1f)
    {
        foreach (GamePiece piece in gamePieces)
        {
            ClearPieceAt(piece.xIndex, piece.yIndex);

            if (falseOffset == 0)
            {
                FillRandomAt(piece.xIndex, piece.yIndex);
            }
            else
            {
                FillRandomAt(piece.xIndex, piece.yIndex, falseOffset, moveTime);
            }




        }
    }



    //Para organizar las fichas que van a caer de manera random
    List<GamePiece> CollapseColumn(int column, float collapseTime = .1f)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        for (int i = 0; i < ancho - 1; i++)
        {
            if (m_allGamePieces[column, i] == null)
            {
                for (int j = i + 1; j < ancho; j++)
                {
                    if (m_allGamePieces[column, j] != null)
                    {
                        m_allGamePieces[column, j].Move(column, i, collapseTime * (j - i));
                        m_allGamePieces[column, i] = m_allGamePieces[column, j];
                        m_allGamePieces[column, i].SetCoord(column, i);

                        if (!movingPieces.Contains(m_allGamePieces[column, i]))
                        {
                            movingPieces.Add(m_allGamePieces[column, i]);
                        }

                        m_allGamePieces[column, j] = null;
                        break;
                    }
                }
            }

        }


        return movingPieces;
    }

    //Colapsa las columnas de manera ordenada
    List<GamePiece> CollapseColumn(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<int> columnsToColapse = GetColumns(gamePieces);

        foreach (int column in columnsToColapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }

        return movingPieces;
    }

    //Obtiene las columnas random
    private List<int> GetColumns(List<GamePiece> gamePieces)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece piece in gamePieces)
        {
            if (!columns.Contains(piece.xIndex))
            {
                columns.Add(piece.xIndex);
            }
        }

        return columns;
    }


    IEnumerator ClearAndRefillRoutine(List<GamePiece> gamePieces)
    {
        m_playerInputEnabled = true;
        List<GamePiece> matches = gamePieces;

        do
        {
            //Se activa la animación al hacer match manual
            foreach (GamePiece piece in matches)
            {

                if (matches.Count == 3)
                {
                    int cantidadPuntos = 10;

                    m_puntaje.SumatoriaPuntos(cantidadPuntos);

                }
                if (matches.Count == 4)
                {
                    int cantidadPuntos = 20;

                    m_puntaje.SumatoriaPuntos(cantidadPuntos);

                }
                if (matches.Count == 5)
                {
                    int cantidadPuntos = 30;

                    m_puntaje.SumatoriaPuntos(cantidadPuntos);

                }
                if (matches.Count >= 6)
                {
                    int cantidadPuntos = 40;

                    m_puntaje.SumatoriaPuntos(cantidadPuntos);

                }

                piece.GetComponentInChildren<Animator>().SetBool("Explosion", true);

            }

            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            yield return null;
            yield return StartCoroutine(RefillRoutine());
            matches = FindAllMatches();
            yield return new WaitForSeconds(.5f);
        }
        while (matches.Count != 0);
        m_playerInputEnabled = true;
    }

    //Límpia y colapsa las fichas
    IEnumerator ClearAndCollapseRoutine(List<GamePiece> gamePieces)
    {
        myCount++;
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<GamePiece> matches = new List<GamePiece>();
        HighlightPieces(gamePieces);
        yield return new WaitForSeconds(.5f);
        bool isFinished = false;


        while (!isFinished)
        {
            ClearPieceAt(gamePieces);
            yield return new WaitForSeconds(.25f);

            movingPieces = CollapseColumn(gamePieces);
            while (!IsCollapsed(gamePieces))
            {
                yield return null;
            }

            //Sonido cuando las fichas hacen match
            AudioSource.PlayClipAtPoint(audioFX, gameObject.transform.position);

            yield return new WaitForSeconds(.5f);

            matches = FindMatchesAt(movingPieces);

            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {

                //Activa la animacion cuando se hace un match al caer
                foreach (GamePiece piece in matches)
                {


                    if (matches.Count == 3)
                    {
                        int cantidadPuntos = 10 * myCount;

                        m_puntaje.SumatoriaPuntos(cantidadPuntos);

                    }
                    if (matches.Count == 4)
                    {
                        int cantidadPuntos = 20 * myCount;

                        m_puntaje.SumatoriaPuntos(cantidadPuntos);

                    }
                    if (matches.Count == 5)
                    {
                        int cantidadPuntos = 30 * myCount;

                        m_puntaje.SumatoriaPuntos(cantidadPuntos);

                    }
                    if (matches.Count >= 6)
                    {
                        int cantidadPuntos = 40 * myCount;

                        m_puntaje.SumatoriaPuntos(cantidadPuntos);

                    }

                    piece.GetComponentInChildren<Animator>().SetBool("Explosion", true);
                }

                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }
        yield return null;
    }

    IEnumerator RefillRoutine()
    {
        FillBoard(10, .5f);
        yield return null;
    }

    //Velocidad para que las fichas cambien de lugars
    public bool IsCollapsed(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (piece.transform.position.y - (float)piece.yIndex > 0.001f)
                {
                    return false;
                }
            }
        }

        return true;
    }
}