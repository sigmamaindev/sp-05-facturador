# Facturador Backend (.NET 8)

Este proyecto corresponde al backend del sistema de facturación electrónica Facturador, desarrollado con `.NET 8` en `Linux/Windows/Mac` utilizando Visual Studio Code.

El sistema está diseñado para manejar facturación electrónica según las normas del SRI para Ecuador, gestión de productos, inventario multi-bodega, clientes, establecimientos, puntos de emisión, y todos los procesos necesarios para la emisión, almacenamiento y control de facturas electrónicas.

## Estructura del Proyecto

El backend está organizado siguiendo una arquitectura por capas para mantener una alta mantenibilidad y escalabilidad:

- API → Controladores Web API y punto de entrada de la aplicación. Expone los endpoints REST.

- Core → Entidades, lógica de dominio y reglas de negocio.

- Infrastructure → Acceso a datos, servicios externos (SRI, correo, etc.) e implementaciones concretas.

Esta separación facilita el mantenimiento, las pruebas unitarias y futuras integraciones.

## Requisitos previos

1. Tener instalado Visual Studio Code.

2. Instalar la extensión C# Dev Kit para IntelliSense, depuración y gestión de proyectos.

3. Tener instalado el .NET 8 SDK. Verificar con:

```sh
dotnet --version
```

4. Base de datos PostgreSQL (local o en la nube, como Railway).

5. Configurar las variables de entorno necesarias (cadena de conexión, claves del SRI, etc.).

## Guía paso a paso

### 1. Crear la carpeta del proyecto

```sh
mkdir backend
cd backend
```

### 2. Crear la solución principal

La solución (.sln) organiza los distintos proyectos bajo un mismo entorno:

```sh
dotnet new sln
```

Esto generará un archivo `backend.sln`.

### 3. Crear los proyectos

### API

Punto de entrada de la aplicación (controladores y configuración inicial):

```sh
dotnet new webapi -o API --controllers
```

### Core

Contiene las entidades, reglas de negocio, y lógica de dominio:

```sh
dotnet new classlib -o Core
```

### Infrastructure

Contiene la lógica de acceso a datos, repositorios, servicios externos (SRI, correo, etc.), y migraciones de la base de datos:

```sh
dotnet new classlib -o Infrastructure
```

### 4. Agregar los proyectos a la solución

```sh
dotnet sln add API
dotnet sln add Core
dotnet sln add Infrastructure
```

### 5. Agregar las referencias entre proyectos

```sh
# Desde el proyecto API
dotnet add reference ../Core
dotnet add reference ../Infrastructure

# Desde el proyecto Infrastructure
dotnet add reference ../Core
```

### 6. Restaurar dependencias y compilar

```sh
dotnet restore
dotnet build
```

## Migraciones y Base de Datos

Cada vez que se realicen cambios en las entidades del dominio (por ejemplo, agregar campos, relaciones, o nuevas tablas), deben ejecutarse los siguientes pasos para actualizar la base de datos.

## 1. Crear una nueva migración

Desde la carpeta raíz del proyecto (donde está el .sln):

```sh
dotnet ef migrations add NombreDeLaMigracion -p Infrastructure/ -s API/
```

## 2. Aplicar la migración a la base de datos

```sh
dotnet ef database update -p Infrastructure/ -s API/
```

## Notas importantes

### Despliegue en Railway:
Cuando se realicen migraciones en el entorno de producción, es necesario conectarse al contenedor o entorno de Railway desde la terminal.
Antes de ejecutar el comando de actualización de la base de datos (dotnet ef database update), reemplaza temporalmente las variables de conexión locales por las variables externas de Railway (por ejemplo, el ConnectionString de producción).
Esto garantiza que la migración se aplique correctamente sobre la base de datos en la nube.

### Importante:
Una vez ejecutada la migración, vuelve a restaurar las variables internas (locales) en el archivo .env o en las configuraciones del proyecto.
Esto evita conexiones innecesarias a la base de datos de producción y, por tanto, previene consumos o costos adicionales en Railway.