using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CineGest.Models
{
    public class Cinemas
    {
        /// <summary>
        /// Referencia o cinema
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do cinema
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Capacidade do cinema
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Nome da cidade
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Localização do cinema
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Lista das sessões no cinema
        /// </summary>
        public ICollection<Sessions> CinemaList { get; set; }

    }
}
