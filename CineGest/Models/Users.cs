using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineGest.Models
{
    public class Users
    {
        /// <summary>
        /// Referencia o utilizador
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do utilizador
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Nome do utilizador
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nome do utilizador
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Data de nascimento do utilizador
        /// </summary>
        public DateTime DoB { get; set; }

        /// <summary>
        /// nome da fotografia do utilizador
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Referência o cargo
        /// </summary>
        [ForeignKey(nameof(Role))]
        public int RoleFK { get; set; }
        public Roles Role { get; set; }

        /// <summary>
        /// Lista de bilhetes associados ao utilizador
        /// </summary>
        public ICollection<Tickets> TicketList { get; set; }
    }
}
