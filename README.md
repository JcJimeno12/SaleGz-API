# Actualizar Base de Datos y Migraciones (Entity Framework Core)

## 1. Crear o modificar el modelo

Ejemplo:

```csharp
public TipoEntidad TipoEntidad { get; set; }
```

---

## 2. Crear la migración

Desde la carpeta del proyecto Infrastructure:

```bash
dotnet ef migrations add NombreDeLaMigracion
```

Ejemplo:

```bash
dotnet ef migrations add AddTipoEntidadToClientes
```

---

## 3. Verificar que la migración se creó

Debe aparecer una nueva carpeta o archivos dentro de:

```bash
Migrations/
```

Con nombres similares a:

```bash
20260526120000_AddTipoEntidadToClientes.cs
20260526120000_AddTipoEntidadToClientes.Designer.cs
AppDbContextModelSnapshot.cs
```

---

## 4. Aplicar cambios a la base de datos

```bash
dotnet ef database update
```

Esto ejecutará la migración y actualizará la base de datos.

---

# Comandos útiles

## Ver lista de migraciones

```bash
dotnet ef migrations list
```

---

## Eliminar última migración (si NO se ha aplicado)

```bash
dotnet ef migrations remove
```

---

## Revertir base de datos a una migración anterior

```bash
dotnet ef database update NombreMigracionAnterior
```

Ejemplo:

```bash
dotnet ef database update InitialCreate
```

---

## Crear migración sin aplicarla

```bash
dotnet ef migrations add NombreMigracion
```

---

## Aplicar todas las migraciones pendientes

```bash
dotnet ef database update
```

---

# Flujo recomendado

## Cuando agregas/modificas entidades:

1. Modificar entidad/modelo
2. Ejecutar:

```bash
dotnet build
```

3. Crear migración:

```bash
dotnet ef migrations add NombreMigracion
```

4. Aplicar cambios:

```bash
dotnet ef database update
```

---

# Nota importante

Si `dotnet ef migrations add` falla:

1. Ejecutar:

```bash
dotnet build
```

2. Corregir errores de compilación
3. Volver a ejecutar la migración

Entity Framework NO puede crear migraciones si el proyecto no compila.
# API-Rest
