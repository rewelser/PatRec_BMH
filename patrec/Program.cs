using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace patrec {
    class Program {
        static void Main(string[] args) {


            ////////////////////////////
            // Boyer-Moore-Horspool test
            int[] points = new int[] { 9, 5, 4, 2, 9, 9, 8, 6, 3, 6, 6, 6, 2, 4, 6, 1, 5, 6, 0, 7 };

            int[] neur_1 = new int[] { 9, 5 };
            int[] neur_2 = new int[] { 9, 5, 4 };
            int[] neur_3 = new int[] { 6, 3, 6, 6, 6, 2, 4 };
            int[] neur_4 = new int[] { 9, 8, 6, 3, 6, 6, 6 };
            int[] neur_5 = new int[] { 6, 0, 7 };
            int[] neur_6 = new int[] { 6, 2, 4, 6, 1, 5 };

            int ap_1;
            int ap_2;
            int ap_3;
            int ap_4;
            int ap_5;
            int ap_6;

            // 1 should be mismatch = { S, R, U, T, R, U, T, R }
            // short = { 12342342 }
            int[] points2 = new int[] { 1, 2, 3, 4, 2, 3, 4, 2 };

            // 5 should be mismatch = { S, S, S, S, S, T, R, U, T, R }
            // short = { 1111142342 }
            int[] points3 = new int[] { 1, 1, 1, 1, 1, 4, 2, 3, 4, 2 };

            // 5 should be mismatch = { S, S, S, S, T, T, R, U, T, R }
            // short = { 1111442342 }
            int[] points4 = new int[] { 1, 1, 1, 1, 4, 4, 2, 3, 4, 2 };

            // short = { 1112442342 }
            int[] points5 = new int[] { 1, 1, 1, 2, 4, 4, 2, 3, 4, 2 };

            // pattern representing = { T, R, U, T, R }
            // short = { 42342 }
            int[] pattern = new int[] { 4, 2, 3, 4, 2 };
            //                        {       3, 4, 4 }
            //                        {       3, 4, 4 }

            //int[] pattern = { 4, 3, 4, 4, 2 };

            // Test preprocess by itself, otherwise unnecessary as is called in Search_bmh();
            //Preprocess(pattern);

            // Uncomment to test BMH algorithm
            //Debug.WriteLine(Search_bmh(points2, pattern));


            ////////////////////////////
            // Intensity calculations test

            //int[] pixels = new int[] {561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,561,255};
            int[] pixels = new int[] {1,2,1,0,3,4,3,1,6,9,7,5,3,2};

            List<int>[] sites = Intensity2(pixels);


            for (int i = 0; i < sites.Length; i++) {
                if (i == 0) {
                    Debug.WriteLine("Minima are: ");
                } else {
                    Debug.WriteLine("Maxima are: ");
                }
                for (int j = 0; j < sites[i].Count; j++) {
                    Debug.WriteLine("[" + sites[i][j] + "]: " + pixels[sites[i][j]]);
                }
                
            }
        }

        // Intensity thingie
        static List<int> Intensity(int[] pixels) {
            List<int> sites = new List<int>();
            int running_total = 0;
            int running_avg = 0;
            int max_dif_so_far = 0;
            for (int i = 0; i < pixels.Length; i++) {
                running_total += pixels[i];
                int old_avg = running_avg;
                running_avg = (running_total / (i + 1));

                if (i == 0) {
                    Debug.WriteLine(running_avg);
                }


                if (i != 0 && (Math.Abs(old_avg - running_avg) > max_dif_so_far)) {
                    max_dif_so_far = Math.Abs(old_avg - running_avg);
                    sites.Add(max_dif_so_far);
                }
            }

            return sites;
        }

        // Intensity thingie 2
        static List<int>[] Intensity2(int[] pixels) {
            List<int>[] sites = new List<int>[2];
            List<int> minima = new List<int>();
            List<int> maxima = new List<int>();

            for (int i = 0; i < pixels.Length; i++) {
                if (i != 0 && i != pixels.Length - 1) {
                    if ((pixels[i+1] < pixels[i]) && (pixels[i-1] < pixels[i])) {
                        maxima.Add(i);
                    } else if ((pixels[i + 1] > pixels[i]) && (pixels[i - 1] > pixels[i])) {
                        minima.Add(i);
                    }
                } else if (i == 0) {
                    if (pixels[i+1] < pixels[i]) {
                        maxima.Add(i);
                    } else if (pixels[i + 1] > pixels[i]) {
                        minima.Add(i);
                    }
                } else if (i == pixels.Length - 1) {
                    if (pixels[i-1] < pixels[i]) {
                        maxima.Add(i);
                    } else if (pixels[i - 1] > pixels[i]) {
                        minima.Add(i);
                    }
                }
            }
            sites[0] = minima;
            sites[1] = maxima;
            return sites;
        }

        // Boyer-Moore-horspool Search Algo
        // Create a bad match table
        static Dictionary<int, int> Preprocess(int[] pattern) {
            Dictionary<int, int> table = new Dictionary<int, int>();

            for (int i = pattern.Length - 1; i >= 0; i--) {
                if (!table.ContainsKey(pattern[i])) {
                    table.Add(pattern[i], i);
                }
            }

            table.Add(-1, -1);
            foreach (KeyValuePair<int, int> kvp in table) {
                Debug.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            return table;
        }

        // Search algo
        static int Search_bmh(int[] haystack, int[] p) {
            Dictionary<int, int> table = Preprocess(p);
            List<int> h = haystack.ToList();
            int skip = 0;
            int index = 0;
            string readout = "";
            while (h.Count >= p.Length) {
                for (int i = p.Length - 1; i >= 0; i--) {

                    if (p[i] != h[i]) {
                        if (table.ContainsKey(h[i])) {
                            skip = i - table[h[i]];
                            readout = p[i] + "vs" + h[i] + " (" + i + ") - (" + table[h[i]] + ")";

                            // Good suffix
                            if (skip < 0) {
                                skip = i - skip;
                                readout = "(" + i + ") - (" + skip + ")";
                            }
                        }
                        else {
                            skip = i - table[-1];
                            readout = "(" + i + ") - (" + table[-1] + ")";
                        }
                        i = -1;
                    }
                    else {
                        if (i == 0 && p[i] == h[i]) {
                            return index;
                        }
                    }

                }

                Debug.WriteLine(readout);
                Debug.WriteLine(skip);
                Debug.WriteLine("-------");
                index += skip;
                
                h.RemoveRange(0, skip);
                skip = 0;

                for (int i = 0; i < h.Count; i++) {
                    Debug.Write(h[i] + ", ");
                }
                Debug.WriteLine("");
                Debug.WriteLine("");
            }
            return -1;

        }
    }
}
