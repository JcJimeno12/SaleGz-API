

INSERT INTO Empresas (Nombre, Correo, Direccion, Telefono, RNC, TipoEmpresa, FechaRegistro)
VALUES ('LogicGz', 'logicGz@gmail.com', 'Calle Principal #1', '809-588-0000', '123456789', 0, GETDATE())
 
INSERT INTO Usuarios (EmpresaId, Nombre, Correo, Contrasena, TipoUsuario, Pin, FechaRegistro)
VALUES (
    1,
    'SuperAdmin',
    'superadmin@salegz.com',
    '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', -- contraseña: "password"
    2,
    1234,
    GETDATE()
)

--Correo: superadmin@salegz.com
--Contraseña: password


UPDATE Usuarios
SET Contrasena = '$2a$12$7LVBz5m4JPbsjwJTjVkrZe.DKTh410bPydoY3NwrnU6FQF2lJVgmi'
WHERE UsuarioId = 1;




select * from Controles


INSERT INTO Controles (
    EmpresaId,
    NumOrden,
    LimiteDiaPago,
    TamanoFactura,
    ITBS,
    LimiteConteoFiscal,
    LimiteConteoConsumo,
    ItbsCalculo,
    MostrarTelefonoCliente,
    MostrarDireccionCliente,
    TipoOrden,
    LimiteConteoRegimenEspecial,
    LimiteConteoGubernamental
)
VALUES (
    1,
    0,   
    0,
    0,
    18,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0
);
