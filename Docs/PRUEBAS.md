# Registro de pruebas PA4

Estado al preparar este paquete: revisión de código realizada; compilación Unity, juego y build **pendientes**. Completar resultado, fecha y evidencia tras ejecutar. La validación del menú comprueba configuración; no prueba gameplay.

| Caso | Acción | Resultado esperado | Resultado observado |
|---|---|---|---|
| Inicio | Abrir juego y esperar | Briefing, cursor libre, tiempo inmóvil, jugador detenido | Pendiente |
| Movimiento | WASD, diagonal, Shift y espacio | Movimiento normalizado, cámara sin saltos, gravedad y salto coherentes | Pendiente |
| Colisión | Chocar con paredes, cajas, techo y caminar rampa | Sin atravesar objetos, sin caída en uniones | Pendiente |
| Terminal A B C | Mirar y mantener E; salir del rango; volver | Aumenta progreso solo al enfocar, conserva avance, cuenta una sola reparación | Pendiente |
| Alcance y bloqueo | Intentar usar consola desde lejos o tras una pared | Raycast bloqueado, sin activación | Pendiente |
| Reactor prematuro | Usar reactor con 0, 1 y 2 terminales | Mensaje de bloqueo; sin extracción posible | Pendiente |
| Reactor | Completar 3 y usar consola | Shader/luz/partículas y sonido; objetivo cambia a extracción | Pendiente |
| IA patrulla | Observar desde fuera del rango | Recorre waypoints y rampa sin atascarse | Pendiente |
| IA persecución | Entrar en línea de visión | Cambia a Chase, se aproxima sin cruzar paredes | Pendiente |
| IA búsqueda | Ocultarse tras geometría | Busca última posición durante 3 s y vuelve a patrulla | Pendiente |
| Daño | Dejar acercarse enemigo | Resta 20 como máximo cada 1.5 s | Pendiente |
| Derrota salud | Recibir cinco impactos | Pantalla de derrota; movimiento y tiempo detenidos | Pendiente |
| Derrota tiempo | Esperar 5 minutos en lugar seguro | Derrota por colapso | Pendiente |
| Victoria | Reactor estable y E en extracción | Pantalla de misión completada | Pendiente |
| Pausa | Esc durante movimiento o reparación | Tiempo, progreso y IA detenidos; cursor libre; continuar funciona | Pendiente |
| Reinicio | Desde pausa, victoria y derrota | 100 salud, 0 terminales, reactor inactivo, tiempo 300, briefing | Pendiente |
| Audio y VFX | Inicio, reparación, daño, reactor y fin | Ambiente y señales audibles; partículas sin cuadrados magenta | Pendiente |
| Pantalla | 1280×720 y 1920×1080; ventana reducida | HUD y botones legibles y dentro de pantalla | Pendiente |
| Optimización | Profiler + diagnóstico de IA durante 20 s | Intervalo configurado 0.2 s; registrar CPU y cantidad real de decisiones | Pendiente |
| Build externo | Ejecutar .exe con carpeta completa | Mismo flujo y controles, sin error crítico | Pendiente |
| Duración | Cronometrar partida de alguien nuevo | Medir duración y ajustar para aproximarse a 3–7 min | Pendiente |

No escribir “aprobado” sin haber ejecutado el caso. Registrar errores y repetir solamente los casos afectados tras corregirlos. Un playtest externo ayuda a comprobar si las instrucciones se entienden.
