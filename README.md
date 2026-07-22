![Banner del juego](Assets/images/banner.jpeg)

# 🏫 Juego Campus Nocturno (Estilo PS1)

Un juego de sigilo y exploración en 3D con estética retro de PlayStation 1 (LowPoly). La premisa sigue a un estudiante frustrado tras desaprobar un examen final, quien decide infiltrarse en la universidad de noche (Pabellón J) para recuperar los exámenes del profesor.

## 🛠️ Requisitos e Instalación

Para abrir este proyecto y reproducir las animaciones y cinemáticas correctamente, necesitas el siguiente entorno:

### 1. Motor Gráfico
* **Unity Editor:** Versión `2022.3.62f3` (Built-in Render Pipeline).
* Se recomienda usar Unity Hub para gestionar la versión.

### 2. Paquetes de Unity (Package Manager)
Asegúrate de instalar o verificar estos paquetes desde `Window > Package Manager` (en el Unity Registry):
* **ProBuilder:** Utilizado para el bloqueo de niveles (Level Blockout) y la arquitectura de los cuartos.
* **Timeline:** Utilizado para secuenciar las cinemáticas (Ej: la escena inicial del estudiante en su cuarto).

### 3. Assets Externos Utilizados
* **Modelos 3D:** Muebles *Low Poly* y maniquí humanoide genérico.
* **Texturas:** Texturas de baja resolución (256x256) con el `Filter Mode` configurado en `Point (no filter)` para lograr el efecto "crujiente" de PS1.
* **Animaciones (Mixamo):** El proyecto utiliza animaciones extraídas de Mixamo aplicadas a un rig `Humanoid`. Para editar o agregar nuevas, importar los archivos `.fbx` (Without Skin) y ajustar el *Animation Type* a Humanoid en el Inspector.

## 🎬 A donde ir para crear la escena:
1. Window > Sequencing > Timeline
