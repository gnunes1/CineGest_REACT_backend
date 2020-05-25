using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGest.Models
{
    /// <summary>
    /// Sessão dos filmes
    /// </summary>
    public class Sessions
    {
        /// <summary>
        /// referência a sessão
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Relaciona o cinema ao filme
        /// </summary>
        [ForeignKey(nameof(Cinema))]
        public int CinemaFK { get; set; }
        public Cinemas Cinema { get; set; }

        /// <summary>
        /// relaciona o filme à sala
        /// </summary>
        [ForeignKey(nameof(Movie))]
        public int MovieFK { get; set; }
        public Movies Movie { get; set; }

        /// <summary>
        /// hora de início do filme
        /// </summary>
        [DataType(DataType.Time)]
        public DateTime Start_Time { get; set; }

        /// <summary>
        /// data de início do filme
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// data de fim do filme
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Número de lugares ocupados na sessão
        /// </summary>
        public int Occupated_seats { get; set; }

        /// <summary>
        /// Lista de bilhetes associados a esta sessão do filme
        /// </summary>
        public ICollection<Tickets> Cinema_MovieList { get; set; }

    }
}
