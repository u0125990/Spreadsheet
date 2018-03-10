//HulkBuster

using System;
using System.Collections.Generic;
using System.Linq;
using Formulas;
using System.Text.RegularExpressions;
using Dependencies;
using System.IO;
using System.Xml;

namespace SS {

    /// <summary>
    /// An SpreadSheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string s is a valid cell name if and only if it consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names.  On the other hand, 
    /// "Z", "X07", and "hello" are not valid cell names.
    /// 
    /// A spreadsheet contains a unique cell corresponding to each possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important, and it is important that you understand the distinction and use
    /// the right term when writing code, writing comments, and asking questions.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In an empty spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  The value 
    /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
    /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
    /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
    /// is a double, as specified in Formula.Evaluate.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet {

        /// <summary>
        /// All cells that have been used.
        /// </summary>
        private Dictionary<String, Cell> activeCells;

        /// <summary>
        /// Dependancy Graph containg all cell dependencies.
        /// </summary>
        private DependencyGraph dg;

        /// <summary>
        /// Valid Regex used in class
        /// </summary>
        private Regex isValid;

        /// <summary>
        /// Spreadsheet changed variable
        /// If yes, true
        /// </summary>
        private bool changed;

        /// <summary>
        /// Constructor for spreadsheet
        /// Creates an empty Spreadsheet whose IsValid regular expression accepts every string.
        /// </summary>
        public Spreadsheet() {
            activeCells = new Dictionary<String, Cell>();
            dg = new DependencyGraph();
            changed = false;
            this.isValid = new Regex(".+");
        }

        /// Creates an empty Spreadsheet whose IsValid regular expression is provided as the parameter
        public Spreadsheet(Regex isValid) : this() {
            this.isValid = isValid;
        }

        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        ///
        /// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
        /// specification.  
        ///
        /// If there's a problem reading source, throws an IOException.
        ///
        /// Else if the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  
        ///
        /// Else if the IsValid string contained in source is not a valid C# regular expression, throws
        /// a SpreadsheetReadException.  (If the exception is not thrown, this regex is referred to
        /// below as oldIsValid.)
        ///
        /// Else if there is a duplicate cell name in the source, throws a SpreadsheetReadException.
        /// (Two cell names are duplicates if they are identical after being converted to upper case.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a 
        /// SpreadsheetReadException.  (Use oldIsValid in place of IsValid in the definition of 
        /// cell name validity.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a
        /// SpreadsheetVersionException.  (Use newIsValid in place of IsValid in the definition of
        /// cell name validity.)
        ///
        /// Else if there's a formula that causes a circular dependency, throws a SpreadsheetReadException. 
        ///
        /// Else, create a Spreadsheet that is a duplicate of the one encoded in source except that
        /// the new Spreadsheet's IsValid regular expression should be newIsValid.
        public Spreadsheet(TextReader source, Regex newIsValid) : this(newIsValid) {
            Load(source);
        }

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed {
            get => changed;
            protected set { this.changed = Changed; }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        /// <exception cref="SS.InvalidNameException">Name cannot be null or invalid</exception>
        public override object GetCellContents(string name) {

            CheckName(name);
            name = name.ToUpper();

            Cell requested = MakeCell(name);           

            return requested.GetContents();
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException">If name is null or invalid</exception>
        public override object GetCellValue(string name) {
            CheckName(name);
            return MakeCell(name).GetValue(); 
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells() {
            foreach (KeyValuePair<String, Cell> cell in activeCells) {
                if (!cell.Value.GetContents().Equals(String.Empty) && !cell.Value.GetContents().Equals("")) {
                    yield return cell.Key;
                }
            }
        }

        /// <summary>
        ///  Reads an XML file representing a spreadsheet into this spreadsheet.
        /// </summary>
        /// <param name="origin"></param>
        /// <exception cref="IOException">If problem reading file</exception>
        /// <exception cref="SpreadsheetReadException">If error reading contents</exception>
        /// <exception cref="SpreadsheetVersionException">If error parsing from old version</exception>
        private void Load(TextReader origin) {
            //XML Reader
            bool setVal = false;
            Regex oldIsValid = null;
            Regex newIsValid = this.isValid;
            XmlReader reader = null;

            try {
                reader = XmlReader.Create(origin);
            } catch {
                throw new IOException();
            }

            //While there is data being read
            while (reader.Read()) {
                if (reader.IsStartElement()) {
                    switch (reader.Name) {
                        case "spreadsheet":

                            //Find + Set isValid
                            //If invalid regex throw Spreadsheetreadexception
                            try {
                                oldIsValid = new Regex(reader["IsValid"]);
                            } catch {
                                throw new SpreadsheetReadException("Invalid Regex expression");
                            }

                            //Regex must be set before reaching cells
                            setVal = true;
                            break;

                        case "cell":
                            if (!setVal) {
                                throw new SS.SpreadsheetReadException("XML file is not correctly formmatted.");
                            }

                            String name = reader["name"];
                            String content = reader["contents"];

                            //A cell must have content
                            if (content == null) {
                                throw new SpreadsheetReadException("Cell has no contents");
                            }

                            //Checking old regex value with name
                            if (!oldIsValid.IsMatch(name)) {
                                throw new SpreadsheetReadException(name + " is not a valid cell name");
                            }

                            //Checking new regex value with name
                            if (!newIsValid.IsMatch(name)) {
                                throw new SpreadsheetVersionException(name + " is not a valid cell name");
                            }

                            //If begins with = then try to make formula ant test old and new regex
                            if (content.Count() > 0 && content[0].Equals('=')) {
                                string f = content.Substring(1);

                                //Test old Regex and if invalid SpreadsheetReadException
                                try {
                                    Formula form = new Formula(f, s => s.ToUpper(), s => oldIsValid.IsMatch(s));
                                } catch {
                                    throw new SpreadsheetReadException("Formula is invalid");
                                }

                                //Test new Regex and if invalid SpreadsheetVersionException
                                try {
                                    Formula form = new Formula(f, s => s.ToUpper(), s => newIsValid.IsMatch(s));
                                } catch {
                                    throw new SpreadsheetVersionException("Formula is invalid");
                                }
                            }

                            //if cell has been added already
                            if (activeCells.ContainsKey(name.ToUpper())) {
                                throw new SpreadsheetReadException("Duplicate cell in xml");
                            }

                            //Try setting cell, if error occurs set SpreadsheetReadException
                            try {      
                                //Set cell contents with the name and content
                                SetContentsOfCell(name, content);
                            } catch (Exception e) {
                                throw new SS.SpreadsheetReadException("Error parsing contents to cell " + e);
                            }       
                            break;
                    }
                }
            }

        }

        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the IsValid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        /// <param name="dest"></param>
        /// <exception cref="IOException">If any problems arise from saving</exception>
        public override void Save(TextWriter dest) {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            try {
                using (XmlWriter writer = XmlWriter.Create(dest, settings)) {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("IsValid", isValid.ToString());

                    foreach (String name in GetNamesOfAllNonemptyCells()) {

                        //Start with cell
                        writer.WriteStartElement("cell");

                        //CellName
                        Cell thisCell = MakeCell(name);
                        writer.WriteAttributeString("name", thisCell.GetName());

                        //Cell Contents
                        //If cell is a formula add equal sign to front
                        if (thisCell.GetContents().GetType() == typeof(Formula)) {
                            writer.WriteAttributeString("contents", "=" + GetCellContents(thisCell.GetName()).ToString());
                        } else if (double.TryParse(GetCellContents(thisCell.GetName()).ToString(), out double number)) {
                            writer.WriteAttributeString("contents", number.ToString());
                        } else {
                            writer.WriteAttributeString("contents", GetCellContents(thisCell.GetName()).ToString());
                        }

                        //End of line
                        writer.WriteString("");
                        writer.WriteEndElement();
                    }

                    //End of spreadsheet
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                    changed = false;
                }
            } catch {
                throw new IOException();
            }
        }
    

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <exception cref="SS.InvalidNameException">Name cannot be null or invalid</exception>
        protected override ISet<string> SetCellContents(string name, double number) {
            SetCellAssist(name, number, number.GetType());

            //Get cells that need to be recalculated
            ISet<String> cellsToRecalculate = new HashSet<String>(GetCellsToRecalculate(name));

            //recalculte them
            Recalculate(cellsToRecalculate);

            return cellsToRecalculate;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text) {
            if (text == null) {
                throw new ArgumentException();
            }
            SetCellAssist(name, text, text.GetType());
            
            //Get cells that need to be recalculated
            ISet<String> cellsToRecalculate = new HashSet<String>(GetCellsToRecalculate(name));

            //recalculte them
            Recalculate(cellsToRecalculate);

            return cellsToRecalculate;
        }

        /// <summary>
        /// Requires that all of the variables in formula are valid cell names.
        /// 
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <exception cref="SS.InvalidNameException">If name is Null or invalid</exception>
        /// <exception cref="SS.CircularException">If Cel contains circular reference</exception>
        protected override ISet<string> SetCellContents(string name, Formula formula) {

            //This cell will depend on these cells
            List<String> dependants = new List<String>(formula.GetVariables());

            //check if adding and dependants will result in circular ref.
            CheckCircular(name, new Stack<String>(dependants), new List<String>());

            //Passed Circular checks | Assign
            SetCellAssist(name, formula, formula.GetType());
            

            //Check for circular reference
            foreach (String s in dependants) {
                try {
                    CheckName(s);
                } catch {
                    MakeCell(name).SetContents(String.Empty);
                    throw new FormulaFormatException(s + " is not a valid variable");
                }
                

                //Passed test add dependency
                dg.AddDependency(s, name);

            }
            
            //Get cells that need to be recalculated
            ISet<String> cellsToRecalculate = new HashSet<String>(GetCellsToRecalculate(name));

            //Don't calculate if all dependants will be formula errors anyway.
            if (GetCellValue(name).GetType() != typeof(SS.FormulaError)) {
                //recalculate them
                Recalculate(cellsToRecalculate);
            }

            return cellsToRecalculate;
        }

        /// <summary>
        /// /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override ISet<string> SetContentsOfCell(string name, string content) {
            changed = true;

            if (content == null) {
                throw new ArgumentNullException();
            }

            CheckName(name);
            name = name.ToUpper();

            //Try to parse content as double
            if(Double.TryParse(content, out double doubContent)) {
                return SetCellContents(name, doubContent);
            }


            //If begins with = then try to make formula
            if (content.Count() > 0 && content[0].Equals('=')) {
                string f = content.Substring(1);
                Formula form = new Formula(f , s=>s.ToUpper(), s=> isValid.IsMatch(s));
                return SetCellContents(name, form);
            }

            //For a string
            return SetCellContents(name, content);
            
        }

        /// <summary>
        /// Recalculates cells in set by setting resetting contents and values
        /// </summary>
        /// <param name="cellsToRecalculate"></param>
        private void Recalculate(ISet<String> cellsToRecalculate) {

            //If any of these are a formula recalculate
            foreach (String s in cellsToRecalculate) {              
                if (activeCells[s].GetContents().GetType() == typeof(Formula)) {

                    String tempContents = activeCells[s].GetContents().ToString();
                    SetValue(s, (Formula)activeCells[s].GetContents(), Lookup);
                    activeCells[s].SetContents(new Formula(tempContents));
                }
            }
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name) {
            CheckName(name);
            foreach (String s in dg.GetDependents(name)) {
                yield return s;
            }
        }

        /// <summary>
        /// Either finds active cell or creates a new one and adds to ActiveCell
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Cell MakeCell(String name) {
            Cell NewCell;

            name = name.ToUpper();

            //Either find or create a cell
            if (activeCells.ContainsKey(name)) {
                NewCell = activeCells[name];
            } else {
                NewCell = new Cell(name);
                activeCells.Add(name, NewCell);
            }

            return NewCell;
        }

        /// <summary>
        /// Assist for multiple SetCellcontents methods
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="T"></param>
        /// <exception cref="SS.InvalidNameException"></exception>
        private void SetCellAssist(String name, Object content, Type T) {

            name = name.ToUpper();
            MakeCell(name);
            
            //If content is a double
            if (T == typeof(Double)) {
                activeCells[name].SetValue(content);
                activeCells[name].SetContents(content);
            }

            //If content is a string
            if (T == typeof(String)) {
                activeCells[name].SetValue(content);
                activeCells[name].SetContents(content);
            }

            //If content is a formula
            if (T == typeof(Formula)) {
                String tempContents = content.ToString();
                SetValue(name, (Formula)content, Lookup);
                activeCells[name].SetContents(new Formula(tempContents));
            }
        }

        private void SetValue(String name, Formula form, Lookup lookup) {
            
            try {
                activeCells[name.ToUpper()].SetValue(form.Evaluate(lookup));
            } catch (Exception e) {
                activeCells[name.ToUpper()].SetValue(new FormulaError(e + ""));
            }

        }


        /// <summary>
        /// Return true if name is valid
        /// Otherwise throws a InvalidNameExeption
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="SS.InvalidNameException"></exception>
        private void CheckName(String name) {
            //If name is null then throw InvalidNameException
            if (name == null) {
                throw new InvalidNameException();
            }
            //If name is not valid then throw InvalidNameException
            if (!isValid.IsMatch(name)) {
                throw new InvalidNameException();
            }
            return;
        }

        /// <summary>
        /// Performs lookup function for formuls
        /// Given the name of a cell returns it's value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="FormulaEvaluationException">if number not found</exception>
        private Double Lookup(String name) {
            return (Double)activeCells[name.ToUpper()].GetValue();
        }

        /// <summary>
        /// For each cell name in queue, searches for any dependees that equal name.
        /// If a match is found throws a CircularDependance exception.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="q"></param>
        /// <param name="visited"></param>
        /// <exception cref="SS.CircularException">If a circular reference is found</exception>
        private void CheckCircular(string name, Stack<string> q, List<String> visited) {
            //If the queue is not empty
            while (q.Count > 0) {
                
                //If the value at the top of the queue matches name
                //Circular reference was found
                if (q.Peek().Equals(name)) {
                    throw new CircularException();
                }
                //If not add any of the dependees of the first element
                // that have not already been visited.
                foreach (String s in dg.GetDependees(q.Pop())) {
                    if (!visited.Contains(s)) {
                        q.Push(s);
                        visited.Add(s);
                    }
                }

                //Run this method again
                CheckCircular(name, q, visited);
            }
        }
    }
}