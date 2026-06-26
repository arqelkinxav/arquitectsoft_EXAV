# arquitectSoft (EXAV)

Aplicación de escritorio **Windows (WinForms, C# / .NET Framework 4.7.2)** para
procesar despieces: lee archivos **TXT**, los procesa contra un **catálogo en
MySQL** y exporta los resultados a **Excel**.

> Este repositorio es la copia de trabajo propia de EXAV. Base de datos: **MySQL 8.0**.

---

## 1. Requisitos

| Herramienta | Versión | Para qué |
|---|---|---|
| **MySQL Server** | 8.0.x | Base de datos (usa colación `utf8mb4_0900_ai_ci`, exclusiva de MySQL 8 — **no** sirve MariaDB/XAMPP) |
| **MySQL Workbench** | cualquiera | Ver/editar la base con interfaz gráfica (opcional pero recomendado) |
| **Visual Studio** | 2022 o superior | Compilar y ejecutar (incluye MSBuild y NuGet) |

---

## 2. Montar la base de datos local (primera vez)

> Los pasos usan la línea de comandos. La ruta de `mysql.exe` suele ser:
> `C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe`

**a) Crear la base:**
```sql
CREATE DATABASE arquitectdb CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
```

**b) Permitir crear funciones** (sin esto, la importación falla con `ERROR 1418`):
```sql
SET GLOBAL log_bin_trust_function_creators = 1;
```

**c) Importar el dump más reciente** (incluye tablas, datos, procedimientos y funciones):
```bash
mysql -u root -p arquitectdb < "Database Backup/Dump20220616.sql"
```

**d) Crear el usuario que el programa espera** (`remote` / `poseidon`):
```sql
CREATE USER 'remote'@'localhost' IDENTIFIED BY 'poseidon';
CREATE USER 'remote'@'%'         IDENTIFIED BY 'poseidon';
GRANT ALL PRIVILEGES ON arquitectdb.* TO 'remote'@'localhost';
GRANT ALL PRIVILEGES ON arquitectdb.* TO 'remote'@'%';
FLUSH PRIVILEGES;
```

**Verificar que entró todo** (debe dar 18 tablas, 24 procedimientos, 2 funciones):
```sql
SELECT COUNT(*) FROM information_schema.tables    WHERE table_schema='arquitectdb' AND table_type='BASE TABLE';
SELECT COUNT(*) FROM information_schema.routines  WHERE routine_schema='arquitectdb' AND routine_type='PROCEDURE';
SELECT COUNT(*) FROM information_schema.routines  WHERE routine_schema='arquitectdb' AND routine_type='FUNCTION';
```

---

## 3. Compilar y ejecutar

```bash
# 1) Restaurar las librerías (NuGet)
nuget restore arquitectSoft.sln

# 2) Compilar
MSBuild arquitectSoft.sln -p:Configuration=Debug
```

O simplemente: abrir `arquitectSoft.sln` en Visual Studio y pulsar **F5**
(VS restaura los paquetes y compila solo).

El ejecutable queda en `arquitectSoft/bin/Debug/arquitectSoft.exe`.

**Login del programa:** usuario `admin`, contraseña `admin08`.

---

## 4. ¿Dónde se configura la conexión a la base?

En [`arquitectSoft/Generals/Conexion.cs`](arquitectSoft/Generals/Conexion.cs)
(arriba de todo). Cambia `host` según dónde esté la base:

```csharp
static string host     = "localhost";   // local en esta PC
//static string host   = "10.11.0.254"; // servidor de la empresa
static string database = "arquitectdb";
static string userDB   = "remote";
static string password = "poseidon";
```

> El SQL que usa el programa está en
> [`arquitectSoft/Generals/Constantes.cs`](arquitectSoft/Generals/Constantes.cs).

---

## 5. Estructura del proyecto

```
arquitectSoft/
├─ FrmLogin.cs, FrmMDIPrincipal.cs ...   Formularios (pantallas)
├─ View/                                 Más formularios
├─ Dto/                                  Clases de datos (una fila = un objeto)
├─ Generals/
│  ├─ Conexion.cs                        Única clase que abre MySQL  ← conexión
│  └─ Constantes.cs                      Todo el SQL como texto
├─ Database Backup/                      Dumps .sql de la base
└─ libs/MySqlBackup.dll                  Librería de backup (vendorizada)
```

---

## 6. Problemas comunes

| Síntoma | Causa | Solución |
|---|---|---|
| `ERROR 1418` al importar | MySQL bloquea crear funciones | Ejecutar `SET GLOBAL log_bin_trust_function_creators = 1;` antes de importar |
| Los análisis fallan tras restaurar | El backup del *programa* (MySqlBackup) **no** incluye procedimientos | Restaurar desde un dump hecho con `mysqldump --routines` |
| No compila: falta `MySqlBackup` | (Histórico) referenciaba una DLL fuera del repo | Ya resuelto: la DLL está en `libs/` |
| `CS1705` versión de MySql.Data | Desajuste de versiones | El proyecto usa `MySql.Data 8.0.32` (alineado con MySqlBackup.NET) |
| `NullReferenceException` al cargar TXT | El servidor MySQL en modo estricto rechaza un parámetro vacío que el programa manda a un procedimiento (`ERROR 1366`) | Quitar `STRICT_TRANS_TABLES` del `sql-mode` en `my.ini` (ver más abajo) — el servidor de la empresa corre en modo no estricto |
| `FileNotFoundException: LOGO.jpg` al exportar | Faltaba el logo junto al `.exe` | Ya resuelto: el csproj copia `Resources\LOGO.jpg` a la salida en cada build |

---

## 7. Notas

- Las tablas `proyecto`, `proyecto_pt`, `tbauxanchura` son **temporales de cálculo**
  (se vacían en cada corrida). El programa **no** archiva proyectos; el dato valioso
  y persistente es el **catálogo** (componentes, subcomponentes, acabados, etc.).
- **`sql_mode` no estricto (importante).** El programa pasa a veces parámetros
  numéricos vacíos a los procedimientos. Para que funcione, el servidor MySQL debe
  correr **sin** `STRICT_TRANS_TABLES`. En Windows se ajusta en
  `C:\ProgramData\MySQL\MySQL Server 8.0\my.ini` (línea `sql-mode=...`) y se reinicia
  el servicio. **El mismo ajuste hay que aplicarlo en cualquier servidor** (incluida la nube).
- **Configuración regional (punto vs coma).** Varios cálculos parsean números usando
  la configuración regional de Windows. Si los TXT traen decimales con un separador
  distinto al de Windows, los números se malinterpretan. Por ahora hay que igualar el
  símbolo decimal de Windows al de los TXT (pendiente arreglar en el código para que
  use un formato fijo e independiente de la máquina).
- `bin/`, `obj/` y `packages/` no se suben al repo (se regeneran al compilar/restaurar).
