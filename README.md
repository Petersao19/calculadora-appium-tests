# Automatización Calculadora Windows — Appium + C#

Prueba técnica QA — Farmacias Super Económica
Automaticé el flujo de validación de la Calculadora de Windows usando Appium con WinAppDriver, C# y NUnit.

---

## Requisitos

- Windows 10/11 con Modo desarrollador activado
- .NET 8 SDK
- Visual Studio 2022
- WinAppDriver instalado

---

## Configuración inicial

**Activar modo desarrollador:**
Configuración → Sistema → Avanzado → Para desarrolladores → activar Modo para desarrolladores

**Instalar WinAppDriver:**
Descargar el instalador desde: https://github.com/microsoft/WinAppDriver/releases, la versión WinAppDriver v1.2.1

Instalar `WindowsApplicationDriver.msi`. Queda en:`C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe`

**Clonar el proyecto:**
```bash
git clone https://github.com/Petersao19/calculadora-appium-tests.git
cd calculadora-appium-tests
dotnet restore
```

---

## Cómo ejecutar las pruebas

Primero abrir WinAppDriver y dejarlo corriendo:
```
C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe
```
Debe mostrar: `Windows Application Driver listening for requests at: http://127.0.0.1:4723/`

Luego ejecutar las pruebas:
```bash
dotnet test
```
O desde Visual Studio: Prueba → Ejecutar todas las pruebas

---

## Flujo automatizado

1. Inicia la Calculadora de Windows
2. Verifica que abre en modo Estándar
3. Realiza la operación 9 + 3 y valida que el resultado es 12
4. Limpia con el botón C y verifica que muestra 0
5. Realiza 5 / 0 y valida el mensaje de error
6. Realiza 6 / 0 — se detecta bug, se genera reporte automático
7. Cierra la aplicación

---

## Bug encontrado

**BUG-001 — Calculadora no muestra 0 al realizar 6 / 0**

La calculadora muestra "No se puede dividir entre cero" en lugar de 0.

Pasos para reproducir:
1. Abrir Calculadora de Windows
2. Ingresar 6
3. Presionar /
4. Ingresar 0
5. Presionar =

Resultado esperado: 0
Resultado obtenido: "No se puede dividir entre cero"

Severidad: Alta — Reportado en Jira SCRUM-5
Evidencia en carpeta /Capturas

---

## Estructura

CalculadoraAppiumTests/
├ UnitTest1.cs      — pruebas automatizadas
└ README.md

Las carpetas Capturas/ y Bugs/ se generan automáticamente al ejecutar las pruebas.

**Peter Bazurto** — QA Engineer
github.com/Petersao19