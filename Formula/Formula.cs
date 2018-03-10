// Skeleton written by Joe Zachary for CS 3500, January 2017

/***********************
 * u1080787
 * Kevin Claiborne
 * PS2
 * *********************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Formulas {

    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public struct Formula {


        /// <summary>
        /// List of strings to be used within class to represent formula.
        /// </summary>
        private List<String> tokenList;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="normalizer"></param>
        /// <param name="validator"></param>
        /// <exception cref="System.ArgumentNullException">If formula, normalizer, or validator are null</exception>
        public Formula(String formula, Normalizer normalizer, Validator validator) {

            if (formula == null || normalizer == null || validator == null) {
                throw new ArgumentNullException("Null parametewas found");
            }

            tokenList = GetTokens(formula).ToList<String>();
            List<String> tempList = new List<string>();
            int lCount = 0;
            int rCount = 0;

            //First Check - Is there anything in formaula and is there tokens.
            if (formula.Length > 0 && tokenList.Count > 0) {


                //check if variable or double
                int varCount = 0;
                for (int i = 0; i < tokenList.Count; i++) {
                    if (IsVar(tokenList[i]) || IsDouble(tokenList[i])) {
                        varCount++;
                        Double temp;

                        //Check if not double normilize and validate
                        if (!Double.TryParse(tokenList[i], out temp)) {
                            tokenList[i] = normalizer(tokenList[i]);
                            if (!validator(tokenList[i])) {
                                throw new FormulaFormatException("Validation of variable failed");
                            }
                        }
                    }
                }


                //if no variables or doubles exist, no math can be performed
                if (varCount == 0) {
                    throw new FormulaFormatException("There were no numbers or variable in the fotula");
                } //end check

                //first char check
                String current = formula[0] + "";
                for (int i = 0; i < formula.Length - 1; i++) {
                    if (!IsParenth(formula[i] + "")) {
                        if (IsOp(formula[i] + "")) {
                            throw new FormulaFormatException("Illegal first charcacter: " + formula[i]);
                        }
                        break;
                    } else {
                        if (formula[i].Equals(")")) {
                            throw new FormulaFormatException("Right parenthesis before left parenthesis");
                        }
                    }
                }


                //For checking last used token
                String last = null;

                foreach (String s in tokenList) {

                    //if valid token
                    if (IsDouble(s) || IsVar(s) || IsOp(s) || IsParenth(s)) {

                        if (last != null) {

                            //Operation after parenthesis check
                            if (last == "(" && IsOp(s)) {
                                throw new FormulaFormatException("Operation found directly after opening parenthesis");
                            }

                            //two repeat variables check
                            if (IsVar(last) || IsDouble(last)) {
                                if (IsVar(s) || IsDouble(s)) {
                                    throw new FormulaFormatException(s + " cannot follow a " + last);
                                }
                                if (s == "(") {
                                    throw new FormulaFormatException(s + " cannot follow a " + last);
                                }

                            }

                            //tow repeat operations check
                            if (IsOp(last)) {
                                if (IsOp(s)) {
                                    throw new FormulaFormatException("Mulitple operations without varibales");
                                }
                            }

                            //Back to back parenthesis check
                            if (last == ")") {
                                if (s == "(") {
                                    throw new FormulaFormatException("Opening parenthesis without operand");
                                }
                                if (IsVar(s) || IsDouble(s)) {
                                    throw new FormulaFormatException(s + " cannot follow a " + last);
                                }
                            }
                        }

                        //Parenthesis balance checking
                        if (s == "(") {
                            lCount += 1;
                        }
                        if (s == ")") {
                            rCount += 1;
                        }
                        if (rCount > lCount) {
                            throw new FormulaFormatException("Too many right parenthesis.");
                        }

                        //token passed all tests add to list
                        if (IsVar(s) || IsDouble(s) || IsOp(s) || IsParenth(s)) {
                            tempList.Add(s);
                        }

                        //set last token
                        last = s;
                    } else {

                        // was not variable, operation, or parenthesis
                        throw new FormulaFormatException(s + " is not a valid token");
                    }
                }

            } else {
                //need something to calculate
                throw new FormulaFormatException("Formula is blank");
            }

            //Last Char Check
            if (IsOp(tokenList[tokenList.Count - 1])) {
                throw new FormulaFormatException("Rogue operation found at the end of statement");
            }

            //Final Parenthesis Check
            if (lCount > rCount) {
                throw new FormulaFormatException("Too Many Left Parenthesis.");
            }
            tokenList = tempList;
        }

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true) {

        }



        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        /// <param name="lookup"></param>
        /// <returns>double</returns>
        /// <exception cref="System.ArgumentNullException">If lookup is null</exception>
        public double Evaluate(Lookup lookup) {

            if (lookup == null) {
                throw new ArgumentNullException("Lookup cannot be null");
            }

            if (tokenList == null) {
                tokenList = new List<String> {
                    "0"
                };
            }

            //Try replacing all variables in list with values
            try {
                for (int i = 0; i <= tokenList.Count - 1; i++) {
                    if (Char.IsLetter(tokenList[i][0])) {
                        Double value = lookup(tokenList[i]);
                        tokenList[i] = value.ToString();
                    }
                }
            } catch (Exception e) {
                throw new FormulaEvaluationException("The variables do not have matching values." + e);
            }

            //prep for infix to postfix evluation
            Stack<double> valStack = new Stack<Double>();
            Stack<String> opStack = new Stack<String>();
            Double number;

            //read from left to right
            foreach (String t in tokenList) {

                //following steps on assignment page.

                //if double and opstack is not empty
                if (Double.TryParse(t, out number)) {
                    if (opStack.Count > 0 && (opStack.Peek().Equals("*") || opStack.Peek().Equals("/"))) {
                        valStack.Push(ApplyMath(opStack.Pop(), valStack.Pop(), number));
                    } else {
                        valStack.Push(number);
                    }

                    //if t is a + or i, opstack is not empty, and + or - is already at top of opStack
                } else if ("+-".Contains(t)) {
                    if (opStack.Count > 0 && "+-".Contains(opStack.Peek())) {
                        number = valStack.Pop();
                        valStack.Push(ApplyMath(opStack.Pop(), valStack.Pop(), number));
                    }
                    opStack.Push(t);

                    // t is * or /
                } else if ("*/".Contains(t)) {
                    opStack.Push(t);

                    // if t is a left parenthesis
                } else if (t.Equals("(")) {
                    opStack.Push(t);

                    // if t is a right parenthesis
                } else if (t.Equals(")")) {
                    // and ther is a + or - is already at top of opStack
                    if (opStack.Count > 0 && "+-".Contains(opStack.Peek())) {
                        number = valStack.Pop();
                        valStack.Push(ApplyMath(opStack.Pop(), valStack.Pop(), number));
                    }
                    opStack.Pop();

                    //Top of stack is * or /
                    if (opStack.Count > 0 && "*/".Contains(opStack.Peek())) {
                        number = valStack.Pop();
                        valStack.Push(ApplyMath(opStack.Pop(), valStack.Pop(), number));
                    }
                }
            }

            //Is there still a value left.
            if (valStack.Count == 0) {
                throw new FormulaEvaluationException("No values to return");
            }

            //were out, is there more than one number on the valstack?
            //if so lets combine
            while (valStack.Count > 1) {
                number = valStack.Pop();
                if (opStack.Count > 0) {
                    valStack.Push(ApplyMath(opStack.Pop(), valStack.Pop(), number));
                } else {
                    throw new FormulaEvaluationException("No operator found");
                }
            }

            //one value to return
            return valStack.Pop();
        }

        /// <summary>
        /// Returns set of variable in formula.
        /// </summary>
        /// <returns></returns>
        public ISet<String> GetVariables() {
            List<String> varList = new List<String>();


            if (tokenList != null) {
                foreach (String s in tokenList) {
                    if (IsVar(s)) {
                        varList.Add(s);
                    }
                }
            }

            ISet<String> set = new HashSet<String>(varList);

            return set;
        }

        /// <summary>
        /// Returns result of mathmatical operation for an operator +, -, *, or /
        /// and the number to the left and right of the operation.
        /// <para>Example: left "operator" right; 6 + 2</para>
        /// </summary>
        /// <param name="op"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If operator is null</exception>
        private Double ApplyMath(String op, Double left, Double right) {

            //left and right can never be null.
            if (op == null) {
                throw new ArgumentNullException("Missing required parameters for math.");
            }

            switch (op) {
                case "*":
                    return left * right;
                case "/":
                    if (right == 0) {
                        throw new FormulaEvaluationException("Divide by zero error");
                    } else {
                        return left / right;
                    }
                case "+":
                    return left + right;
                case "-":
                    return left - right;
                default:
                    throw new FormatException("Invalid operator " + op);

            }
        }

        /// <summary>
        /// Returns formula in normalize form.
        /// </summary>
        /// <returns></returns>
        override //Override built in ToString Method
        public String ToString() {
            String result = "";
            if (tokenList != null) {
                foreach (String s in tokenList) {
                    result += s;
                }
                return result;
            }
            return "0";
        }

        /// <summary>
        /// Returns true if string is a Double.
        /// </summary>
        /// <param name="doub"></param>
        /// <returns>Boolean</returns>
        /// /// <exception cref="System.ArgumentNullException">If doub is null</exception>
        private Boolean IsDouble(String doub) {

            if (doub == null) {
                throw new ArgumentNullException("Nothing to check");
            }

            Double number = 0;

            if (Double.TryParse(doub, out number)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Returns true if string is variable such as x.
        /// </summary>
        /// <param name="testVar"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If testVar is null</exception>
        private Boolean IsVar(String testVar) {

            if (testVar == null) {
                throw new ArgumentNullException("Nothing to check");
            }

            Regex r = new Regex("[a-zA-Z]");

            String s = testVar[0] + "";

            if (r.IsMatch(s)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Returns true if string is +,-,/,*. Otherwise returns false.
        /// </summary>
        /// <param name="testOP"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If testop is null</exception>
        private Boolean IsOp(String testop) {

            if (testop == null) {
                throw new ArgumentNullException("Nothing to check");
            }

            String operations = "+-/*";

            if (operations.Contains(testop)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Returns true if string is ( or ). Otherwise returns false.
        /// </summary>
        /// <param name="parenthesis"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If parenthesis is null</exception>
        private Boolean IsParenth(String parenthesis) {

            if (parenthesis == null) {
                throw new ArgumentNullException("Nothing to check");
            }

            String operations = "()";

            if (operations.Contains(parenthesis)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// <exception cref="System.ArgumentNullException">If formula is null</exception>
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula) {

            if (formula == null) {
                throw new ArgumentNullException("Formula is null");
            }


            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";

            // PLEASE NOTE:  I have added white space to this regex to make it more readable.
            // When the regex is used, it is necessary to include a parameter that says
            // embedded white space should be ignored.  See below for an example of this.
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern.  It contains embedded white space that must be ignored when
            // it is used.  See below for an example of this.  This pattern is useful for 
            // splitting a string into tokens.
            String splittingPattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            // PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
            /// in the pattern.
            foreach (String s in Regex.Split(formula, splittingPattern, RegexOptions.IgnorePatternWhitespace)) {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline)) {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string var);

    /// <summary>
    ///  The purpose of a Normalizer is to convert variables into a canonical form.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public delegate string Normalizer(string s);

    /// <summary>
    /// The purpose of a Validator is to impose extra restrictions on the validity of a variable
    /// , beyond the ones already built into the Formula definition.  
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public delegate bool Validator(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    [Serializable]
    public class UndefinedVariableException : Exception {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable) {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    [Serializable]
    public class FormulaFormatException : Exception {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message) {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : Exception {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message) {
        }
    }
}
