# Portal Gamer - Catálogo de videojuegos (WPF + MySQL)

Proyecto de Desarrollo de Interfaces (2º DAM). Aplicación WPF en .NET 8 con login,
alta de usuarios y catálogo de videojuegos. Desde la Práctica 3 los usuarios se
validan contra una base de datos MySQL.

## Cómo ejecutarlo

1. Ejecutar `database.sql` en MySQL 8 como root (crea la base de datos
   `catalogo_videojuegos`, la tabla `Usuarios` y el usuario `catalogo_app`).
2. Abrir `Practica1_PortalVideojuegos.slnx` en Visual Studio y pulsar F5.
3. Usuarios de prueba: `admin` / `1234` y `alumno` / `dam`.

## Estructura

- `Data/DatabaseHelper.cs`: cadena de conexión y apertura/cierre de la conexión.
- `F_Login.xaml(.cs)`: login con consulta parametrizada, alta de usuarios y control de errores.
- `MainWindow.xaml(.cs)`: Home del catálogo con "Cerrar sesión".
- `database.sql`: script de la base de datos.
