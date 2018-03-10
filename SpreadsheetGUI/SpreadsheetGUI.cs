//Hulk Buster

using SSGui;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace SpreadsheetGUI {
    public partial class SpreadsheetGui : Form, SpreadsheetView {

        /// <summary>
        /// Intializes Spreadsheet
        /// </summary>
        public SpreadsheetGui() {
            InitializeComponent();
        }

        /// <summary>
        /// Fired when a file is chosen with a file dialog.
        /// The parameter is the chosen filename.
        /// </summary>
        public event Action<string> FileChosenEvent;

        /// <summary>
        /// Fired when a close action is requested.
        /// </summary>
        public event Action CloseEvent;

        /// <summary>
        /// Fired when a new action is requested.
        /// </summary>
        public event Action NewEvent;

        /// <summary>
        /// Fires when a cell is updated
        /// </summary>
        public event Action<int, int, string> UpdateEvent;

        /// <summary>
        /// Fires When a cell is selected
        /// </summary>
        public event Action<string> SelectionEvent;

        /// <summary>
        /// Fired when Save is chosen
        /// </summary>
        public event Action<string> SaveEvent;

        /// <summary>
        /// Inital load events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Spreadsheet_Load(object sender, EventArgs e) {
            spreadsheetPanel1.SetSelection(0, 0);
            spreadsheetPanel1_SelectionChanged(spreadsheetPanel1);
            ErrorLabel.Visible = false;
        }

        /// <summary>
        /// Catches arrow keys for switching cells
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Spreadsheet_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right
                    || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) {
                ArrowKeys(e);
                spreadsheetPanel1_SelectionChanged(spreadsheetPanel1);
            }
        }


        /// <summary>
        /// Closes window
        /// </summary>
        public void DoClose() {
            Close();
        }


        /// <summary>
        /// Opens new Window
        /// </summary>
        public void OpenNew() {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        /// <summary>
        /// Open an existinfg file in a new window
        /// </summary>
        /// <param name="filename"></param>
        public void OpenExisting(String filename) {
            SpreadsheetApplicationContext.GetContext().OpenNew(filename);
        }

        /// <summary>
        /// Handles "New" menu item click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewItem_Click(object sender, EventArgs e) {

            if (NewEvent != null) {
                NewEvent();
            }
        }

        /// <summary>
        /// Handles the Click event of the openItem control.
        /// </summary>
        private void OpenItem_Click(object sender, EventArgs e) {

            fileDialog1.Filter = "SpreadSheet files (*.ss)|*.ss|All files (*.*)|*.*";
            fileDialog1.FilterIndex = 1;
            fileDialog1.RestoreDirectory = true;
            DialogResult result = fileDialog1.ShowDialog();
            if (result == DialogResult.Yes || result == DialogResult.OK) {
                if (FileChosenEvent != null) {
                    FileChosenEvent(fileDialog1.FileName);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the closeItem control.
        /// </summary>
        private void CloseItem_Click(object sender, EventArgs e) {
            if (CloseEvent != null) {
                CloseEvent();
            }
        }

        /// <summary>
        /// Handles when enter is pressed while inside Contentbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentBox_KeyDown(object sender, KeyEventArgs e) {

            //When keypressed is enter
            switch (e.KeyCode) {
                case Keys.Enter:
                    if (UpdateEvent != null) {

                        //Update Cell
                        int row, col;
                        spreadsheetPanel1.GetSelection(out col, out row);
                        UpdateEvent(col, row, ContentBox.Text);
                        spreadsheetPanel1.Select();

                        //Move to next cell
                        Spreadsheet_KeyDown(spreadsheetPanel1, new KeyEventArgs(Keys.Down));
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles new cell being selected and helpts to update
        /// Content Boxes.
        /// </summary>
        /// <param name="sender"></param>
        private void spreadsheetPanel1_SelectionChanged(SpreadsheetPanel sender) {
            if (SelectionEvent != null) {
                int row, col;
                spreadsheetPanel1.GetSelection(out col, out row);
                SelectionEvent(GetName(col, row));
                ContentBox.Select();
                ContentBox.SelectionStart = 0;
                ContentBox.SelectionLength = ContentBox.Text.Length;
            }
        }

        /// <summary>
        /// Dispalys the Value of a cell.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="Value"></param>
        public void DrawCell(int col, int row, String Value) {
            spreadsheetPanel1.SetValue(col, row, Value);
        }

        /// <summary>
        /// Opens the help menu if clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {
            SpreadsheetApplicationContext.GetContext().OpenHelp();
        }

        /// <summary>
        /// When called determines the arrow key.
        /// Shifts to the appropriate cell and scrolls page when necessary.
        /// </summary>
        /// <param name="e"></param>
        private void ArrowKeys(KeyEventArgs e) {
            int row, col;
            spreadsheetPanel1.GetSelection(out col, out row);
            double firstRow = spreadsheetPanel1.GetVScollPosition();
            double lastRow = ((spreadsheetPanel1.Bounds.Height - 30 - 20) / 20) + spreadsheetPanel1.GetVScollPosition();
            double mostLeft = spreadsheetPanel1.GetHScollPosition();
            double mostRight = ((spreadsheetPanel1.Bounds.Width - 30 - 20) / 80) + spreadsheetPanel1.GetHScollPosition();

            if (e.KeyCode == Keys.Right) {
                spreadsheetPanel1.SetSelection(col + 1, row);
                e.Handled = true;
                if (col + 1 == mostRight) {
                    spreadsheetPanel1.SetHScollPosition(1);

                }
            }

            if (e.KeyCode == Keys.Left) {
                spreadsheetPanel1.SetSelection(col - 1, row);
                e.Handled = true;
                if (col == mostLeft) {
                    spreadsheetPanel1.SetHScollPosition(-1);
                }
            }

            if (e.KeyCode == Keys.Down) {
                spreadsheetPanel1.SetSelection(col, row + 1);
                e.Handled = true;
                if (row + 1 == lastRow) {
                    spreadsheetPanel1.SetVScrollPosition(1);
                }

            }

            if (e.KeyCode == Keys.Up) {
                spreadsheetPanel1.SetSelection(col, row - 1);
                e.Handled = true;
                if (row == firstRow) {
                    spreadsheetPanel1.SetVScrollPosition(-1);
                }
            }
        }

        /// <summary>
        /// Updates the Text in NameBox
        /// </summary>
        /// <param name="name"></param>
        public void UpdateNameBox(string name) {
            this.NameBox.Text = name;
        }

        /// <summary>
        /// Updates the text in ContentBox
        /// </summary>
        /// <param name="content"></param>
        public void UpdateContentBox(string content) {
            this.ContentBox.Text = content;
        }

        /// <summary>
        /// Updates text in VlaueBox
        /// </summary>
        /// <param name="value"></param>
        public void UpdateValueBox(string value) {
            this.ValueBox.Text = value;
        }

        /// <summary>
        /// Converts col and row integers in into Uppercase Cell name
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public String GetName(int col, int row) {
            char alph = (char)(col + 65);
            int num = row + 1;
            String name = alph.ToString() + num.ToString();

            return name;
        }

        /// <summary>
        /// Changes the color of File text when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileToolStripMenuItem_Click(object sender, EventArgs e) {
            if (fileToolStripMenuItem.Enabled) {
                fileToolStripMenuItem.ForeColor = Color.FromArgb(49, 52, 62);
                optionsToolStripMenuItem.ForeColor = Color.White;
            }
            else {
                fileToolStripMenuItem.ForeColor = Color.White;
            }
        }

        /// <summary>
        /// Changes the color of Options text when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (fileToolStripMenuItem.Enabled) {
                optionsToolStripMenuItem.ForeColor = Color.FromArgb(49, 52, 62);
                fileToolStripMenuItem.ForeColor = Color.White;
            }
            else {
                optionsToolStripMenuItem.ForeColor = Color.White;
            }
        }

        /// <summary>
        /// Changes File and Options back to white.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip1_EnabledChanged(object sender, EventArgs e) {
            optionsToolStripMenuItem.ForeColor = Color.White;
            fileToolStripMenuItem.ForeColor = Color.White;
        }


        /// <summary>
        /// When error occurs makes ErrorLabel visible and updates text.
        /// If error has not occured, updates text and renders invisible.
        /// </summary>
        /// <param name="hasError"></param>
        /// <param name="error"></param>
        public void UpdateErrorLabel(bool hasError, string error) {
            if (hasError) {
                ErrorLabel.Visible = true;
                ErrorLabel.Text = error;
            }
            else {
                ErrorLabel.Visible = false;
                ErrorLabel.Text = "";
            }
        }

        /// <summary>
        /// Handles Save click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {

            saveFileDialog1.Filter = "SpreadSheet files (*.ss)|*.ss|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.Yes || result == DialogResult.OK) {
                if (SaveEvent != null) {
                    SaveEvent(saveFileDialog1.FileName);
                }
            }
        }
    }
}

