using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WFConfin.Models
{
    public class Pessoa
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O campo Nome deve ter entre 3 e 200 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [StringLength(20, ErrorMessage = "O campo Telefone deve ter até 20 caracteres")]
        public string Telefone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salario { get; set; }

        [StringLength(20, ErrorMessage = "O campo deve ter até 20 caracteres")]
        public string Genero { get; set; }

        [JsonIgnore]
        public Guid? CidadeId{  get; set; }

        public Pessoa()
        {
            Id = Guid.NewGuid();
        }

        //Relacionament Entity Framework
        [JsonIgnore]
        public Cidade Cidade { get; set; }







    }
}
