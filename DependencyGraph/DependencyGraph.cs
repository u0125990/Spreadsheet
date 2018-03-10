// Skeleton implementation written by Joe Zachary for CS 3500, January 2018.

/***********************
 * u1080787
 * Kevin Claiborne
 * PS2
 * *********************/


using SS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dependencies {
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph {


        /// <summary>
        /// Hashtable of cells
        /// </summary>
        private Dictionary<String, Cell> cellDict;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph() {
            cellDict = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Creates a dependancy graph from another dependancy graph.
        /// </summary>
        /// <param name="dg"></param>
        public DependencyGraph(DependencyGraph dg) {

            cellDict = new Dictionary<string, Cell>();

            foreach (String s in dg.GetCells().Keys.ToList<String>()) {
                foreach(String t in dg.GetDependents(s)) {
                    AddDependency(s, t);
                }
            }
        }

        /// <summary>
        /// Returns list of cells
        /// </summary>
        /// <returns></returns>
        private Dictionary<String, Cell> GetCells() {
            return cellDict;
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size {
            get {
                List<Cell> cells = cellDict.Values.ToList<Cell>();
                int size = 0;
                foreach (Cell c in cells) {
                    size += c.getDep().Count;
                }
                return size;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Requires s != null.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">If parameter is null</exception>
        public bool HasDependents(string s) {

            if (s != null) {
                Cell tempCell;

                if (!cellDict.TryGetValue(s, out tempCell)) {
                    return false;
                } else {
                    if (tempCell.getDep().Count > 0) {
                        return true;
                    } else {
                        return false;
                    }
                }
            } else {
                throw new ArgumentNullException("String s cannot be null");
            }
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.  Requires s != null.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">If parameter is null</exception>
        public bool HasDependees(string s) {
            if (s != null) {
                
                foreach (KeyValuePair<String, Cell> c in cellDict) {
                    if (c.Value.getDep().ContainsKey(s)) {
                        return true;
                    }
                }
                return false;
            } else {
                throw new ArgumentNullException("String s cannot be null");
            }
        }

        /// <summary>
        /// Enumerates dependents(s).  Requires s != null.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">If parameter is null</exception>
        public IEnumerable<string> GetDependents(string s) {
            if (s != null) {

                Cell tempCell;

                if (cellDict.TryGetValue(s, out tempCell)) {
                    foreach (KeyValuePair<String, Cell> c in tempCell.getDep()) {
                        yield return c.Key;
                    }
                }

            } else {
                throw new ArgumentNullException("String s cannot be null");
            }
        }

        /// <summary>
        /// Enumerates dependees(s).  Requires s != null.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">If parameter is null</exception>
        public IEnumerable<string> GetDependees(string s) {

            if (s != null) {


                foreach (KeyValuePair<String, Cell> c in cellDict) {
                    Cell tempcell;

                    if (c.Value.getDep().TryGetValue(s, out tempcell)) {
                      yield return c.Value.GetName();
                    }
                }    
            } else {
                throw new ArgumentNullException("String s cannot be null");
            }
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">If parameters are null</exception>
        public void AddDependency(string s, string t) {
            if (s != null & t != null) {

                

                //create cell
                if(!this.cellDict.TryGetValue(s, out Cell tempCell)) {
                    tempCell = new Cell(s);
                }
                
                //tempcell
                if (!cellDict.TryGetValue(s, out Cell dependent)) {
                    dependent = new Cell(s);
                }

                //Creat second Cell
                if (!tempCell.getDep().ContainsKey(t)) {

                    tempCell.getDep().Add(t, dependent);
                    if (!cellDict.ContainsKey(s)) {
                        cellDict.Add(s, tempCell);
                    }
                    
                }
            } else {
                throw new ArgumentNullException("String s and t cannot be null");
            }
        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">If parameters are null</exception>
        public void RemoveDependency(string s, string t) {
            if (s != null & t != null) {

                Cell tempcell;
                if (cellDict.TryGetValue(s, out tempcell)) {
                    if (tempcell.getDep().ContainsKey(t)) {
                        tempcell.getDep().Remove(t);
                    }
                }

            } else {
                throw new ArgumentNullException("String s and t cannot be null");
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">If parameters are null</exception>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents) {

            if (s != null) {
                if (!cellDict.TryGetValue(s, out Cell dependee)) {
                    dependee = new Cell(s);
                }

                //RemoveAll
                dependee.getDep().Clear();

                //Add New Ones
                foreach (String t in newDependents) {
                    if (t != null) {
                        AddDependency(s, t);
                    } else {
                        throw new ArgumentNullException("String t cannot be null: IEnumerable contains a null");
                    }
                }
            } else {
                throw new ArgumentNullException("String s cannot be null");
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">If parameters are null</exception>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees) {

            if (t != null) {

                if (!cellDict.TryGetValue(t, out Cell dependee)) {
                    dependee = new Cell(t);
                }


                //Remove everything in there
                foreach (KeyValuePair<String, Cell> c in cellDict) {
                    RemoveDependency(c.Value.GetName(), t);
                }

                foreach (String s in newDependees) {
                    if (s != null) {
                        AddDependency(s, t);
                    } else {
                        throw new ArgumentNullException("String s cannot be null: IEnumerable contains a null");
                    }
                }
            } else {
                throw new ArgumentNullException("String t cannot be null");
            }
        }
    }
}
