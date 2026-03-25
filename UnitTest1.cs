using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.IO;

namespace CalculadoraAppiumTests
{
    [TestFixture]
    public class CalculadoraTests
    {
        private WindowsDriver<WindowsElement> _driver;
        private const string AppiumUrl = "http://127.0.0.1:4723";
        private const string CapturasFolder = "Capturas";
        private const string BugsFolder = "Bugs";

        [SetUp]
        public void SetUp()
        {
            var options = new AppiumOptions();
            options.AddAdditionalCapability("app", "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
            options.AddAdditionalCapability("deviceName", "WindowsPC");

            _driver = new WindowsDriver<WindowsElement>(new Uri(AppiumUrl), options);

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            Directory.CreateDirectory(CapturasFolder);
            Directory.CreateDirectory(BugsFolder);
        }

        [Test]
        [Description("FLujo completo: suma, clear, división por cero y reporte de bug")]
        public void FlujoCompleto_CalculadoraWindows()
        {
            // PASO 1: verificar calculadora inicia en modo "Estándar".
            string titulo = _driver.Title;
            Assert.That(titulo, Does.Contain("Calculadora"), "La calculadora debe estar abierta");
            TomarCaptura("PASO01_ModoEstandar");

            // PASO 2:  la operación: 9 + 3 = 12
            _driver.FindElementByName("Nueve").Click();
            System.Threading.Thread.Sleep(500);
            _driver.FindElementByName("Más").Click();
            System.Threading.Thread.Sleep(500);
            _driver.FindElementByName("Tres").Click();
            System.Threading.Thread.Sleep(500);
            _driver.FindElementByName("Es igual a").Click();
            System.Threading.Thread.Sleep(500);

            var resultado = _driver.FindElementByAccessibilityId("CalculatorResults");
            TomarCaptura("PASO02_Suma_9_3");
            Assert.That(resultado.Text, Does.Contain("12"),"9 + 3 debe ser 12");

            // PASO 3: Limpiar la operación (Clear).
            _driver.FindElementByAccessibilityId("clearButton").Click();
            System.Threading.Thread.Sleep(500);
            resultado = _driver.FindElementByAccessibilityId("CalculatorResults");
            TomarCaptura("PASO03_Clear");
            Assert.That(resultado.Text, Does.Contain("0"),"Después de Clear debe mostrar 0");

            // PASO 4: 5 / 0 = error esperado
            _driver.FindElementByName("Cinco").Click();
            _driver.FindElementByAccessibilityId("divideButton").Click();
            _driver.FindElementByName("Cero").Click();
            _driver.FindElementByName("Es igual a").Click();

            resultado = _driver.FindElementByAccessibilityId("CalculatorResults");
            TomarCaptura("PASO04_Error_5_0");
            Assert.That(resultado.Text, Does.Contain("cero").Or.Contain("zero").Or.Contain("Cannot"), "5/0 debe mostrar error de división entre cero");

            
            _driver.FindElementByAccessibilityId("clearEntryButton").Click();
            System.Threading.Thread.Sleep(500);

            // PASO 5: 6 / 0 = se espera 0 pero es un bug
            _driver.FindElementByName("Seis").Click();
            System.Threading.Thread.Sleep(500);
            _driver.FindElementByAccessibilityId("divideButton").Click();
            System.Threading.Thread.Sleep(500);
            _driver.FindElementByName("Cero").Click();
            System.Threading.Thread.Sleep(500);
            _driver.FindElementByName("Es igual a").Click();
            System.Threading.Thread.Sleep(3000);

            resultado = _driver.FindElementByAccessibilityId("CalculatorResults");
            TomarCaptura("PASO05_Error_6_0");

            bool muestraCero = resultado.Text.Contains("0") && !resultado.Text.Contains("cero") && !resultado.Text.Contains("zero");

            if (!muestraCero)
            {
                ReportarBug(
                    id: "BUG-001",
                    titulo: "Calculadora no muestra 0 al realizar 6 / 0",
                    pasos: "1. Abrir Calculadora Windows\n" +
                           "2. Ingresar 6\n" +
                           "3. Presionar /\n" +
                           "4. Ingresar 0\n" +
                           "5. Presionar =",
                    resultadoEsperado: "El display debe mostrar 0",
                    resultadoObtenido: resultado.Text
                );
            }

            // PASO 6: cerrar la aplicación
            _driver.CloseApp();

            // confirmamos el bug
            Assert.That(muestraCero, Is.False, "BUG-001 confirmado: 6/0 muestra error en lugar de 0");
        }

        private void TomarCaptura(string nombre)
        {
            var captura = _driver.GetScreenshot();
            string ruta = Path.Combine(CapturasFolder, $"{nombre}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            captura.SaveAsFile(ruta);
        }

        private void ReportarBug(string id, string titulo, string pasos, string resultadoEsperado, string resultadoObtenido)
        {
            string ruta = Path.Combine(BugsFolder, $"{id}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            string contenido =
                $"REPORTE DE BUG - FARMACIAS SUPER ECONOMICA\n" +
                $"ID: {id}\n" +
                $"Titulo: {titulo}\n" +
                $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}\n" +
                $"Severidad: Alta\n" +
                $"Prioridad: Alta\n" +
                $"Reportado por: Peter Bazurto\n\n" +
                $"PASOS PARA REPRODUCIR:\n{pasos}\n\n" +
                $"RESULTADO ESPERADO:\n{resultadoEsperado}\n\n" +
                $"RESULTADO OBTENIDO:\n{resultadoObtenido}";

            File.WriteAllText(ruta, contenido);
        }

        [TearDown]
        public void TearDown()
        {
            _driver?.Dispose();
        }
    }
}