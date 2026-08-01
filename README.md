# Sistema de Análisis de Ventas - ETL & Data Warehouse

Sistema completo de extracción, transformación y carga (ETL) desarrollado en .NET 9. Su objetivo es consolidar información comercial proveniente de múltiples orígenes en un Data Warehouse bajo un esquema de estrella (Star Schema), optimizando las consultas para su posterior análisis en herramientas de Business Intelligence.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server)
[![Entity Framework Core](https://img.shields.io/badge/EF_Core-33b27f?style=for-the-badge&logo=nuget&logoColor=white)](https://docs.microsoft.com/ef/core/)
[![Power BI](https://img.shields.io/badge/Power_BI-F2C811?style=for-the-badge&logo=powerbi&logoColor=black)](https://powerbi.microsoft.com/)

---

## Descripción del Proyecto

El proyecto implementa un modelo de datos estructurado para centralizar información de ventas. Está diseñado para procesar y cargar grandes volúmenes de datos aplicando prácticas de ingeniería de software como Clean Architecture y principios SOLID.

### Características Principales
- **Arquitectura Multifuente:** Extracción integrada desde archivos CSV, APIs REST y bases de datos relacionales.
- **Worker Service:** El proceso ETL opera como un servicio en segundo plano, adecuado para automatización de tareas.
- **Optimización de Carga:** Uso de operaciones DDL (TRUNCATE TABLE) y ejecución asíncrona concurrente para minimizar los tiempos de procesamiento.
- **Trazabilidad:** Sistema de logging estructurado para facilitar la auditoría y depuración técnica.

---

## Arquitectura del Data Warehouse

El modelo de base de datos (DW_AnalisisVentas) utiliza un esquema de estrella para garantizar un rendimiento óptimo en procesos de lectura analítica.

### Tablas de Dimensión
- DimCustomer: Atributos descriptivos de los clientes (Nombre, Email, Ciudad, País).
- DimProduct: Información del catálogo de productos y sus categorías.
- DimDate: Dimensión temporal para el análisis histórico.
- DimSource: Registro del sistema de origen (CSV, API, Base de Datos Externa).

### Tabla de Hechos
- FactSales: Almacena métricas transaccionales (Cantidad, Precio Unitario, Venta Total) y las llaves foráneas correspondientes al modelo dimensional.

---

## Flujo del Proceso ETL

El motor de la aplicación ejecuta una secuencia definida de operaciones:

1. **Limpieza (Clean):**
   - Vaciado de la tabla de hechos mediante TRUNCATE TABLE.
   - Limpieza de las tablas de dimensión.
2. **Extracción (Extract):**
   - Ejecución concurrente de extractores inyectados por dependencias.
   - Procesamiento de archivos .csv, peticiones HTTP y consultas T-SQL.
3. **Transformación (Transform):**
   - Mapeo y normalización de los datos extraídos hacia un Data Transfer Object unificado en memoria.
4. **Carga (Load):**
   - Inserción transaccional de dimensiones y hechos utilizando Entity Framework Core.

---

## Integración con Power BI

El Data Warehouse resultante está preparado para su conexión con Power BI. 

### Pasos de conexión:
1. Abrir Power BI Desktop.
2. Seleccionar Obtener datos > SQL Server.
3. Indicar el servidor local y la base de datos DW_AnalisisVentas.
4. Importar las tablas dimensionales y de hechos. El modelo de estrella será detectado automáticamente mediante las relaciones configuradas.

### Análisis Sugeridos:
- **Distribución Geográfica:** Relación entre DimCustomer.CountryName y FactSales.TotalSale.
- **Evolución Temporal:** Análisis de ingresos utilizando la jerarquía de DimDate.
- **Rendimiento por Fuente:** Contribución de ventas categorizada mediante DimSource.

---

## Configuración y Despliegue

### Requisitos
- .NET 9 SDK
- SQL Server (LocalDB, Express o Developer)

### Ejecución Local

1. **Clonar el repositorio**
```bash
git clone https://github.com/Johanvasquezdev/ETLVentas-Warehouse.git
cd ETLVentas-Warehouse
```

2. **Configurar Cadenas de Conexión**
En el archivo ETLVentas.DW.workerLoad/appsettings.json, modificar las cadenas de conexión según el entorno:
```json
"ConnectionStrings": {
    "DWConnection": "Server=localhost;Database=DW_AnalisisVentas;Trusted_Connection=True;TrustServerCertificate=True;Command Timeout=300;",
    "ExternalDbConnection": "Server=localhost;Database=AnalisisDeVentas;Trusted_Connection=True;TrustServerCertificate=True;Command Timeout=300;"
}
```

3. **Ejecutar la API (Fuente de Datos)**
```bash
cd ETLVentas.DW.API
dotnet run --urls="https://localhost:7001"
```

4. **Ejecutar el Proceso ETL**
En una terminal separada:
```bash
cd ETLVentas.DW.workerLoad
dotnet run
```
