## WalletApp.API

Prueba técnica para el cargo de **Backend Developer en Payphone**.  
Consiste en una API REST que permite gestionar billeteras digitales y realizar transferencias entre ellas, con seguridad, validaciones y pruebas.

---

## Tecnologías utilizadas

- **.NET 8**
- **Entity Framework Core**
- **SQLite** (para ejecución)
- **InMemoryDB** (para pruebas)
- **Clean Architecture**
- **Autenticación JWT**
- **xUnit + FluentAssertions**
- **Swagger (OpenAPI)**

---

## Funcionalidades implementadas

- CRUD de billeteras (`/api/wallets`)
- Transferencias entre billeteras (`/api/movements/transfer`)
- Historial de movimientos por billetera (`/api/movements/wallet/{id}`)
- Validaciones: saldo suficiente, billeteras existentes, datos correctos
- Autenticación JWT para proteger endpoints críticos
- Pruebas unitarias e integración con base en memoria
- Documentación interactiva vía Swagger

---

## Autenticación

Para acceder a endpoints protegidos, primero genera un token:
POST /api/auth/login { "username": "cualquiera", "password": "abc123" }
	

Luego copia el token y haz clic en el botón **"Authorize"** en Swagger.  
Formato: `Bearer eyJhbGciOi...`

---

## Endpoints y acceso

| Método | Ruta                                 | Requiere login |
|--------|--------------------------------------|----------------|
| POST   | `/api/auth/login`                    | NO             |
| GET    | `/api/wallets`                       | SI             |
| POST   | `/api/wallets`                       | SI             |
| PUT    | `/api/wallets/{id}`                  | SI             |
| DELETE | `/api/wallets/{id}`                  | SI             |
| POST   | `/api/movements/transfer`            | SI             |
| GET    | `/api/movements/wallet/{walletId}`   | SI             |

---

## Cómo ejecutar el proyecto

1. Clona el repositorio:
git clone https://github.com/mariela7/WalletAPI.git
cd WalletApp

2. Ejecuta la migración:
dotnet ef database update --project WalletApp.Infrastructure --startup-project WalletApp.API

3. Ejecuta la aplicación:
dotnet run --project WalletApp.API

## Cómo ejecutar los test
dotnet test

Incluye:
- Pruebas unitarias de servicios (WalletService, MovementService)
- Pruebas de integración reales con HTTP client simulando uso del API
- Validaciones de seguridad y errores esperados

## Autora
Mariela Párraga
Ingeniera en Sistemas - Backend Developer
Manta, Ecuador
marielaparragab@gmail.com
