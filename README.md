# Invernadero 7

Proyecto 3D para la evaluación sustitutoria de Desarrollo de Videojuegos.

## Abrir el proyecto correcto

Esta rama (`sustitutoria-invernadero7`) contiene el proyecto completo **Invernadero 7**.
Descarga el ZIP desde esta rama o clona indicando su nombre:

```bash
git clone --branch sustitutoria-invernadero7 --single-branch https://github.com/Quin-n-Gon-Jinn/EclipseSector_PA3.git Invernadero7
```

En Unity Hub, agrega la carpeta que contiene `Assets`, `Packages` y `ProjectSettings`.
Utiliza **Unity 6000.6.0f1**, con **Built-in Render Pipeline**.
Abre `Assets/Invernadero7/Scenes/Invernadero7.unity` y pulsa Play.
El escenario y sus sistemas se generan al iniciar la escena.

## Juego

Explora un invernadero, resuelve tres puzzles (presión, secuencia y canales) y vuelve a la consola central para restablecer el riego. Evita al robot de mantenimiento.

- Tiempo disponible: 360 segundos.
- Energía inicial: 100; contacto con robot: -25, con protección de 2,5 segundos entre impactos.
- Victoria: tres circuitos restaurados y consola activada.
- Derrota: tiempo o energía agotados.

| Control | Acción |
|---|---|
| WASD | Movimiento |
| Ratón | Cámara |
| Shift | Correr |
| E | Interacción |
| Esc | Cerrar panel o pausar |

## Sistemas

- Movimiento con CharacterController e interacción por Raycast.
- Tres puzzles con registro de progreso.
- IA de patrulla, persecución y búsqueda; rutas A* sobre una rejilla.
- HUD, menús, pausa, victoria, derrota y reinicio.
- Ambiente y efectos sonoros generados al inicio.
- Partículas de riego y materiales emisivos.
- Detección cada 0,2 s y actualización de rutas cada 0,5 s.
- Rejilla estática inicializada una vez; luces y partículas limitadas.

## Código

`WorldBuilder`, `GameSession`, `PlayerMotor`, `RepairTerminal`, `MaintenanceRobot`, `GridNavigator` y `SoundBank` separan las responsabilidades principales. `ProjectTools` aporta las opciones del Editor.

## Ejecutable de Windows

Detén Play y elige **Invernadero 7 > 2 - Generar ejecutable Windows**.
El resultado se crea en `Builds/Windows`. Para distribuirlo, comprime esa carpeta completa.
El ejecutable generado en la laptop no está incluido en este repositorio de fuentes.

## Pruebas

El 17 de octubre de 2026 se documentó la ejecución en el Editor y en Windows, incluyendo una partida con victoria, tres circuitos y 75 % de energía. El autor confirmó las pruebas funcionales de audio, derrota y reinicio. No se realizaron mediciones comparativas de FPS o CPU con Profiler.

Consulta `LEEME_PRIMERO.txt` y `GUIA_ENTREGA.md` para los controles, las soluciones de prueba y el plan de validación.
