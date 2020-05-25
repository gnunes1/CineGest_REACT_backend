using System.ComponentModel.DataAnnotations;

namespace CineGest.Models
{
    public class Roles
    {
        /// <summary>
        /// Identifica o role
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do role
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
