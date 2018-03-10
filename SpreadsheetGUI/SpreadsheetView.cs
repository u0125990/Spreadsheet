using System;

namespace SpreadsheetGUI {
    public interface SpreadsheetView {

        event Action CloseEvent;

        event Action NewEvent;

        event Action<string> SaveEvent;

        event Action<int,int,string> UpdateEvent;

        event Action<string> FileChosenEvent;

        event Action<string> SelectionEvent;
        
        void OpenExisting(String filename);
            
        void DoClose();

        void OpenNew();

        void DrawCell(int col, int row, String Value);

        void UpdateNameBox(String name);

        void UpdateContentBox(String content);

        void UpdateValueBox(String value);

        void UpdateErrorLabel(bool hasError, String error);

        String GetName(int col, int row);
    }   
}