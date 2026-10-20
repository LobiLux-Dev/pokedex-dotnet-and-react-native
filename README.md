# Pokédex — API en C# + app React Native (Expo 57)

Monorepo con una API REST en **.NET 10** que consume **PokeAPI** y una app móvil en
**React Native + Expo 57** con **navegación nativa** (Expo Router sobre `native-stack` y
native tabs del sistema).

```
pokedex/
├── src/
│   ├── Pokedex.Domain/         Entidades del dominio (sin dependencias externas)
│   ├── Pokedex.Application/    Casos de uso, DTOs, mapeo y abstracciones
│   ├── Pokedex.Infrastructure/ Cliente HTTP de PokeAPI, caché HybridCache y resiliencia
│   └── Pokedex.Api/            Controllers, Problem Details, Swagger y health check
├── tests/
│   └── Pokedex.Tests/          Tests unitarios + tests de integración de la API
└── mobile/                     App Expo 57 (Expo Router, native tabs + native stack)
```

---

## Requisitos

| Herramienta | Versión |
| ----------- | ------- |
| .NET SDK     | 10.0    |
| Node.js      | 22.13+ (probado con 26.11) |
| Expo         | SDK 57 (`expo@~57.0.30`) |

No hace falta Docker ni base de datos: los favoritos se guardan en el dispositivo.

---

## 1. Levantar la API

```bash
cd src/Pokedex.Api
dotnet run
```

La API queda escuchando en `http://0.0.0.0:5080` (ver `Properties/launchSettings.json`).

- Swagger UI: <http://localhost:5080/swagger>
- Health check: <http://localhost:5080/health>
- Documento OpenAPI: <http://localhost:5080/openapi/v1.json>

### Endpoints

| Método | Ruta | Descripción |
| ------ | ---- | ----------- |
| `GET`  | `/api/v1/pokemon?page=1&pageSize=20&search=&type=` | Listado paginado con búsqueda por nombre/número y filtro por tipo |
| `GET`  | `/api/v1/pokemon/{nameOrId}` | Ficha completa (stats, tipos, habilidades, sprites, movimientos, evolución) |
| `GET`  | `/api/v1/pokemon/{nameOrId}/evolution` | Cadena evolutiva |
| `GET`  | `/api/v1/types` | Tipos disponibles con el número de Pokémon de cada uno |
| `GET`  | `/health` | Health check |

Los errores usan **RFC 7807** (`application/problem+json`): 404 para Pokémon inexistente,
502 cuando PokeAPI no está disponible.

### Configuración (`appsettings.json`)

```json
"PokeApi": {
  "BaseUrl": "https://pokeapi.co/api/v2/",
  "SpriteBaseUrl": "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon",
  "ArtworkBaseUrl": "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork",
  "CatalogCacheDuration": "12:00:00",
  "DetailCacheDuration": "06:00:00",
  "MoveCacheDuration": "7.00:00:00",
  "MaxConcurrency": 6
}
```

### Decisiones de diseño del backend

- **Catálogo en memoria**: PokeAPI no ofrece búsqueda por nombre ni filtro por tipo, así que la
  API construye un índice (los ~1351 Pokémon y los 20 tipos, obtenidos de `/type/{name}`) y lo cachea
  12 h. Así la búsqueda y el filtrado son instantáneos y no se hacen N peticiones al listar.
- **Caché `HybridCache`**: cada Pokémon, especie, movimiento y cadena evolutiva se cachea por separado.
- **Resiliencia**: `AddStandardResilienceHandler` aplica reintentos con backoff exponencial,
  *circuit breaker* y límites de tiempo en cada petición a PokeAPI.
- **Errores**: los errores de negocio (`PokemonNotFoundException`) y de infraestructura
  (`ExternalServiceException`) se traducen a `ProblemDetails` en `GlobalExceptionHandler`.

---

## 2. Levantar la app móvil

```bash
cd mobile
npm install
npx expo start
```

Elige plataforma con `npx expo start --android | --ios | --web`, o pulsa `a` / `i` / `w` en la terminal.

### Apuntar la app a la API

La app **detecta sola** el host correcto (probando `/health` en este orden):

1. `EXPO_PUBLIC_API_URL` si está definida (copia `.env.example` a `.env`).
2. El host del servidor de desarrollo de Expo → sirve en un **dispositivo físico** por Wi-Fi.
3. `http://10.0.2.2:5080` en el **emulador de Android**, `http://localhost:5080` en iOS y web.

```bash
# .env  →  dispositivo físico en la misma red
EXPO_PUBLIC_API_URL=http://192.168.1.50:5080
```

> El emulador de Android alcanza el `localhost` del equipo mediante `10.0.2.2`.

### Funcionalidades

- **Listado** paginado con scroll infinito, búsqueda por nombre o número y filtro por tipo.
- **Ficha**: arte oficial, datos básicos, descripción (español si PokeAPI la tiene), estadísticas,
  habilidades, cadena evolutiva y movimientos agrupados por método de aprendizaje.
- **Favoritos**: se guardan en el dispositivo con `AsyncStorage` y aparecen en su propia pestaña.

### Navegación

| Capa | Implementación |
| ---- | -------------- |
| Pestañas | `expo-router/unstable-native-tabs` → `UITabBarController` (iOS) / `BottomNavigationView` (Android) |
| Pila | `expo-router` `<Stack>` → `@react-navigation/native-stack` sobre `react-native-screens` |

Ambas usan los componentes nativos de cada plataforma (transiciones, botón atrás, barra de
pestañas del sistema).

Árbol de rutas:

```
mobile/src/app/
├── _layout.tsx                  NativeTabs (pestañas Pokédex / Favoritos)
├── (pokedex)/
│   ├── _layout.tsx              Stack nativo
│   ├── index.tsx                Listado con búsqueda, filtro y scroll infinito
│   └── pokemon/[name].tsx       Ficha
└── favorites/
    ├── _layout.tsx              Stack nativo
    ├── index.tsx                Favoritos guardados
    └── pokemon/[name].tsx       Ficha (pila propia de la pestaña)
```

> Cada pestaña tiene su propia pila nativa, así abrir una ficha desde "Favoritos" no cambia de
> pestaña.

---

## 3. Tests

```bash
dotnet test
```

25 tests que cubren la capa de aplicación (paginación, filtros, búsqueda, normalización de la
query, mapeo) y la API completa vía `WebApplicationFactory` (enrutado, serialización y
`ProblemDetails`), con el repositorio de PokeAPI sustituido por un doble en memoria.

---

## 4. Comprobaciones de calidad

```bash
cd mobile
npx tsc --noEmit    # tipado estricto
npx expo lint       # ESLint (eslint-config-expo)
npx expo-doctor     # diagnóstico del proyecto Expo
```
