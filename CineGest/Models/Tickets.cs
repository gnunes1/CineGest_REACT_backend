using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGest.Models
{
    public class Tickets
    {
        /// <summary>
        /// Id do bilhete
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Relaciona o bilhete com a sessão 
        /// </summary>
        [ForeignKey(nameof(Cinema_Movie))]
        public int Cinema_MovieFK { get; set; }
        public Sessions Cinema_Movie { get; set; }

        /// <summary>
        /// Relaciona o utilizador com a sessão do filme
        /// </summary>
        [ForeignKey(nameof(User))]
        public int UserFK { get; set; }
        public Users User { get; set; }

        /// <summary>
        /// Lugar respetivo ao bilhete comprado para a sessão
        /// </summary>
        public int Seat { get; set; }
    }
}
