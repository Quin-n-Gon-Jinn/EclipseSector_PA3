# Eclipse Sector PA4

Avance de implementación del final sobre el PA3. Este paquete contiene código y herramientas de preparación. **No es todavía una entrega final validada: no incluye un ejecutable probado, capturas del PA4 ni video.**

## Abrir y preparar en la laptop

1. En Unity Hub, abre la carpeta raíz que contiene Assets, Packages y ProjectSettings. Usa Unity **6000.6.0f1**, la versión registrada en el PA3. No abras solamente Assets.
2. Espera la importación de paquetes. Revisa Console y corrige cualquier error rojo antes de continuar.
3. Ejecuta **Tools > Eclipse Final > 1 Preparar misión PA4**. Crea `Assets/Scenes/EclipseFinal.unity`, los materiales y el NavMesh a partir de la escena original. No reconstruyas el blockout con los menús antiguos del PA3.
4. En Game, pulsa Play e INICIAR MISIÓN. El menú de preparación conserva una escena final ya existente: no borra ajustes posteriores.
5. Prueba todos los casos de `PRUEBAS.md`. Ajusta posiciones si algún panel queda oculto. La cámara del PA4 se coloca correctamente dentro del jugador y la cinemática antigua queda deshabilitada en la copia.
6. Ejecuta **Tools > Eclipse Final > 3 Generar Windows x64**. Instala Windows Build Support desde Unity Hub si se solicita. El resultado va a `Builds/Windows/EclipseSector.exe`.
7. Abre el ejecutable fuera del Editor y repite victoria, derrota, pausa, reinicio, audio y navegación. Guarda evidencia. Para compartirlo, comprime **toda la carpeta Windows**, incluyendo `_Data`, UnityPlayer.dll y demás archivos; el .exe solo no basta.

## Objetivo y controles

Tres terminales ámbar: sala inicial (A), pasillo (B), extremo izquierdo de sala del reactor (C). Mira el panel y mantén E; el progreso se conserva al interrumpir. Al completar los tres, activa con E la consola azul del reactor y regresa al panel verde de extracción en la pared posterior de la sala inicial.

WASD, mouse, Shift, espacio, E y Esc. El centinela patrulla por waypoints, persigue con detección por distancia y Raycast, y busca en la última posición conocida. Cada impacto resta 20 de integridad con 1.5 segundos de protección. Derrota por integridad cero, tiempo agotado o caída fuera del nivel. Hay pausa y reinicio. El límite es 5 minutos; **la duración real no está medida**. Ajustar para una experiencia de aproximadamente 3 a 7 minutos sin esperas artificiales.

## Responsabilidades de los scripts

- MissionController: estados, objetivo, tiempo, daño y resultado.
- PlayerInteractor: Raycast desde cámara; interacción sin atravesar paredes.
- RepairTerminal, ReactorConsole, ExtractionConsole: progresión y condiciones.
- SentinelAI: máquina de estados y navegación con NavMeshAgent.
- MissionHUD: información, instrucciones, pausa y resultado con IMGUI escalado a la pantalla.
- MissionAudio: ambiente y señales. Sonidos sintetizados originales guardados en WAV.
- EclipseFinalBuilder: escena final, referencias, NavMesh, validación y build explícito.

## Optimización que se puede explicar

La decisión de IA se ejecuta cada 0.2 segundos (aproximadamente 5 veces por segundo), mientras el NavMeshAgent mantiene el movimiento. SetDestination se actualiza solo cuando el destino cambia al menos 0.5 unidades o no existe ruta. Se conservan luces baked del PA3 y las luces nuevas de indicadores no generan sombras. No se declara una mejora porcentual sin medir con Profiler.

## Entregables restantes

PDF de 4 a 6 páginas con capturas reales y resultados observados; repositorio completo con escena final y NavMesh generados; build probado; video de 5 a 7 minutos. El informe provisional y guion de este paquete facilitan completarlos, pero no sustituyen la ejecución real.

## Trabajo individual

La consigna es grupal. En clase 8, 1:34:17–1:35:14, ante falta de respuesta, el docente indica intentar nuevamente la comunicación e incluir la evidencia en el informe. Adjuntar el nuevo intento y el anterior con fechas; no atribuir trabajo a otros ni asumir autorización incondicional o nota garantizada.
