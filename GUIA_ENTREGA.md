# Invernadero 7 - Guia de entrega y sustentacion

## Propuesta
Juego 3D de exploracion y reparacion en primera persona. Una averia detiene
el sistema de riego. El jugador restaura tres circuitos mediante puzzles y
regresa a la consola central. Un robot averiado patrulla y persigue al jugador.
Se gana al reparar todo y activar la consola; se pierde por tiempo o energia.
Duracion objetivo: 3 a 6 minutos, a confirmar con tu prueba real.

## Arquitectura
| Archivo | Responsabilidad |
|---|---|
| WorldBuilder.cs | Construye escenario, materiales, luces, vegetacion, jugador, terminales, robot y particulas al comenzar. |
| GameSession.cs | Estado de partida, temporizador, energia, puzzles, progreso, interfaz, pausa y finales. |
| PlayerMotor.cs | Movimiento con CharacterController, camara y Raycast de interaccion. |
| RepairTerminal.cs | Enlace entre terminal fisico y puzzle o consola central. |
| MaintenanceRobot.cs | Estados Patrol, Chase y Search, deteccion y movimiento por rutas. |
| GridNavigator.cs | Rejilla de navegacion calculada una vez y busqueda A*. |
| SoundBank.cs | Generacion inicial de clips, ambiente y reproduccion reutilizando AudioSource. |
| ProjectTools.cs | Herramientas exclusivas del Editor para escena y Build. |

## IA y optimizacion que puedes demostrar
- Patrol: recorre seis puntos de referencia, con rutas A*.
- Chase: detecta al jugador hasta 12 m si los maceteros no bloquean la vision.
- Search: busca en la ultima posicion conocida y luego retoma patrulla.
- Un Raycast de deteccion cada 0.2 s (5 por segundo), no cada frame.
- Recalculo de ruta cada 0.5 s (2 por segundo), no cada frame.
- La rejilla comprueba obstaculos estaticos una vez. Usa la capa 8.
- Dos luces direccionales: solo una proyecta sombras. Las tiras luminosas
  son materiales emisivos, no cientos de luces.
- Sonidos y materiales se crean al principio y se reutilizan en los eventos.
- Los efectos de agua tienen un maximo de 90 particulas por sistema.

Estas son tecnicas implementadas. Su impacto en rendimiento debe medirse en
el equipo real con Stats o Profiler; no inventes cifras de FPS ni porcentajes.

## Plan de pruebas (rellena con resultados reales)
| Caso | Accion | Resultado esperado | Resultado observado |
|---|---|---|---|
| Inicio | Play, luego Comenzar | Menu, controles y temporizador activos | Pendiente |
| Movimiento | WASD, raton y Shift | Desplazamiento correcto sin atravesar paredes | Pendiente |
| Colisiones | Chocar con paredes y maceteros | Detencion sin cruzarlos | Pendiente |
| Interaccion | Mirar panel a menos de 3.5 m, E | Indica accion y abre puzzle | Pendiente |
| Ambar | Probar 5 y luego 6 | Rechaza 5, acepta 6, progreso aumenta | Pendiente |
| Azul | Fallar y luego completar secuencia | Reinicia paso al fallar y acepta orden correcto | Pendiente |
| Violeta | Alinear canales y conectar | Solo acepta GOTA, HOJA, SOL | Pendiente |
| Repeticion | Usar panel ya reparado | No duplica el progreso | Pendiente |
| IA patrulla | Observar al robot sin acercarte | Recorre puntos y rodea maceteros | Pendiente |
| IA persecucion | Acercarte con vision despejada | Sensor rojo y persecucion | Pendiente |
| IA perdida de vision | Ocultarte tras macetero | Busca y luego retoma patrulla | Pendiente |
| Dano | Permitir contacto con robot | Resta 25 con 2.5 s entre impactos | Pendiente |
| Pausa | Esc fuera de puzzle | Se detienen tiempo e IA | Pendiente |
| Agua y audio | Reparar circuito | Particulas de agua y confirmacion sonora | Pendiente |
| Consola temprana | Activar sin reparar todo | Indica circuitos faltantes | Pendiente |
| Victoria | Reparar 3 y activar consola | Pantalla de victoria, cursor libre | Pendiente |
| Derrota | Esperar 6 min o perder energia | Pantalla de derrota | Pendiente |
| Reinicio | Volver a intentar | Regresa al menu, datos restablecidos | Pendiente |
| Build | Ejecutar .exe fuera del Editor | Partida completa con audio e interfaz | Pendiente |

Revisa tambien que el raton quede libre al pausar y que la interfaz se vea
en 1280x720 y en el tamaño de pantalla que usaras para grabar.

## Video propuesto (6 minutos)
- 0:00-0:40: nombre, premisa, objetivo y controles.
- 0:40-2:50: gameplay, interaccion y resolucion de los tres puzzles.
- 2:50-3:40: patrulla, deteccion, persecucion, cobertura y dano.
- 3:40-4:10: agua, audio, HUD y condicion de victoria.
- 4:10-5:20: mostrar scripts de IA, rejilla y frecuencias de calculo.
- 5:20-6:00: mostrar Build fuera de Unity y comentar pruebas reales.
Ajusta las tomas a la partida real para que el video completo dure 5 a 7 min.

## Informe solicitado: 4 a 6 paginas PDF
1. Portada breve, descripcion, objetivo y controles.
2. Entorno 3D y mecanicas. Capturas de gameplay y puzzles.
3. Sistemas: IA, interfaz, audio y particulas. Captura del robot y del riego.
4. Arquitectura y tecnica de optimizacion, con fragmento de codigo relevante.
5. Tabla de pruebas REALES, resultados, limitaciones y conclusiones.
6. Si hace falta: enlaces al repositorio, Build y video con permisos probados.
No afirmes que las pruebas estan aprobadas hasta realizarlas.

## Archivos de entrega
- Proyecto GitHub: Assets, Packages, ProjectSettings, documentos y .gitignore.
- No subas Library, Temp, Logs ni el ejecutable al repositorio fuente.
- Entrega el Build en ZIP separado, o enlace accesible si la plataforma limita tamaño.
- Video de 5 a 7 minutos y PDF de 4 a 6 paginas.
- Probar enlaces con otra cuenta o en incognito antes de entregar.
Este paquete no publica nada automaticamente en GitHub.

## Referencias tecnicas
Documentacion oficial de Unity (consultada para la estructura de herramientas):
- https://docs.unity3d.com/6000.0/Documentation/ScriptReference/BuildPipeline.BuildPlayer.html
- https://docs.unity3d.com/6000.0/Documentation/ScriptReference/TextMesh.html
- https://docs.unity3d.com/6000.0/Documentation/ScriptReference/ParticleSystem.html
- https://docs.unity3d.com/6000.0/Documentation/ScriptReference/UnityEngine.IMGUIModule.html
