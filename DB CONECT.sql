
 

INSERT INTO Pw_Empresas (Nombre, Correo, Direccion, Telefono, RNC, TipoEmpresa, FechaRegistro)
VALUES ('LogicGz', 'logicGz@gmail.com', 'Calle Principal #1', '809-588-0000', '123456789', 0, GETDATE())


INSERT INTO Pw_Usuarios (EmpresaId, Nombre, Correo, Contrasena, TipoUsuario, FechaRegistro, Pin)
VALUES (1, 'Administrador', 'admin@gmail.com', 
'$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2uheWG/igi.', 
0, GETDATE(), 1234)



select * from  Pw_Empresas


SELECT name FROM sys.databases WHERE name = 'SaleGzSaaS'


UPDATE Pw_Usuarios 
SET Contrasena = '$2a$11$wuH9QL3bhe5MmpIrl7biQ.uThdsvPdO40C1ZstYRlp4q2nWRqBduW'
WHERE Correo = 'admin@gmail.com'