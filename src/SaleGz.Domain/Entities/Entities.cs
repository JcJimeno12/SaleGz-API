using SaleGz.Domain.Enums;

namespace SaleGz.Domain.Entities;

public class Empresa
{
    public int EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? Whatsapp { get; set; }
    public string? RNC { get; set; }
    public TipoEmpresa TipoEmpresa { get; set; }
    public DateTime FechaRegistro { get; set; }

    // Navegación
    public ICollection<Usuario> Usuarios { get; set; } = [];
}

public class Usuario
{
    public int UsuarioId { get; set; }
    public int EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;  // Hasheada con BCrypt
    public TipoUsuario TipoUsuario { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int Pin { get; set; }

    // Navegación
    public Empresa Empresa { get; set; } = null!;
}

public class Cliente
{
    public int ClienteId { get; set; }
    public int EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Cedula { get; set; }
    public string? Direccion { get; set; }
    public decimal Credito { get; set; }
    public EstadoGeneral Estado { get; set; }
    public bool Sexo { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public DateTime FechaRegistro { get; set; }
    public TipoCliente Tipo { get; set; }
    public TipoEntidad TipoEntidad { get; set; }
}

public class Categoria
{
    public int CategoriaId { get; set; }
    public int EmpresaId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public EstadoGeneral Estado { get; set; }

    // Navegación
    public ICollection<Producto> Productos { get; set; } = [];
}

public class Producto
{
    public int ProductoId { get; set; }
    public int EmpresaId { get; set; }
    public int CategoriaId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Referencia { get; set; }
    public decimal Cantidad { get; set; }
    public decimal Costo { get; set; }
    public int Minimo { get; set; }
    public decimal Precio1 { get; set; }
    public decimal Precio2 { get; set; }
    public decimal Precio3 { get; set; }
    public TipoITBIS ITBIS { get; set; }
    public string? CodigoBarra { get; set; }

    // Navegación
    public Categoria Categoria { get; set; } = null!;
}
