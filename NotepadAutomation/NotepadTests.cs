using NUnit.Framework;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using FlaUI.Core;
using Bogus;

namespace NotepadAutomation
{
    public class Tests
    {
       
        
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
        TimeSpan timeOut = TimeSpan.FromSeconds(20);
        var app = Application.Launch("notepad.exe");
        Retry.DefaultTimeout = timeOut;
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                window.Title.Should().Be("Untitled - Notepad");
                var FileTab = window.FindAllDescendants().Single(x => x.Name == "File").AsMenuItem().WaitUntilClickable();
                FileTab.Click();

                var OpenTab = window.FindAllDescendants().Single(x => x.Name == "Open...").AsMenuItem().WaitUntilClickable();
                OpenTab.Click();


                Retry.WhileTrue(() => window.ModalWindows.Length == 1);
                var openFile = window.ModalWindows[0].AsWindow();
                openFile.Name.Should().Be("Open");

                var edit = openFile.FindFirstChild("1148").WaitUntilClickable();
                edit.Click();

                Retry.WhileTrue(() => edit.FrameworkAutomationElement.HasKeyboardFocus);
                Keyboard.Type(@"C:\Users\yyuur\source\repos\Practice6\NotepadAutomation\Test.txt");
                var fileOpen = openFile.FindFirstChild("1");
                Retry.DefaultTimeout = timeOut;
                fileOpen.WaitUntilClickable(timeOut);
                fileOpen.Click();



                Retry.WhileTrue(() => window.Title.Equals("Test.txt - Notepad"));

               
                var editText = window.FindAllDescendants().Single(x => x.Name == "Text Editor").AsTextBox();
               editText.Text.Should().Be("Hello");
                editText.Enter("SomeWords");


                FileTab = window.FindAllDescendants().Single(x => x.Name == "File").AsMenuItem().WaitUntilClickable();
                FileTab.Click();

                var SaveTab = window.FindAllDescendants().Single(x => x.Name == "Save As...").AsMenuItem().WaitUntilClickable();
                SaveTab.Click();


                var faker = new Faker();
                var filename = String.Format("Test{0}",faker.System.FileName("txt"));
                var path = @"C:\Users\yyuur\source\repos\Practice6\NotepadAutomation\Test.txt";


                Retry.WhileTrue(() => window.ModalWindows.Length == 1);
                var saveFile = window.ModalWindows[0].AsWindow();

                Keyboard.Type(path + filename);
                var fileName1 = saveFile.FindAllDescendants(x => x.ByClassName("AppControlHost")).First(x => x.ClassName == "File filename:");
                fileName1.AsTextBox().Text.Should().Be(path + filename);
                saveFile.FindFirstChild("1").AsButton().Invoke();


                app.Close();

                Retry.WhileFalse(() => File.Exists(path + filename));
                File.ReadAllText(path + filename).Should().Be("SomeWords");
                File.Delete(path + filename);
            }
        }
    }
}