using System;
using System.Collections.Generic;
using Formulas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace SpreadsheetTests {
    [TestClass]
    public class UnitTest1 {

        /// <summary>
        /// Makes sure GetContents returns a formula
        /// Type must be Formula
        /// A1 and A2 are different
        /// </summary>
        [TestMethod]
        public void SetFormula1() {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=X1+Y3");

            ss.SetContentsOfCell("A2", "=X1+Z6");

            Formula expected = new Formula("X1+Z6");
            Formula result = (Formula)ss.GetCellContents("A2");
            Assert.AreEqual(expected.ToString(), result.ToString());

            expected = new Formula("X1+Y3");
            result = (Formula)ss.GetCellContents("A1");
            Assert.IsTrue(expected.GetType() == ss.GetCellContents("A1").GetType());
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        /// <summary>
        /// Makes sure GetContents returns a Double
        /// Type must be Double
        /// A1 and A2 are different
        /// </summary>
        [TestMethod]
        public void SetDouble1() {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "10");

            ss.SetContentsOfCell("A2", "20");

            Double expected = 10;
            Double result = (Double)ss.GetCellContents("A1");
            Assert.AreEqual(expected, result);
            Assert.IsTrue(expected.GetType() == ss.GetCellContents("A1").GetType());

            expected = 20;
            result = (Double)ss.GetCellContents("A2");
            Assert.AreEqual(expected, result);
            Assert.IsTrue(expected.GetType() == ss.GetCellContents("A2").GetType());
        }

        /// <summary>
        /// Makes sure GetContents returns a String
        /// Type must be String
        /// A1 and A2 are different
        /// </summary>
        [TestMethod]
        public void SetFormula3() {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "This is a string");
            ss.SetContentsOfCell("A2", "This is another String");

            String expected = "This is a string";
            String result = (String)ss.GetCellContents("A1");
            Assert.AreEqual(expected, result);

            expected = "This is another String";
            result = (String)ss.GetCellContents("A2");
            Assert.IsTrue(expected.GetType() == ss.GetCellContents("A1").GetType());
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CheckCircular() {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B1*2");
            ss.SetContentsOfCell("B1", "=C1*2");
            ss.SetContentsOfCell("C1", "=A1*2");

        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CheckDeeperCircular() {
            AbstractSpreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("A1", "=B1*C1*D1+A11");

            for (int i = 2; i <= 10; i++) {
                ss.SetContentsOfCell("A" + i, "=B" + i + "*C" + i + "*D" + i);
            }

            ss.SetContentsOfCell("A11", "=A1+2");

        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CheckDeeperCircular2() {
            AbstractSpreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("A1", "=A2*2");

            for (int i = 2; i <= 10; i++) {
                ss.SetContentsOfCell("A" + i, "=A" + (i + 1) + "*A" + (i + 2) + "*A" + (i + 3));
            }

            ss.SetContentsOfCell("A11", "=A1+2");

        }


        /// <summary>
        /// Making sure circular reference is not thrown
        /// checking that names of non emptycells count is right
        /// Names of non empty cells matches expected
        /// Setting Cell to String.empty results in lower count
        /// </summary>
        [TestMethod]
        public void Afewformulas() {
            AbstractSpreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("A1", "=A2*2");

            for (int i = 2; i <= 10; i++) {
                ss.SetContentsOfCell("A" + i, "=A" + (i + 1) + "*A" + (i + 2) + "*A" + (i + 3));
            }

            int count = 0;
            foreach (String s in ss.GetNamesOfAllNonemptyCells()) {
                count++;
            }
            //should be 10 (A1:A10)
            Assert.AreEqual(count, 10);

            List<String> expected = new List<string> {
                   "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "A10" };

            List<String> results = new List<String>(ss.GetNamesOfAllNonemptyCells());

            results.Sort();
            expected.Sort();

            count = 0;
            for (int i = 0; i < 10; i++) {
                if (expected[i] == results[i]) {
                    count++;
                }
            }

            Assert.IsTrue(count == 10);


            for (int i = 0; i < 10; i++) {
                ss.SetContentsOfCell("A" + (i + 1), String.Empty);
                count = 0;
                foreach (String s in ss.GetNamesOfAllNonemptyCells()) {
                    count++;
                }

                //removing 1 each time
                Assert.AreEqual(9 - i, count);
            }
        }

        /// <summary>
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        [TestMethod]
        public void SetCellReturn1() {

            AbstractSpreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("B1", "=A1*2");
            ss.SetContentsOfCell("C1", "=B1+A1");
            List<String> returned = new List<String>(ss.SetContentsOfCell("A1", "10"));
            List<String> expected = new List<String> { "A1", "B1", "C1" };

            int count = 0;
            for (int i = 0; i < returned.Count; i++) {
                if (returned[i] == expected[i]) {
                    count++;
                }
            }

            Assert.AreEqual(3, count);
        }

        /// <summary>
        /// Name Exception X07 is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SS.InvalidNameException))]
        public void NameIsssue1() {

            AbstractSpreadsheet ss = new Spreadsheet(new Regex("^[a-zA-Z]+[1-9]\\d*"));

            ss.SetContentsOfCell("B01", "=A1*2");
        }

        /// <summary>
        /// Z is not valid 
        /// Should return InvalidNameException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SS.InvalidNameException))]
        public void NameIsssue2() {
            AbstractSpreadsheet ss = new Spreadsheet(new Regex("^[a-zA-Z]+[1-9]\\d*$"));
            ss.SetContentsOfCell("Z", "=A1*2");
        }

        /// <summary>
        /// null is not valid 
        /// Should return InvalidNameException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SS.InvalidNameException))]
        public void NameIsssue3() {

            AbstractSpreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell(null, "=A1*2");

        }

        /// <summary>
        /// null is not valid 
        /// Should return InvalidNameException
        /// </summary>
        [TestMethod]
        public void SaveFile() {

            AbstractSpreadsheet ss = new Spreadsheet(new Regex("^.*$"));

            ss.SetContentsOfCell("A1", "1.5");
            ss.SetContentsOfCell("A2", "8.0");
            ss.SetContentsOfCell("A3", "=A1*A2+23");
            ss.SetContentsOfCell("B2", "Hello");
            StreamWriter sw = new StreamWriter("../../../Spreadsheet/SpreadSheetTest.xml");
            ss.Save(sw);
            sw.Close();

            String results = "";
            String expected = "";
            using (StreamReader sr = new StreamReader("../../../Spreadsheet/SpreadSheetTest.xml")) {
                while (sr.ReadLine() != null) {
                    results += sr.ReadLine();
                }
                sr.Close();
            }

            using (StreamReader sr = new StreamReader("../../../Spreadsheet/SampleSavedSpreadsheet.xml")) {
                while (sr.ReadLine() != null) {
                    expected += sr.ReadLine();
                }
                sr.Close();
            }


            Assert.AreEqual(expected, results);

        }

        /// <summary>
        /// Test to ensure changed method is working
        /// </summary>
        [TestMethod]
        public void changed() {

            AbstractSpreadsheet ss = new Spreadsheet(new Regex("^.*$"));
            Assert.IsFalse(ss.Changed);

            ss.SetContentsOfCell("A1", "1.5");
            ss.SetContentsOfCell("A2", "=A1+.5");
            Assert.IsTrue(ss.Changed);

            StreamWriter sw = new StreamWriter("../../../Spreadsheet/SpreadSheetTest.xml");
            ss.Save(sw);
            sw.Close();

            Assert.IsFalse(ss.Changed);
        }

        /// <summary>
        /// Test new Spreadsheet Ctor
        /// Failed and good
        /// </summary>
        [TestMethod]
        public void ConstructorThreeArg1() {

            AbstractSpreadsheet ss = new Spreadsheet(new Regex("^.*$"));
            String filename = "../../../Spreadsheet/SpreadSheetTest.xml";
            TextReader openfile = null;
            TextWriter savefile = null;


            ss.SetContentsOfCell("A1A", "1.5");
            ss.SetContentsOfCell("A2", "=A1+.5");
            ss.Save(savefile = File.CreateText(filename));
            savefile.Close();
            
            AbstractSpreadsheet ss2;

            try {
                ss2 = new Spreadsheet(openfile = File.OpenText(filename), new Regex("^[a-zA-Z]+[1-9]\\d*$"));
                openfile.Close();
            } catch (Exception e) {
                Assert.AreEqual(typeof(SpreadsheetVersionException), e.GetType());
            }

            //Empty and check
            ss.SetContentsOfCell("A1A", String.Empty);
            ss.SetContentsOfCell("A2", String.Empty);
            var results = new List<String>(ss.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(0, results.Count);

            ss.SetContentsOfCell("A1", "1.5");
            ss.SetContentsOfCell("A2", "=A1+0.5");
            openfile.Close();
            ss.Save(savefile = File.CreateText(filename));
            savefile.Close();

            ss2 = new Spreadsheet(openfile = File.OpenText(filename), new Regex("^[a-zA-Z]+[1-9]\\d*$"));
            Assert.AreEqual(ss2.GetCellContents("A1").ToString(), "1.5");
            Assert.AreEqual(ss2.GetCellContents("A2").ToString(), "A1+0.5");
        }

        /// <summary>
        /// GetValue should 
        /// return 1 = 1.5
        /// return 2 = .5
        /// return 3 = 2.0
        /// return 4 = "Hello"
        /// return 5 = formulaerror
        /// </summary>
        [TestMethod]
        public void CheckValue1() {

            AbstractSpreadsheet ss = new Spreadsheet(new Regex("^.*$"));
            
            ss.SetContentsOfCell("A1", "1.5");
            Assert.AreEqual(ss.GetCellValue("A1").ToString(), "1.5");

            ss.SetContentsOfCell("A2", "=.5");
            Assert.AreEqual(ss.GetCellValue("A2").ToString(), "0.5");

            ss.SetContentsOfCell("A3", "=A1+A2");
            Assert.AreEqual(ss.GetCellValue("A3").ToString(), "2");

            ss.SetContentsOfCell("A4", "Hello");
            Assert.AreEqual(ss.GetCellValue("A4").ToString(), "Hello");

            //Divide by zero error
            ss.SetContentsOfCell("A5", "=A4/A6");
            Assert.AreEqual(ss.GetCellValue("A5").GetType(), typeof(FormulaError));

            //empty cell error
            ss.SetContentsOfCell("A6", "=B1+2");
            Assert.AreEqual(ss.GetCellValue("A5").GetType(), typeof(FormulaError));


        }
    }
}
