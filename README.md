<div align="center">
  
# 🚀 ETL & Data Warehouse - Análisis de Ventas
  
**Un sistema completo de extracción, transformación y carga (ETL) construido en .NET 9 para poblar un Data Warehouse corporativo con Arquitectura en Estrella (Star Schema), optimizado para Inteligencia de Negocios (BI) y grandes volúmenes de datos.**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server)
[![Entity Framework Core](https://img.shields.io/badge/EF_Core-33b27f?style=for-the-badge&logo=nuget&logoColor=white)](https://docs.microsoft.com/ef/core/)
[![Power BI](https://img.shields.io/badge/Power_BI-F2C811?style=for-the-badge&logo=powerbi&logoColor=black)](https://powerbi.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-ff69b4?style=for-the-badge)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

</div>

---

## 📖 Sobre el Proyecto

Este proyecto es una solución a nivel empresarial que implementa un modelo de datos estructurado (Data Warehouse) diseñado específicamente para centralizar la información de ventas proveniente de múltiples orígenes. Está pensado para procesar **decenas de miles de registros en milisegundos**, utilizando las mejores prácticas de ingeniería de datos.

### 🌟 Características Principales:
- **Multifuente (Multi-Source):** Extracción automatizada y unificada desde Archivos **CSV**, **APIs REST**, y **Bases de Datos Relacionales (SQL)**.
- **Worker Service Integrado:** El proceso ETL está construido como un servicio en segundo plano (Background Worker), ideal para despliegues programados en la nube o servidores on-premise.
- **Rendimiento Extremo:** Optimizado con operaciones DDL (TRUNCATE TABLE) e Inserciones por Lotes (Bulk Inserts) para limpiar y repoblar las tablas de hechos de manera casi instantánea.
- **Limpieza de Consola y Logging:** Salida de logs limpia y estéticamente organizada para auditoría rápida en terminal.

---

## 🏗 Arquitectura del Data Warehouse

El Data Warehouse (DW_AnalisisVentas) utiliza un **Modelo de Estrella (Star Schema)**, que es el estándar de oro en Inteligencia de Negocios (BI), garantizando tiempos de lectura ultrarrápidos para herramientas analíticas.

### Tablas de Dimensión (Dimensiones)
- 🏬 DimCustomer: Detalles de los clientes (Nombre, Email, Ciudad, País).
- 📦 DimProduct: Detalles de los productos comercializados y categorías.
- 📅 DimDate: Dimensión de tiempo para permitir análisis de tendencias (Año, Mes, Día, Trimestre).
- 🔗 DimSource: Trazabilidad del origen del dato (CSV, API, Base de Datos Externa).

### Tabla de Hechos (Fact Table)
- 📊 FactSales: Almacena las métricas transaccionales (Cantidad, Precio Unitario, Venta Total) y las Llaves Foráneas (Foreign Keys) que enlazan con todas las dimensiones.

---

## ⚙️ El Proceso ETL (Extract, Transform, Load)

El motor de la aplicación sigue una tubería (pipeline) secuencial y robusta:

1. **Clean (Limpieza):**
   - Vaciado ultrarrápido de la tabla de hechos (TRUNCATE TABLE FactSales).
   - Limpieza de las tablas de dimensión con reseteo de semillas de identidad (DBCC CHECKIDENT).
2. **Extract (Extracción):**
   - Se inician simultáneamente extractores inyectados por dependencias.
   - Lee archivos desnormalizados (.csv), realiza peticiones HTTP a la API (GET /api/ventas), y ejecuta consultas T-SQL en bases de datos legadas.
3. **Transform (Transformación):**
   - Todos los datos provenientes de distintas fuentes son mapeados a un **Data Transfer Object Unificado (VentaExtraidaDto)**.
4. **Load (Carga):**
   - Generación en memoria de todas las Dimensiones únicas.
   - Generación de la tabla de Hechos referenciando las nuevas Dimensiones.
   - Inserción transaccional usando **Entity Framework Core 9**.

---

## 📊 Integración con Power BI y Dashboards

Este Data Warehouse está **preparado desde el primer día para conectarse con Power BI** u otras herramientas analíticas (Tableau, Looker, Excel).

### ¿Cómo conectarse?
1. Abre **Power BI Desktop**.
2. Haz clic en **Obtener datos** > **SQL Server**.
3. Ingresa tu servidor local (localhost o tu instancia SQL) y la base de datos DW_AnalisisVentas.
4. Importa las tablas DimCustomer, DimDate, DimProduct, DimSource y FactSales.
5. Power BI **detectará automáticamente el modelo de estrella** gracias a las llaves foráneas definidas.

### Análisis Sugeridos (Insights):
- **Ventas por Geografía:** Mapa de calor de clientes cruzando DimCustomer.CountryName con FactSales.TotalSale.
- **Evolución Temporal:** Gráfico de líneas cruzando DimDate.MonthName y DimDate.Year con ingresos totales.
- **Rendimiento de Origen:** Gráfico de pastel para ver qué fuente (DimSource) aporta más ventas a la compañía.

---

## 🛠 Instalación y Configuración

### Prerrequisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- SQL Server (LocalDB, Express, o Developer)

### Pasos de Ejecución

1. **Clonar el repositorio**
`ash
git clone https://github.com/Johanvasquezdev/ETLVentas-Warehouse.git
cd ETLVentas-Warehouse
`

2. **Configurar Cadenas de Conexión**
En el archivo ETLVentas.DW.workerLoad/appsettings.json, asegúrate de apuntar a tu servidor SQL local.
`json
"ConnectionStrings": {
    "DWConnection": "Server=localhost;Database=DW_AnalisisVentas;Trusted_Connection=True;TrustServerCertificate=True;Command Timeout=300;",
    "ExternalDbConnection": "Server=localhost;Database=AnalisisDeVentas;Trusted_Connection=True;TrustServerCertificate=True;Command Timeout=300;"
}
`

3. **Arrancar la API Simulada (Fuente de Datos)**
`ash
cd ETLVentas.DW.API
dotnet run --urls="https://localhost:7001"
`

4. **Arrancar el Motor ETL (Worker)**
Abre otra terminal y ejecuta:
`ash
cd ETLVentas.DW.workerLoad
dotnet run
`

---

<div align="center">
  <i>Desarrollado con pasión utilizando C#, .NET 9 y Clean Architecture.</i> 🚀
</div>
