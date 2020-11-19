using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace merge
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Tuple<int, int>> unmergedList = new List<Tuple<int, int>>();

            //generate or read unmerged list
            Console.WriteLine("Select mode: (1 = read list from List.txt, 2 = random generated list");
            int mode = Convert.ToInt32(Console.ReadLine());

            switch(mode) {
                case 1: {
                    string[] lines = System.IO.File.ReadAllLines("./List.txt");
                    foreach(string line in lines) {
                        string[] split = line.Split(",");
                        unmergedList.Add(new Tuple<int, int>(int.Parse(Regex.Match(split[0], @"\d+").Value), int.Parse(Regex.Match(split[1], @"\d+").Value)));
                    }
                    break;
                }
                case 2: {
                    Console.WriteLine("Enter Interval number range:");
                    int numberRange = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter List size:");
                    int listSize = Convert.ToInt32(Console.ReadLine());
                    unmergedList = generateList(numberRange, listSize);
                    break;
                }
                default: {
                    Console.WriteLine("Wrong input: options are 1 and 2");
                    Console.ReadLine();
                    System.Environment.Exit(0);
                    break;
                }
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            //merge list
            List<Tuple<int, int>> mergedList = merge(unmergedList);
            
            Console.WriteLine("Merged List:");
            mergedList.ForEach(Console.WriteLine);
            watch.Stop();
            Console.WriteLine("Execution Time in ms:");
            Console.WriteLine(watch.ElapsedMilliseconds);
            Console.ReadLine();
        }

        static List<Tuple<int, int>> merge(List<Tuple<int, int>> inputList) {
            //sort list based on first interval value in asc order
            Console.Write("Sorting...");
            inputList.Sort((x,y) => x.Item1.CompareTo(y.Item1));
            Console.WriteLine("done!");
            Console.WriteLine("Input List:");
            inputList.ForEach(Console.WriteLine);

            /* Merge list:
                (a,b) , (c,d)
                possible cases;
                    - b between c and d => replace (a,b) with (a,d) and remove (c,d)
                    - b > d => (c,d) is inside (a,b) => remove (c,d)
                    - b < c => no more merge with (a,b) possible => start merging with (c,d)
            */
            Console.Write("Merging...");
            for(int i = 0; i < inputList.Count - 1;) {
                Tuple<int, int> currentElement = inputList[i];
                Tuple<int, int> nextElement = inputList[i+1];
                switch(compareTuple(currentElement, nextElement)) {
                    case 0: {
                        i++;
                        break;
                    }
                    case 1: {
                        inputList[i] = new Tuple<int, int>(currentElement.Item1, nextElement.Item2);
                        inputList.RemoveAt(i+1);
                        break;
                    }
                    case 2: {
                        inputList.RemoveAt(i+1);
                        break;
                    }
                    default: {
                        Console.WriteLine("error");
                        System.Environment.Exit(0);
                        break;
                    }
                }
            }
            Console.WriteLine("done!");
            return inputList;
        }

        /* generates list of Tuple<int, int> with randomized values */
        static List<Tuple<int,int>> generateList(int numberRange, int listSize) {
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            Random rnd = new Random();
            for(int i = 0; i < listSize; i++) {
                int a = rnd.Next(1, numberRange);
                int b = rnd.Next(a, numberRange);
                Tuple<int, int> element = new Tuple<int, int>(a, b);
                list.Add(element);
            }
            return list;
        }

        //input1 (a,b)
        //input2 (c,d)
        static int compareTuple (Tuple<int, int> input1, Tuple<int, int> input2) {
            if(input1.Item2 < input2.Item1) {
                return 0; //no overlap
            } else if(input1.Item1 <= input2.Item1 && input1.Item2 <= input2.Item2) {
                return 1; //overlap
            } else if(input1.Item1 <= input2.Item1 && input2.Item2 <= input1.Item2) {
                return 2; //input2 inside input1
            } else {
                return -1;
            }
        }
    }
}
