// Written by Joe Zachary for CS 3500, January 2017.

/***********************
 * u1080787
 * Kevin Claiborne
 * PS2
 * *********************/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using System.Text.RegularExpressions;

namespace FormulaTestCases {
    /// <summary>
    /// These test cases are in no sense comprehensive!  They are intended to show you how
    /// client code can make use of the Formula class, and to show you how to create your
    /// own (which we strongly recommend).  To run them, pull down the Test menu and do
    /// Run > All Tests.
    /// </summary>
    [TestClass]
    public class UnitTests {
        /// <summary>
        /// This tests that a syntactically incorrect parameter to Formula results
        /// in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct1() {
            Formula f = new Formula("_");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct2() {
            Formula f = new Formula("2++3");
        }

        /// <summary>
        /// Test Formula with no arguments
        /// </summary>
        [TestMethod]
        public void Construct12() {
            Formula f = new Formula();
            double expected = 0;
            Assert.IsTrue(expected == f.Evaluate(Lookup4));
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct3() {
            Formula f = new Formula("2 3");
        }

        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate1() {
            Formula f = new Formula("2+3");
            Assert.AreEqual(f.Evaluate(v => 0), 5.0, 1e-6);
        }

        /// <summary>
        /// The Formula consists of a single variable (x5).  The value of
        /// the Formula depends on the value of x5, which is determined by
        /// the delegate passed to Evaluate.  Since this delegate maps all
        /// variables to 22.5, the return value should be 22.5.
        /// </summary>
        [TestMethod]
        public void Evaluate2() {
            Formula f = new Formula("x5");
            Assert.AreEqual(f.Evaluate(v => 22.5), 22.5, 1e-6);
        }

        /// <summary>
        /// Here, the delegate passed to Evaluate always throws a
        /// UndefinedVariableException (meaning that no variables have
        /// values).  The test case checks that the result of
        /// evaluating the Formula is a FormulaEvaluationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate3() {
            Formula f = new Formula("x + y");
            f.Evaluate(v => { throw new UndefinedVariableException(v); });
        }

        /// <summary>
        /// The delegate passed to Evaluate is defined below.  We check
        /// that evaluating the formula returns in 10.
        /// </summary>
        [TestMethod]
        public void Evaluate4() {
            Formula f = new Formula("x + y");
            Assert.AreEqual(f.Evaluate(Lookup4), 10.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate5() {
            Formula f = new Formula("(x + y) * (z / x) * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }


        /// <summary>
        /// A variable immedietely follows a number.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct4() {
            Formula f = new Formula("x + 3x");
        }

        /// <summary>
        /// Invalid token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct5() {
            Formula f = new Formula("3+3-#");
        }

        /// <summary>
        /// Empty formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct6() {
            Formula f = new Formula("");
        }

        /// <summary>
        /// One Variable
        /// </summary>
        [TestMethod]
        public void Evaluate8() {
            Formula f = new Formula("x");
            Assert.AreEqual(f.Evaluate(Lookup4), 4.0);
        }

        /// <summary>
        /// One bigger number
        /// </summary>
        [TestMethod]
        public void Evaluate10() {
            Formula f = new Formula("3000");
            Assert.AreEqual(f.Evaluate(Lookup4), 3000);
        }

        /// <summary>
        /// Unbalanced left parenthesis
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct7() {
            Formula f = new Formula("((6+3)");
        }

        /// <summary>
        /// Check ToString Method
        /// </summary>
        [TestMethod]
        public void Construct14() {
            Formula f = new Formula("(6+3)");
            String result = f.ToString();
            Assert.AreEqual(result, "(6+3)");
        }

        /// <summary>
        /// Check ToString Method, 3 argument Ctor
        /// Should capitalize and match.
        /// </summary>
        [TestMethod]
        public void Construct15() {
            Formula f = new Formula("(x2+y3 + z3 -X3)", s => s.ToUpper(), Validator1);
            String result = f.ToString();
            Assert.AreEqual(result, "(X2+Y3+Z3-X3)"); 
        }

        /// <summary>
        /// Check Validation
        /// Expecting formula format exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct16() {
            Formula f = new Formula("(x2+y3 + z3 -X)", s => s.ToUpper(), Validator1);
        }

        /// <summary>
        /// Construct formula using another formula ToString Method 
        /// Confirm matching ToString
        /// </summary>
        [TestMethod]
        public void Construct17() {
            Formula f1 = new Formula("x1+3");
            Formula f2 = new Formula(f1.ToString(), s => s, s => true);
            Assert.AreEqual(f1.ToString(), f2.ToString());
        }


        /// <summary>
        /// Unbalanced right parenthesis
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct8() {
            Formula f = new Formula("((6+3))-4)");
        }

        /// <summary>
        /// Bad first token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct9() {
            Formula f = new Formula("-((6+3))-4");
        }

        /// <summary>
        /// Bad last token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct10() {
            Formula f = new Formula("((6+3))*");
        }

        /// <summary>
        /// Two variables seperated by space.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct11() {
            Formula f = new Formula("6+1*x x");
        }

        /// <summary>
        /// Two variables next to one another, but xx not in lookup.
        /// Should not be 44
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate9() {
            Formula f = new Formula("6+1*xx");
            f.Evaluate(Lookup4);
        }

        /// <summary>
        /// Matching results reasonable length
        /// Double parenthesis, all operators, some spaces, some no spaces
        /// </summary>
        [TestMethod]
        public void Evaluate6() {
            Formula f = new Formula("z + ((x+y+3*4)*(100-96)+((5+2)-1+z)) / 2");
            double result =  8 + ((4 + 6 + 3 * 4) * (100 - 96) + ((5 + 2) - 1 + 8)) / 2;
            Assert.AreEqual(f.Evaluate(Lookup4), result);
        }

        /// <summary>
        /// Divide by 0
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate7() {
            Formula f = new Formula("(x+y) / 0");
            f.Evaluate(Lookup4);
        }

        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Lookup4(String v) {
            switch (v) {
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                default: throw new UndefinedVariableException(v);
            }
        }
        public bool Validator1(String v) {
            Regex r = new Regex("[a-zA-Z]{1}[0-9a-zA-Z]{1}");

            if (r.IsMatch(v)) {
                return true;
            } else return false;
        }
    }
}
