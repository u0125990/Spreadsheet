/***********************
 * u1080787
 * Kevin Claiborne
 * PS2
 * *********************/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Test cases for use in testing Dependency Graph
/// </summary>
namespace DependancyTestCases {
    [TestClass]
    public class UnitTests {

        /// <summary>
        /// Test Add One Null String for s
        /// </summary>
        [ExpectedException(typeof(System.ArgumentNullException))]
        [TestMethod]
        public void AddOneNullS() {
            DependencyGraph dg = new DependencyGraph();

            String s = null;
            dg.AddDependency(s , "A1");
            Assert.IsFalse(dg.HasDependents(""));
        }

        /// <summary>
        /// Test Add One Null String for t
        /// </summary>
        [ExpectedException(typeof(System.ArgumentNullException))]
        [TestMethod]
        public void AddOneNullT() {
            DependencyGraph dg = new DependencyGraph();

            String t = null;
            dg.AddDependency("A1", t);
            Assert.IsFalse(dg.HasDependents("A1"));
        }

        /// <summary>
        /// Testing Size
        /// Test Add One Dependency
        /// Should return 1 dependency
        /// </summary>
        [TestMethod]
        public void AddOneAndCheckSize() {
            DependencyGraph dg = new DependencyGraph();

            dg.AddDependency("A1", "A3");
            dg.AddDependency("A1", "A3");

            int size = dg.Size;

            Assert.AreEqual(1, dg.Size);
        }

        /// <summary>
        /// Testing Size
        /// Test Add a lot of Dependencies
        /// And make sure the number matches
        /// </summary>
        [TestMethod]
        public void AddAlotAndCheckSize() {
            DependencyGraph dg = new DependencyGraph();
            List<String> cellList = GetListOfCells(1, 26, 1, 50); //A1:Z50
            List<String> dependencies = new List<string>();

            Random r = new Random();
            
            for (int i = 0; i < 5000; i++) {

                int first = r.Next(0, 1299);
                int second = r.Next(0, 1299);

                if (cellList[first] != cellList[second]) {

                    String s = ("(" + cellList[first] + "), (" + cellList[second] + ")");

                    dg.AddDependency(cellList[first], cellList[second]);
                    if (!dependencies.Contains(s)) {
                        dependencies.Add(s);
                    }
                } 
            }

            Assert.AreEqual(dependencies.Count, dg.Size);
        }

        /// <summary>
        /// Adding multiple Dependants macthes List
        /// </summary>
        [TestMethod]
        public void AddCheckReultsMatchExpected() {
            DependencyGraph dg = new DependencyGraph();
            List<String> cellList = GetListOfCells(1, 3, 1, 10);

            List<String> expected = new List<String>();
            //Add 10 dependencies from list
            for (int i = 1; i <= 10; i++) {
                dg.AddDependency(cellList[0], cellList[i]);
                expected.Add(cellList[i]);
            }


            List<String> result = new List<String>();
            

            foreach (String s in dg.GetDependents(cellList[0])) {
                result.Add(s);
            }

            result.Sort();
            expected.Sort();

            int notEqual = 0;
            for (int i = 0; i< result.Count; i++) {
                if (!result[i].Equals(expected[i])) {
                    notEqual++;
                }
            }

            Assert.AreEqual(0, notEqual);
        }

        /// <summary>
        /// Adding multiple Dependants
        /// Than Removing them all
        /// Checks that hasdependants should be false
        /// </summary>
        [TestMethod]
        public void AddRemoveEmpty() {
            DependencyGraph dg = new DependencyGraph();
            List<String> cellList = GetListOfCells(1, 3, 1, 10);

            //Add 10 dependencies from list
            for (int i = 1; i <= 10; i++) {
                dg.AddDependency(cellList[0], cellList[i]);
            }

            //remove all dependencies
            int current = 1;
            while (dg.Size > 0) {
                dg.RemoveDependency(cellList[0], cellList[current]);
                current++;
            }
            Assert.IsFalse(dg.HasDependents(cellList[0]));
        }

        /// <summary>
        /// Adding multiple Dependees and matching
        /// results.
        /// </summary>
        [TestMethod]
        public void CheckGetDependeesResults() {
            DependencyGraph dg = new DependencyGraph();
            List<String> cellList = GetListOfCells(1, 3, 1, 10);

            List<String> expected = new List<String>();
            //Add 10 dependencies from list
            for (int i = 1; i <= 10; i++) {
                dg.AddDependency(cellList[i], cellList[0]);
                expected.Add(cellList[i]);
            }


            List<String> result = new List<String>();


            foreach (String s in dg.GetDependees(cellList[0])) {
                result.Add(s);
            }

            result.Sort();
            expected.Sort();

            int notEqual = 0;
            for (int i = 0; i < result.Count; i++) {
                if (!result[i].Equals(expected[i])) {
                    notEqual++;
                }
            }

            Assert.AreEqual(0, notEqual);
        }

        /// <summary>
        /// Adding multiple Dependees
        /// Than Removing them all
        /// Checks that hasdependees should be false
        /// </summary>
        [TestMethod]
        public void AddRemoveDependees() {
            DependencyGraph dg = new DependencyGraph();
            List<String> cellList = GetListOfCells(1, 3, 1, 10);

            //Add 10 dependencies from list
            for (int i = 1; i <= 10; i++) {
                dg.AddDependency(cellList[i], cellList[0]);
            }

            //remove all dependencies
            int current = 1;
            while (dg.Size > 0) {
                dg.RemoveDependency(cellList[current], cellList[0]);
                current++;
            }
            Assert.IsFalse(dg.HasDependees(cellList[0]));
        }

        /// <summary>
        /// Replace all dependants
        /// </summary>
        [TestMethod]
        public void ReplaceDependentsMatches() {
            DependencyGraph dg = new DependencyGraph();
            List<String> cellList = GetListOfCells(1, 3, 1, 10);
            List<String> newDepends = new List<string>();

            //Add 10 dependencies from list
            for (int i = 1; i <= 10; i++) {
                dg.AddDependency(cellList[0], cellList[i]);
            }

            //Add Another 10 cells to alt list
            for (int i = 11; i <= 20; i++) {
                newDepends.Add(cellList[i]);
            }

            dg.ReplaceDependents(cellList[0],newDepends);

            List<String> result = new List<String>();

            foreach (String s in dg.GetDependees(cellList[0])) {
                result.Add(s);
            }

            result.Sort();
            newDepends.Sort();

            int notEqual = 0;
            for (int i = 0; i < result.Count; i++) {
                if (!result[i].Equals(newDepends[i])) {
                    notEqual++;
                }
            }

            Assert.AreEqual(0, notEqual);
        }


        /// <summary>
        /// Replace all Dependees, Check Matches
        /// </summary>
        [TestMethod]
        public void ReplaceDependeesMatches() {
            DependencyGraph dg = new DependencyGraph();
            List<String> cellList = GetListOfCells(1, 3, 1, 10);
            List<String> newDepends = new List<string>();

            //Add 10 dependencies from list
            for (int i = 1; i <= 10; i++) {
                dg.AddDependency(cellList[i], cellList[0]);
            }

            //Add Another 10 cells to alt list
            for (int i = 11; i <= 20; i++) {
                newDepends.Add(cellList[i]);
            }

            dg.ReplaceDependees(cellList[0], newDepends);

            List<String> result = new List<String>();

            foreach (String s in dg.GetDependees(cellList[0])) {
                result.Add(s);
            }

            result.Sort();
            newDepends.Sort();

            int notEqual = 0;
            for (int i = 0; i < result.Count; i++) {
                if (result[i].Equals(newDepends[i])) {
                    notEqual++;
                }
            }

            Assert.AreEqual(10, notEqual);
        }

        /// <summary>
        /// Check that adding new dependants will a null value
        /// doesn't mess anything up. (Should not have a null in new list)
        /// </summary>
        [ExpectedException(typeof(System.ArgumentNullException))]
        [TestMethod]
        public void ReplaceWithListContainingOneNull() {
            DependencyGraph dg = new DependencyGraph();
            List<String> cellList = GetListOfCells(1,3,1,10); //A1:C10
            List<String> newDepends = new List<string>();
          
            //Add 10 dependencies from list
            for (int i = 1; i <= 10; i++) {
                dg.AddDependency(cellList[i], cellList[0]);
            }

            //Add Another 10 cells to alt list
            for (int i = 11; i <= 20; i++) {
                newDepends.Add(cellList[i]);
            }

            newDepends[5] = null;
            dg.ReplaceDependees(cellList[0], newDepends);

            List<String> result = new List<String>();

            foreach (String s in dg.GetDependees(cellList[0])) {
                result.Add(s);
            }

            newDepends.Remove(null);

            result.Sort();
            newDepends.Sort();

            int equal = 0;
            for (int i = 0; i < result.Count; i++) {
                if (result[i].Equals(newDepends[i])) {
                    equal++;
                }
            }

            Assert.AreEqual(9, equal);
        }

        /// <summary>
        /// add timing Check 100,000 randomly placed
        /// </summary>
        [TestMethod]
        public void TimingAdding100K() {
            DependencyGraph dg = new DependencyGraph();
            List<String> cellList = GetListOfCells(1, 26, 1, 200); //A1:Z200
            
            Random r = new Random();

            Stopwatch stopWatch = new Stopwatch();
            TimeSpan t = stopWatch.Elapsed;


            while (dg.Size <= 100000) {

                int first = r.Next(0, 1299);
                int second = r.Next(0, 1299);

                if (cellList[first] != cellList[second]) {

                    String s = ("(" + cellList[first] + "), (" + cellList[second] + ")");

                    //start before add
                    stopWatch.Start();
                    dg.AddDependency(cellList[first], cellList[second]);
                    stopWatch.Stop();
                    
                    //stop after add (no need to time loop)
                    t += stopWatch.Elapsed;
                }
            }


            double elapsed = t.Milliseconds;

            Assert.IsTrue(elapsed < 1000);
            }

        /// <summary>
        /// remove timing Check
        /// Remove from large graph
        /// </summary>
        [TestMethod]
        public void ABunchOfTiming() {
            DependencyGraph dg = new DependencyGraph();
            List<String> cellList = GetListOfCells(1, 26, 1, 200); //A1:Z200

            Random r = new Random();

            Stopwatch stopWatch = new Stopwatch();
            TimeSpan t = stopWatch.Elapsed;

            //add 50,000
            for (int i = 0; i < 50000; i++) {

                int first = r.Next(0, 1299);
                int second = r.Next(0, 1299);

                if (cellList[first] != cellList[second]) {

                    String s = ("(" + cellList[first] + "), (" + cellList[second] + ")");
                    dg.AddDependency(cellList[first], cellList[second]);                    
                }
            }

            //Add known value
            dg.AddDependency("Z8", "A5");
            dg.AddDependency("Z8", "A6");
            dg.AddDependency("Z8", "A7");
            dg.AddDependency("Z9", "A7");
            dg.AddDependency("Z10", "A7");

            //add 50,000 more
            while (dg.Size <= 100000) {
                //add 50,000 more(if they don't exist)
               
                int first = r.Next(0, 1299);
                int second = r.Next(0, 1299);

                if (cellList[first] != cellList[second]) {

                    String s = ("(" + cellList[first] + "), (" + cellList[second] + ")");
                    dg.AddDependency(cellList[first], cellList[second]);
                    
                }
            }

            //Test removing
            stopWatch.Start();
            dg.RemoveDependency("Z8", "A5");
            stopWatch.Stop();
            double elapsed = t.Milliseconds;
            Assert.IsTrue(elapsed < 1000);

            //Test Getting dependents
            stopWatch = new Stopwatch();
            t = stopWatch.Elapsed;
            stopWatch.Start();
            var nl = dg.GetDependents("Z8");
            stopWatch.Stop();
            elapsed = t.Milliseconds;
            Assert.IsTrue(elapsed < 1000);

            //Test replacing dependants
            stopWatch = new Stopwatch();
            t = stopWatch.Elapsed;
            stopWatch.Start();
            dg.ReplaceDependees("A3", nl);
            stopWatch.Stop();
            elapsed = t.Milliseconds;
            Assert.IsTrue(elapsed < 1000);


            //Test Getting dependees
            stopWatch = new Stopwatch();
            t = stopWatch.Elapsed;
            stopWatch.Start();
            nl = dg.GetDependees("A7");
            stopWatch.Stop();
            elapsed = t.Milliseconds;
            Assert.IsTrue(elapsed < 1000);

            //Test replacing dependees
            stopWatch = new Stopwatch();
            t = stopWatch.Elapsed;
            stopWatch.Start();
            dg.ReplaceDependees("A7", nl);
            stopWatch.Stop();
            elapsed = t.Milliseconds;
            Assert.IsTrue(elapsed < 1000);
        }

        /// <summary>
        /// Test creating a graph from another graph
        /// and ensures changes to one do not effect the other.
        /// </summary>
        [TestMethod]
        public void TestNewGraph() {
            DependencyGraph dg = new DependencyGraph();
            List<String> cellList = GetListOfCells(1, 3, 1, 50);           

            //Add 10 dependencies from list
            for (int i = 1; i <= 10; i++) {
                dg.AddDependency(cellList[i], cellList[0]);
            }

            DependencyGraph dg2 = new DependencyGraph(dg);

            dg2.AddDependency(cellList[5], cellList[6]);
            dg.AddDependency(cellList[5], cellList[6]);

            List<String> dg2results = new List<String>();
            List<String> dg1results = new List<String>();

            foreach (String s in dg2.GetDependees(cellList[0])) {
                dg2results.Add(s);
            }
            foreach (String s in dg2.GetDependees(cellList[6])) {
                dg2results.Add(s);
            }

            foreach (String s in dg.GetDependees(cellList[0])) {
                dg1results.Add(s);
            }

            int countMatch = 0;
            for (int i = 0; i < dg1results.Count; i++) {
                if (dg1results[i] == dg2results[i]) {
                    countMatch++;
                }
            }



            Assert.AreEqual(10, countMatch);
        }

        /// <summary>
        /// Provides a handy list of cells from culumn to row
        /// Column maps to A_Z (1-26), Row must be greater than 0.
        /// </summary>
        /// <returns></returns>
        private static List<String> GetListOfCells(int startCol, int endCol, int startRow,int endRow) {
            List<String> cellList = new List<string>();

            //column A:Z
            if (startCol >= 1 && startCol <= 26 && endCol >= 1 && endCol <= 26 
                    && startRow > 0 && endRow > 0 && startRow <= endRow && startCol <= endCol) {
                
                //Actually need 1 less for corect letters
                startCol--;

                //add A1:C10 to cellslist
                for (int i = startCol; i < endCol; i++) {
                    for (int j = startRow; j <= endRow; j++) {
                        String cell1 = (char)('a' + i) + "" + j;
                        cellList.Add(cell1.ToUpper());
                    }
                }
            }
            return cellList;
        }
    } 
} 