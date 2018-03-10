//Hulk Buster

using Formulas;
using SS;
using SSGui;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SpreadsheetGUI {
    public class Controller {

        private SpreadsheetView window;

        private Spreadsheet ssModule;

        /// <summary>
        /// Constructor contollor for when a new/blank spreadsheet is added.
        /// </summary>
        /// <param name="window"></param>
        public Controller(SpreadsheetView window) {
            this.window = window;
            ssModule = new Spreadsheet(new Regex("^[a-zA-Z]{1}[1-9]{1}[0-9]?$"));
            window.NewEvent += HandleNew;
            window.FileChosenEvent += HandleFileChosen;
            window.CloseEvent += HandleClose;
            window.SelectionEvent += HandleChange;
            window.UpdateEvent += HandleUpdate;
            window.SaveEvent += HandleSave;
        }

        /// <summary>
        /// Constructor for opening spreadsheet.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="filename"></param>
        public Controller(SpreadsheetView window,String filename) : this(window) {
            
            TextReader openfile = null;
            try {
                ssModule = new Spreadsheet(openfile = File.OpenText(filename), new Regex("^[a-zA-Z]{1}[1-9]{1}[0-9]?$"));
                window.NewEvent += HandleNew;
                DrawFromFile();
            } catch {
                MessageBox.Show("Error occured when trying to open file.");
            } 
        }

        /// <summary>
        /// Handles a request to close the window
        /// </summary>
        private void HandleClose() {

            DialogResult dialogResult = MessageBox.Show("Are you sure you wan't to close this window?", "Unsaved Data Warning", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes) {
                window.DoClose();
            }
            
        }

        /// <summary>
        /// Handles a request to open a new window.
        /// </summary>
        private void HandleNew() {
            window.OpenNew();
        }


        /// <summary>
        /// When a cell is selected, 
        /// updates the textboxes for contents, name, and value.
        /// </summary>
        /// <param name="name"></param>
        private void HandleChange(String name) {
            Object content = ssModule.GetCellContents(name);
            String convertContents;

            if (content.GetType() == typeof(Formula)) {
                convertContents = "=" + content.ToString();
            } else {
                convertContents = content.ToString();
            }
            window.UpdateContentBox(convertContents);
            window.UpdateValueBox(ssModule.GetCellValue(name).ToString());
            window.UpdateNameBox(name);
        }

        /// <summary>
        /// When content box is changed, updates cell's contents
        /// redraws form values. If an error occurs updates Error Label.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="content"></param>
        private void HandleUpdate(int col, int row, String content) {

            String name = window.GetName(col, row);
            try {
                ssModule.SetContentsOfCell(name, content);
                
                window.UpdateErrorLabel(false, "");
                window.DrawCell(col, row, ssModule.GetCellValue(name).ToString());
                DrawFromFile();
                HandleChange(name);
            } catch (Exception e) {
                window.UpdateErrorLabel(true, "You have attempted to add a " + e.GetType().ToString() + " at " + name);
            }
                
        }

        /// <summary>
        /// Handles a request to open a file.
        /// </summary>
        private void HandleFileChosen(String filename) {
            window.OpenExisting(filename);
        }

        /// <summary>
        /// Saves the spreadsheet
        /// </summary>
        /// <param name="filename"></param>
        private void HandleSave(String filename) {
            TextWriter saveFile = null;
            ssModule.Save(saveFile = File.CreateText(filename));
            saveFile.Close();
        }

        /// <summary>
        /// Updates all cell values in spreasheet panel.
        /// </summary>
        private void DrawFromFile() {

            string temp = "";
            foreach (String cell in ssModule.GetNamesOfAllNonemptyCells()) {
                temp += cell + ", ";
                int col = cell[0] - 65;
                int row = Convert.ToInt32(cell.Substring(1)) - 1;

                window.DrawCell(col, row, ssModule.GetCellValue(cell).ToString());
            }
        }
    }
}
