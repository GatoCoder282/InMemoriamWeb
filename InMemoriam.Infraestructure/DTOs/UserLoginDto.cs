namespace InMemoriam.Infraestructure.DTOs
{
    public class UserLoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!; // Actualmente el proyecto no almacena contraseñas: ver nota abajo
    }
}