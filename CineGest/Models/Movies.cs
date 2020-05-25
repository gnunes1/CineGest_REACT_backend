﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CineGest.Models
{
    public class Movies
    {
        /// <summary>
        /// Referencia o filme
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do filme
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Descricao do filme
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Categorias do filme
        /// </summary>
        public string Genres { get; set; }

        /// <summary>
        /// Duracao do filme
        /// </summary>
        [Required]
        [DataType(DataType.Time)]
        public DateTime Duration { get; set; }

        /// <summary>
        /// Idade minima para assistir ao filme
        /// </summary>
        public int Min_age { get; set; }

        /// <summary>
        /// Serve para meter o filme em destaque
        /// </summary>
        [Required]
        public bool Highlighted { get; set; }

        /// <summary>
        /// Todas as sessões do filme
        /// </summary>
        public ICollection<Sessions> CinemaMovieList { get; set; }

    }
}
