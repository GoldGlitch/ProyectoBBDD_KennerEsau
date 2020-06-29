using UnityEngine;
using System.Collections;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Este script maneja todas las comunicaciones con la base de datos.
/// </summary>
public class HighScoreManager : MonoBehaviour {

    /// <summary>
    /// La cadena de conexión, esta cadena indica la ruta a la base de datos
    /// </summary>
    private string connectionString;

    /// <summary>
    /// Esta lista contiene todas las mejores puntuaciones
    /// </summary>
    private List<HighScore> highScores = new List<HighScore>();

    /// <summary>
    /// Este prefabricado se usa cuando necesitamos crear un nuevo puntaje alto
    /// </summary>
    public GameObject scorePrefab;

    /// <summary>
    /// Este es el padre de todos los objetos de puntaje alto
    /// </summary>
    public Transform scoreParent;

    /// <summary>
    /// Indica cuántos puntajes mostraremos al jugador
    /// </summary>
    public int topRanks;

    /// <summary>
    /// La cantidad de puntajes que guardaremos en la base de datos
    /// </summary>
    public int saveScores;

    /// <summary>
    /// El campo de entrada de nombre
    /// </summary>
    public InputField enterName;

    /// <summary>
    /// El diálogo para ingresar el nombre del jugador
    /// </summary>
    public GameObject nameDialog;
    public GameObject delatedDialog;

    // Use esto para la inicialización
    void Start ()
    {
        //Establece la cadena de conexiones como la ruta de datos predeterminada dentro de la carpeta de activos
        connectionString = "URI=file:" + Application.dataPath + "/HighScoreDB.sqlite";

        //Crea la base de datos si no existe
        CreateTable();

        //Borra los puntajes extra
        DeleteExtraScore();

        //Muestra las puntuaciones al jugador.
        ShowScores();
	}

    // La actualización se llama una vez por fotograma
    void Update ()
   {
        if (Input.GetKeyDown(KeyCode.Escape)) //Si presionamos escape, entonces queremos mostrar u ocultar el diálogo entername
        {
            nameDialog.SetActive(!nameDialog.activeSelf);

        }
       

    }

    /// <summary>
    /// Crea una tabla si no existe
    /// </summary>
    private void CreateTable()
    {
        //Crea la conexión
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            //Abre la conexión
            dbConnection.Open();

            //Crea un comando para que podamos ejecutarlo en la base de datos.
            using (IDbCommand dbCmd = dbConnection.CreateCommand()) 
            {
                //Crea la consulta
                string sqlQuery = String.Format("CREATE TABLE if not exists HighScores (PlayerID INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL  UNIQUE , Name TEXT NOT NULL , Score INTEGER NOT NULL , Date DATETIME NOT NULL  DEFAULT CURRENT_DATE)");

                //Da el sqlQuery al comando
                dbCmd.CommandText = sqlQuery;

                //Ejecuta a la comadna
                dbCmd.ExecuteScalar();

                //Cierra las conexiones
                dbConnection.Close();
            }
        }
    }

    /// <summary>
    /// Se llama cuando el jugador está presionando el botón OK
    /// </summary>
    public void EnterName()
    {

        if (enterName.text != string.Empty) //Se asegura de que tengamos algo de texto para ingresar
        {
            int score = UnityEngine.Random.Range(1, 500); //Genera una puntuación aleatoria.

            InsertScore(enterName.text, score); //Inserta el puntaje en la base de datos
            

            enterName.text = string.Empty; //restablece el campo de texto

            ShowScores(); //Obtiene los puntajes de la base de datos.

        }
    }

    public void delete()
    {
        if (enterName.text != string.Empty) //Se asegura de que tengamos algo de texto para ingresar
        {
            int id = Convert.ToInt32( enterName.text); //Genera una puntuación aleatoria.

            DeleteScore(id); //Elimina el puntaje en la base de datos

            enterName.text = string.Empty; //restablece el campo de texto

            ShowScores(); //Obtiene los puntajes de la base de datos.

        }
    }

    /// <summary>
    /// Inserta el puntaje en la base de datos
    /// </summary>
    /// <param name="name">El nombre del jugador</param>
    /// <param name="newScore">El puntaje del jugador</param>
    private void InsertScore(string name, int newScore)
    {
        GetScores(); //Obtiene los puntajes de la base de datos

        int hsCount = highScores.Count; //Almacena la cantidad de puntajes

        if (highScores.Count > 0) //Si tenemos más de 0 puntajes altos
        {
            HighScore lowestScore = highScores[highScores.Count - 1]; //Crea una referencia a la puntuación más baja.

            //Si el puntaje más bajo necesita ser reemplazado
            if (lowestScore != null && saveScores > 0 && highScores.Count >= saveScores && newScore > lowestScore.Score)
            {
                DeleteScore(lowestScore.ID); //Elimina la puntuación más baja.

                hsCount--; //Reduce la cantidad de puntajes, para que sepamos si debemos insertar un nuevo puntaje
            }
        }
        if (hsCount < saveScores) //Si hay espacio en la lista de mejores puntuaciones, inserte una nueva puntuación
        {
            //Crea una conexión de base de datos.
            using (IDbConnection dbConnection = new SqliteConnection(connectionString)) 
            {
                //Abre la conexión
                dbConnection.Open();

                //Crea un comentario en la base de datos.
                using (IDbCommand dbCmd = dbConnection.CreateCommand())
                {
                    //Crea una consulta para insertar el nuevo puntaje
                    string sqlQuery = String.Format("INSERT INTO HighScores(Name,Score) VALUES(\"{0}\",\"{1}\")", name, newScore);
                    
                    dbCmd.CommandText = sqlQuery; //Da la consulta al texto del comando
                    dbCmd.ExecuteScalar(); //Ejecuta la consulta
                    dbConnection.Close();//Cierra la conexion


                }
            }
        }
    }

    /// <summary>
    /// Obtiene los puntajes de la base de datos
    /// </summary>
    private void GetScores()
    {
        //Borra la lista de mejores puntuaciones para que podamos obtener las nuevas puntuaciones
        highScores.Clear();

        //Crea una conexión de base de datos.
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            //Abre la conexión
            dbConnection.Open();

            //Crea un comentario en la base de datos.
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                //Selecciona todo, desde las mejores puntuaciones
                string sqlQuery = "SELECT * FROM HighScores";

                //alimenta la consulta al comando
                dbCmd.CommandText = sqlQuery;

                //Crea un lector y lo ejecuta para que podamos cargar las puntuaciones más altas
                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read()) //Mientras tengamos algo que leer

                    {
                        //Agrega el nuevo puntaje alto a la lista de puntajes altos
                        highScores.Add(new HighScore(reader.GetInt32(0), reader.GetInt32(2), reader.GetString(1), reader.GetDateTime(3)));
                    }

                    //Cierra la conexión
                    dbConnection.Close();
                    reader.Close();
                }
            }
        }

        highScores.Sort(); //Ordena la puntuación más alta de mayor a menor
    }

    /// <summary>
    /// Elimina una entrada específica en la base de datos.
    /// </summary>
    /// <param name="id">El ID de la base de datos de puntajes</param>
    private void DeleteScore(int id)
    {
        //Crea una conexión de base de datos.
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open(); //Abre la conexión

            //Crea un comando de base de datos.
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {

                //Crea una consulta
                string sqlQuery = String.Format("DELETE FROM HighScores WHERE PlayerID = \"{0}\"", id);

                //Alimenta la consulta al comando
                dbCmd.CommandText = sqlQuery;

                //Ejecuta el comando
                dbCmd.ExecuteScalar();

                //Cierra la conexión
                dbConnection.Close();


            }
        }
    }

    /// <summary>
    /// Muestra las puntuaciones al jugador
    /// </summary>
    private void ShowScores()
    {
        GetScores(); //Obtiene los puntajes de la base de datos

        //Recorre todos los puntajes
        foreach (GameObject score in GameObject.FindGameObjectsWithTag("Score"))
        {
            //Destruye todos los puntajes antiguos
            Destroy(score);
        }

        for (int i = 0; i < topRanks; i++) //Este bucle asegura que solo se muestren las x llagas superiores
        {
            if (i <= highScores.Count - 1) //Se asegura de que no obtengamos una excepción de índice fuera de límites
            {
                GameObject tmpObjec = Instantiate(scorePrefab); //Crea una nueva puntuación

                HighScore tmpScore = highScores[i]; //Obtiene la puntuación más alta actual

                //Establece la puntuación de los objetos.
                tmpObjec.GetComponent<HighScoreScript>().SetScore(tmpScore.Name, tmpScore.Score.ToString(), "#" + (i + 1).ToString());

                tmpObjec.transform.SetParent(scoreParent); //Establece la puntuación del padre

                tmpObjec.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); //Se asegura de que el objeto tenga la escala correcta
            }

        }
    }

    /// <summary>
    /// Elimina los puntajes adicionales, esto se basa en la variable saveScores
    /// </summary>
    private void DeleteExtraScore()
    {
        GetScores(); //Obtiene los puntajes actuales

        if (saveScores <= highScores.Count) //si la cantidad de puntajes para guardar es menor que la cantidad de puntajes guardados
        {
            int deleteCount = highScores.Count - saveScores; //Almacenar el número de puntajes para eliminar

            highScores.Reverse(); //Invierte el orden para que nos sea más fácil eliminar las puntuaciones más bajas

            using (IDbConnection dbConnection = new SqliteConnection(connectionString)) //Crea una conexión

            {
                dbConnection.Open(); //Abre la conexión

                using (IDbCommand dbCmd = dbConnection.CreateCommand()) //Crea un comando
                {
                    for (int i = 0; i < deleteCount; i++) //Borra los puntajes
                    {
                        //Crea el sqlQuery para eliminar la puntuación más alta
                        string sqlQuery = String.Format("DELETE FROM HighScores WHERE PlayerID = \"{0}\"", highScores[i].ID);

                        //Alimenta la consulta al comandoTexto
                        dbCmd.CommandText = sqlQuery;

                        dbCmd.ExecuteScalar(); //Ejecuta el comando
                    }

                    dbConnection.Close(); //Cierra la conexión


                }
            }
        }
    }
}
