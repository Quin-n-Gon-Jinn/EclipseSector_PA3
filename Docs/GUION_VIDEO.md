# Guion de demostración PA4

Duración objetivo 6 minutos. Grabar cuando la escena final y el build estén probados. Mostrar gameplay real. No afirmar optimizaciones medidas ni pruebas realizadas sin evidencia.

| Tiempo | Pantalla | Explicación |
|---|---|---|
| 0:00–0:40 | Briefing y comienzo en ejecutable | Soy Franco Anthony Ayala Odar. Presento Eclipse Sector, un juego 3D de exploración y supervivencia. Debemos reparar tres terminales, estabilizar el reactor y regresar a extracción. |
| 0:40–1:35 | Movimiento, cámara, rampa, terminal | El entorno conserva los materiales y la iluminación del PA3. El HUD presenta integridad, tiempo y progreso. La interacción se realiza mirando el panel dentro del alcance de un Raycast; mantener E realiza la reparación. |
| 1:35–2:30 | Centinela patrullando y persiguiendo | La IA usa una máquina de estados: patrulla, persecución y búsqueda. NavMeshAgent calcula la navegación. La detección combina distancia y línea de visión; ocultarse hace que busque la última posición observada antes de regresar a patrulla. Mostrar comportamiento real. |
| 2:30–3:30 | Reactor bloqueado, completar terminales, activación | La progresión está integrada: la consola no activa el reactor hasta completar los tres terminales. La activación cambia shader, luz, partículas, sonido y objetivo. |
| 3:30–4:15 | Extracción, victoria, reinicio; breve derrota y pausa | El juego tiene inicio, objetivo y condición de victoria. También hay derrota por daño o tiempo agotado. La pausa detiene los sistemas y el reinicio restablece la misión. |
| 4:15–5:10 | SentinelAI.cs y Profiler | Las decisiones de IA se limitan a intervalos de 0.2 segundos. SetDestination evita actualizar el destino si no cambió al menos 0.5 unidades. El movimiento del agente continúa entre decisiones. Mostrar mediciones registradas; si no hay comparación, decir que es una técnica aplicada sin cuantificar mejora. |
| 5:10–6:00 | Estructura scripts, pruebas, carpeta build | Los sistemas separan misión, interacción, IA, UI y audio. Mostrar registro de pruebas completado y ejecutable externo. Concluir explicando una mejora concreta y una limitación observada. |

Mencionar la ejecución individual de forma breve y factual si corresponde, y documentar la coordinación en el informe. Evitar inventar participación de integrantes. Conservar enlace del repositorio, build y video para la entrega.
