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
| `GET`  | `/api/v1/pokemon?page=1&pageSize=20&search=&type=&generation=` | Listado paginado con búsqueda por nombre/número (coincidencia parcial) y filtros por tipo y generación |
| `GET`  | `/api/v1/pokemon/{nameOrId}` | Ficha completa (stats, tipos, habilidades, sprites, movimientos, evolución) |
| `GET`  | `/api/v1/pokemon/{nameOrId}/evolution` | Cadena evolutiva |
| `GET`  | `/api/v1/types` | Tipos disponibles con el número de Pokémon de cada uno |
| `GET`  | `/api/v1/recipes?page=1&pageSize=12&search=&category=&difficulty=&maxMinutes=&tag=` | Recetario con búsqueda y filtros |
| `GET`  | `/api/v1/recipes/{idOrSlug}` | Receta completa (ingredientes, pasos, alérgenos) |
| `GET`  | `/api/v1/recipes/facets` | Valores filtrables del recetario con su recuento |
| `GET`  | `/api/v1/seafood?page=1&pageSize=12&search=&category=&sustainability=&maxPrepMinutes=` | Guía de mariscos con búsqueda y filtros |
| `GET`  | `/api/v1/seafood/{idOrSlug}` | Ficha de marisco (origen, preparación, sabor, maridaje, nutrición) |
| `GET`  | `/api/v1/seafood/facets` | Valores filtrables de la guía con su recuento |
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
- **Precalentado en arranque**: `CatalogWarmupService` construye en segundo plano el índice de tipos,
  el catálogo y los catálogos semilla. La primera petición real pasa de ~3 s a ~15 ms
  (`Warmup:Enabled` en `appsettings.json`).
- **Transporte**: respuestas comprimidas (Brotli/Gzip) y `Cache-Control: public, max-age=300` en los
  GET del catálogo, para que la app y los proxies no repitan descargas.
- **Caché `HybridCache`**: cada Pokémon, especie, movimiento y cadena evolutiva se cachea por separado.
- **Resiliencia**: `AddStandardResilienceHandler` aplica reintentos con backoff exponencial,
  *circuit breaker* y límites de tiempo en cada petición a PokeAPI. Los fallos de transporte
  (timeouts, circuit breaker, DNS) se traducen a `ExternalServiceException` → **502**, no 500.
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

> Si publicas la versión web (`npx expo export --platform web`), el servidor estático debe
> reescribir las rutas profundas a `index.html` (regla SPA habitual) para que el enrutado del
> lado del cliente funcione.

### Apuntar la app a la API

La app **detecta sola** el host correcto (probando `/health` en este orden, con 800 ms por candidato):

1. `EXPO_PUBLIC_API_URL` si está definida (copia `.env.example` a `.env`).
2. El host del servidor de desarrollo de Expo → sirve en un **dispositivo físico** por Wi-Fi.
3. `http://10.0.2.2:5080` en el **emulador de Android**, `http://localhost:5080` en iOS y web.

```bash
# .env  →  dispositivo físico en la misma red
EXPO_PUBLIC_API_URL=http://192.168.1.50:5080
```

> El emulador de Android alcanza el `localhost` del equipo mediante `10.0.2.2`.

### Funcionalidades

- **Pokédex**: listado paginado con scroll infinito, búsqueda por nombre o número con
  **coincidencia parcial** (`"chu"` → Pikachu, Raichu, Pichu; `"saur"` → Bulbasaur, Ivysaur,
  Venusaur) y filtros por tipo y generación. El buscador y los filtros son una **barra fija** que
  no se pierde al hacer scroll; se filtran en la misma pantalla contra la API y el chip activo se
  deselecciona al volver a pulsarlo.
- **Ficha**: arte oficial, datos básicos, descripción (español si PokeAPI la tiene), estadísticas,
  habilidades, cadena evolutiva y movimientos agrupados por método de aprendizaje.
- **Recetario**: 22 recetas inspiradas en Pokémon (ingredientes, pasos, dificultad, alérgenos y
  consejos) con búsqueda por título/ingrediente/Pokémon y filtros por categoría, dificultad y tiempo.
- **Seafood**: 26 entradas con el Pokémon y el marisco real que representa (origen, temporada,
  preparación, sabor, maridaje y valor nutricional) con búsqueda y filtros por categoría,
  sostenibilidad y tiempo de preparación.
- **Favoritos**: se guardan en el dispositivo con `AsyncStorage` y aparecen en su propia pestaña.
- **Sin red**: si el dispositivo arranca sin conexión, la app no deja entrar y muestra un mensaje de
  error con opción de reintentar; se desbloquea sola en cuanto vuelve la conexión. Si el estado de
  red no se puede determinar, la app **abre igual** (nunca se queda esperando).

Las cuatro secciones comparten el mismo componente de búsqueda y filtros (`CatalogToolbar`), y
todos los datos se sirven desde la API .NET (nunca directamente de PokeAPI).

### Rendimiento del arranque

- La API precalienta sus cachés en segundo plano, así que las peticiones del arranque responden
  en ~15 ms y comprimidas.
- Las consultas de las pestañas se lanzan al montar (React Query deduplica y cachea 5 min). Se
  probó a diferirlas por pestaña, pero dependía del foco del navegador nativo y podía dejar la
  pantalla cargando; se revirtió por fiabilidad.

### Datasets editoriales

`src/Pokedex.Infrastructure/Seed/*.seed.json` contiene el recetario y la guía de mariscos.
Se incrustan como recursos del ensamblado y se cargan en la caché `HybridCache` al arrancar.

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
├── _layout.tsx                  NativeTabs (Pokédex / Recetario / Seafood / Favoritos)
├── (pokedex)/
│   ├── _layout.tsx              Stack nativo
│   ├── index.tsx                Listado con búsqueda, filtro y scroll infinito
│   └── pokemon/[name].tsx       Ficha
├── recipes/
│   ├── _layout.tsx              Stack nativo
│   ├── index.tsx                Recetario con búsqueda y filtros
│   └── [slug].tsx               Receta completa
├── seafood/
│   ├── _layout.tsx              Stack nativo
│   ├── index.tsx                Guía de mariscos con búsqueda y filtros
│   └── [slug].tsx               Ficha de marisco
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

> Con las secciones nuevas son **63 tests**: Recetario, Seafood, sus endpoints, el transporte
> (compresión, caché) y la búsqueda por coincidencia parcial / filtro de generación.

---

## 4. Comprobaciones de calidad

```bash
cd mobile
npx tsc --noEmit    # tipado estricto
npx expo lint       # ESLint (eslint-config-expo)
npx expo-doctor     # diagnóstico del proyecto Expo
```
