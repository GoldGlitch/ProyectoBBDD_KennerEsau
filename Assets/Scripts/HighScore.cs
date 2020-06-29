using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Esta es la clase de puntaje alto, se usa para manejar todos nuestros puntajes altos
/// </summary>
class HighScore : IComparable<HighScore>
{
    /// <summary>
    /// The score
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// El nombre del dueño de las puntuaciones más altas
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// La fecha en que se hizo la puntuación más alta
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// El ID de la base de datos de puntajes altos
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// El constructor de Highscore

    /// </summary>
    /// <param name="id">El ID de la base de datos de la puntuación más alta</param>
    /// <param name="score">El marcador</param>
    /// <param name="name">El nombre de las mejores puntuaciones owner</param>
    /// <param name="date">La fecha en que se creó la puntuación más alta</param>
    public HighScore(int id, int score, string name, DateTime date)
    {
        this.Score = score;
        this.Name = name;
        this.ID = id;
        this.Date = date;
    }

    /// <summary>
    /// Comparar con se utiliza para ordenar las puntuaciones más altas en una lista
    /// </summary>
    /// <param name="other">El puntaje, que estamos comparando este puntaje con</param>
    public int CompareTo(HighScore other)
    {
        //first > second return -1
        //first < second return 1
        //first == second return 0

        if (other.Score < this.Score) //Si el otro puntaje es menor que este
        {
            return -1;
        }
        else if (other.Score > this.Score) //Si el otro puntaje es mayor que este
        {
            return 1;
        }
        else if (other.Date < this.Date) //Si los puntajes son iguales, entonces debemos verificar la fecha
        {
            return -1;
        }
        else if (other.Date > this.Date)
        {
            return 1; 
        }

        //devolveremos 0 si los puntajes y las fechas son idénticos.
        return 0;
    }
}
