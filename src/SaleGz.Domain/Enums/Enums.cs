namespace SaleGz.Domain.Enums;

public enum TipoUsuario
{
    Admin = 0,
    Normal = 1,
    SuperAdmin = 2
}

public enum EstadoCompra
{
    Contado = 0,
    Credito = 1,
    Saldado = 2,
    Cancelado = 3
}

public enum TipoITBIS
{
    Exento = 0,
    Gravado = 1
}

public enum TipoEmpresa
{
    Fisica = 0,
    Juridica = 1
}

public enum TamanoFactura
{
    Pequena = 0,
    Grande = 1
}

public enum ItbsCalculo
{
    SacadoDelPrecio = 0,    // ITBS incluido en el precio
    AgregadoAlPrecio = 1    // ITBS se suma al precio
}

public enum EstadoGeneral
{
    Inactivo = 0,
    Activo = 1
}

public enum TipoCliente
{
    Regular = 0,
    Credito = 1,
    Mayorista = 2
}

public enum TipoEntidad
{
    Cliente = 0,
    Proveedor = 1,
    Ambos = 2
}