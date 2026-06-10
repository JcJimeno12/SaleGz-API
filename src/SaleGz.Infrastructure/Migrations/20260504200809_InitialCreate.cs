using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaleGz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.CategoriaId);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cedula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credito = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Sexo = table.Column<bool>(type: "bit", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "ConsumosGastos",
                columns: table => new
                {
                    ConsumosGastosId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumosGastos", x => x.ConsumosGastosId);
                });

            migrationBuilder.CreateTable(
                name: "Controles",
                columns: table => new
                {
                    ControlesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    LimiteDiaPago = table.Column<int>(type: "int", nullable: false),
                    TamanoFactura = table.Column<int>(type: "int", nullable: false),
                    ITBS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LimiteConteoFiscal = table.Column<int>(type: "int", nullable: false),
                    LimiteConteoConsumo = table.Column<int>(type: "int", nullable: false),
                    ItbsCalculo = table.Column<int>(type: "int", nullable: false),
                    MostrarTelefonoCliente = table.Column<bool>(type: "bit", nullable: false),
                    MostrarDireccionCliente = table.Column<bool>(type: "bit", nullable: false),
                    NumOrden = table.Column<int>(type: "int", nullable: false),
                    TipoOrden = table.Column<int>(type: "int", nullable: false),
                    LimiteConteoRegimenEspecial = table.Column<int>(type: "int", nullable: false),
                    LimiteConteoGubernamental = table.Column<int>(type: "int", nullable: false),
                    FechaLimiteFiscal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaLimiteConsumo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaLimiteGubernamental = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaLimiteRegimenEspecial = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Controles", x => x.ControlesId);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instagram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Facebook = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Whatsapp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RNC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoEmpresa = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.EmpresaId);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Minimo = table.Column<int>(type: "int", nullable: false),
                    Precio1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Precio2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Precio3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ITBIS = table.Column<int>(type: "int", nullable: false),
                    CodigoBarra = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.ProductoId);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "CategoriaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoUsuario = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Pin = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_Usuarios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    CompraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.CompraId);
                    table.ForeignKey(
                        name: "FK_Compras_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId");
                    table.ForeignKey(
                        name: "FK_Compras_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                columns: table => new
                {
                    CotizacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ITBIS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => x.CotizacionId);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId");
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cuadres",
                columns: table => new
                {
                    CuadreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P5 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P10 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P20 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P25 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P50 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P100 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P200 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P500 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P1000 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P2000 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCheque = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalTarjeta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalTransaccion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantidadCheque = table.Column<int>(type: "int", nullable: false),
                    CantidadTarjeta = table.Column<int>(type: "int", nullable: false),
                    CantidadTransaccion = table.Column<int>(type: "int", nullable: false),
                    FondoInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VentasSistema = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Diferencia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HoraApertura = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoraCierre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoCuadre = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuadres", x => x.CuadreId);
                    table.ForeignKey(
                        name: "FK_Cuadres_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    FacturaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ITBS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Pagado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Exento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Gravado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    NumOrden = table.Column<int>(type: "int", nullable: false),
                    NombreCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarjeta = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.FacturaId);
                    table.ForeignKey(
                        name: "FK_Facturas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId");
                    table.ForeignKey(
                        name: "FK_Facturas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gastos",
                columns: table => new
                {
                    GastosId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gastos", x => x.GastosId);
                    table.ForeignKey(
                        name: "FK_Gastos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inv_Movimientos",
                columns: table => new
                {
                    MovimientoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nota = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StockAnterior = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockActual = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inv_Movimientos", x => x.MovimientoId);
                    table.ForeignKey(
                        name: "FK_Inv_Movimientos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inv_Movimientos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioPermisos",
                columns: table => new
                {
                    UsuarioPermisoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    VerFacturas = table.Column<bool>(type: "bit", nullable: false),
                    CrearFacturas = table.Column<bool>(type: "bit", nullable: false),
                    CancelarFacturas = table.Column<bool>(type: "bit", nullable: false),
                    VerCotizaciones = table.Column<bool>(type: "bit", nullable: false),
                    CrearCotizaciones = table.Column<bool>(type: "bit", nullable: false),
                    VerCompras = table.Column<bool>(type: "bit", nullable: false),
                    CrearCompras = table.Column<bool>(type: "bit", nullable: false),
                    VerGastos = table.Column<bool>(type: "bit", nullable: false),
                    CrearGastos = table.Column<bool>(type: "bit", nullable: false),
                    VerCuadres = table.Column<bool>(type: "bit", nullable: false),
                    CrearCuadres = table.Column<bool>(type: "bit", nullable: false),
                    VerClientes = table.Column<bool>(type: "bit", nullable: false),
                    CrearClientes = table.Column<bool>(type: "bit", nullable: false),
                    VerProductos = table.Column<bool>(type: "bit", nullable: false),
                    VerReportes = table.Column<bool>(type: "bit", nullable: false),
                    VerInventario = table.Column<bool>(type: "bit", nullable: false),
                    CrearInventario = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioPermisos", x => x.UsuarioPermisoId);
                    table.ForeignKey(
                        name: "FK_UsuarioPermisos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompraDetalles",
                columns: table => new
                {
                    CompraDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompraDetalles", x => x.CompraDetalleId);
                    table.ForeignKey(
                        name: "FK_CompraDetalles_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "CompraId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompraDetalles_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuentasPorPagar",
                columns: table => new
                {
                    CuentaPorPagarDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasPorPagar", x => x.CuentaPorPagarDetalleId);
                    table.ForeignKey(
                        name: "FK_CuentasPorPagar_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "CompraId");
                    table.ForeignKey(
                        name: "FK_CuentasPorPagar_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateTable(
                name: "CotizacionDetalles",
                columns: table => new
                {
                    CotizacionDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CotizacionId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ITBIS = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionDetalles", x => x.CotizacionDetalleId);
                    table.ForeignKey(
                        name: "FK_CotizacionDetalles_Cotizaciones_CotizacionId",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizaciones",
                        principalColumn: "CotizacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CotizacionDetalles_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consumos",
                columns: table => new
                {
                    ConsumoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaId = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    Conteo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumos", x => x.ConsumoId);
                    table.ForeignKey(
                        name: "FK_Consumos_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "FacturaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuentasPorCobrar",
                columns: table => new
                {
                    CuentaPorCobrarDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasPorCobrar", x => x.CuentaPorCobrarDetalleId);
                    table.ForeignKey(
                        name: "FK_CuentasPorCobrar_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "FacturaId");
                    table.ForeignKey(
                        name: "FK_CuentasPorCobrar_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateTable(
                name: "FacturaDetalles",
                columns: table => new
                {
                    FacturaDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ITBS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comision = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaDetalles", x => x.FacturaDetalleId);
                    table.ForeignKey(
                        name: "FK_FacturaDetalles_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "FacturaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturaDetalles_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fiscals",
                columns: table => new
                {
                    FiscalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaId = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    Conteo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fiscals", x => x.FiscalId);
                    table.ForeignKey(
                        name: "FK_Fiscals_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "FacturaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gubernamentales",
                columns: table => new
                {
                    GubernamentalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaId = table.Column<int>(type: "int", nullable: false),
                    Conteo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gubernamentales", x => x.GubernamentalId);
                    table.ForeignKey(
                        name: "FK_Gubernamentales_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "FacturaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotaCreditos",
                columns: table => new
                {
                    NotaCreditoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaId = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Itbis = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comprobante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoComprobante = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Conteo = table.Column<int>(type: "int", nullable: false),
                    RazonModificacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaCreditos", x => x.NotaCreditoId);
                    table.ForeignKey(
                        name: "FK_NotaCreditos_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "FacturaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegimenesEspeciales",
                columns: table => new
                {
                    RegimenEspecialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaId = table.Column<int>(type: "int", nullable: false),
                    Conteo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegimenesEspeciales", x => x.RegimenEspecialId);
                    table.ForeignKey(
                        name: "FK_RegimenesEspeciales_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "FacturaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GastoDetalles",
                columns: table => new
                {
                    GastosDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GastosId = table.Column<int>(type: "int", nullable: false),
                    ConsumosGastosId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GastosId1 = table.Column<int>(type: "int", nullable: false),
                    ConsumoGastoConsumosGastosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GastoDetalles", x => x.GastosDetalleId);
                    table.ForeignKey(
                        name: "FK_GastoDetalles_ConsumosGastos_ConsumoGastoConsumosGastosId",
                        column: x => x.ConsumoGastoConsumosGastosId,
                        principalTable: "ConsumosGastos",
                        principalColumn: "ConsumosGastosId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GastoDetalles_Gastos_GastosId1",
                        column: x => x.GastosId1,
                        principalTable: "Gastos",
                        principalColumn: "GastosId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotaCreditoDetalles",
                columns: table => new
                {
                    NotaCreditoDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotaCreditoId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Itbis = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comision = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Detalles = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaCreditoDetalles", x => x.NotaCreditoDetalleId);
                    table.ForeignKey(
                        name: "FK_NotaCreditoDetalles_NotaCreditos_NotaCreditoId",
                        column: x => x.NotaCreditoId,
                        principalTable: "NotaCreditos",
                        principalColumn: "NotaCreditoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotaCreditoDetalles_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompraDetalles_CompraId",
                table: "CompraDetalles",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_CompraDetalles_ProductoId",
                table: "CompraDetalles",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ClienteId",
                table: "Compras",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_UsuarioId",
                table: "Compras",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Consumos_FacturaId",
                table: "Consumos",
                column: "FacturaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionDetalles_CotizacionId",
                table: "CotizacionDetalles",
                column: "CotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionDetalles_ProductoId",
                table: "CotizacionDetalles",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_ClienteId",
                table: "Cotizaciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_UsuarioId",
                table: "Cotizaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuadres_UsuarioId",
                table: "Cuadres",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_FacturaId",
                table: "CuentasPorCobrar",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_UsuarioId",
                table: "CuentasPorCobrar",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorPagar_CompraId",
                table: "CuentasPorPagar",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorPagar_UsuarioId",
                table: "CuentasPorPagar",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalles_FacturaId",
                table: "FacturaDetalles",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalles_ProductoId",
                table: "FacturaDetalles",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_ClienteId",
                table: "Facturas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_UsuarioId",
                table: "Facturas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Fiscals_FacturaId",
                table: "Fiscals",
                column: "FacturaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GastoDetalles_ConsumoGastoConsumosGastosId",
                table: "GastoDetalles",
                column: "ConsumoGastoConsumosGastosId");

            migrationBuilder.CreateIndex(
                name: "IX_GastoDetalles_GastosId1",
                table: "GastoDetalles",
                column: "GastosId1");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_UsuarioId",
                table: "Gastos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Gubernamentales_FacturaId",
                table: "Gubernamentales",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Inv_Movimientos_Fecha",
                table: "Inv_Movimientos",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Inv_Movimientos_ProductoId",
                table: "Inv_Movimientos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inv_Movimientos_UsuarioId",
                table: "Inv_Movimientos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaCreditoDetalles_NotaCreditoId",
                table: "NotaCreditoDetalles",
                column: "NotaCreditoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaCreditoDetalles_ProductoId",
                table: "NotaCreditoDetalles",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaCreditos_FacturaId",
                table: "NotaCreditos",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegimenesEspeciales_FacturaId",
                table: "RegimenesEspeciales",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermisos_UsuarioId",
                table: "UsuarioPermisos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpresaId",
                table: "Usuarios",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompraDetalles");

            migrationBuilder.DropTable(
                name: "Consumos");

            migrationBuilder.DropTable(
                name: "Controles");

            migrationBuilder.DropTable(
                name: "CotizacionDetalles");

            migrationBuilder.DropTable(
                name: "Cuadres");

            migrationBuilder.DropTable(
                name: "CuentasPorCobrar");

            migrationBuilder.DropTable(
                name: "CuentasPorPagar");

            migrationBuilder.DropTable(
                name: "FacturaDetalles");

            migrationBuilder.DropTable(
                name: "Fiscals");

            migrationBuilder.DropTable(
                name: "GastoDetalles");

            migrationBuilder.DropTable(
                name: "Gubernamentales");

            migrationBuilder.DropTable(
                name: "Inv_Movimientos");

            migrationBuilder.DropTable(
                name: "NotaCreditoDetalles");

            migrationBuilder.DropTable(
                name: "RegimenesEspeciales");

            migrationBuilder.DropTable(
                name: "UsuarioPermisos");

            migrationBuilder.DropTable(
                name: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "ConsumosGastos");

            migrationBuilder.DropTable(
                name: "Gastos");

            migrationBuilder.DropTable(
                name: "NotaCreditos");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Facturas");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
